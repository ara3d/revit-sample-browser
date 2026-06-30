using Ara3D.Bowerbird.RevitSamples.AecAgent;
using Autodesk.Revit.DB;
using Newtonsoft.Json.Linq;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Ara3D.Bowerbird.RevitSamples;

public sealed class CreateChangeSetHandler : ToolHandlerBase
{
    public override McpToolDescriptor Descriptor => Desc(
        "aec.create_change_set",
        "Builds a declarative edit plan without executing it.",
        Schema.Object(
            ("intent", Schema.String("Human-readable intent")),
            ("operations", new JObject
            {
                ["type"] = "array",
                ["items"] = new JObject { ["type"] = "object" },
            })),
        ToolRiskClass.Write,
        tier: "semantic");

    public override Task<McpToolResult> InvokeAsync(JObject arguments, HostContext context)
    {
        var intent = arguments?["intent"]?.Value<string>() ?? "Agent change set";
        var ops = (arguments?["operations"] as JArray ?? [])
            .OfType<JObject>()
            .Select(o => new ChangeOp
            {
                Kind = Enum.TryParse<ChangeOpKind>(o["kind"]?.Value<string>(), true, out var kind)
                    ? kind
                    : ChangeOpKind.SetParameter,
                Payload = o["payload"] as JObject ?? o,
            })
            .ToList();

        var set = context.ChangeSetSession.Create(intent, ops);
        return TaskResult(McpToolResult.Success(new JObject
        {
            ["change_set_id"] = set.Id,
            ["intent"] = set.Intent,
            ["operation_count"] = set.Operations.Count,
            ["risk_level"] = set.RiskLevel,
            ["requires_approval"] = set.RequiresApproval,
            ["audit_resource"] = $"audit://changesets/{set.Id}",
        }));
    }
}

public sealed class ValidateChangeSetHandler : ToolHandlerBase
{
    public override McpToolDescriptor Descriptor => Desc(
        "aec.validate_change_set",
        "Validates a change set against the current model context.",
        Schema.ObjectReq(("change_set_id", Schema.String("Change set id")), "change_set_id"),
        ToolRiskClass.Write,
        tier: "semantic",
        requiresRevit: true);

    public override Task<McpToolResult> InvokeAsync(JObject arguments, HostContext context)
    {
        var id = arguments?["change_set_id"]?.Value<string>();
        if (string.IsNullOrEmpty(id))
            throw new McpProtocolException(-32602, "change_set_id is required");

        if (!context.ChangeSetSession.TryGet(id, out var set))
            return TaskResult(McpToolResult.Failure("Unknown change set.", "unknown_change_set"));

        var result = context.Bridge.RunOnRevitThread(app =>
        {
            var doc = app.ActiveUIDocument?.Document;
            if (doc == null)
                return McpToolResult.Failure("No document open", "no_document");

            var (valid, issues) = ChangeSetApplier.Validate(doc, set);
            return McpToolResult.Success(new JObject
            {
                ["change_set_id"] = id,
                ["valid"] = valid,
                ["issues"] = issues,
                ["risk_level"] = set.RiskLevel,
                ["requires_approval"] = set.RequiresApproval,
            });
        });
        return TaskResult(result);
    }
}

public sealed class PreviewChangesHandler : ToolHandlerBase
{
    public override McpToolDescriptor Descriptor => Desc(
        "aec.preview_changes",
        "Returns a human-readable preview of a change set before execution.",
        Schema.ObjectReq(("change_set_id", Schema.String("Change set id")), "change_set_id"),
        ToolRiskClass.Write,
        tier: "semantic",
        requiresRevit: true);

    public override Task<McpToolResult> InvokeAsync(JObject arguments, HostContext context)
    {
        var id = arguments?["change_set_id"]?.Value<string>();
        if (string.IsNullOrEmpty(id))
            throw new McpProtocolException(-32602, "change_set_id is required");

        if (!context.ChangeSetSession.TryGet(id, out var set))
            return TaskResult(McpToolResult.Failure("Unknown change set.", "unknown_change_set"));

        var result = context.Bridge.RunOnRevitThread(app =>
        {
            var doc = app.ActiveUIDocument?.Document;
            if (doc == null)
                return McpToolResult.Failure("No document open", "no_document");

            var preview = ChangeSetApplier.BuildPreview(doc, set);
            var approvalToken = set.RequiresApproval
                ? context.ChangeSetSession.CreateApprovalToken(id)
                : null;

            preview["approval_token"] = approvalToken;
            preview["audit_resource"] = $"audit://changesets/{id}";
            preview["next_step"] = set.RequiresApproval
                ? "Call aec.apply_changes with change_set_id and approval_token."
                : "Call aec.apply_changes with change_set_id.";

            return McpToolResult.Success(preview);
        });
        return TaskResult(result);
    }
}

public sealed class ApplyChangesHandler : ToolHandlerBase
{
    public override McpToolDescriptor Descriptor => Desc(
        "aec.apply_changes",
        "Applies a validated change set through the Revit transaction system.",
        Schema.Object(
            ("change_set_id", Schema.String("Change set id")),
            ("approval_token", Schema.String("Approval token when required"))),
        ToolRiskClass.Write,
        tier: "semantic",
        requiresRevit: true);

    public override Task<McpToolResult> InvokeAsync(JObject arguments, HostContext context)
    {
        var id = arguments?["change_set_id"]?.Value<string>();
        var approvalToken = arguments?["approval_token"]?.Value<string>();
        if (string.IsNullOrEmpty(id))
            throw new McpProtocolException(-32602, "change_set_id is required");

        if (!context.ChangeSetSession.TryGet(id, out var set))
            return TaskResult(McpToolResult.Failure("Unknown change set.", "unknown_change_set"));

        if (set.RequiresApproval && !context.ChangeSetSession.IsApproved(approvalToken, id))
            return TaskResult(McpToolResult.Failure("Approval required before applying this change set.", "approval_required"));

        var result = context.Bridge.RunOnRevitThread(app =>
        {
            var doc = app.ActiveUIDocument?.Document;
            if (doc == null)
                return McpToolResult.Failure("No document open", "no_document");

            var (valid, issues) = ChangeSetApplier.Validate(doc, set);
            if (!valid)
            {
                return McpToolResult.Failure("Change set failed validation.", "validation_failed", new JObject
                {
                    ["issues"] = issues,
                });
            }

            int applied;
            System.Collections.Generic.List<long> changedIds;
            using (var tx = new Transaction(doc, $"MCP: {set.Intent}"))
            {
                tx.Start();
                (applied, changedIds) = ChangeSetApplier.Apply(doc, set);
                tx.Commit();
            }

            var transaction = new AppliedTransaction
            {
                ChangeSetId = id,
                Intent = set.Intent,
                AppliedOperations = applied,
                ChangedElementIds = changedIds,
                Details = new JObject
                {
                    ["view_name"] = doc.ActiveView?.Name,
                    ["transaction_name"] = $"MCP: {set.Intent}",
                },
            };
            context.ChangeSetSession.MarkApplied(set, transaction);

            return McpToolResult.Success(new JObject
            {
                ["change_set_id"] = id,
                ["transaction_id"] = transaction.Id,
                ["applied_operations"] = applied,
                ["changed_element_ids"] = new JArray(changedIds),
                ["audit_resource"] = $"audit://transactions/{transaction.Id}",
            });
        });
        return TaskResult(result);
    }
}

public sealed class UndoLastAgentChangeHandler : ToolHandlerBase
{
    public override McpToolDescriptor Descriptor => Desc(
        "aec.undo_last_agent_change",
        "Reports the last applied agent change set and whether host undo is available.",
        Schema.Object(),
        ToolRiskClass.Write,
        tier: "semantic");

    public override Task<McpToolResult> InvokeAsync(JObject arguments, HostContext context)
    {
        var last = context.ChangeSetSession.GetLastApplied();
        return TaskResult(McpToolResult.Success(new JObject
        {
            ["last_change_set_id"] = last?.Id,
            ["last_intent"] = last?.Intent,
            ["last_transaction_id"] = last?.LastTransactionId,
            ["undo_available"] = false,
            ["message"] = "Use Revit Undo (Ctrl+Z) for the last MCP transaction when available.",
            ["audit_resource"] = last?.LastTransactionId != null
                ? $"audit://transactions/{last.LastTransactionId}"
                : null,
        }));
    }
}

public sealed class ClassifyOperationRiskHandler : ToolHandlerBase
{
    public override McpToolDescriptor Descriptor => Desc(
        "aec.classify_operation_risk",
        "Classifies risk for a proposed change set or tool name.",
        Schema.Object(
            ("change_set_id", Schema.String("Optional change set id")),
            ("tool_name", Schema.String("Optional tool name"))),
        tier: "semantic");

    public override Task<McpToolResult> InvokeAsync(JObject arguments, HostContext context)
    {
        var changeSetId = arguments?["change_set_id"]?.Value<string>();
        var toolName = arguments?["tool_name"]?.Value<string>();

        if (!string.IsNullOrEmpty(changeSetId)
            && context.ChangeSetSession.TryGet(changeSetId, out var set))
        {
            return TaskResult(McpToolResult.Success(new JObject
            {
                ["risk_level"] = set.RiskLevel,
                ["requires_approval"] = set.RequiresApproval,
            }));
        }

        if (!string.IsNullOrEmpty(toolName))
        {
            return TaskResult(McpToolResult.Success(new JObject
            {
                ["tool_name"] = toolName,
                ["risk_level"] = toolName.StartsWith("aec.color_", StringComparison.OrdinalIgnoreCase) ? "low"
                    : toolName.StartsWith("aec.apply_", StringComparison.OrdinalIgnoreCase) ? "medium"
                    : "read",
            }));
        }

        return TaskResult(McpToolResult.Failure("Provide change_set_id or tool_name.", "missing_input"));
    }
}

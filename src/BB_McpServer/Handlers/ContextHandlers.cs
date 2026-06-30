using Ara3D.Bowerbird.RevitSamples.AecAgent;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Newtonsoft.Json.Linq;
using System.Linq;
using System.Threading.Tasks;

namespace Ara3D.Bowerbird.RevitSamples;

public sealed class EchoHandler : ToolHandlerBase
{
    public override McpToolDescriptor Descriptor => Desc(
        "echo",
        "Echoes the message back to the client.",
        Schema.ObjectReq(("message", Schema.String("Message to echo")), "message"));

    public override Task<McpToolResult> InvokeAsync(JObject arguments, HostContext context)
    {
        var message = arguments?["message"]?.Value<string>();
        if (string.IsNullOrEmpty(message))
            throw new McpProtocolException(-32602, "Missing required argument: message");

        return TaskResult(McpToolResult.Success(new JObject { ["message"] = $"hello {message}" }));
    }
}

public sealed class RevitDocumentInfoHandler : ToolHandlerBase
{
    public override McpToolDescriptor Descriptor => Desc(
        "revit_document_info",
        "Returns the active Revit document title and path.",
        Schema.Object(),
        requiresRevit: true);

    public override Task<McpToolResult> InvokeAsync(JObject arguments, HostContext context)
    {
        var result = context.Bridge.RunOnRevitThread(app =>
        {
            var doc = app.ActiveUIDocument?.Document;
            if (doc == null)
                return McpToolResult.Failure("No document open", "no_document");

            return McpToolResult.Success(new JObject
            {
                ["title"] = doc.Title,
                ["path"] = string.IsNullOrEmpty(doc.PathName) ? null : doc.PathName,
                ["is_saved"] = !string.IsNullOrEmpty(doc.PathName),
            });
        });
        return TaskResult(result);
    }
}

public sealed class GetHostContextHandler : ToolHandlerBase
{
    public override McpToolDescriptor Descriptor => Desc(
        "aec.get_host_context",
        "Returns host app, document, units, active view, selection mode, and capabilities.",
        Schema.Object(),
        requiresRevit: true);

    public override Task<McpToolResult> InvokeAsync(JObject arguments, HostContext context)
    {
        var result = context.Bridge.RunOnRevitThread(app =>
        {
            var uidoc = app.ActiveUIDocument;
            var doc = uidoc?.Document;
            if (doc == null)
                return McpToolResult.Failure("No document open", "no_document");

            var view = doc.ActiveView;
            var selIds = uidoc.Selection.GetElementIds();
            return McpToolResult.Success(new JObject
            {
                ["host"] = "revit",
                ["application"] = app.Application.VersionName,
                ["document"] = new JObject
                {
                    ["title"] = doc.Title,
                    ["path"] = string.IsNullOrEmpty(doc.PathName) ? null : doc.PathName,
                    ["is_workshared"] = doc.IsWorkshared,
                    ["is_family"] = doc.IsFamilyDocument,
                },
                ["active_view"] = new JObject
                {
                    ["id"] = view?.Id.Value,
                    ["name"] = view?.Name,
                    ["type"] = view?.ViewType.ToString(),
                },
                ["selection_count"] = selIds.Count,
                ["units"] = doc.GetUnits().GetFormatOptions(SpecTypeId.Length)?.GetUnitTypeId()?.TypeId,
            });
        });
        return TaskResult(result);
    }
}

public sealed class GetActiveViewHandler : ToolHandlerBase
{
    public override McpToolDescriptor Descriptor => Desc(
        "aec.get_active_view",
        "Returns the active view id, name, type, level, template, and section box state.",
        Schema.Object(),
        requiresRevit: true);

    public override Task<McpToolResult> InvokeAsync(JObject arguments, HostContext context)
    {
        var result = context.Bridge.RunOnRevitThread(app =>
        {
            var doc = app.ActiveUIDocument?.Document;
            if (doc == null)
                return McpToolResult.Failure("No document open", "no_document");

            var view = doc.ActiveView;
            if (view == null)
                return McpToolResult.Failure("No active view", "no_active_view");

            var levelName = view.GenLevel?.Name;
            var template = view.ViewTemplateId != ElementId.InvalidElementId
                ? doc.GetElement(view.ViewTemplateId)?.Name
                : null;

            JObject sectionBox = null;
            if (view is View3D view3d && view3d.IsSectionBoxActive)
            {
                var box = view3d.GetSectionBox();
                sectionBox = McpHandlerHelpers.BboxToJson(box);
            }

            return McpToolResult.Success(new JObject
            {
                ["view_id"] = view.Id.Value,
                ["name"] = view.Name,
                ["view_type"] = view.ViewType.ToString(),
                ["level"] = levelName,
                ["view_template"] = template,
                ["is_template"] = view.IsTemplate,
                ["can_be_printed"] = view.CanBePrinted,
                ["section_box"] = sectionBox,
                ["image_resource"] = "aec://view/active/image",
            });
        });
        return TaskResult(result);
    }
}

public sealed class ListCapabilitiesHandler : ToolHandlerBase
{
    public override McpToolDescriptor Descriptor => Desc(
        "aec.list_capabilities",
        "Lists registered tools, resources, prompts, and risk classes exposed by this server.",
        Schema.Object());

    public override Task<McpToolResult> InvokeAsync(JObject arguments, HostContext context)
    {
        return TaskResult(McpToolResult.Success(new JObject
        {
            ["hosts"] = new JArray("revit"),
            ["supports_resources"] = true,
            ["supports_prompts"] = true,
            ["supports_change_sets"] = true,
            ["supports_dynamic_tools"] = true,
            ["tool_count"] = context?.ToolCount ?? 0,
            ["tools"] = new JArray(context?.ToolNames ?? []),
            ["catalog_entries"] = context?.Catalog?.Entries.Count ?? 0,
        }));
    }
}

public sealed class GetSelectionHandler : ToolHandlerBase
{
    public override McpToolDescriptor Descriptor => Desc(
        "aec.get_selection",
        "Returns the current Revit selection with stable element references.",
        Schema.Object(),
        requiresRevit: true);

    public override Task<McpToolResult> InvokeAsync(JObject arguments, HostContext context)
    {
        var result = context.Bridge.RunOnRevitThread(app =>
        {
            var uidoc = app.ActiveUIDocument;
            var doc = uidoc?.Document;
            if (doc == null)
                return McpToolResult.Failure("No document open", "no_document");

            var items = uidoc.Selection.GetElementIds()
                .Select(id =>
                {
                    var e = doc.GetElement(id);
                    return new JObject
                    {
                        ["element_id"] = id.Value,
                        ["unique_id"] = e?.UniqueId,
                        ["name"] = e?.Name,
                        ["category"] = e?.Category?.Name,
                        ["class"] = e?.GetType().Name,
                    };
                })
                .ToList();

            return McpToolResult.Success(new JObject { ["selection"] = new JArray(items) });
        });
        return TaskResult(result);
    }
}

public sealed class ReportStatusHandler : ToolHandlerBase
{
    public override McpToolDescriptor Descriptor => Desc(
        "aec.report_status",
        "Summarizes warnings, failures, blocked operations, and recent agent tool activity.",
        Schema.Object(("include_audit", Schema.Integer("Include last N audit entries"))));

    public override Task<McpToolResult> InvokeAsync(JObject arguments, HostContext context)
    {
        var includeAudit = arguments?["include_audit"]?.Value<int?>() ?? 20;
        var result = context.Bridge.RunOnRevitThread(app =>
        {
            var doc = app.ActiveUIDocument?.Document;
            var warnings = new JArray();
            if (doc != null)
            {
                foreach (var f in doc.GetWarnings())
                {
                    warnings.Add(new JObject
                    {
                        ["description"] = f.GetDescriptionText(),
                        ["severity"] = f.GetSeverity().ToString(),
                    });
                }
            }

            return McpToolResult.Success(new JObject
            {
                ["document_open"] = doc != null,
                ["warning_count"] = warnings.Count,
                ["warnings"] = warnings,
                ["audit"] = context.Audit.GetEntries(includeAudit),
            });
        });
        return TaskResult(result);
    }
}

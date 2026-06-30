using Ara3D.Bowerbird.RevitSamples.AecAgent;
using Autodesk.Revit.DB;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Ara3D.Bowerbird.RevitSamples;

internal static class ChangeSetApplier
{
    public static (bool Valid, JArray Issues) Validate(Document doc, ChangeSet set)
    {
        var issues = new JArray();
        if (set.Operations.Count == 0)
            issues.Add(Issue("empty", "Change set has no operations."));

        if (doc == null)
        {
            issues.Add(Issue("no_document", "No document open."));
            return (false, issues);
        }

        var view = doc.ActiveView;
        if (view == null)
            issues.Add(Issue("no_active_view", "No active view."));

        foreach (var op in set.Operations)
        {
            switch (op.Kind)
            {
                case ChangeOpKind.SetParameter:
                    ValidateSetParameter(doc, op, issues);
                    break;
                case ChangeOpKind.SetViewOverride:
                    ValidateSetViewOverride(doc, op, issues);
                    break;
                case ChangeOpKind.DeleteElements:
                    issues.Add(Issue("unsupported", "DeleteElements is not yet supported by the MCP applier."));
                    break;
                case ChangeOpKind.CreateElements:
                    issues.Add(Issue("unsupported", "CreateElements is not yet supported by the MCP applier."));
                    break;
            }
        }

        return (issues.Count == 0, issues);
    }

    public static JObject BuildPreview(Document doc, ChangeSet set)
    {
        var affected = new JArray();
        var parameterNames = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        var warnings = new JArray();

        foreach (var op in set.Operations)
        {
            var elementId = op.Payload["element_id"]?.Value<long?>();
            if (elementId == null) continue;

            var e = doc?.GetElement(new ElementId(elementId.Value));
            if (e == null)
            {
                warnings.Add(new JObject
                {
                    ["code"] = "element_not_found",
                    ["element_id"] = elementId,
                    ["operation"] = op.Kind.ToString(),
                });
                continue;
            }

            affected.Add(new JObject
            {
                ["element_id"] = e.Id.Value,
                ["unique_id"] = e.UniqueId,
                ["name"] = e.Name,
                ["category"] = e.Category?.Name,
                ["operation"] = op.Kind.ToString(),
            });

            if (op.Kind == ChangeOpKind.SetParameter)
            {
                var name = op.Payload["parameter_name"]?.Value<string>();
                if (!string.IsNullOrEmpty(name))
                    parameterNames.Add(name);
            }

            if (op.Kind == ChangeOpKind.SetViewOverride)
            {
                var paramName = op.Payload["parameter_name"]?.Value<string>();
                if (!string.IsNullOrEmpty(paramName))
                    parameterNames.Add(paramName);
            }
        }

        var summary = set.Operations
            .GroupBy(o => o.Kind)
            .Select(g => new JObject
            {
                ["kind"] = g.Key.ToString(),
                ["count"] = g.Count(),
            })
            .ToList();

        var approvalMessage = set.RequiresApproval
            ? $"Approve change set '{set.Intent}' ({set.Operations.Count} operations, risk {set.RiskLevel})."
            : null;

        return new JObject
        {
            ["change_set_id"] = set.Id,
            ["intent"] = set.Intent,
            ["summary"] = new JArray(summary),
            ["affected_elements"] = affected,
            ["affected_element_count"] = affected.Count,
            ["parameter_names"] = new JArray(parameterNames.OrderBy(n => n)),
            ["warnings"] = warnings,
            ["risk_level"] = set.RiskLevel,
            ["requires_approval"] = set.RequiresApproval,
            ["approval_message"] = approvalMessage,
        };
    }

    public static (int Applied, List<long> ChangedElementIds) Apply(Document doc, ChangeSet set)
    {
        var changed = new List<long>();
        var applied = 0;

        var viewOverrideOps = set.Operations.Where(o => o.Kind == ChangeOpKind.SetViewOverride).ToList();
        var otherOps = set.Operations.Where(o => o.Kind != ChangeOpKind.SetViewOverride).ToList();

        foreach (var op in otherOps)
        {
            if (ApplyOperation(doc, op))
            {
                applied++;
                var id = op.Payload["element_id"]?.Value<long?>();
                if (id != null)
                    changed.Add(id.Value);
            }
        }

        if (viewOverrideOps.Count > 0)
        {
            var grouped = ApplyGroupedViewOverrides(doc, viewOverrideOps);
            applied += grouped.Applied;
            changed.AddRange(grouped.ChangedElementIds);
        }

        return (applied, changed.Distinct().ToList());
    }

    static (int Applied, List<long> ChangedElementIds) ApplyGroupedViewOverrides(Document doc, List<ChangeOp> ops)
    {
        var paramName = ops.Select(o => o.Payload["parameter_name"]?.Value<string>())
            .FirstOrDefault(n => !string.IsNullOrWhiteSpace(n));

        var elementIds = ops
            .Select(o => o.Payload["element_id"]?.Value<long?>())
            .Where(id => id != null)
            .Select(id => new ElementId(id!.Value))
            .ToList();

        var groups = new Dictionary<string, List<ElementId>>();
        foreach (var id in elementIds)
        {
            var e = doc.GetElement(id);
            if (e == null) continue;

            var key = "(fixed)";
            if (!string.IsNullOrWhiteSpace(paramName))
            {
                var p = e.LookupParameter(paramName);
                key = p?.AsValueString() ?? p?.AsString() ?? "(empty)";
            }

            if (!groups.ContainsKey(key))
                groups[key] = [];
            groups[key].Add(id);
        }

        var palette = BuildPalette(groups.Count);
        var applied = 0;
        var changed = new List<long>();
        var i = 0;
        foreach (var (_, ids) in groups)
        {
            var color = palette[i++ % palette.Count];
            var ogs = new OverrideGraphicSettings()
                .SetProjectionLineColor(color)
                .SetSurfaceForegroundPatternColor(color);
            foreach (var id in ids)
            {
                doc.ActiveView.SetElementOverrides(id, ogs);
                applied++;
                changed.Add(id.Value);
            }
        }

        return (applied, changed);
    }

    static bool ApplyOperation(Document doc, ChangeOp op)
    {
        switch (op.Kind)
        {
            case ChangeOpKind.SetParameter:
            {
                var elementId = op.Payload["element_id"]?.Value<long?>();
                var name = op.Payload["parameter_name"]?.Value<string>();
                var value = op.Payload["value"];
                if (elementId == null || string.IsNullOrEmpty(name)) return false;
                var e = doc.GetElement(new ElementId(elementId.Value));
                var p = e?.LookupParameter(name);
                return p != null && McpHandlerHelpers.TrySetParameter(p, value, out _);
            }
            case ChangeOpKind.SetViewOverride:
                return true;
            default:
                return false;
        }
    }

    static void ValidateSetParameter(Document doc, ChangeOp op, JArray issues)
    {
        var elementId = op.Payload["element_id"]?.Value<long?>();
        var name = op.Payload["parameter_name"]?.Value<string>();
        if (elementId == null)
            issues.Add(Issue("missing_element_id", "SetParameter requires element_id."));
        if (string.IsNullOrEmpty(name))
            issues.Add(Issue("missing_parameter_name", "SetParameter requires parameter_name."));
        if (elementId == null || string.IsNullOrEmpty(name)) return;

        var e = doc.GetElement(new ElementId(elementId.Value));
        if (e == null)
        {
            issues.Add(Issue("element_not_found", $"Element {elementId} not found."));
            return;
        }

        var p = e.LookupParameter(name);
        if (p == null)
            issues.Add(Issue("parameter_not_found", $"Parameter '{name}' not found on element {elementId}."));
        else if (p.IsReadOnly)
            issues.Add(Issue("parameter_read_only", $"Parameter '{name}' is read-only on element {elementId}."));
    }

    static void ValidateSetViewOverride(Document doc, ChangeOp op, JArray issues)
    {
        var elementId = op.Payload["element_id"]?.Value<long?>();
        if (elementId == null)
        {
            issues.Add(Issue("missing_element_id", "SetViewOverride requires element_id."));
            return;
        }

        var e = doc.GetElement(new ElementId(elementId.Value));
        if (e == null)
            issues.Add(Issue("element_not_found", $"Element {elementId} not found."));
        else if (doc.ActiveView != null && !doc.ActiveView.CanBePrinted)
            issues.Add(Issue("view_override", "Active view may not support graphic overrides."));
    }

    static JObject Issue(string code, string message)
        => new() { ["code"] = code, ["message"] = message };

    static List<Color> BuildPalette(int count)
    {
        return
        [
            new Color(220, 80, 80),
            new Color(80, 140, 220),
            new Color(90, 180, 90),
            new Color(220, 180, 60),
            new Color(170, 90, 200),
            new Color(80, 180, 180),
        ];
    }
}

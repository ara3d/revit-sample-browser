using Ara3D.Bowerbird.RevitSamples.AecAgent;
using Ara3D.Utils;
using Autodesk.Revit.DB;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Ara3D.Bowerbird.RevitSamples;

public sealed class CaptureViewImageHandler : ToolHandlerBase
{
    public override McpToolDescriptor Descriptor => Desc(
        "aec.capture_view_image",
        "Exports the active view to a PNG and returns the output path.",
        Schema.Object(("output_path", Schema.String("Optional output PNG path"))),
        ToolRiskClass.ViewOverride,
        requiresRevit: true);

    public override Task<McpToolResult> InvokeAsync(JObject arguments, HostContext context)
    {
        var outputPath = arguments?["output_path"]?.Value<string>();
        var result = context.Bridge.RunOnRevitThread(app =>
        {
            var doc = app.ActiveUIDocument?.Document;
            if (doc == null)
                return McpToolResult.Failure("No document open", "no_document");

            var path = string.IsNullOrWhiteSpace(outputPath)
                ? Config.OutputDir.RelativeFile($"{doc.Title}-view.png".ToValidFileName())
                : new Ara3D.Utils.FilePath(outputPath);

            Config.OutputDir.Create();
            CommandSaveCurrentViewAsPng.ExportCurrentViewToPng(doc, path);
            McpResourceProvider.SetLatestViewImage(path.ToString());

            return McpToolResult.Success(new JObject
            {
                ["path"] = path.ToString(),
                ["view_name"] = doc.ActiveView?.Name,
                ["resource_uri"] = "aec://view/active/image",
            });
        });
        return TaskResult(result);
    }
}

public sealed class ColorByParameterHandler : ToolHandlerBase
{
    public override McpToolDescriptor Descriptor => Desc(
        "aec.color_by_parameter",
        "Applies view overrides to elements grouped by a parameter value.",
        Schema.Object(
            ("element_ids", new JObject { ["type"] = "array", ["items"] = new JObject { ["type"] = "integer" } }),
            ("parameter_name", Schema.String("Parameter name to group/color by")),
            ("change_set_id", Schema.String("Approved change set id for write operations"))),
        ToolRiskClass.ViewOverride,
        requiresRevit: true);

    public override Task<McpToolResult> InvokeAsync(JObject arguments, HostContext context)
    {
        var ids = (arguments?["element_ids"] as JArray ?? [])
            .Select(t => new ElementId(t.Value<long>()))
            .ToList();
        var parameterName = arguments?["parameter_name"]?.Value<string>();
        var changeSetId = arguments?["change_set_id"]?.Value<string>();

        if (ids.Count == 0)
            throw new McpProtocolException(-32602, "element_ids is required");
        if (string.IsNullOrWhiteSpace(parameterName))
            throw new McpProtocolException(-32602, "parameter_name is required");

        if (string.IsNullOrWhiteSpace(changeSetId))
        {
            var preview = context.ChangeSetSession.Create(
                "Color elements by parameter",
                ids.Select(id => new ChangeOp
                {
                    Kind = ChangeOpKind.SetViewOverride,
                    Payload = new JObject
                    {
                        ["element_id"] = id.Value,
                        ["parameter_name"] = parameterName,
                    },
                }));

            return TaskResult(McpToolResult.Success(new JObject
            {
                ["preview_only"] = true,
                ["change_set_id"] = preview.Id,
                ["element_count"] = ids.Count,
                ["message"] = "Call aec.apply_changes with approval to execute view overrides.",
            }, new JObject { ["risk"] = preview.RiskLevel }));
        }

        if (!context.ChangeSetSession.TryGet(changeSetId, out var set))
            return TaskResult(McpToolResult.Failure("Unknown change set.", "unknown_change_set"));

        var result = context.Bridge.RunOnRevitThread(app =>
        {
            var doc = app.ActiveUIDocument?.Document;
            if (doc == null)
                return McpToolResult.Failure("No document open", "no_document");

            var groups = new Dictionary<string, List<ElementId>>();
            foreach (var id in ids)
            {
                var e = doc.GetElement(id);
                if (e == null) continue;
                var p = e.LookupParameter(parameterName);
                var key = p?.AsValueString() ?? p?.AsString() ?? "(empty)";
                if (!groups.ContainsKey(key)) groups[key] = [];
                groups[key].Add(id);
            }

            var palette = BuildPalette(groups.Count);
            var applied = 0;
            using var tx = new Transaction(doc, "MCP Color By Parameter");
            tx.Start();
            var i = 0;
            foreach (var (value, elementIds) in groups)
            {
                var color = palette[i++ % palette.Count];
                var ogs = new OverrideGraphicSettings().SetProjectionLineColor(color).SetSurfaceForegroundPatternColor(color);
                foreach (var id in elementIds)
                {
                    doc.ActiveView.SetElementOverrides(id, ogs);
                    applied++;
                }
            }
            tx.Commit();
            context.ChangeSetSession.MarkApplied(set);

            return McpToolResult.Success(new JObject
            {
                ["applied"] = applied,
                ["groups"] = new JArray(groups.Select(g => new JObject
                {
                    ["parameter_value"] = g.Key,
                    ["count"] = g.Value.Count,
                })),
            });
        });
        return TaskResult(result);
    }

    static List<Color> BuildPalette(int count)
    {
        var colors = new List<Color>
        {
            new(220, 80, 80),
            new(80, 140, 220),
            new(90, 180, 90),
            new(220, 180, 60),
            new(170, 90, 200),
            new(80, 180, 180),
        };
        return colors;
    }
}

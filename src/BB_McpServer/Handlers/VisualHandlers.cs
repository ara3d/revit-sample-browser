using Ara3D.Bowerbird.RevitSamples.AecAgent;
using Ara3D.Utils;
using Autodesk.Revit.DB;
using Newtonsoft.Json.Linq;
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
        "Creates a change set to color elements in the active view grouped by a parameter value.",
        Schema.Object(
            ("element_ids", new JObject { ["type"] = "array", ["items"] = new JObject { ["type"] = "integer" } }),
            ("parameter_name", Schema.String("Parameter name to group/color by"))),
        ToolRiskClass.ViewOverride,
        requiresRevit: true);

    public override Task<McpToolResult> InvokeAsync(JObject arguments, HostContext context)
    {
        var ids = (arguments?["element_ids"] as JArray ?? [])
            .Select(t => new ElementId(t.Value<long>()))
            .ToList();
        var parameterName = arguments?["parameter_name"]?.Value<string>();

        if (ids.Count == 0)
            throw new McpProtocolException(-32602, "element_ids is required");
        if (string.IsNullOrWhiteSpace(parameterName))
            throw new McpProtocolException(-32602, "parameter_name is required");

        var preview = context.ChangeSetSession.Create(
            $"Color elements by {parameterName}",
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
            ["parameter_name"] = parameterName,
            ["risk_level"] = preview.RiskLevel,
            ["requires_approval"] = preview.RequiresApproval,
            ["audit_resource"] = $"audit://changesets/{preview.Id}",
            ["next_steps"] = new JArray(
                "aec.validate_change_set",
                "aec.preview_changes",
                "aec.apply_changes",
                "aec.capture_view_image"),
            ["message"] = "Review the change set, then call aec.apply_changes to execute view overrides.",
        }));
    }
}

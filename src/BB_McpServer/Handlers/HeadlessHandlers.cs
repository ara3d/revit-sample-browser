using Ara3D.Bowerbird.RevitSamples.AecAgent;
using Ara3D.Utils;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Newtonsoft.Json.Linq;
using System.Linq;
using System.Threading.Tasks;

namespace Ara3D.Bowerbird.RevitSamples;

public sealed class ExportAllJsonHandler : ToolHandlerBase
{
    public override McpToolDescriptor Descriptor => Desc(
        "revit_export_all_json",
        "Exports all document elements to a JSON file.",
        Schema.Object(("output_path", Schema.String("Optional output JSON path"))),
        tier: "bowerbird",
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
                ? doc.CurrentFileName().ChangeDirectoryAndExt(Config.OutputDir, ".json").ToString()
                : outputPath;

            doc.SerializeAllElementsAsJson(path);
            return McpToolResult.Success(new JObject
            {
                ["path"] = path,
                ["element_count"] = doc.GetElements().Count,
            });
        });
        return TaskResult(result);
    }
}

public sealed class ListRoomsHandler : ToolHandlerBase
{
    public override McpToolDescriptor Descriptor => Desc(
        "revit_list_rooms",
        "Returns structured room data for all rooms in the active document.",
        Schema.Object(),
        tier: "bowerbird",
        requiresRevit: true);

    public override Task<McpToolResult> InvokeAsync(JObject arguments, HostContext context)
    {
        var result = context.Bridge.RunOnRevitThread(app =>
        {
            var doc = app.ActiveUIDocument?.Document;
            if (doc == null)
                return McpToolResult.Failure("No document open", "no_document");

            var rooms = doc.GetRooms()
                .Select(r =>
                {
                    var data = r.GetRoomData();
                    return new JObject
                    {
                        ["id"] = data.Id,
                        ["name"] = data.Name,
                        ["department"] = data.Department,
                        ["area"] = data.Area,
                        ["level"] = data.LevelName,
                        ["perimeter"] = data.Perimeter,
                    };
                })
                .ToList();

            return McpToolResult.Success(new JObject
            {
                ["count"] = rooms.Count,
                ["rooms"] = new JArray(rooms),
            });
        });
        return TaskResult(result);
    }
}

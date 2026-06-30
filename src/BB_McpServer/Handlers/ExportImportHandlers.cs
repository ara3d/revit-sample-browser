using Ara3D.Bowerbird.RevitSamples.AecAgent;
using Ara3D.Utils;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Ara3D.Bowerbird.RevitSamples;

public sealed class ExportDwgHandler : ToolHandlerBase
{
    public override McpToolDescriptor Descriptor => Desc(
        "aec.export_dwg",
        "Export active or named view to DWG file.",
        Schema.Object(
            ("view", Schema.String("View name or id; defaults to active view")),
            ("output_folder", Schema.String("Output folder")),
            ("file_name", Schema.String("Output file name without extension"))),
        requiresRevit: true);

    public override Task<McpToolResult> InvokeAsync(JObject arguments, HostContext context)
    {
        var viewRef = arguments["view"]?.Value<string>();
        var outputFolder = arguments["output_folder"]?.Value<string>();
        var fileName = arguments["file_name"]?.Value<string>();

        var result = context.Bridge.RunOnRevitThread(app =>
        {
            var doc = McpHandlerHelpers.GetDoc(app);
            if (doc == null) return McpHandlerHelpers.NoDocument();
            var view = McpHandlerHelpers.FindViewByIdOrName(doc, viewRef, app);
            if (view == null) return McpToolResult.Failure("View not found.", "view_not_found");

            var folder = outputFolder ?? Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "BowerbirdExports");
            Directory.CreateDirectory(folder);
            var name = McpHandlerHelpers.SanitizeFileName(fileName ?? view.Name);
            var options = new DWGExportOptions();
            var ok = doc.Export(folder, name, new List<ElementId> { view.Id }, options);
            var path = Path.Combine(folder, name + ".dwg");
            if (!ok) return McpToolResult.Failure("DWG export failed.", "export_failed");
            McpExportTracker.Record(path, new JObject { ["format"] = "dwg", ["view"] = view.Name });
            return McpToolResult.Success(new JObject { ["path"] = path, ["view"] = view.Name });
        });
        return TaskResult(result);
    }
}

public sealed class ExportIfcHandler : ToolHandlerBase
{
    public override McpToolDescriptor Descriptor => Desc(
        "aec.export_ifc",
        "Export document to IFC.",
        Schema.Object(
            ("output_folder", Schema.String("Output folder")),
            ("file_name", Schema.String("Output file name without extension"))),
        requiresRevit: true);

    public override Task<McpToolResult> InvokeAsync(JObject arguments, HostContext context)
    {
        var outputFolder = arguments["output_folder"]?.Value<string>();
        var fileName = arguments["file_name"]?.Value<string>();

        var result = context.Bridge.RunOnRevitThread(app =>
        {
            var doc = McpHandlerHelpers.GetDoc(app);
            if (doc == null) return McpHandlerHelpers.NoDocument();
            var folder = outputFolder ?? Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "BowerbirdExports");
            Directory.CreateDirectory(folder);
            var name = McpHandlerHelpers.SanitizeFileName(fileName ?? doc.Title);
            var options = new IFCExportOptions();
            var ok = doc.Export(folder, name, options);
            var path = Path.Combine(folder, name + ".ifc");
            if (!ok) return McpToolResult.Failure("IFC export failed.", "export_failed");
            McpExportTracker.Record(path, new JObject { ["format"] = "ifc" });
            return McpToolResult.Success(new JObject { ["path"] = path });
        });
        return TaskResult(result);
    }
}

public sealed class ExportPdfHandler : ToolHandlerBase
{
    public override McpToolDescriptor Descriptor => Desc(
        "aec.export_pdf",
        "Export views or sheets to PDF.",
        Schema.Object(
            ("views", Schema.StringArray("View or sheet names")),
            ("output_folder", Schema.String("Output folder"))),
        requiresRevit: true);

    public override Task<McpToolResult> InvokeAsync(JObject arguments, HostContext context)
    {
        var viewNames = (arguments["views"] as JArray ?? []).Select(t => t.Value<string>()).Where(s => !string.IsNullOrWhiteSpace(s)).ToList();
        var outputFolder = arguments["output_folder"]?.Value<string>();

        var result = context.Bridge.RunOnRevitThread(app =>
        {
            var doc = McpHandlerHelpers.GetDoc(app);
            if (doc == null) return McpHandlerHelpers.NoDocument();
            var folder = outputFolder ?? Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "BowerbirdExports");
            Directory.CreateDirectory(folder);

            var viewIds = new List<ElementId>();
            if (viewNames.Count == 0)
                viewIds.Add(doc.ActiveView.Id);
            else
            {
                foreach (var name in viewNames)
                {
                    var view = McpHandlerHelpers.FindViewByName(doc, name)
                        ?? McpHandlerHelpers.FindSheetByName(doc, name) as View;
                    if (view != null) viewIds.Add(view.Id);
                }
            }
            if (viewIds.Count == 0) return McpToolResult.Failure("No views found to export.", "view_not_found");

            var fileName = McpHandlerHelpers.SanitizeFileName(doc.Title) + ".pdf";
            var options = new PDFExportOptions { Combine = true, FileName = fileName };
            var ok = doc.Export(folder, viewIds, options);
            if (!ok) return McpToolResult.Failure("PDF export failed.", "export_failed");
            var path = Path.Combine(folder, options.FileName ?? $"{McpHandlerHelpers.SanitizeFileName(doc.Title)}.pdf");
            McpExportTracker.Record(path, new JObject { ["format"] = "pdf", ["view_count"] = viewIds.Count });
            return McpToolResult.Success(new JObject { ["path"] = path, ["view_count"] = viewIds.Count });
        });
        return TaskResult(result);
    }
}

public sealed class ExportGbxmlHandler : ToolHandlerBase
{
    public override McpToolDescriptor Descriptor => Desc(
        "aec.export_gbxml",
        "Export document to gbXML for energy analysis.",
        Schema.Object(
            ("output_folder", Schema.String("Output folder")),
            ("file_name", Schema.String("Output file name without extension"))),
        requiresRevit: true);

    public override Task<McpToolResult> InvokeAsync(JObject arguments, HostContext context)
    {
        var outputFolder = arguments["output_folder"]?.Value<string>();
        var fileName = arguments["file_name"]?.Value<string>();

        var result = context.Bridge.RunOnRevitThread(app =>
        {
            var doc = McpHandlerHelpers.GetDoc(app);
            if (doc == null) return McpHandlerHelpers.NoDocument();
            var folder = outputFolder ?? Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "BowerbirdExports");
            Directory.CreateDirectory(folder);
            var name = McpHandlerHelpers.SanitizeFileName(fileName ?? doc.Title);
            var options = new GBXMLExportOptions();
            var ok = doc.Export(folder, name, options);
            var path = Path.Combine(folder, name + ".xml");
            if (!ok) return McpToolResult.Failure("gbXML export failed.", "export_failed");
            McpExportTracker.Record(path, new JObject { ["format"] = "gbxml" });
            return McpToolResult.Success(new JObject { ["path"] = path });
        });
        return TaskResult(result);
    }
}

public sealed class Export3dMeshJsonHandler : ToolHandlerBase
{
    public override McpToolDescriptor Descriptor => Desc(
        "aec.export_3d_mesh_json",
        "Export 3D mesh geometry from a 3D view as JSON.",
        Schema.Object(
            ("view", Schema.String("3D view name; defaults to active view")),
            ("output_path", Schema.String("Optional output JSON path"))),
        tier: "bowerbird",
        requiresRevit: true);

    public override Task<McpToolResult> InvokeAsync(JObject arguments, HostContext context)
    {
        var viewRef = arguments["view"]?.Value<string>();
        var outputPath = arguments["output_path"]?.Value<string>();

        var result = context.Bridge.RunOnRevitThread(app =>
        {
            var doc = McpHandlerHelpers.GetDoc(app);
            if (doc == null) return McpHandlerHelpers.NoDocument();
            var view = McpHandlerHelpers.FindViewByIdOrName(doc, viewRef, app) as View3D;
            if (view == null || view.ViewType != ViewType.ThreeD)
                return McpToolResult.Failure("A 3D view is required.", "invalid_view");

            var meshItems = new JArray();
            var collector = new FilteredElementCollector(doc, view.Id).WhereElementIsNotElementType();
            foreach (var element in collector.Take(200))
            {
                var geom = element.get_Geometry(new Options { View = view });
                if (geom == null) continue;
                var meshes = new JArray();
                CollectMeshData(geom, meshes);
                if (meshes.Count > 0)
                {
                    meshItems.Add(new JObject
                    {
                        ["element_id"] = element.Id.Value,
                        ["name"] = element.Name,
                        ["meshes"] = meshes,
                    });
                }
            }

            var path = McpHandlerHelpers.EnsureOutputPath(outputPath, McpHandlerHelpers.SanitizeFileName(view.Name + "_mesh"), ".json");
            var payload = new JObject { ["view"] = view.Name, ["elements"] = meshItems };
            File.WriteAllText(path, payload.ToString());
            McpExportTracker.Record(path, new JObject { ["format"] = "mesh_json", ["view"] = view.Name });
            return McpToolResult.Success(new JObject { ["path"] = path, ["element_count"] = meshItems.Count });
        });
        return TaskResult(result);
    }

    public static void CollectMeshData(GeometryElement geom, JArray meshes)
    {
        foreach (var obj in geom)
        {
            if (obj is Solid solid)
            {
                foreach (Face face in solid.Faces)
                {
                    var mesh = face.Triangulate();
                    if (mesh == null) continue;
                    var verts = new JArray(mesh.Vertices.Select(v => new JArray(v.X, v.Y, v.Z)));
                    meshes.Add(new JObject { ["vertices"] = verts, ["triangle_count"] = mesh.NumTriangles });
                }
            }
            else if (obj is GeometryInstance inst)
            {
                CollectMeshData(inst.GetInstanceGeometry(), meshes);
            }
        }
    }
}

public sealed class ExportColladaHandler : ToolHandlerBase
{
    public override McpToolDescriptor Descriptor => Desc(
        "aec.export_collada",
        "Export visible 3D geometry to Collada DAE (simplified JSON mesh sidecar when Collada writer unavailable).",
        Schema.Object(
            ("view", Schema.String("3D view name")),
            ("output_path", Schema.String("Output path (.dae or .json)"))),
        tier: "bowerbird",
        requiresRevit: true);

    public override Task<McpToolResult> InvokeAsync(JObject arguments, HostContext context)
    {
        var viewRef = arguments["view"]?.Value<string>();
        var outputPath = arguments["output_path"]?.Value<string>();

        var result = context.Bridge.RunOnRevitThread(app =>
        {
            var doc = McpHandlerHelpers.GetDoc(app);
            if (doc == null) return McpHandlerHelpers.NoDocument();
            var view = McpHandlerHelpers.FindViewByIdOrName(doc, viewRef, app) as View3D;
            if (view == null) return McpToolResult.Failure("3D view required.", "invalid_view");

            var path = McpHandlerHelpers.EnsureOutputPath(outputPath, McpHandlerHelpers.SanitizeFileName(view.Name + "_collada"), ".json");
            var meshItems = new JArray();
            var collector = new FilteredElementCollector(doc, view.Id).WhereElementIsNotElementType();
            foreach (var element in collector.Take(200))
            {
                var geom = element.get_Geometry(new Options { View = view });
                if (geom == null) continue;
                var meshes = new JArray();
                Export3dMeshJsonHandler.CollectMeshData(geom, meshes);
                if (meshes.Count > 0)
                {
                    meshItems.Add(new JObject
                    {
                        ["element_id"] = element.Id.Value,
                        ["name"] = element.Name,
                        ["meshes"] = meshes,
                    });
                }
            }

            var payload = new JObject { ["view"] = view.Name, ["elements"] = meshItems };
            File.WriteAllText(path, payload.ToString());
            McpExportTracker.Record(path, new JObject { ["format"] = "collada_json", ["view"] = view.Name });
            return McpToolResult.Success(new JObject
            {
                ["path"] = path,
                ["note"] = "Collada export emitted as mesh JSON compatible payload.",
            });
        });
        return TaskResult(result);
    }
}

public sealed class ExportAllElementsJsonHandler : ToolHandlerBase
{
    public override McpToolDescriptor Descriptor => Desc(
        "aec.export_all_elements_json",
        "Export full document element data as JSON.",
        Schema.Object(("output_path", Schema.String("Optional output JSON path"))),
        tier: "bowerbird",
        requiresRevit: true);

    public override Task<McpToolResult> InvokeAsync(JObject arguments, HostContext context)
    {
        var outputPath = arguments["output_path"]?.Value<string>();
        var result = context.Bridge.RunOnRevitThread(app =>
        {
            var doc = McpHandlerHelpers.GetDoc(app);
            if (doc == null) return McpHandlerHelpers.NoDocument();
            var path = string.IsNullOrWhiteSpace(outputPath)
                ? doc.CurrentFileName().ChangeDirectoryAndExt(Config.OutputDir, ".json").ToString()
                : outputPath;
            doc.SerializeAllElementsAsJson(path);
            McpExportTracker.Record(path, new JObject { ["format"] = "elements_json" });
            return McpToolResult.Success(new JObject
            {
                ["path"] = path,
                ["element_count"] = doc.GetElements().Count,
            });
        });
        return TaskResult(result);
    }
}

public sealed class ExportRoomsJsonHandler : ToolHandlerBase
{
    public override McpToolDescriptor Descriptor => Desc(
        "aec.export_rooms_json",
        "Export room data as structured JSON.",
        Schema.Object(("output_path", Schema.String("Optional output JSON path"))),
        tier: "bowerbird",
        requiresRevit: true);

    public override Task<McpToolResult> InvokeAsync(JObject arguments, HostContext context)
    {
        var outputPath = arguments["output_path"]?.Value<string>();
        var result = context.Bridge.RunOnRevitThread(app =>
        {
            var doc = McpHandlerHelpers.GetDoc(app);
            if (doc == null) return McpHandlerHelpers.NoDocument();

            var rooms = doc.GetRooms().Select(r =>
            {
                var data = r.GetRoomData();
                return new JObject
                {
                    ["id"] = data.Id,
                    ["unique_id"] = r.UniqueId,
                    ["name"] = data.Name,
                    ["number"] = r.Number,
                    ["department"] = data.Department,
                    ["area"] = data.Area,
                    ["level"] = data.LevelName,
                    ["perimeter"] = data.Perimeter,
                    ["unplaced"] = r.Area <= 0 || r.Location == null,
                };
            }).ToList();

            var path = McpHandlerHelpers.EnsureOutputPath(outputPath, "rooms", ".json");
            var payload = new JObject { ["count"] = rooms.Count, ["rooms"] = new JArray(rooms) };
            File.WriteAllText(path, payload.ToString());
            McpExportTracker.Record(path, new JObject { ["format"] = "rooms_json", ["count"] = rooms.Count });
            return McpToolResult.Success(new JObject { ["path"] = path, ["count"] = rooms.Count });
        });
        return TaskResult(result);
    }
}

public sealed class ExportScheduleExcelHandler : ToolHandlerBase
{
    public override McpToolDescriptor Descriptor => Desc(
        "aec.export_schedule_excel",
        "Export one or more schedules to Excel-compatible CSV files.",
        Schema.Object(
            ("schedules", Schema.StringArray("Schedule names; exports all when omitted")),
            ("output_folder", Schema.String("Output folder"))),
        requiresRevit: true);

    public override Task<McpToolResult> InvokeAsync(JObject arguments, HostContext context)
    {
        var scheduleNames = (arguments["schedules"] as JArray ?? []).Select(t => t.Value<string>()).Where(s => !string.IsNullOrWhiteSpace(s)).ToList();
        var outputFolder = arguments["output_folder"]?.Value<string>();

        var result = context.Bridge.RunOnRevitThread(app =>
        {
            var doc = McpHandlerHelpers.GetDoc(app);
            if (doc == null) return McpHandlerHelpers.NoDocument();
            var folder = outputFolder ?? Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "BowerbirdExports");
            Directory.CreateDirectory(folder);

            var schedules = scheduleNames.Count == 0
                ? new FilteredElementCollector(doc).OfClass(typeof(ViewSchedule)).Cast<ViewSchedule>().ToList()
                : scheduleNames.Select(n => McpHandlerHelpers.FindScheduleByName(doc, n)).Where(s => s != null).ToList();

            var exported = new JArray();
            foreach (var schedule in schedules)
            {
                var path = Path.Combine(folder, McpHandlerHelpers.SanitizeFileName(schedule.Name) + ".csv");
                var rows = ScheduleMcpHelpers.ReadScheduleRows(schedule);
                var sb = new System.Text.StringBuilder();
                if (rows.Count > 0 && rows[0] is JObject first)
                {
                    var headers = first.Properties().Select(p => p.Name).ToList();
                    sb.AppendLine(string.Join(",", headers));
                    foreach (JObject row in rows)
                        sb.AppendLine(string.Join(",", headers.Select(h => row[h]?.ToString() ?? "")));
                }
                File.WriteAllText(path, sb.ToString());
                exported.Add(new JObject { ["schedule"] = schedule.Name, ["path"] = path });
            }

            McpExportTracker.Record(folder, new JObject { ["format"] = "schedule_excel_csv", ["count"] = exported.Count });
            return McpToolResult.Success(new JObject { ["output_folder"] = folder, ["exports"] = exported });
        });
        return TaskResult(result);
    }
}

using Ara3D.Bowerbird.RevitSamples.AecAgent;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Architecture;
using Autodesk.Revit.UI;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Ara3D.Bowerbird.RevitSamples;

public sealed class CreateDirectShapeBoxHandler : ToolHandlerBase
{
    public override McpToolDescriptor Descriptor => Desc(
        "aec.create_direct_shape_box",
        "Create a box solid as DirectShape at a point.",
        Schema.ObjectWithRequired(
            [
                ("origin", new JObject { ["type"] = "array", ["items"] = new JObject { ["type"] = "number" }, ["minItems"] = 2, ["description"] = "[x,y,z] origin" }),
                ("width", Schema.Integer("Width in feet")),
                ("depth", Schema.Integer("Depth in feet")),
                ("height", Schema.Integer("Height in feet")),
            ],
            "origin", "width", "depth", "height"),
        ToolRiskClass.Write,
        requiresRevit: true);

    public override Task<McpToolResult> InvokeAsync(JObject arguments, HostContext context)
    {
        var origin = McpHandlerHelpers.ParsePoint(arguments["origin"]);
        var width = arguments["width"]?.Value<double>() ?? 10;
        var depth = arguments["depth"]?.Value<double>() ?? 10;
        var height = arguments["height"]?.Value<double>() ?? 10;

        var result = context.Bridge.RunOnRevitThread(app =>
        {
            var doc = McpHandlerHelpers.GetDoc(app);
            if (doc == null) return McpHandlerHelpers.NoDocument();

            return McpHandlerHelpers.RunTransaction(doc, "MCP Create DirectShape Box", () =>
            {
                var solid = McpHandlerHelpers.CreateBoxSolid(origin, width, depth, height);
                var id = McpHandlerHelpers.CreateDirectShape(doc, solid);
                return McpToolResult.Success(new JObject
                {
                    ["element_id"] = id.Value,
                    ["origin"] = new JArray(origin.X, origin.Y, origin.Z),
                    ["width"] = width,
                    ["depth"] = depth,
                    ["height"] = height,
                });
            });
        });
        return TaskResult(result);
    }
}

public sealed class CreateDirectShapeCylinderHandler : ToolHandlerBase
{
    public override McpToolDescriptor Descriptor => Desc(
        "aec.create_direct_shape_cylinder",
        "Create a cylinder solid as DirectShape.",
        Schema.ObjectWithRequired(
            [
                ("base_center", new JObject { ["type"] = "array", ["items"] = new JObject { ["type"] = "number" }, ["minItems"] = 2 }),
                ("radius", Schema.Integer("Radius in feet")),
                ("height", Schema.Integer("Height in feet")),
            ],
            "base_center", "radius", "height"),
        ToolRiskClass.Write,
        requiresRevit: true);

    public override Task<McpToolResult> InvokeAsync(JObject arguments, HostContext context)
    {
        var baseCenter = McpHandlerHelpers.ParsePoint(arguments["base_center"]);
        var radius = arguments["radius"]?.Value<double>() ?? 5;
        var height = arguments["height"]?.Value<double>() ?? 10;

        var result = context.Bridge.RunOnRevitThread(app =>
        {
            var doc = McpHandlerHelpers.GetDoc(app);
            if (doc == null) return McpHandlerHelpers.NoDocument();

            return McpHandlerHelpers.RunTransaction(doc, "MCP Create DirectShape Cylinder", () =>
            {
                var solid = McpHandlerHelpers.CreateCylinderSolid(baseCenter, radius, height);
                var id = McpHandlerHelpers.CreateDirectShape(doc, solid);
                return McpToolResult.Success(new JObject
                {
                    ["element_id"] = id.Value,
                    ["base_center"] = new JArray(baseCenter.X, baseCenter.Y, baseCenter.Z),
                    ["radius"] = radius,
                    ["height"] = height,
                });
            });
        });
        return TaskResult(result);
    }
}

public sealed class CreateDirectShapeExtrusionHandler : ToolHandlerBase
{
    public override McpToolDescriptor Descriptor => Desc(
        "aec.create_direct_shape_extrusion",
        "Extrude a 2D polyline profile to a solid DirectShape.",
        Schema.ObjectWithRequired(
            [
                ("profile", new JObject { ["type"] = "array", ["items"] = new JObject { ["type"] = "array" }, ["description"] = "[[x,y], ...] closed profile" }),
                ("height", Schema.Integer("Extrusion height in feet")),
                ("base_z", Schema.Integer("Base elevation Z")),
            ],
            "profile", "height"),
        ToolRiskClass.Write,
        requiresRevit: true);

    public override Task<McpToolResult> InvokeAsync(JObject arguments, HostContext context)
    {
        var profile = arguments["profile"] as JArray;
        var height = arguments["height"]?.Value<double>() ?? 10;
        var baseZ = arguments["base_z"]?.Value<double>() ?? 0;

        var result = context.Bridge.RunOnRevitThread(app =>
        {
            var doc = McpHandlerHelpers.GetDoc(app);
            if (doc == null) return McpHandlerHelpers.NoDocument();

            return McpHandlerHelpers.RunTransaction(doc, "MCP Create DirectShape Extrusion", () =>
            {
                IList<CurveLoop> loops;
                try { loops = McpHandlerHelpers.ParseProfile(profile, baseZ); }
                catch (Exception ex) { return McpToolResult.Failure(ex.Message, "invalid_profile"); }

                var solid = GeometryCreationUtilities.CreateExtrusionGeometry(loops, XYZ.BasisZ, height);
                var id = McpHandlerHelpers.CreateDirectShape(doc, solid);
                return McpToolResult.Success(new JObject
                {
                    ["element_id"] = id.Value,
                    ["height"] = height,
                    ["profile_point_count"] = profile?.Count ?? 0,
                });
            });
        });
        return TaskResult(result);
    }
}

public sealed class GetElementGeometryJsonHandler : ToolHandlerBase
{
    public override McpToolDescriptor Descriptor => Desc(
        "aec.get_element_geometry_json",
        "Extract solid/mesh geometry of elements as JSON (vertices and triangle indices).",
        Schema.ObjectWithRequired(
            [("element_ids", new JObject { ["type"] = "array", ["items"] = new JObject { ["type"] = "integer" } })],
            "element_ids"),
        requiresRevit: true);

    public override Task<McpToolResult> InvokeAsync(JObject arguments, HostContext context)
    {
        var ids = (arguments["element_ids"] as JArray ?? []).Select(t => new ElementId(t.Value<long>())).ToList();

        var result = context.Bridge.RunOnRevitThread(app =>
        {
            var doc = McpHandlerHelpers.GetDoc(app);
            if (doc == null) return McpHandlerHelpers.NoDocument();
            if (ids.Count == 0) return McpToolResult.Failure("element_ids required", "missing_argument");

            var view = doc.ActiveView;
            var items = new JArray();
            foreach (var id in ids)
            {
                var element = doc.GetElement(id);
                if (element == null) continue;
                items.Add(ExtractGeometry(element, view));
            }

            return McpToolResult.Success(new JObject { ["elements"] = items, ["count"] = items.Count });
        });
        return TaskResult(result);
    }

    static JObject ExtractGeometry(Element element, View view)
    {
        var geom = element.get_Geometry(new Options { View = view, ComputeReferences = false });
        var meshes = new JArray();
        CollectMeshes(geom, meshes);
        return new JObject
        {
            ["element_id"] = element.Id.Value,
            ["name"] = element.Name,
            ["meshes"] = meshes,
        };
    }

    static void CollectMeshes(GeometryElement geom, JArray meshes)
    {
        if (geom == null) return;
        foreach (var obj in geom)
        {
            switch (obj)
            {
                case Solid solid when solid.Faces.Size > 0:
                    var tri = new JArray();
                    var verts = new JArray();
                    var index = 0;
                    foreach (Face face in solid.Faces)
                    {
                        var mesh = face.Triangulate();
                        if (mesh == null) continue;
                        foreach (var v in mesh.Vertices)
                            verts.Add(new JArray(v.X, v.Y, v.Z));
                        for (var i = 0; i < mesh.NumTriangles; i++)
                        {
                            var t = mesh.get_Triangle(i);
                            tri.Add(new JArray(index + (int)t.get_Index(0), index + (int)t.get_Index(1), index + (int)t.get_Index(2)));
                        }
                        index += mesh.Vertices.Count;
                    }
                    meshes.Add(new JObject { ["vertices"] = verts, ["triangles"] = tri });
                    break;
                case GeometryInstance inst:
                    CollectMeshes(inst.GetInstanceGeometry(), meshes);
                    break;
            }
        }
    }
}

public sealed class ComputeElementBoundingBoxHandler : ToolHandlerBase
{
    public override McpToolDescriptor Descriptor => Desc(
        "aec.compute_element_bounding_box",
        "Return world-space bounding boxes for one or more elements.",
        Schema.ObjectWithRequired(
            [("element_ids", new JObject { ["type"] = "array", ["items"] = new JObject { ["type"] = "integer" } })],
            "element_ids"),
        requiresRevit: true);

    public override Task<McpToolResult> InvokeAsync(JObject arguments, HostContext context)
    {
        var ids = (arguments["element_ids"] as JArray ?? []).Select(t => new ElementId(t.Value<long>())).ToList();
        var result = context.Bridge.RunOnRevitThread(app =>
        {
            var doc = McpHandlerHelpers.GetDoc(app);
            if (doc == null) return McpHandlerHelpers.NoDocument();
            var view = doc.ActiveView;
            var items = ids.Select(id =>
            {
                var e = doc.GetElement(id);
                if (e == null) return new JObject { ["element_id"] = id.Value, ["error"] = "not_found" };
                return new JObject
                {
                    ["element_id"] = id.Value,
                    ["name"] = e.Name,
                    ["bounding_box"] = McpHandlerHelpers.BboxToJson(e.get_BoundingBox(view)),
                };
            }).ToList();
            return McpToolResult.Success(new JObject { ["elements"] = new JArray(items) });
        });
        return TaskResult(result);
    }
}

public sealed class ComputeMaterialQuantitiesHandler : ToolHandlerBase
{
    public override McpToolDescriptor Descriptor => Desc(
        "aec.compute_material_quantities",
        "Compute material volumes for selected or queried elements.",
        Schema.Object(
            ("element_ids", new JObject { ["type"] = "array", ["items"] = new JObject { ["type"] = "integer" } }),
            ("category", Schema.String("Category name when element_ids omitted"))),
        requiresRevit: true);

    public override Task<McpToolResult> InvokeAsync(JObject arguments, HostContext context)
    {
        var result = context.Bridge.RunOnRevitThread(app =>
        {
            var doc = McpHandlerHelpers.GetDoc(app);
            if (doc == null) return McpHandlerHelpers.NoDocument();

            IEnumerable<Element> elements;
            var idTokens = arguments["element_ids"] as JArray;
            if (idTokens is { Count: > 0 })
                elements = idTokens.Select(t => doc.GetElement(new ElementId(t.Value<long>()))).Where(e => e != null);
            else
            {
                var category = arguments["category"]?.Value<string>() ?? "Walls";
                elements = McpHandlerHelpers.ElementsByCategory(doc, category);
            }

            var items = new JArray();
            foreach (var e in elements.Take(500))
            {
                var mats = new JArray();
                foreach (var matId in e.GetMaterialIds(false))
                {
                    var mat = doc.GetElement(matId) as Material;
                    var vol = e.GetMaterialVolume(matId);
                    mats.Add(new JObject
                    {
                        ["material_id"] = matId.Value,
                        ["material_name"] = mat?.Name,
                        ["volume"] = vol,
                    });
                }
                items.Add(new JObject
                {
                    ["element_id"] = e.Id.Value,
                    ["name"] = e.Name,
                    ["materials"] = mats,
                });
            }

            return McpToolResult.Success(new JObject { ["elements"] = items, ["count"] = items.Count });
        });
        return TaskResult(result);
    }
}

public sealed class FindIntersectingElementsHandler : ToolHandlerBase
{
    public override McpToolDescriptor Descriptor => Desc(
        "aec.find_intersecting_elements",
        "Find elements whose bounding boxes intersect a given bounding box.",
        Schema.ObjectWithRequired(
            [
                ("min", new JObject { ["type"] = "array", ["items"] = new JObject { ["type"] = "number" }, ["minItems"] = 3 }),
                ("max", new JObject { ["type"] = "array", ["items"] = new JObject { ["type"] = "number" }, ["minItems"] = 3 }),
                ("category", Schema.String("Optional category filter")),
            ],
            "min", "max"),
        requiresRevit: true);

    public override Task<McpToolResult> InvokeAsync(JObject arguments, HostContext context)
    {
        var min = McpHandlerHelpers.ParsePoint(arguments["min"]);
        var max = McpHandlerHelpers.ParsePoint(arguments["max"]);
        var category = arguments["category"]?.Value<string>();

        var result = context.Bridge.RunOnRevitThread(app =>
        {
            var doc = McpHandlerHelpers.GetDoc(app);
            if (doc == null) return McpHandlerHelpers.NoDocument();

            var outline = new Outline(min, max);
            var collector = new FilteredElementCollector(doc).WherePasses(new BoundingBoxIntersectsFilter(outline))
                .WhereElementIsNotElementType();
            if (!string.IsNullOrWhiteSpace(category))
            {
                var cat = McpHandlerHelpers.FindCategory(doc, category);
                if (cat != null) collector = collector.OfCategoryId(cat.Id);
            }

            var items = collector.Take(500).Select(McpHandlerHelpers.ElementRef).ToList();
            return McpToolResult.Success(new JObject { ["count"] = items.Count, ["elements"] = new JArray(items) });
        });
        return TaskResult(result);
    }
}

public sealed class ComputeWallFaceAreasHandler : ToolHandlerBase
{
    public override McpToolDescriptor Descriptor => Desc(
        "aec.compute_wall_face_areas",
        "Compute net and gross wall face areas for rooms with opening deductions.",
        Schema.Object(
            ("room_ids", new JObject { ["type"] = "array", ["items"] = new JObject { ["type"] = "integer" } }),
            ("level", Schema.String("Optional level name filter"))),
        requiresRevit: true);

    public override Task<McpToolResult> InvokeAsync(JObject arguments, HostContext context)
    {
        var result = context.Bridge.RunOnRevitThread(app =>
        {
            var doc = McpHandlerHelpers.GetDoc(app);
            if (doc == null) return McpHandlerHelpers.NoDocument();

            var roomIds = (arguments["room_ids"] as JArray ?? []).Select(t => new ElementId(t.Value<long>())).ToHashSet();
            var levelName = arguments["level"]?.Value<string>();

            var rooms = new FilteredElementCollector(doc).OfCategory(BuiltInCategory.OST_Rooms)
                .WhereElementIsNotElementType().Cast<Room>()
                .Where(r => r.Area > 0 && r.Location != null)
                .Where(r => roomIds.Count == 0 || roomIds.Contains(r.Id))
                .Where(r =>
                {
                    if (string.IsNullOrWhiteSpace(levelName)) return true;
                    var level = doc.GetElement(r.LevelId) as Level;
                    return level != null && level.Name.IndexOf(levelName, StringComparison.OrdinalIgnoreCase) >= 0;
                });

            var options = new SpatialElementBoundaryOptions
            {
                SpatialElementBoundaryLocation = SpatialElementBoundaryLocation.Finish,
            };
            var calc = new SpatialElementGeometryCalculator(doc, options);
            var items = new JArray();

            foreach (var room in rooms.Take(100))
            {
                try
                {
                    var results = calc.CalculateSpatialElementGeometry(room);
                    var faces = new JArray();
                    foreach (Face face in results.GetGeometry().Faces)
                    {
                        var infos = results.GetBoundaryFaceInfo(face);
                        foreach (var info in infos)
                        {
                            if (info.SubfaceType != SubfaceType.Side) continue;
                            var host = doc.GetElement(info.SpatialBoundaryElement.HostElementId);
                            faces.Add(new JObject
                            {
                                ["host_element_id"] = host?.Id.Value,
                                ["host_category"] = host?.Category?.Name,
                                ["face_area"] = face.Area,
                            });
                        }
                    }
                    items.Add(new JObject
                    {
                        ["room_id"] = room.Id.Value,
                        ["room_name"] = room.Name,
                        ["room_area"] = room.Area,
                        ["wall_faces"] = faces,
                    });
                }
                catch
                {
                    items.Add(new JObject { ["room_id"] = room.Id.Value, ["error"] = "geometry_calc_failed" });
                }
            }

            return McpToolResult.Success(new JObject { ["rooms"] = items, ["count"] = items.Count });
        });
        return TaskResult(result);
    }
}

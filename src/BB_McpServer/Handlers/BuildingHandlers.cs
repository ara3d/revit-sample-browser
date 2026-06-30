using Ara3D.Bowerbird.RevitSamples.AecAgent;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Architecture;
using Autodesk.Revit.DB.Structure;
using Autodesk.Revit.UI;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Ara3D.Bowerbird.RevitSamples;

public sealed class CreateWallHandler : ToolHandlerBase
{
    public override McpToolDescriptor Descriptor => Desc(
        "aec.create_wall",
        "Create a wall from start/end points, level, type, and height.",
        Schema.ObjectWithRequired(
            [
                ("start", new JObject { ["type"] = "array", ["items"] = new JObject { ["type"] = "number" }, ["minItems"] = 2 }),
                ("end", new JObject { ["type"] = "array", ["items"] = new JObject { ["type"] = "number" }, ["minItems"] = 2 }),
                ("level", Schema.String("Level name")),
                ("wall_type", Schema.String("Wall type name")),
                ("height", Schema.Integer("Unconnected height in feet")),
            ],
            "start", "end"),
        ToolRiskClass.Write,
        requiresRevit: true);

    public override Task<McpToolResult> InvokeAsync(JObject arguments, HostContext context)
    {
        var start = McpHandlerHelpers.ParsePoint(arguments["start"]);
        var end = McpHandlerHelpers.ParsePoint(arguments["end"], start.Z);
        var levelName = arguments["level"]?.Value<string>();
        var wallTypeName = arguments["wall_type"]?.Value<string>();
        var height = arguments["height"]?.Value<double>() ?? 10;

        var result = context.Bridge.RunOnRevitThread(app =>
        {
            var doc = McpHandlerHelpers.GetDoc(app);
            if (doc == null) return McpHandlerHelpers.NoDocument();
            var level = McpHandlerHelpers.FindLevel(doc, levelName);
            if (level == null) return McpToolResult.Failure("Level not found.", "level_not_found");
            var wallType = McpHandlerHelpers.FindWallType(doc, wallTypeName);
            if (wallType == null) return McpToolResult.Failure("Wall type not found.", "type_not_found");

            return McpHandlerHelpers.RunTransaction(doc, "MCP Create Wall", () =>
            {
                var line = Line.CreateBound(start, end);
                var wall = Wall.Create(doc, line, wallType.Id, level.Id, height, 0, false, false);
                return McpToolResult.Success(new JObject
                {
                    ["element_id"] = wall.Id.Value,
                    ["level"] = level.Name,
                    ["wall_type"] = wallType.Name,
                });
            });
        });
        return TaskResult(result);
    }
}

public sealed class CreateFloorHandler : ToolHandlerBase
{
    public override McpToolDescriptor Descriptor => Desc(
        "aec.create_floor",
        "Create a floor from a boundary profile.",
        Schema.ObjectWithRequired(
            [
                ("profile", new JObject { ["type"] = "array", ["items"] = new JObject { ["type"] = "array" } }),
                ("level", Schema.String("Level name")),
                ("floor_type", Schema.String("Floor type name")),
            ],
            "profile"),
        ToolRiskClass.Write,
        requiresRevit: true);

    public override Task<McpToolResult> InvokeAsync(JObject arguments, HostContext context)
    {
        var profile = arguments["profile"] as JArray;
        var levelName = arguments["level"]?.Value<string>();
        var floorTypeName = arguments["floor_type"]?.Value<string>();
        var baseZ = arguments["base_z"]?.Value<double>() ?? 0;

        var result = context.Bridge.RunOnRevitThread(app =>
        {
            var doc = McpHandlerHelpers.GetDoc(app);
            if (doc == null) return McpHandlerHelpers.NoDocument();
            var level = McpHandlerHelpers.FindLevel(doc, levelName);
            if (level == null) return McpToolResult.Failure("Level not found.", "level_not_found");
            var floorType = McpHandlerHelpers.FindFloorType(doc, floorTypeName);
            if (floorType == null) return McpToolResult.Failure("Floor type not found.", "type_not_found");

            return McpHandlerHelpers.RunTransaction(doc, "MCP Create Floor", () =>
            {
                IList<CurveLoop> loops;
                try { loops = McpHandlerHelpers.ParseProfile(profile, baseZ); }
                catch (Exception ex) { return McpToolResult.Failure(ex.Message, "invalid_profile"); }

                var floor = Floor.Create(doc, loops, floorType.Id, level.Id);
                return McpToolResult.Success(new JObject
                {
                    ["element_id"] = floor.Id.Value,
                    ["level"] = level.Name,
                    ["floor_type"] = floorType.Name,
                });
            });
        });
        return TaskResult(result);
    }
}

public sealed class CreateCeilingHandler : ToolHandlerBase
{
    public override McpToolDescriptor Descriptor => Desc(
        "aec.create_ceiling",
        "Create a ceiling from a boundary profile.",
        Schema.ObjectWithRequired(
            [
                ("profile", new JObject { ["type"] = "array", ["items"] = new JObject { ["type"] = "array" } }),
                ("level", Schema.String("Level name")),
                ("ceiling_type", Schema.String("Ceiling type name")),
            ],
            "profile"),
        ToolRiskClass.Write,
        requiresRevit: true);

    public override Task<McpToolResult> InvokeAsync(JObject arguments, HostContext context)
    {
        var profile = arguments["profile"] as JArray;
        var levelName = arguments["level"]?.Value<string>();
        var ceilingTypeName = arguments["ceiling_type"]?.Value<string>();
        var baseZ = arguments["base_z"]?.Value<double>() ?? 0;

        var result = context.Bridge.RunOnRevitThread(app =>
        {
            var doc = McpHandlerHelpers.GetDoc(app);
            if (doc == null) return McpHandlerHelpers.NoDocument();
            var level = McpHandlerHelpers.FindLevel(doc, levelName);
            if (level == null) return McpToolResult.Failure("Level not found.", "level_not_found");
            var ceilingType = McpHandlerHelpers.FindCeilingType(doc, ceilingTypeName);
            if (ceilingType == null) return McpToolResult.Failure("Ceiling type not found.", "type_not_found");

            return McpHandlerHelpers.RunTransaction(doc, "MCP Create Ceiling", () =>
            {
                IList<CurveLoop> loops;
                try { loops = McpHandlerHelpers.ParseProfile(profile, baseZ); }
                catch (Exception ex) { return McpToolResult.Failure(ex.Message, "invalid_profile"); }

                var ceiling = Ceiling.Create(doc, loops, ceilingType.Id, level.Id);
                return McpToolResult.Success(new JObject
                {
                    ["element_id"] = ceiling.Id.Value,
                    ["level"] = level.Name,
                    ["ceiling_type"] = ceilingType.Name,
                });
            });
        });
        return TaskResult(result);
    }
}

public sealed class CreateRoomHandler : ToolHandlerBase
{
    public override McpToolDescriptor Descriptor => Desc(
        "aec.create_room",
        "Create a room at a point on a level.",
        Schema.ObjectWithRequired(
            [
                ("point", new JObject { ["type"] = "array", ["items"] = new JObject { ["type"] = "number" }, ["minItems"] = 2 }),
                ("level", Schema.String("Level name")),
                ("name", Schema.String("Optional room name")),
            ],
            "point"),
        ToolRiskClass.Write,
        requiresRevit: true);

    public override Task<McpToolResult> InvokeAsync(JObject arguments, HostContext context)
    {
        var point = McpHandlerHelpers.ParsePoint(arguments["point"]);
        var levelName = arguments["level"]?.Value<string>();
        var name = arguments["name"]?.Value<string>();

        var result = context.Bridge.RunOnRevitThread(app =>
        {
            var doc = McpHandlerHelpers.GetDoc(app);
            if (doc == null) return McpHandlerHelpers.NoDocument();
            var level = McpHandlerHelpers.FindLevel(doc, levelName);
            if (level == null) return McpToolResult.Failure("Level not found.", "level_not_found");

            return McpHandlerHelpers.RunTransaction(doc, "MCP Create Room", () =>
            {
                var room = doc.Create.NewRoom(level, new UV(point.X, point.Y));
                if (!string.IsNullOrWhiteSpace(name))
                    room.Name = name;
                return McpToolResult.Success(new JObject
                {
                    ["element_id"] = room.Id.Value,
                    ["name"] = room.Name,
                    ["level"] = level.Name,
                });
            });
        });
        return TaskResult(result);
    }
}

public sealed class CreateColumnHandler : ToolHandlerBase
{
    public override McpToolDescriptor Descriptor => Desc(
        "aec.create_column",
        "Place a column family instance at a point on a level.",
        Schema.ObjectWithRequired(
            [
                ("point", new JObject { ["type"] = "array", ["items"] = new JObject { ["type"] = "number" }, ["minItems"] = 2 }),
                ("level", Schema.String("Level name")),
                ("family", Schema.String("Family name")),
                ("type", Schema.String("Type name")),
            ],
            "point"),
        ToolRiskClass.Write,
        requiresRevit: true);

    public override Task<McpToolResult> InvokeAsync(JObject arguments, HostContext context)
    {
        var point = McpHandlerHelpers.ParsePoint(arguments["point"]);
        var levelName = arguments["level"]?.Value<string>();
        var family = arguments["family"]?.Value<string>();
        var type = arguments["type"]?.Value<string>();

        var result = context.Bridge.RunOnRevitThread(app =>
        {
            var doc = McpHandlerHelpers.GetDoc(app);
            if (doc == null) return McpHandlerHelpers.NoDocument();
            var level = McpHandlerHelpers.FindLevel(doc, levelName);
            if (level == null) return McpToolResult.Failure("Level not found.", "level_not_found");
            var symbol = McpHandlerHelpers.FindFamilySymbol(doc, family, type, BuiltInCategory.OST_Columns)
                ?? McpHandlerHelpers.FindFamilySymbol(doc, family, type, BuiltInCategory.OST_StructuralColumns);
            if (symbol == null) return McpToolResult.Failure("Column type not found.", "type_not_found");

            return McpHandlerHelpers.RunTransaction(doc, "MCP Create Column", () =>
            {
                if (!symbol.IsActive) symbol.Activate();
                var instance = doc.Create.NewFamilyInstance(point, symbol, level, StructuralType.NonStructural);
                return McpToolResult.Success(new JObject
                {
                    ["element_id"] = instance.Id.Value,
                    ["family"] = symbol.FamilyName,
                    ["type"] = symbol.Name,
                    ["level"] = level.Name,
                });
            });
        });
        return TaskResult(result);
    }
}

public sealed class CreateBeamHandler : ToolHandlerBase
{
    public override McpToolDescriptor Descriptor => Desc(
        "aec.create_beam",
        "Create a structural beam between two points on a level.",
        Schema.ObjectWithRequired(
            [
                ("start", new JObject { ["type"] = "array", ["items"] = new JObject { ["type"] = "number" }, ["minItems"] = 2 }),
                ("end", new JObject { ["type"] = "array", ["items"] = new JObject { ["type"] = "number" }, ["minItems"] = 2 }),
                ("level", Schema.String("Level name")),
                ("family", Schema.String("Family name")),
                ("type", Schema.String("Type name")),
            ],
            "start", "end"),
        ToolRiskClass.Write,
        requiresRevit: true);

    public override Task<McpToolResult> InvokeAsync(JObject arguments, HostContext context)
    {
        var start = McpHandlerHelpers.ParsePoint(arguments["start"]);
        var end = McpHandlerHelpers.ParsePoint(arguments["end"], start.Z);
        var levelName = arguments["level"]?.Value<string>();
        var family = arguments["family"]?.Value<string>();
        var type = arguments["type"]?.Value<string>();

        var result = context.Bridge.RunOnRevitThread(app =>
        {
            var doc = McpHandlerHelpers.GetDoc(app);
            if (doc == null) return McpHandlerHelpers.NoDocument();
            var level = McpHandlerHelpers.FindLevel(doc, levelName);
            if (level == null) return McpToolResult.Failure("Level not found.", "level_not_found");
            var symbol = McpHandlerHelpers.FindFamilySymbol(doc, family, type, BuiltInCategory.OST_StructuralFraming);
            if (symbol == null) return McpToolResult.Failure("Beam type not found.", "type_not_found");

            return McpHandlerHelpers.RunTransaction(doc, "MCP Create Beam", () =>
            {
                if (!symbol.IsActive) symbol.Activate();
                var line = Line.CreateBound(start, end);
                var instance = doc.Create.NewFamilyInstance(line, symbol, level, StructuralType.Beam);
                return McpToolResult.Success(new JObject
                {
                    ["element_id"] = instance.Id.Value,
                    ["family"] = symbol.FamilyName,
                    ["type"] = symbol.Name,
                    ["level"] = level.Name,
                });
            });
        });
        return TaskResult(result);
    }
}

public sealed class CreateLevelHandler : ToolHandlerBase
{
    public override McpToolDescriptor Descriptor => Desc(
        "aec.create_level",
        "Create a new level at an elevation.",
        Schema.ObjectWithRequired(
            [
                ("elevation", Schema.Integer("Elevation in feet")),
                ("name", Schema.String("Optional level name")),
            ],
            "elevation"),
        ToolRiskClass.Write,
        requiresRevit: true);

    public override Task<McpToolResult> InvokeAsync(JObject arguments, HostContext context)
    {
        var elevation = arguments["elevation"]?.Value<double>() ?? 0;
        var name = arguments["name"]?.Value<string>();

        var result = context.Bridge.RunOnRevitThread(app =>
        {
            var doc = McpHandlerHelpers.GetDoc(app);
            if (doc == null) return McpHandlerHelpers.NoDocument();

            return McpHandlerHelpers.RunTransaction(doc, "MCP Create Level", () =>
            {
                var level = Level.Create(doc, elevation);
                if (!string.IsNullOrWhiteSpace(name))
                    level.Name = name;
                return McpToolResult.Success(new JObject
                {
                    ["element_id"] = level.Id.Value,
                    ["name"] = level.Name,
                    ["elevation"] = level.Elevation,
                });
            });
        });
        return TaskResult(result);
    }
}

public sealed class CreateGridLineHandler : ToolHandlerBase
{
    public override McpToolDescriptor Descriptor => Desc(
        "aec.create_grid_line",
        "Create a grid line between two points.",
        Schema.ObjectWithRequired(
            [
                ("start", new JObject { ["type"] = "array", ["items"] = new JObject { ["type"] = "number" }, ["minItems"] = 2 }),
                ("end", new JObject { ["type"] = "array", ["items"] = new JObject { ["type"] = "number" }, ["minItems"] = 2 }),
                ("name", Schema.String("Optional grid name")),
            ],
            "start", "end"),
        ToolRiskClass.Write,
        requiresRevit: true);

    public override Task<McpToolResult> InvokeAsync(JObject arguments, HostContext context)
    {
        var start = McpHandlerHelpers.ParsePoint(arguments["start"]);
        var end = McpHandlerHelpers.ParsePoint(arguments["end"], start.Z);
        var name = arguments["name"]?.Value<string>();

        var result = context.Bridge.RunOnRevitThread(app =>
        {
            var doc = McpHandlerHelpers.GetDoc(app);
            if (doc == null) return McpHandlerHelpers.NoDocument();

            return McpHandlerHelpers.RunTransaction(doc, "MCP Create Grid", () =>
            {
                var grid = Grid.Create(doc, Line.CreateBound(start, end));
                if (!string.IsNullOrWhiteSpace(name))
                    grid.Name = name;
                return McpToolResult.Success(new JObject
                {
                    ["element_id"] = grid.Id.Value,
                    ["name"] = grid.Name,
                });
            });
        });
        return TaskResult(result);
    }
}

public sealed class PlaceFamilyInstanceHandler : ToolHandlerBase
{
    public override McpToolDescriptor Descriptor => Desc(
        "aec.place_family_instance",
        "Place any family instance by family/type name at a point on a level.",
        Schema.ObjectWithRequired(
            [
                ("point", new JObject { ["type"] = "array", ["items"] = new JObject { ["type"] = "number" }, ["minItems"] = 2 }),
                ("family", Schema.String("Family name")),
                ("type", Schema.String("Type name")),
                ("level", Schema.String("Level name")),
                ("category", Schema.String("Optional category hint")),
            ],
            "point", "family", "type"),
        ToolRiskClass.Write,
        requiresRevit: true);

    public override Task<McpToolResult> InvokeAsync(JObject arguments, HostContext context)
    {
        var point = McpHandlerHelpers.ParsePoint(arguments["point"]);
        var family = arguments["family"]?.Value<string>();
        var type = arguments["type"]?.Value<string>();
        var levelName = arguments["level"]?.Value<string>();
        var categoryName = arguments["category"]?.Value<string>();
        BuiltInCategory? bic = null;
        if (!string.IsNullOrWhiteSpace(categoryName) && Enum.TryParse<BuiltInCategory>(categoryName, true, out var parsed))
            bic = parsed;

        var result = context.Bridge.RunOnRevitThread(app =>
        {
            var doc = McpHandlerHelpers.GetDoc(app);
            if (doc == null) return McpHandlerHelpers.NoDocument();
            var level = McpHandlerHelpers.FindLevel(doc, levelName);
            var symbol = McpHandlerHelpers.FindFamilySymbol(doc, family, type, bic);
            if (symbol == null) return McpToolResult.Failure("Family type not found.", "type_not_found");

            return McpHandlerHelpers.RunTransaction(doc, "MCP Place Family Instance", () =>
            {
                if (!symbol.IsActive) symbol.Activate();
                FamilyInstance instance = level != null
                    ? doc.Create.NewFamilyInstance(point, symbol, level, StructuralType.NonStructural)
                    : doc.Create.NewFamilyInstance(point, symbol, StructuralType.NonStructural);
                return McpToolResult.Success(new JObject
                {
                    ["element_id"] = instance.Id.Value,
                    ["family"] = symbol.FamilyName,
                    ["type"] = symbol.Name,
                    ["level"] = level?.Name,
                });
            });
        });
        return TaskResult(result);
    }
}

public sealed class CreateRectangularFloorPlanHandler : ToolHandlerBase
{
    public override McpToolDescriptor Descriptor => Desc(
        "aec.create_rectangular_floor_plan",
        "Create walls, floor, and rooms for a rectangular layout in one transaction.",
        Schema.ObjectWithRequired(
            [
                ("origin", new JObject { ["type"] = "array", ["items"] = new JObject { ["type"] = "number" }, ["minItems"] = 2 }),
                ("width", Schema.Integer("Overall width in feet")),
                ("depth", Schema.Integer("Overall depth in feet")),
                ("height", Schema.Integer("Wall height in feet")),
                ("level", Schema.String("Level name")),
                ("rooms", new JObject
                {
                    ["type"] = "array",
                    ["items"] = new JObject
                    {
                        ["type"] = "object",
                        ["properties"] = new JObject
                        {
                            ["name"] = new JObject { ["type"] = "string" },
                            ["x"] = new JObject { ["type"] = "number" },
                            ["y"] = new JObject { ["type"] = "number" },
                        },
                    },
                    ["description"] = "Room placement points inside footprint",
                }),
            ],
            "width", "depth"),
        ToolRiskClass.Write,
        requiresRevit: true);

    public override Task<McpToolResult> InvokeAsync(JObject arguments, HostContext context)
    {
        var origin = McpHandlerHelpers.ParsePoint(arguments["origin"]);
        var width = arguments["width"]?.Value<double>() ?? 40;
        var depth = arguments["depth"]?.Value<double>() ?? 30;
        var height = arguments["height"]?.Value<double>() ?? 10;
        var levelName = arguments["level"]?.Value<string>();
        var rooms = (arguments["rooms"] as JArray ?? []).OfType<JObject>().ToList();

        var result = context.Bridge.RunOnRevitThread(app =>
        {
            var doc = McpHandlerHelpers.GetDoc(app);
            if (doc == null) return McpHandlerHelpers.NoDocument();
            var level = McpHandlerHelpers.FindLevel(doc, levelName);
            if (level == null) return McpToolResult.Failure("Level not found.", "level_not_found");
            var wallType = McpHandlerHelpers.FindWallType(doc, null);
            var floorType = McpHandlerHelpers.FindFloorType(doc, null);
            if (wallType == null || floorType == null)
                return McpToolResult.Failure("Default wall/floor types not found.", "type_not_found");

            return McpHandlerHelpers.RunTransaction(doc, "MCP Create Rectangular Floor Plan", () =>
            {
                var p0 = origin;
                var p1 = origin + new XYZ(width, 0, 0);
                var p2 = origin + new XYZ(width, depth, 0);
                var p3 = origin + new XYZ(0, depth, 0);
                var wallIds = new JArray();
                foreach (var seg in new[] { (p0, p1), (p1, p2), (p2, p3), (p3, p0) })
                {
                    var wall = Wall.Create(doc, Line.CreateBound(seg.Item1, seg.Item2), wallType.Id, level.Id, height, 0, false, false);
                    wallIds.Add(wall.Id.Value);
                }

                var floorLoop = McpHandlerHelpers.CreateRectLoop(width, depth, origin);
                var floor = Floor.Create(doc, [floorLoop], floorType.Id, level.Id);
                var roomIds = new JArray();
                if (rooms.Count == 0)
                {
                    var room = doc.Create.NewRoom(level, new UV(origin.X + width / 2, origin.Y + depth / 2));
                    roomIds.Add(new JObject { ["element_id"] = room.Id.Value, ["name"] = room.Name });
                }
                else
                {
                    foreach (var r in rooms)
                    {
                        var rx = r["x"]?.Value<double>() ?? origin.X + width / 2;
                        var ry = r["y"]?.Value<double>() ?? origin.Y + depth / 2;
                        var room = doc.Create.NewRoom(level, new UV(rx, ry));
                        var roomName = r["name"]?.Value<string>();
                        if (!string.IsNullOrWhiteSpace(roomName)) room.Name = roomName;
                        roomIds.Add(new JObject { ["element_id"] = room.Id.Value, ["name"] = room.Name });
                    }
                }

                return McpToolResult.Success(new JObject
                {
                    ["level"] = level.Name,
                    ["wall_ids"] = wallIds,
                    ["floor_id"] = floor.Id.Value,
                    ["rooms"] = roomIds,
                    ["width"] = width,
                    ["depth"] = depth,
                    ["height"] = height,
                });
            });
        });
        return TaskResult(result);
    }
}

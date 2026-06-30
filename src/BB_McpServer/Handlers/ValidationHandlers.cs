using Ara3D.Bowerbird.RevitSamples.AecAgent;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Architecture;
using Autodesk.Revit.DB.Mechanical;
using Autodesk.Revit.DB.Plumbing;
using Autodesk.Revit.UI;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Ara3D.Bowerbird.RevitSamples;

public sealed class GetModelWarningsDetailedHandler : ToolHandlerBase
{
    public override McpToolDescriptor Descriptor => Desc(
        "aec.get_model_warnings_detailed",
        "Get all model warnings with element IDs, severity, and description.",
        Schema.Object(),
        requiresRevit: true);

    public override Task<McpToolResult> InvokeAsync(JObject arguments, HostContext context)
    {
        var result = context.Bridge.RunOnRevitThread(app =>
        {
            var doc = McpHandlerHelpers.GetDoc(app);
            if (doc == null) return McpHandlerHelpers.NoDocument();

            var warnings = doc.GetWarnings().Select(w => new JObject
            {
                ["description"] = w.GetDescriptionText(),
                ["severity"] = w.GetSeverity().ToString(),
                ["element_ids"] = new JArray(w.GetFailingElements().Select(id => id.Value)),
            }).ToList();

            return McpToolResult.Success(new JObject
            {
                ["count"] = warnings.Count,
                ["warnings"] = new JArray(warnings),
            });
        });
        return TaskResult(result);
    }
}

public sealed class FindUnplacedRoomsHandler : ToolHandlerBase
{
    public override McpToolDescriptor Descriptor => Desc(
        "aec.find_unplaced_rooms",
        "Find rooms that are not bounded or placed.",
        Schema.Object(),
        requiresRevit: true);

    public override Task<McpToolResult> InvokeAsync(JObject arguments, HostContext context)
    {
        var result = context.Bridge.RunOnRevitThread(app =>
        {
            var doc = McpHandlerHelpers.GetDoc(app);
            if (doc == null) return McpHandlerHelpers.NoDocument();

            var rooms = new FilteredElementCollector(doc).OfCategory(BuiltInCategory.OST_Rooms)
                .WhereElementIsNotElementType().Cast<Room>()
                .Where(r => r.Area <= 0 || r.Location == null)
                .Select(r => new JObject
                {
                    ["element_id"] = r.Id.Value,
                    ["name"] = r.Name,
                    ["number"] = r.Number,
                    ["area"] = r.Area,
                    ["level"] = (doc.GetElement(r.LevelId) as Level)?.Name,
                }).ToList();

            return McpToolResult.Success(new JObject { ["count"] = rooms.Count, ["rooms"] = new JArray(rooms) });
        });
        return TaskResult(result);
    }
}

public sealed class FindElementsWithoutParameterHandler : ToolHandlerBase
{
    public override McpToolDescriptor Descriptor => Desc(
        "aec.find_elements_without_parameter",
        "Find elements missing a required parameter value.",
        Schema.ObjectWithRequired(
            [
                ("parameter", Schema.String("Parameter name")),
                ("category", Schema.String("Category name")),
            ],
            "parameter"),
        requiresRevit: true);

    public override Task<McpToolResult> InvokeAsync(JObject arguments, HostContext context)
    {
        var parameterName = arguments["parameter"]?.Value<string>();
        var categoryName = arguments["category"]?.Value<string>();

        var result = context.Bridge.RunOnRevitThread(app =>
        {
            var doc = McpHandlerHelpers.GetDoc(app);
            if (doc == null) return McpHandlerHelpers.NoDocument();

            IEnumerable<Element> elements = string.IsNullOrWhiteSpace(categoryName)
                ? new FilteredElementCollector(doc).WhereElementIsNotElementType().ToElements()
                : McpHandlerHelpers.ElementsByCategory(doc, categoryName);

            var missing = elements
                .Where(e =>
                {
                    var p = e.LookupParameter(parameterName);
                    if (p == null || !p.HasValue) return true;
                    var text = McpHandlerHelpers.GetParameterText(p);
                    return string.IsNullOrWhiteSpace(text);
                })
                .Take(500)
                .Select(McpHandlerHelpers.ElementRef)
                .ToList();

            return McpToolResult.Success(new JObject
            {
                ["parameter"] = parameterName,
                ["count"] = missing.Count,
                ["elements"] = new JArray(missing),
            });
        });
        return TaskResult(result);
    }
}

public sealed class ValidateNamingConventionHandler : ToolHandlerBase
{
    public override McpToolDescriptor Descriptor => Desc(
        "aec.validate_naming_convention",
        "Check element names against a regex pattern by category.",
        Schema.ObjectWithRequired(
            [
                ("pattern", Schema.String("Regex pattern names must match")),
                ("category", Schema.String("Category name")),
            ],
            "pattern"),
        requiresRevit: true);

    public override Task<McpToolResult> InvokeAsync(JObject arguments, HostContext context)
    {
        var pattern = arguments["pattern"]?.Value<string>();
        var categoryName = arguments["category"]?.Value<string>() ?? "Rooms";
        Regex regex;
        try { regex = new Regex(pattern, RegexOptions.IgnoreCase); }
        catch (Exception ex) { return TaskResult(McpToolResult.Failure(ex.Message, "invalid_regex")); }

        var result = context.Bridge.RunOnRevitThread(app =>
        {
            var doc = McpHandlerHelpers.GetDoc(app);
            if (doc == null) return McpHandlerHelpers.NoDocument();

            var violations = McpHandlerHelpers.ElementsByCategory(doc, categoryName)
                .Where(e => !string.IsNullOrWhiteSpace(e.Name) && !regex.IsMatch(e.Name))
                .Take(500)
                .Select(e => new JObject
                {
                    ["element_id"] = e.Id.Value,
                    ["name"] = e.Name,
                    ["category"] = e.Category?.Name,
                }).ToList();

            return McpToolResult.Success(new JObject
            {
                ["pattern"] = pattern,
                ["category"] = categoryName,
                ["violation_count"] = violations.Count,
                ["violations"] = new JArray(violations),
            });
        });
        return TaskResult(result);
    }
}

public sealed class FindLevelLessElementsHandler : ToolHandlerBase
{
    public override McpToolDescriptor Descriptor => Desc(
        "aec.find_level_less_elements",
        "Find elements not assigned to a level.",
        Schema.Object(("category", Schema.String("Optional category filter"))),
        requiresRevit: true);

    public override Task<McpToolResult> InvokeAsync(JObject arguments, HostContext context)
    {
        var categoryName = arguments["category"]?.Value<string>();

        var result = context.Bridge.RunOnRevitThread(app =>
        {
            var doc = McpHandlerHelpers.GetDoc(app);
            if (doc == null) return McpHandlerHelpers.NoDocument();

            IEnumerable<Element> elements = string.IsNullOrWhiteSpace(categoryName)
                ? new FilteredElementCollector(doc).WhereElementIsNotElementType().ToElements()
                : McpHandlerHelpers.ElementsByCategory(doc, categoryName);

            var items = elements
                .Where(e => e.LevelId == ElementId.InvalidElementId)
                .Take(500)
                .Select(McpHandlerHelpers.ElementRef)
                .ToList();

            return McpToolResult.Success(new JObject { ["count"] = items.Count, ["elements"] = new JArray(items) });
        });
        return TaskResult(result);
    }
}

public sealed class CheckRoomDoorRelationshipsHandler : ToolHandlerBase
{
    public override McpToolDescriptor Descriptor => Desc(
        "aec.check_room_door_relationships",
        "Verify doors have valid ToRoom/FromRoom assignments.",
        Schema.Object(),
        requiresRevit: true);

    public override Task<McpToolResult> InvokeAsync(JObject arguments, HostContext context)
    {
        var result = context.Bridge.RunOnRevitThread(app =>
        {
            var doc = McpHandlerHelpers.GetDoc(app);
            if (doc == null) return McpHandlerHelpers.NoDocument();

            var doors = new FilteredElementCollector(doc).OfCategory(BuiltInCategory.OST_Doors)
                .WhereElementIsNotElementType().Cast<FamilyInstance>();

            var issues = new JArray();
            foreach (var door in doors)
            {
                var toRoom = door.ToRoom;
                var fromRoom = door.FromRoom;
                if (toRoom == null && fromRoom == null)
                {
                    issues.Add(new JObject
                    {
                        ["element_id"] = door.Id.Value,
                        ["name"] = door.Name,
                        ["issue"] = "missing_room_assignment",
                    });
                }
                else if (toRoom != null && fromRoom != null && toRoom.Id == fromRoom.Id)
                {
                    issues.Add(new JObject
                    {
                        ["element_id"] = door.Id.Value,
                        ["name"] = door.Name,
                        ["issue"] = "same_to_from_room",
                        ["room_id"] = toRoom.Id.Value,
                    });
                }
            }

            return McpToolResult.Success(new JObject { ["issue_count"] = issues.Count, ["issues"] = issues });
        });
        return TaskResult(result);
    }
}

public sealed class FindOverlappingRoomsHandler : ToolHandlerBase
{
    public override McpToolDescriptor Descriptor => Desc(
        "aec.find_overlapping_rooms",
        "Find rooms whose bounding boxes overlap on the same level.",
        Schema.Object(),
        requiresRevit: true);

    public override Task<McpToolResult> InvokeAsync(JObject arguments, HostContext context)
    {
        var result = context.Bridge.RunOnRevitThread(app =>
        {
            var doc = McpHandlerHelpers.GetDoc(app);
            if (doc == null) return McpHandlerHelpers.NoDocument();
            var view = doc.ActiveView;

            var rooms = new FilteredElementCollector(doc).OfCategory(BuiltInCategory.OST_Rooms)
                .WhereElementIsNotElementType().Cast<Room>()
                .Where(r => r.Area > 0)
                .Select(r => new { Room = r, Bbox = r.get_BoundingBox(view), LevelId = r.LevelId })
                .Where(x => x.Bbox != null)
                .ToList();

            var overlaps = new JArray();
            for (var i = 0; i < rooms.Count; i++)
            {
                for (var j = i + 1; j < rooms.Count; j++)
                {
                    if (rooms[i].LevelId != rooms[j].LevelId) continue;
                    if (BboxesOverlap(rooms[i].Bbox, rooms[j].Bbox))
                    {
                        overlaps.Add(new JObject
                        {
                            ["room_a_id"] = rooms[i].Room.Id.Value,
                            ["room_a_name"] = rooms[i].Room.Name,
                            ["room_b_id"] = rooms[j].Room.Id.Value,
                            ["room_b_name"] = rooms[j].Room.Name,
                        });
                    }
                }
            }

            return McpToolResult.Success(new JObject { ["overlap_count"] = overlaps.Count, ["overlaps"] = overlaps });
        });
        return TaskResult(result);
    }

    static bool BboxesOverlap(BoundingBoxXYZ a, BoundingBoxXYZ b)
        => a.Min.X <= b.Max.X && a.Max.X >= b.Min.X
            && a.Min.Y <= b.Max.Y && a.Max.Y >= b.Min.Y;
}

public sealed class AuditModelHealthHandler : ToolHandlerBase
{
    public override McpToolDescriptor Descriptor => Desc(
        "aec.audit_model_health",
        "Compound BIM health check: warnings, unplaced rooms, level-less elements, and health score.",
        Schema.Object(
            ("category_for_parameters", Schema.String("Category for parameter audit, default Rooms")),
            ("required_parameter", Schema.String("Parameter to check, default Name"))),
        requiresRevit: true);

    public override Task<McpToolResult> InvokeAsync(JObject arguments, HostContext context)
    {
        var category = arguments["category_for_parameters"]?.Value<string>() ?? "Rooms";
        var requiredParam = arguments["required_parameter"]?.Value<string>() ?? "Name";

        var result = context.Bridge.RunOnRevitThread(app =>
        {
            var doc = McpHandlerHelpers.GetDoc(app);
            if (doc == null) return McpHandlerHelpers.NoDocument();

            var warningCount = doc.GetWarnings().Count();
            var unplacedRooms = new FilteredElementCollector(doc).OfCategory(BuiltInCategory.OST_Rooms)
                .WhereElementIsNotElementType().Cast<Room>().Count(r => r.Area <= 0 || r.Location == null);
            var levelLess = new FilteredElementCollector(doc).WhereElementIsNotElementType()
                .Count(e => e.LevelId == ElementId.InvalidElementId && e.Category != null
                    && e.Category.CategoryType == CategoryType.Model);
            var missingParams = McpHandlerHelpers.ElementsByCategory(doc, category)
                .Count(e =>
                {
                    var p = e.LookupParameter(requiredParam);
                    return p == null || !p.HasValue || string.IsNullOrWhiteSpace(McpHandlerHelpers.GetParameterText(p));
                });

            var penalty = warningCount * 2 + unplacedRooms * 3 + Math.Min(levelLess, 100) + missingParams;
            var score = Math.Max(0, 100 - penalty);

            return McpToolResult.Success(new JObject
            {
                ["health_score"] = score,
                ["warning_count"] = warningCount,
                ["unplaced_room_count"] = unplacedRooms,
                ["level_less_element_count"] = levelLess,
                ["missing_parameter_count"] = missingParams,
                ["category_checked"] = category,
                ["parameter_checked"] = requiredParam,
                ["recommendations"] = new JArray(
                    warningCount > 0 ? "Review model warnings with aec.get_model_warnings_detailed." : null,
                    unplacedRooms > 0 ? "Place or delete unplaced rooms with aec.find_unplaced_rooms." : null,
                    missingParams > 0 ? "Fill missing parameter values with aec.set_parameters_bulk." : null),
            });
        });
        return TaskResult(result);
    }
}

public sealed class CheckMepSystemCompletenessHandler : ToolHandlerBase
{
    public override McpToolDescriptor Descriptor => Desc(
        "aec.check_mep_system_completeness",
        "Check duct/pipe systems for open connectors.",
        Schema.Object(("system_type", Schema.String("Duct, Pipe, or All"))),
        requiresRevit: true);

    public override Task<McpToolResult> InvokeAsync(JObject arguments, HostContext context)
    {
        var systemType = arguments["system_type"]?.Value<string>() ?? "All";

        var result = context.Bridge.RunOnRevitThread(app =>
        {
            var doc = McpHandlerHelpers.GetDoc(app);
            if (doc == null) return McpHandlerHelpers.NoDocument();

            var categories = new List<BuiltInCategory>();
            if (systemType.Equals("Duct", StringComparison.OrdinalIgnoreCase) || systemType.Equals("All", StringComparison.OrdinalIgnoreCase))
            {
                categories.Add(BuiltInCategory.OST_DuctCurves);
                categories.Add(BuiltInCategory.OST_DuctFitting);
            }
            if (systemType.Equals("Pipe", StringComparison.OrdinalIgnoreCase) || systemType.Equals("All", StringComparison.OrdinalIgnoreCase))
            {
                categories.Add(BuiltInCategory.OST_PipeCurves);
                categories.Add(BuiltInCategory.OST_PipeFitting);
            }

            var openConnectors = new JArray();
            foreach (var bic in categories.Distinct())
            {
                foreach (var element in new FilteredElementCollector(doc).OfCategory(bic).WhereElementIsNotElementType())
                {
                    if (element is not MEPCurve && element is not FamilyInstance) continue;
                    ConnectorSet connectors = null;
                    if (element is MEPCurve curve) connectors = curve.ConnectorManager?.Connectors;
                    else if (element is FamilyInstance fi) connectors = fi.MEPModel?.ConnectorManager?.Connectors;
                    if (connectors == null) continue;

                    foreach (Connector conn in connectors)
                    {
                        if (!conn.IsConnected)
                        {
                            openConnectors.Add(new JObject
                            {
                                ["element_id"] = element.Id.Value,
                                ["name"] = element.Name,
                                ["category"] = element.Category?.Name,
                                ["connector_id"] = conn.Id,
                            });
                        }
                    }
                }
            }

            return McpToolResult.Success(new JObject
            {
                ["open_connector_count"] = openConnectors.Count,
                ["open_connectors"] = openConnectors,
            });
        });
        return TaskResult(result);
    }
}

using Ara3D.Bowerbird.RevitSamples.AecAgent;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Ara3D.Bowerbird.RevitSamples;

public sealed class Create3dViewHandler : ToolHandlerBase
{
    public override McpToolDescriptor Descriptor => Desc(
        "aec.create_3d_view",
        "Create a new 3D isometric view with optional section box.",
        Schema.Object(
            ("name", Schema.String("View name")),
            ("section_box", new JObject
            {
                ["type"] = "object",
                ["properties"] = new JObject
                {
                    ["min"] = new JObject { ["type"] = "array" },
                    ["max"] = new JObject { ["type"] = "array" },
                },
            })),
        ToolRiskClass.Write,
        requiresRevit: true);

    public override Task<McpToolResult> InvokeAsync(JObject arguments, HostContext context)
    {
        var name = arguments["name"]?.Value<string>();
        var sectionBox = arguments["section_box"] as JObject;

        var result = context.Bridge.RunOnRevitThread(app =>
        {
            var doc = McpHandlerHelpers.GetDoc(app);
            if (doc == null) return McpHandlerHelpers.NoDocument();
            var vft = McpHandlerHelpers.FindViewFamilyType(doc, ViewFamily.ThreeDimensional);
            if (vft == null) return McpToolResult.Failure("3D view family type not found.", "type_not_found");

            return McpHandlerHelpers.RunTransaction(doc, "MCP Create 3D View", () =>
            {
                var view = View3D.CreateIsometric(doc, vft.Id);
                if (!string.IsNullOrWhiteSpace(name)) view.Name = name;
                if (sectionBox != null)
                {
                    var min = McpHandlerHelpers.ParsePoint(sectionBox["min"]);
                    var max = McpHandlerHelpers.ParsePoint(sectionBox["max"]);
                    view.IsSectionBoxActive = true;
                    view.SetSectionBox(new BoundingBoxXYZ { Min = min, Max = max });
                }
                return McpToolResult.Success(new JObject
                {
                    ["view_id"] = view.Id.Value,
                    ["name"] = view.Name,
                });
            });
        });
        return TaskResult(result);
    }
}

public sealed class CreatePlanViewHandler : ToolHandlerBase
{
    public override McpToolDescriptor Descriptor => Desc(
        "aec.create_plan_view",
        "Create a floor plan view at a level.",
        Schema.Object(
            ("level", Schema.String("Level name")),
            ("name", Schema.String("View name"))),
        ToolRiskClass.Write,
        requiresRevit: true);

    public override Task<McpToolResult> InvokeAsync(JObject arguments, HostContext context)
    {
        var levelName = arguments["level"]?.Value<string>();
        var name = arguments["name"]?.Value<string>();

        var result = context.Bridge.RunOnRevitThread(app =>
        {
            var doc = McpHandlerHelpers.GetDoc(app);
            if (doc == null) return McpHandlerHelpers.NoDocument();
            var level = McpHandlerHelpers.FindLevel(doc, levelName);
            if (level == null) return McpToolResult.Failure("Level not found.", "level_not_found");
            var vft = McpHandlerHelpers.FindViewFamilyType(doc, ViewFamily.FloorPlan);
            if (vft == null) return McpToolResult.Failure("Floor plan view family type not found.", "type_not_found");

            return McpHandlerHelpers.RunTransaction(doc, "MCP Create Plan View", () =>
            {
                var view = ViewPlan.Create(doc, vft.Id, level.Id);
                if (!string.IsNullOrWhiteSpace(name)) view.Name = name;
                return McpToolResult.Success(new JObject
                {
                    ["view_id"] = view.Id.Value,
                    ["name"] = view.Name,
                    ["level"] = level.Name,
                });
            });
        });
        return TaskResult(result);
    }
}

public sealed class CreateSectionViewHandler : ToolHandlerBase
{
    public override McpToolDescriptor Descriptor => Desc(
        "aec.create_section_view",
        "Create a section view through a bounding box.",
        Schema.ObjectWithRequired(
            [
                ("min", new JObject { ["type"] = "array", ["items"] = new JObject { ["type"] = "number" }, ["minItems"] = 3 }),
                ("max", new JObject { ["type"] = "array", ["items"] = new JObject { ["type"] = "number" }, ["minItems"] = 3 }),
                ("name", Schema.String("View name")),
            ],
            "min", "max"),
        ToolRiskClass.Write,
        requiresRevit: true);

    public override Task<McpToolResult> InvokeAsync(JObject arguments, HostContext context)
    {
        var min = McpHandlerHelpers.ParsePoint(arguments["min"]);
        var max = McpHandlerHelpers.ParsePoint(arguments["max"]);
        var name = arguments["name"]?.Value<string>();

        var result = context.Bridge.RunOnRevitThread(app =>
        {
            var doc = McpHandlerHelpers.GetDoc(app);
            if (doc == null) return McpHandlerHelpers.NoDocument();
            var vft = McpHandlerHelpers.FindViewFamilyType(doc, ViewFamily.Section);
            if (vft == null) return McpToolResult.Failure("Section view family type not found.", "type_not_found");

            return McpHandlerHelpers.RunTransaction(doc, "MCP Create Section View", () =>
            {
                var center = (min + max) * 0.5;
                var width = Math.Abs(max.X - min.X);
                var height = Math.Abs(max.Z - min.Z);
                var depth = Math.Abs(max.Y - min.Y);
                var transform = Transform.Identity;
                transform.Origin = center;
                var sectionBox = new BoundingBoxXYZ
                {
                    Transform = transform,
                    Min = new XYZ(-width / 2, -depth / 2, -height / 2),
                    Max = new XYZ(width / 2, depth / 2, height / 2),
                };
                var view = ViewSection.CreateSection(doc, vft.Id, sectionBox);
                if (!string.IsNullOrWhiteSpace(name)) view.Name = name;
                return McpToolResult.Success(new JObject
                {
                    ["view_id"] = view.Id.Value,
                    ["name"] = view.Name,
                });
            });
        });
        return TaskResult(result);
    }
}

public sealed class CreateSheetHandler : ToolHandlerBase
{
    public override McpToolDescriptor Descriptor => Desc(
        "aec.create_sheet",
        "Create a new sheet with a title block.",
        Schema.Object(
            ("sheet_number", Schema.String("Sheet number")),
            ("sheet_name", Schema.String("Sheet name")),
            ("title_block", Schema.String("Title block family/type name"))),
        ToolRiskClass.Write,
        requiresRevit: true);

    public override Task<McpToolResult> InvokeAsync(JObject arguments, HostContext context)
    {
        var sheetNumber = arguments["sheet_number"]?.Value<string>();
        var sheetName = arguments["sheet_name"]?.Value<string>();
        var titleBlock = arguments["title_block"]?.Value<string>();

        var result = context.Bridge.RunOnRevitThread(app =>
        {
            var doc = McpHandlerHelpers.GetDoc(app);
            if (doc == null) return McpHandlerHelpers.NoDocument();
            var symbol = McpHandlerHelpers.FindTitleBlock(doc, titleBlock);
            if (symbol == null) return McpToolResult.Failure("Title block not found.", "type_not_found");

            return McpHandlerHelpers.RunTransaction(doc, "MCP Create Sheet", () =>
            {
                if (!symbol.IsActive) symbol.Activate();
                var sheet = ViewSheet.Create(doc, symbol.Id);
                if (!string.IsNullOrWhiteSpace(sheetNumber)) sheet.SheetNumber = sheetNumber;
                if (!string.IsNullOrWhiteSpace(sheetName)) sheet.Name = sheetName;
                return McpToolResult.Success(new JObject
                {
                    ["sheet_id"] = sheet.Id.Value,
                    ["sheet_number"] = sheet.SheetNumber,
                    ["name"] = sheet.Name,
                });
            });
        });
        return TaskResult(result);
    }
}

public sealed class PlaceViewOnSheetHandler : ToolHandlerBase
{
    public override McpToolDescriptor Descriptor => Desc(
        "aec.place_view_on_sheet",
        "Place a view or schedule on a sheet at a point.",
        Schema.ObjectWithRequired(
            [
                ("sheet", Schema.String("Sheet number or name")),
                ("view", Schema.String("View or schedule name")),
                ("location", new JObject { ["type"] = "array", ["items"] = new JObject { ["type"] = "number" }, ["minItems"] = 2 }),
            ],
            "sheet", "view"),
        ToolRiskClass.Write,
        requiresRevit: true);

    public override Task<McpToolResult> InvokeAsync(JObject arguments, HostContext context)
    {
        var sheetRef = arguments["sheet"]?.Value<string>();
        var viewRef = arguments["view"]?.Value<string>();
        var location = McpHandlerHelpers.ParsePoint(arguments["location"], 0);

        var result = context.Bridge.RunOnRevitThread(app =>
        {
            var doc = McpHandlerHelpers.GetDoc(app);
            if (doc == null) return McpHandlerHelpers.NoDocument();
            var sheet = McpHandlerHelpers.FindSheetByName(doc, sheetRef);
            if (sheet == null) return McpToolResult.Failure("Sheet not found.", "sheet_not_found");
            var view = McpHandlerHelpers.FindViewByName(doc, viewRef)
                ?? McpHandlerHelpers.FindScheduleByName(doc, viewRef) as View;
            if (view == null) return McpToolResult.Failure("View not found.", "view_not_found");

            return McpHandlerHelpers.RunTransaction(doc, "MCP Place View On Sheet", () =>
            {
                var vp = Viewport.Create(doc, sheet.Id, view.Id, location);
                return McpToolResult.Success(new JObject
                {
                    ["viewport_id"] = vp?.Id.Value,
                    ["sheet_id"] = sheet.Id.Value,
                    ["view_id"] = view.Id.Value,
                });
            });
        });
        return TaskResult(result);
    }
}

public sealed class ApplyViewTemplateHandler : ToolHandlerBase
{
    public override McpToolDescriptor Descriptor => Desc(
        "aec.apply_view_template",
        "Apply a named view template to one or more views.",
        Schema.ObjectWithRequired(
            [
                ("template", Schema.String("View template name")),
                ("view_names", Schema.StringArray("View names to update")),
            ],
            "template", "view_names"),
        ToolRiskClass.Write,
        requiresRevit: true);

    public override Task<McpToolResult> InvokeAsync(JObject arguments, HostContext context)
    {
        var templateName = arguments["template"]?.Value<string>();
        var viewNames = (arguments["view_names"] as JArray ?? []).Select(t => t.Value<string>()).ToList();

        var result = context.Bridge.RunOnRevitThread(app =>
        {
            var doc = McpHandlerHelpers.GetDoc(app);
            if (doc == null) return McpHandlerHelpers.NoDocument();
            var template = new FilteredElementCollector(doc).OfClass(typeof(View))
                .Cast<View>().FirstOrDefault(v => v.IsTemplate && string.Equals(v.Name, templateName, StringComparison.OrdinalIgnoreCase));
            if (template == null) return McpToolResult.Failure("View template not found.", "template_not_found");

            return McpHandlerHelpers.RunTransaction(doc, "MCP Apply View Template", () =>
            {
                var updated = new JArray();
                foreach (var name in viewNames)
                {
                    var view = McpHandlerHelpers.FindViewByName(doc, name);
                    if (view == null || view.IsTemplate) continue;
                    view.ViewTemplateId = template.Id;
                    updated.Add(view.Id.Value);
                }
                return McpToolResult.Success(new JObject
                {
                    ["template"] = template.Name,
                    ["updated_view_ids"] = updated,
                });
            });
        });
        return TaskResult(result);
    }
}

public sealed class ListViewsAndSheetsHandler : ToolHandlerBase
{
    public override McpToolDescriptor Descriptor => Desc(
        "aec.list_views_and_sheets",
        "List views and sheets with types, levels, and template names.",
        Schema.Object(),
        requiresRevit: true);

    public override Task<McpToolResult> InvokeAsync(JObject arguments, HostContext context)
    {
        var result = context.Bridge.RunOnRevitThread(app =>
        {
            var doc = McpHandlerHelpers.GetDoc(app);
            if (doc == null) return McpHandlerHelpers.NoDocument();

            var views = new FilteredElementCollector(doc).OfClass(typeof(View)).Cast<View>()
                .Where(v => !v.IsTemplate)
                .Select(v =>
                {
                    var template = v.ViewTemplateId != ElementId.InvalidElementId
                        ? doc.GetElement(v.ViewTemplateId) as View
                        : null;
                    string levelName = null;
                    if (v is ViewPlan plan && plan.GenLevel != null)
                        levelName = plan.GenLevel.Name;
                    return new JObject
                    {
                        ["view_id"] = v.Id.Value,
                        ["name"] = v.Name,
                        ["view_type"] = v.ViewType.ToString(),
                        ["level"] = levelName,
                        ["template"] = template?.Name,
                    };
                }).ToList();

            var sheets = new FilteredElementCollector(doc).OfClass(typeof(ViewSheet)).Cast<ViewSheet>()
                .Select(s => new JObject
                {
                    ["sheet_id"] = s.Id.Value,
                    ["sheet_number"] = s.SheetNumber,
                    ["name"] = s.Name,
                }).ToList();

            return McpToolResult.Success(new JObject
            {
                ["views"] = new JArray(views),
                ["sheets"] = new JArray(sheets),
                ["view_count"] = views.Count,
                ["sheet_count"] = sheets.Count,
            });
        });
        return TaskResult(result);
    }
}

public sealed class IsolateCategoryInViewHandler : ToolHandlerBase
{
    public override McpToolDescriptor Descriptor => Desc(
        "aec.isolate_category_in_view",
        "Temporarily isolate one or more categories in the active or named view.",
        Schema.ObjectWithRequired(
            [
                ("categories", Schema.StringArray("Category names")),
                ("view", Schema.String("Optional view name; defaults to active view")),
            ],
            "categories"),
        ToolRiskClass.ViewOverride,
        requiresRevit: true);

    public override Task<McpToolResult> InvokeAsync(JObject arguments, HostContext context)
    {
        var categoryNames = (arguments["categories"] as JArray ?? []).Select(t => t.Value<string>()).ToList();
        var viewName = arguments["view"]?.Value<string>();

        var result = context.Bridge.RunOnRevitThread(app =>
        {
            var doc = McpHandlerHelpers.GetDoc(app);
            if (doc == null) return McpHandlerHelpers.NoDocument();
            var view = McpHandlerHelpers.FindViewByIdOrName(doc, viewName, app);
            if (view == null) return McpToolResult.Failure("View not found.", "view_not_found");

            var catIds = new List<ElementId>();
            foreach (var name in categoryNames)
            {
                var cat = McpHandlerHelpers.FindCategory(doc, name);
                if (cat != null) catIds.Add(cat.Id);
            }
            if (catIds.Count == 0) return McpToolResult.Failure("No valid categories found.", "category_not_found");

            return McpHandlerHelpers.RunTransaction(doc, "MCP Isolate Category", () =>
            {
                view.IsolateCategoriesTemporary(catIds);
                return McpToolResult.Success(new JObject
                {
                    ["view_id"] = view.Id.Value,
                    ["view_name"] = view.Name,
                    ["isolated_categories"] = new JArray(categoryNames),
                });
            });
        });
        return TaskResult(result);
    }
}

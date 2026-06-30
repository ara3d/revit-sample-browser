using Ara3D.Bowerbird.RevitSamples.AecAgent;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Ara3D.Bowerbird.RevitSamples;

public sealed class QueryElementsHandler : ToolHandlerBase
{
    public override McpToolDescriptor Descriptor => Desc(
        "aec.query_elements",
        "Structured query over category, class, level, name, and element ids.",
        Schema.Object(
            ("category", Schema.String("Built-in or custom category name")),
            ("class_name", Schema.String("Revit API class name, e.g. Wall, Room")),
            ("level", Schema.String("Level name")),
            ("name_contains", Schema.String("Case-insensitive name filter")),
            ("element_ids", new JObject { ["type"] = "array", ["items"] = new JObject { ["type"] = "integer" } }),
            ("limit", Schema.Integer("Maximum elements to return"))),
        requiresRevit: true);

    public override Task<McpToolResult> InvokeAsync(JObject arguments, HostContext context)
    {
        var spec = QuerySpec.FromArgs(arguments);
        var result = context.Bridge.RunOnRevitThread(app =>
        {
            var doc = app.ActiveUIDocument?.Document;
            if (doc == null)
                return McpToolResult.Failure("No document open", "no_document");

            var elements = QueryElements(doc, spec).Take(Math.Max(1, spec.Limit)).ToList();
            var items = elements.Select(e => ElementSummary(e)).ToList();
            return McpToolResult.Success(new JObject
            {
                ["count"] = items.Count,
                ["elements"] = new JArray(items),
            }, new JObject { ["truncated"] = elements.Count >= spec.Limit });
        });
        return TaskResult(result);
    }

    internal static IEnumerable<Element> QueryElements(Document doc, QuerySpec spec)
    {
        FilteredElementCollector collector = new(doc);

        if (spec.ElementIds is { Count: > 0 })
        {
            var ids = spec.ElementIds.Select(id => new ElementId(id)).ToList();
            return ids.Select(doc.GetElement).Where(e => e != null);
        }

        collector = collector.WhereElementIsNotElementType();

        if (!string.IsNullOrWhiteSpace(spec.ClassName))
        {
            var type = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(a =>
                {
                    try { return a.GetTypes(); }
                    catch { return []; }
                })
                .FirstOrDefault(t => string.Equals(t.Name, spec.ClassName, StringComparison.OrdinalIgnoreCase)
                    && typeof(Element).IsAssignableFrom(t));

            if (type != null)
                collector = collector.OfClass(type);
        }

        if (!string.IsNullOrWhiteSpace(spec.CategoryName))
        {
            var cat = doc.Settings.Categories
                .Cast<Category>()
                .FirstOrDefault(c => string.Equals(c.Name, spec.CategoryName, StringComparison.OrdinalIgnoreCase));
            if (cat != null)
                collector = collector.OfCategoryId(cat.Id);
        }

        IEnumerable<Element> results = collector.ToElements();
        if (!string.IsNullOrWhiteSpace(spec.LevelName))
        {
            results = results.Where(e =>
            {
                var levelId = e.LevelId;
                if (levelId == ElementId.InvalidElementId) return false;
                var level = doc.GetElement(levelId) as Level;
                return level != null && level.Name.IndexOf(spec.LevelName, StringComparison.OrdinalIgnoreCase) >= 0;
            });
        }

        if (!string.IsNullOrWhiteSpace(spec.NameContains))
        {
            results = results.Where(e =>
                (e.Name ?? "").IndexOf(spec.NameContains, StringComparison.OrdinalIgnoreCase) >= 0);
        }

        return results;
    }

    internal static JObject ElementSummary(Element e)
        => new()
        {
            ["element_id"] = e.Id.Value,
            ["unique_id"] = e.UniqueId,
            ["name"] = e.Name,
            ["category"] = e.Category?.Name,
            ["class"] = e.GetType().Name,
            ["level_id"] = e.LevelId == ElementId.InvalidElementId ? null : e.LevelId.Value,
        };
}

public sealed class ReadElementsHandler : ToolHandlerBase
{
    public override McpToolDescriptor Descriptor => Desc(
        "aec.read_elements",
        "Returns hydrated element summaries for the given element ids.",
        Schema.Object(
            ("element_ids", new JObject
            {
                ["type"] = "array",
                ["items"] = new JObject { ["type"] = "integer" },
                ["description"] = "Element ids to read",
            }),
            ("include_parameters", new JObject { ["type"] = "boolean" })),
        requiresRevit: true);

    public override Task<McpToolResult> InvokeAsync(JObject arguments, HostContext context)
    {
        var ids = arguments?["element_ids"] as JArray ?? [];
        var includeParams = arguments?["include_parameters"]?.Value<bool?>() ?? false;

        var result = context.Bridge.RunOnRevitThread(app =>
        {
            var doc = app.ActiveUIDocument?.Document;
            if (doc == null)
                return McpToolResult.Failure("No document open", "no_document");

            var elements = new JArray();
            foreach (var token in ids)
            {
                if (token.Type != JTokenType.Integer) continue;
                var e = doc.GetElement(new ElementId(token.Value<long>()));
                if (e == null) continue;

                var item = includeParams ? e.ToJson() : QueryElementsHandler.ElementSummary(e);
                elements.Add(item);
            }

            return McpToolResult.Success(new JObject { ["elements"] = elements });
        });
        return TaskResult(result);
    }
}

public sealed class ReadParametersHandler : ToolHandlerBase
{
    public override McpToolDescriptor Descriptor => Desc(
        "aec.read_parameters",
        "Returns parameter metadata and values for an element.",
        Schema.ObjectReq(("element_id", Schema.Integer("Element id to inspect")), "element_id"),
        requiresRevit: true);

    public override Task<McpToolResult> InvokeAsync(JObject arguments, HostContext context)
    {
        var elementId = arguments?["element_id"]?.Value<long?>();
        if (elementId == null)
            throw new McpProtocolException(-32602, "Missing required argument: element_id");

        var result = context.Bridge.RunOnRevitThread(app =>
        {
            var doc = app.ActiveUIDocument?.Document;
            if (doc == null)
                return McpToolResult.Failure("No document open", "no_document");

            var e = doc.GetElement(new ElementId(elementId.Value));
            if (e == null)
                return McpToolResult.Failure($"Element not found: {elementId}", "not_found");

            var parameters = new JArray();
            foreach (Parameter p in e.Parameters)
            {
                if (p?.Definition == null) continue;
                parameters.Add(new JObject
                {
                    ["name"] = p.Definition.Name,
                    ["storage_type"] = p.StorageType.ToString(),
                    ["is_read_only"] = p.IsReadOnly,
                    ["has_value"] = p.HasValue,
                    ["display_value"] = p.AsValueString(),
                    ["value"] = p.GetParameterMapValue(),
                });
            }

            return McpToolResult.Success(new JObject
            {
                ["element_id"] = elementId,
                ["name"] = e.Name,
                ["parameters"] = parameters,
            });
        });
        return TaskResult(result);
    }
}

public sealed class GetModelStatisticsHandler : ToolHandlerBase
{
    public override McpToolDescriptor Descriptor => Desc(
        "aec.get_model_statistics",
        "Returns counts by category, class, level, workset, and phase.",
        Schema.Object(),
        requiresRevit: true);

    public override Task<McpToolResult> InvokeAsync(JObject arguments, HostContext context)
    {
        var result = context.Bridge.RunOnRevitThread(app =>
        {
            var doc = app.ActiveUIDocument?.Document;
            if (doc == null)
                return McpToolResult.Failure("No document open", "no_document");

            var elements = doc.GetElements();
            var byCategory = elements.GroupBy(e => e.Category?.Name ?? "(none)")
                .OrderByDescending(g => g.Count())
                .Take(40)
                .ToDictionary(g => g.Key, g => g.Count());
            var byClass = elements.GroupBy(e => e.GetType().Name)
                .OrderByDescending(g => g.Count())
                .Take(40)
                .ToDictionary(g => g.Key, g => g.Count());
            var byLevel = elements
                .Where(e => e.LevelId != ElementId.InvalidElementId)
                .GroupBy(e => doc.GetElement(e.LevelId)?.Name ?? "(unknown)")
                .OrderByDescending(g => g.Count())
                .Take(40)
                .ToDictionary(g => g.Key, g => g.Count());

            return McpToolResult.Success(new JObject
            {
                ["total_elements"] = elements.Count,
                ["by_category"] = JObject.FromObject(byCategory),
                ["by_class"] = JObject.FromObject(byClass),
                ["by_level"] = JObject.FromObject(byLevel),
                ["room_count"] = doc.GetRooms().Count(),
                ["view_count"] = new FilteredElementCollector(doc).OfClass(typeof(View)).GetElementCount(),
            });
        });
        return TaskResult(result);
    }
}

internal static class ParameterMapExtensions
{
    public static string GetParameterMapValue(this Parameter p)
    {
        if (!p.HasValue) return null;
        return p.StorageType switch
        {
            StorageType.String => p.AsString(),
            StorageType.Integer => p.AsInteger().ToString(),
            StorageType.Double => p.AsValueString() ?? p.AsDouble().ToString("G17"),
            StorageType.ElementId => p.AsElementId().Value.ToString(),
            _ => p.AsValueString(),
        };
    }
}

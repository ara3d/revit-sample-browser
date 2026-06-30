using Ara3D.Bowerbird.RevitSamples.AecAgent;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Ara3D.Bowerbird.RevitSamples;

public sealed class SetParametersBulkHandler : ToolHandlerBase
{
    public override McpToolDescriptor Descriptor => Desc(
        "aec.set_parameters_bulk",
        "Set a parameter value on all elements matching a query.",
        Schema.ObjectWithRequired(
            [
                ("parameter", Schema.String("Parameter name")),
                ("value", Schema.String("Value to set")),
                ("category", Schema.String("Category name")),
                ("name_contains", Schema.String("Optional name filter")),
            ],
            "parameter", "value"),
        ToolRiskClass.Write,
        requiresRevit: true);

    public override Task<McpToolResult> InvokeAsync(JObject arguments, HostContext context)
    {
        var parameterName = arguments["parameter"]?.Value<string>();
        var value = arguments["value"];
        var categoryName = arguments["category"]?.Value<string>();
        var nameContains = arguments["name_contains"]?.Value<string>();

        var result = context.Bridge.RunOnRevitThread(app =>
        {
            var doc = McpHandlerHelpers.GetDoc(app);
            if (doc == null) return McpHandlerHelpers.NoDocument();

            IEnumerable<Element> elements = string.IsNullOrWhiteSpace(categoryName)
                ? new FilteredElementCollector(doc).WhereElementIsNotElementType().ToElements()
                : McpHandlerHelpers.ElementsByCategory(doc, categoryName);

            if (!string.IsNullOrWhiteSpace(nameContains))
                elements = elements.Where(e => (e.Name ?? "").IndexOf(nameContains, StringComparison.OrdinalIgnoreCase) >= 0);

            return McpHandlerHelpers.RunTransaction(doc, "MCP Set Parameters Bulk", () =>
            {
                var updated = new JArray();
                var failures = new JArray();
                foreach (var element in elements.Take(500))
                {
                    var param = element.LookupParameter(parameterName);
                    if (!McpHandlerHelpers.TrySetParameter(param, value, out var error))
                    {
                        failures.Add(new JObject { ["element_id"] = element.Id.Value, ["error"] = error });
                        continue;
                    }
                    updated.Add(element.Id.Value);
                }

                return McpToolResult.Success(new JObject
                {
                    ["parameter"] = parameterName,
                    ["updated_count"] = updated.Count,
                    ["updated_element_ids"] = updated,
                    ["failures"] = failures,
                });
            });
        });
        return TaskResult(result);
    }
}

public sealed class CopyParametersBetweenElementsHandler : ToolHandlerBase
{
    public override McpToolDescriptor Descriptor => Desc(
        "aec.copy_parameters_between_elements",
        "Copy parameter values from a source element to target elements.",
        Schema.ObjectWithRequired(
            [
                ("source_element_id", Schema.Integer("Source element id")),
                ("target_element_ids", new JObject { ["type"] = "array", ["items"] = new JObject { ["type"] = "integer" } }),
                ("parameter_names", Schema.StringArray("Parameter names to copy; copies all writable when omitted")),
            ],
            "source_element_id", "target_element_ids"),
        ToolRiskClass.Write,
        requiresRevit: true);

    public override Task<McpToolResult> InvokeAsync(JObject arguments, HostContext context)
    {
        var sourceId = new ElementId(arguments["source_element_id"]?.Value<long>() ?? 0);
        var targetIds = (arguments["target_element_ids"] as JArray ?? []).Select(t => new ElementId(t.Value<long>())).ToList();
        var parameterNames = (arguments["parameter_names"] as JArray ?? []).Select(t => t.Value<string>()).Where(s => !string.IsNullOrWhiteSpace(s)).ToList();

        var result = context.Bridge.RunOnRevitThread(app =>
        {
            var doc = McpHandlerHelpers.GetDoc(app);
            if (doc == null) return McpHandlerHelpers.NoDocument();
            var source = doc.GetElement(sourceId);
            if (source == null) return McpToolResult.Failure("Source element not found.", "not_found");

            return McpHandlerHelpers.RunTransaction(doc, "MCP Copy Parameters", () =>
            {
                var sourceParams = new Dictionary<string, Parameter>(StringComparer.OrdinalIgnoreCase);
                foreach (Parameter p in source.Parameters)
                {
                    if (p.Definition == null || p.IsReadOnly) continue;
                    var name = p.Definition.Name;
                    if (parameterNames.Count > 0 && !parameterNames.Contains(name, StringComparer.OrdinalIgnoreCase))
                        continue;
                    sourceParams[name] = p;
                }

                var updatedTargets = new JArray();
                foreach (var targetId in targetIds)
                {
                    var target = doc.GetElement(targetId);
                    if (target == null) continue;
                    var copied = 0;
                    foreach (var kv in sourceParams)
                    {
                        var targetParam = target.LookupParameter(kv.Key);
                        if (targetParam == null || targetParam.IsReadOnly) continue;
                        var src = kv.Value;
                        var ok = src.StorageType switch
                        {
                            StorageType.String => targetParam.Set(src.AsString()),
                            StorageType.Integer => targetParam.Set(src.AsInteger()),
                            StorageType.Double => targetParam.Set(src.AsDouble()),
                            StorageType.ElementId => targetParam.Set(src.AsElementId()),
                            _ => false,
                        };
                        if (ok) copied++;
                    }
                    updatedTargets.Add(new JObject { ["element_id"] = targetId.Value, ["parameters_copied"] = copied });
                }

                return McpToolResult.Success(new JObject { ["targets"] = updatedTargets });
            });
        });
        return TaskResult(result);
    }
}

public sealed class ExportParametersCsvHandler : ToolHandlerBase
{
    public override McpToolDescriptor Descriptor => Desc(
        "aec.export_parameters_csv",
        "Export parameter name/value pairs for a category to CSV.",
        Schema.ObjectWithRequired(
            [
                ("category", Schema.String("Category name")),
                ("output_path", Schema.String("Optional output CSV path")),
            ],
            "category"),
        requiresRevit: true);

    public override Task<McpToolResult> InvokeAsync(JObject arguments, HostContext context)
    {
        var categoryName = arguments["category"]?.Value<string>();
        var outputPath = arguments["output_path"]?.Value<string>();

        var result = context.Bridge.RunOnRevitThread(app =>
        {
            var doc = McpHandlerHelpers.GetDoc(app);
            if (doc == null) return McpHandlerHelpers.NoDocument();
            var elements = McpHandlerHelpers.ElementsByCategory(doc, categoryName).Take(1000).ToList();
            var path = McpHandlerHelpers.EnsureOutputPath(outputPath, McpHandlerHelpers.SanitizeFileName(categoryName + "_parameters"), ".csv");

            var lines = new List<string> { "ElementId,UniqueId,Name,Parameter,Value" };
            foreach (var element in elements)
            {
                foreach (Parameter p in element.Parameters)
                {
                    if (p.Definition == null) continue;
                    var value = McpHandlerHelpers.GetParameterText(p) ?? "";
                    lines.Add($"{element.Id.Value},{element.UniqueId},{Escape(element.Name)},{Escape(p.Definition.Name)},{Escape(value)}");
                }
            }

            File.WriteAllLines(path, lines);
            McpExportTracker.Record(path, new JObject { ["format"] = "parameters_csv", ["category"] = categoryName });
            return McpToolResult.Success(new JObject { ["path"] = path, ["element_count"] = elements.Count });
        });
        return TaskResult(result);
    }

    static string Escape(string value)
    {
        value ??= "";
        if (value.Contains(',')) return $"\"{value.Replace("\"", "\"\"")}\"";
        return value;
    }
}

public sealed class ImportParametersCsvHandler : ToolHandlerBase
{
    public override McpToolDescriptor Descriptor => Desc(
        "aec.import_parameters_csv",
        "Import parameter values from CSV; match elements by UniqueId or ElementId.",
        Schema.ObjectWithRequired(
            [("csv_path", Schema.String("Path to CSV file"))],
            "csv_path"),
        ToolRiskClass.Write,
        requiresRevit: true);

    public override Task<McpToolResult> InvokeAsync(JObject arguments, HostContext context)
    {
        var csvPath = arguments["csv_path"]?.Value<string>();
        if (string.IsNullOrWhiteSpace(csvPath) || !File.Exists(csvPath))
            return TaskResult(McpToolResult.Failure("csv_path not found.", "file_not_found"));

        var lines = File.ReadAllLines(csvPath).Skip(1).Where(l => !string.IsNullOrWhiteSpace(l)).ToList();

        var result = context.Bridge.RunOnRevitThread(app =>
        {
            var doc = McpHandlerHelpers.GetDoc(app);
            if (doc == null) return McpHandlerHelpers.NoDocument();

            return McpHandlerHelpers.RunTransaction(doc, "MCP Import Parameters CSV", () =>
            {
                var updated = 0;
                var failures = new JArray();
                foreach (var line in lines)
                {
                    var parts = ParseCsvLine(line);
                    if (parts.Count < 5) continue;
                    Element element = null;
                    if (long.TryParse(parts[0], out var id))
                        element = doc.GetElement(new ElementId(id));
                    element ??= doc.GetElement(parts[1]);
                    if (element == null)
                    {
                        failures.Add(new JObject { ["line"] = line, ["error"] = "element_not_found" });
                        continue;
                    }

                    var param = element.LookupParameter(parts[3]);
                    if (!McpHandlerHelpers.TrySetParameter(param, parts[4], out var error))
                    {
                        failures.Add(new JObject { ["element_id"] = element.Id.Value, ["parameter"] = parts[3], ["error"] = error });
                        continue;
                    }
                    updated++;
                }

                return McpToolResult.Success(new JObject { ["updated_count"] = updated, ["failures"] = failures });
            });
        });
        return TaskResult(result);
    }

    static List<string> ParseCsvLine(string line)
    {
        var parts = new List<string>();
        var current = "";
        var inQuotes = false;
        for (var i = 0; i < line.Length; i++)
        {
            var ch = line[i];
            if (ch == '"') inQuotes = !inQuotes;
            else if (ch == ',' && !inQuotes)
            {
                parts.Add(current);
                current = "";
            }
            else current += ch;
        }
        parts.Add(current);
        return parts;
    }
}

public sealed class ListProjectParametersHandler : ToolHandlerBase
{
    public override McpToolDescriptor Descriptor => Desc(
        "aec.list_project_parameters",
        "List project parameters with bound categories and storage types.",
        Schema.Object(),
        requiresRevit: true);

    public override Task<McpToolResult> InvokeAsync(JObject arguments, HostContext context)
    {
        var result = context.Bridge.RunOnRevitThread(app =>
        {
            var doc = McpHandlerHelpers.GetDoc(app);
            if (doc == null) return McpHandlerHelpers.NoDocument();

            var map = doc.ParameterBindings;
            var it = map.ForwardIterator();
            var items = new JArray();
            while (it.MoveNext())
            {
                if (it.Key is not Definition def) continue;
                var binding = it.Current as Binding;
                var categories = new JArray();
                if (binding is ElementBinding eb)
                {
                    foreach (Category cat in eb.Categories)
                        categories.Add(cat.Name);
                }

                items.Add(new JObject
                {
                    ["name"] = def.Name,
                    ["storage_type"] = (def as InternalDefinition)?.GetDataType()?.TypeId,
                    ["categories"] = categories,
                });
            }

            return McpToolResult.Success(new JObject { ["count"] = items.Count, ["parameters"] = items });
        });
        return TaskResult(result);
    }
}

public sealed class FindElementsByParameterValueHandler : ToolHandlerBase
{
    public override McpToolDescriptor Descriptor => Desc(
        "aec.find_elements_by_parameter_value",
        "Find elements where a parameter matches a value or regex.",
        Schema.ObjectWithRequired(
            [
                ("parameter", Schema.String("Parameter name")),
                ("value", Schema.String("Value or regex pattern")),
                ("category", Schema.String("Category name")),
                ("use_regex", new JObject { ["type"] = "boolean" }),
            ],
            "parameter", "value"),
        requiresRevit: true);

    public override Task<McpToolResult> InvokeAsync(JObject arguments, HostContext context)
    {
        var parameterName = arguments["parameter"]?.Value<string>();
        var value = arguments["value"]?.Value<string>();
        var categoryName = arguments["category"]?.Value<string>();
        var useRegex = arguments["use_regex"]?.Value<bool>() ?? false;

        var result = context.Bridge.RunOnRevitThread(app =>
        {
            var doc = McpHandlerHelpers.GetDoc(app);
            if (doc == null) return McpHandlerHelpers.NoDocument();

            IEnumerable<Element> elements = string.IsNullOrWhiteSpace(categoryName)
                ? new FilteredElementCollector(doc).WhereElementIsNotElementType().ToElements()
                : McpHandlerHelpers.ElementsByCategory(doc, categoryName);

            var matches = elements
                .Where(e => McpHandlerHelpers.ParameterMatches(e.LookupParameter(parameterName), value, useRegex))
                .Take(500)
                .Select(e => new JObject
                {
                    ["element_id"] = e.Id.Value,
                    ["name"] = e.Name,
                    ["value"] = McpHandlerHelpers.GetParameterText(e.LookupParameter(parameterName)),
                }).ToList();

            return McpToolResult.Success(new JObject { ["count"] = matches.Count, ["elements"] = new JArray(matches) });
        });
        return TaskResult(result);
    }
}

public sealed class ComputeParameterStatisticsHandler : ToolHandlerBase
{
    public override McpToolDescriptor Descriptor => Desc(
        "aec.compute_parameter_statistics",
        "Compute min/max/avg/count for a numeric parameter across a category.",
        Schema.ObjectWithRequired(
            [
                ("parameter", Schema.String("Parameter name")),
                ("category", Schema.String("Category name")),
            ],
            "parameter", "category"),
        requiresRevit: true);

    public override Task<McpToolResult> InvokeAsync(JObject arguments, HostContext context)
    {
        var parameterName = arguments["parameter"]?.Value<string>();
        var categoryName = arguments["category"]?.Value<string>();

        var result = context.Bridge.RunOnRevitThread(app =>
        {
            var doc = McpHandlerHelpers.GetDoc(app);
            if (doc == null) return McpHandlerHelpers.NoDocument();

            var values = McpHandlerHelpers.ElementsByCategory(doc, categoryName)
                .Select(e => e.LookupParameter(parameterName))
                .Where(p => p != null && p.HasValue && p.StorageType == StorageType.Double)
                .Select(p => p.AsDouble())
                .ToList();

            if (values.Count == 0)
                return McpToolResult.Success(new JObject { ["count"] = 0, ["message"] = "No numeric values found." });

            return McpToolResult.Success(new JObject
            {
                ["parameter"] = parameterName,
                ["category"] = categoryName,
                ["count"] = values.Count,
                ["min"] = values.Min(),
                ["max"] = values.Max(),
                ["avg"] = values.Average(),
            });
        });
        return TaskResult(result);
    }
}

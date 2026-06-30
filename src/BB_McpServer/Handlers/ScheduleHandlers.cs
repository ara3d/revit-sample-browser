using Ara3D.Bowerbird.RevitSamples.AecAgent;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ara3D.Bowerbird.RevitSamples;

internal static class ScheduleMcpHelpers
{
    public static JArray ReadScheduleRows(ViewSchedule schedule)
    {
        var table = schedule.GetTableData();
        var body = table.GetSectionData(SectionType.Body);
        var header = table.GetSectionData(SectionType.Header);
        var columns = new List<string>();
        if (header != null && header.NumberOfColumns > 0)
        {
            for (var c = 0; c < header.NumberOfColumns; c++)
                columns.Add(schedule.GetCellText(SectionType.Header, 0, c) ?? $"Col{c}");
        }
        else if (body != null)
        {
            for (var c = 0; c < body.NumberOfColumns; c++)
                columns.Add(schedule.Definition.GetField(c)?.GetName() ?? $"Col{c}");
        }

        var rows = new JArray();
        if (body == null) return rows;
        for (var r = 0; r < body.NumberOfRows; r++)
        {
            var row = new JObject();
            for (var c = 0; c < body.NumberOfColumns; c++)
            {
                var colName = c < columns.Count ? columns[c] : $"Col{c}";
                row[colName] = schedule.GetCellText(SectionType.Body, r, c);
            }
            rows.Add(row);
        }
        return rows;
    }

    public static JArray ListScheduleFields(ViewSchedule schedule)
    {
        var fields = new JArray();
        for (var i = 0; i < schedule.Definition.GetFieldCount(); i++)
        {
            var field = schedule.Definition.GetField(i);
            fields.Add(new JObject
            {
                ["index"] = i,
                ["name"] = field.GetName(),
                ["column_heading"] = field.ColumnHeading,
                ["hidden"] = field.IsHidden,
            });
        }
        return fields;
    }
}

public sealed class CreateScheduleHandler : ToolHandlerBase
{
    public override McpToolDescriptor Descriptor => Desc(
        "aec.create_schedule",
        "Create a view schedule for a category with specified fields.",
        Schema.ObjectWithRequired(
            [
                ("category", Schema.String("Category name, e.g. Rooms, Walls")),
                ("name", Schema.String("Schedule name")),
                ("fields", Schema.StringArray("Built-in or shared parameter field names")),
            ],
            "category"),
        ToolRiskClass.Write,
        requiresRevit: true);

    public override Task<McpToolResult> InvokeAsync(JObject arguments, HostContext context)
    {
        var categoryName = arguments["category"]?.Value<string>();
        var scheduleName = arguments["name"]?.Value<string>();
        var fields = (arguments["fields"] as JArray ?? []).Select(t => t.Value<string>()).Where(s => !string.IsNullOrWhiteSpace(s)).ToList();

        var result = context.Bridge.RunOnRevitThread(app =>
        {
            var doc = McpHandlerHelpers.GetDoc(app);
            if (doc == null) return McpHandlerHelpers.NoDocument();
            var cat = McpHandlerHelpers.FindCategory(doc, categoryName);
            if (cat == null) return McpToolResult.Failure("Category not found.", "category_not_found");

            return McpHandlerHelpers.RunTransaction(doc, "MCP Create Schedule", () =>
            {
                var schedule = ViewSchedule.CreateSchedule(doc, cat.Id);
                if (!string.IsNullOrWhiteSpace(scheduleName)) schedule.Name = scheduleName;
                var def = schedule.Definition;
                var added = new JArray();
                if (fields.Count == 0)
                {
                    fields = ["Name"];
                }

                foreach (var fieldName in fields)
                {
                    var field = TryAddField(def, doc, fieldName);
                    if (field != null) added.Add(fieldName);
                }

                return McpToolResult.Success(new JObject
                {
                    ["schedule_id"] = schedule.Id.Value,
                    ["name"] = schedule.Name,
                    ["category"] = cat.Name,
                    ["fields_added"] = added,
                });
            });
        });
        return TaskResult(result);
    }

    static ScheduleField TryAddField(ScheduleDefinition def, Document doc, string fieldName)
    {
        var schedulable = def.GetSchedulableFields()
            .FirstOrDefault(f => string.Equals(f.GetName(doc), fieldName, StringComparison.OrdinalIgnoreCase));
        if (schedulable == null) return null;
        return def.AddField(schedulable);
    }
}

public sealed class ReadScheduleDataHandler : ToolHandlerBase
{
    public override McpToolDescriptor Descriptor => Desc(
        "aec.read_schedule_data",
        "Read schedule rows as structured JSON.",
        Schema.ObjectWithRequired(
            [("schedule", Schema.String("Schedule name"))],
            "schedule"),
        requiresRevit: true);

    public override Task<McpToolResult> InvokeAsync(JObject arguments, HostContext context)
    {
        var scheduleName = arguments["schedule"]?.Value<string>();
        var result = context.Bridge.RunOnRevitThread(app =>
        {
            var doc = McpHandlerHelpers.GetDoc(app);
            if (doc == null) return McpHandlerHelpers.NoDocument();
            var schedule = McpHandlerHelpers.FindScheduleByName(doc, scheduleName);
            if (schedule == null) return McpToolResult.Failure("Schedule not found.", "schedule_not_found");

            var rows = ScheduleMcpHelpers.ReadScheduleRows(schedule);
            return McpToolResult.Success(new JObject
            {
                ["schedule"] = schedule.Name,
                ["row_count"] = rows.Count,
                ["rows"] = rows,
            });
        });
        return TaskResult(result);
    }
}

public sealed class AddScheduleFilterHandler : ToolHandlerBase
{
    public override McpToolDescriptor Descriptor => Desc(
        "aec.add_schedule_filter",
        "Add a filter to an existing schedule.",
        Schema.ObjectWithRequired(
            [
                ("schedule", Schema.String("Schedule name")),
                ("field", Schema.String("Field name")),
                ("filter_type", Schema.String("Equal, NotEqual, GreaterThan, LessThan, Contains")),
                ("value", Schema.String("Filter value")),
            ],
            "schedule", "field", "value"),
        ToolRiskClass.Write,
        requiresRevit: true);

    public override Task<McpToolResult> InvokeAsync(JObject arguments, HostContext context)
    {
        var scheduleName = arguments["schedule"]?.Value<string>();
        var fieldName = arguments["field"]?.Value<string>();
        var filterTypeName = arguments["filter_type"]?.Value<string>() ?? "Equal";
        var value = arguments["value"]?.Value<string>();

        var result = context.Bridge.RunOnRevitThread(app =>
        {
            var doc = McpHandlerHelpers.GetDoc(app);
            if (doc == null) return McpHandlerHelpers.NoDocument();
            var schedule = McpHandlerHelpers.FindScheduleByName(doc, scheduleName);
            if (schedule == null) return McpToolResult.Failure("Schedule not found.", "schedule_not_found");
            if (!Enum.TryParse<ScheduleFilterType>(filterTypeName, true, out var filterType))
                return McpToolResult.Failure("Invalid filter_type.", "invalid_filter");

            return McpHandlerHelpers.RunTransaction(doc, "MCP Add Schedule Filter", () =>
            {
                var def = schedule.Definition;
                var field = FindField(def, fieldName);
                if (field == null) return McpToolResult.Failure("Field not found on schedule.", "field_not_found");
                def.AddFilter(new ScheduleFilter(field.FieldId, filterType, value));
                return McpToolResult.Success(new JObject
                {
                    ["schedule"] = schedule.Name,
                    ["field"] = fieldName,
                    ["filter_type"] = filterType.ToString(),
                    ["value"] = value,
                });
            });
        });
        return TaskResult(result);
    }

    static ScheduleField FindField(ScheduleDefinition def, string fieldName)
    {
        for (var i = 0; i < def.GetFieldCount(); i++)
        {
            var field = def.GetField(i);
            if (string.Equals(field.GetName(), fieldName, StringComparison.OrdinalIgnoreCase)
                || string.Equals(field.ColumnHeading, fieldName, StringComparison.OrdinalIgnoreCase))
                return field;
        }
        return null;
    }
}

public sealed class AddScheduleSortHandler : ToolHandlerBase
{
    public override McpToolDescriptor Descriptor => Desc(
        "aec.add_schedule_sort",
        "Add a sort/group field to a schedule.",
        Schema.ObjectWithRequired(
            [
                ("schedule", Schema.String("Schedule name")),
                ("field", Schema.String("Field name")),
                ("ascending", new JObject { ["type"] = "boolean", ["description"] = "Sort ascending" }),
            ],
            "schedule", "field"),
        ToolRiskClass.Write,
        requiresRevit: true);

    public override Task<McpToolResult> InvokeAsync(JObject arguments, HostContext context)
    {
        var scheduleName = arguments["schedule"]?.Value<string>();
        var fieldName = arguments["field"]?.Value<string>();
        var ascending = arguments["ascending"]?.Value<bool>() ?? true;

        var result = context.Bridge.RunOnRevitThread(app =>
        {
            var doc = McpHandlerHelpers.GetDoc(app);
            if (doc == null) return McpHandlerHelpers.NoDocument();
            var schedule = McpHandlerHelpers.FindScheduleByName(doc, scheduleName);
            if (schedule == null) return McpToolResult.Failure("Schedule not found.", "schedule_not_found");

            return McpHandlerHelpers.RunTransaction(doc, "MCP Add Schedule Sort", () =>
            {
                var def = schedule.Definition;
                ScheduleField field = null;
                for (var i = 0; i < def.GetFieldCount(); i++)
                {
                    var f = def.GetField(i);
                    if (string.Equals(f.GetName(), fieldName, StringComparison.OrdinalIgnoreCase))
                    {
                        field = f;
                        break;
                    }
                }
                if (field == null) return McpToolResult.Failure("Field not found.", "field_not_found");

                var sort = new ScheduleSortGroupField(field.FieldId)
                {
                    ShowHeader = true,
                    ShowFooter = false,
                    SortOrder = ascending ? ScheduleSortOrder.Ascending : ScheduleSortOrder.Descending,
                };
                def.AddSortGroupField(sort);
                return McpToolResult.Success(new JObject
                {
                    ["schedule"] = schedule.Name,
                    ["field"] = fieldName,
                    ["ascending"] = ascending,
                });
            });
        });
        return TaskResult(result);
    }
}

public sealed class ExportScheduleCsvHandler : ToolHandlerBase
{
    public override McpToolDescriptor Descriptor => Desc(
        "aec.export_schedule_csv",
        "Export a named schedule to CSV on disk.",
        Schema.ObjectWithRequired(
            [
                ("schedule", Schema.String("Schedule name")),
                ("output_path", Schema.String("Optional output CSV path")),
            ],
            "schedule"),
        requiresRevit: true);

    public override Task<McpToolResult> InvokeAsync(JObject arguments, HostContext context)
    {
        var scheduleName = arguments["schedule"]?.Value<string>();
        var outputPath = arguments["output_path"]?.Value<string>();

        var result = context.Bridge.RunOnRevitThread(app =>
        {
            var doc = McpHandlerHelpers.GetDoc(app);
            if (doc == null) return McpHandlerHelpers.NoDocument();
            var schedule = McpHandlerHelpers.FindScheduleByName(doc, scheduleName);
            if (schedule == null) return McpToolResult.Failure("Schedule not found.", "schedule_not_found");

            var path = McpHandlerHelpers.EnsureOutputPath(outputPath, McpHandlerHelpers.SanitizeFileName(schedule.Name), ".csv");
            var rows = ScheduleMcpHelpers.ReadScheduleRows(schedule);
            var sb = new StringBuilder();
            if (rows.Count > 0 && rows[0] is JObject first)
            {
                var headers = first.Properties().Select(p => p.Name).ToList();
                sb.AppendLine(string.Join(",", headers.Select(EscapeCsv)));
                foreach (JObject row in rows)
                    sb.AppendLine(string.Join(",", headers.Select(h => EscapeCsv(row[h]?.ToString()))));
            }

            File.WriteAllText(path, sb.ToString());
            McpExportTracker.Record(path, new JObject { ["format"] = "csv", ["schedule"] = schedule.Name });
            return McpToolResult.Success(new JObject { ["path"] = path, ["row_count"] = rows.Count });
        });
        return TaskResult(result);
    }

    static string EscapeCsv(string value)
    {
        value ??= "";
        if (value.Contains(',') || value.Contains('"') || value.Contains('\n'))
            return $"\"{value.Replace("\"", "\"\"")}\"";
        return value;
    }
}

public sealed class ExportScheduleHtmlHandler : ToolHandlerBase
{
    public override McpToolDescriptor Descriptor => Desc(
        "aec.export_schedule_html",
        "Export a named schedule to HTML on disk.",
        Schema.ObjectWithRequired(
            [
                ("schedule", Schema.String("Schedule name")),
                ("output_path", Schema.String("Optional output HTML path")),
            ],
            "schedule"),
        requiresRevit: true);

    public override Task<McpToolResult> InvokeAsync(JObject arguments, HostContext context)
    {
        var scheduleName = arguments["schedule"]?.Value<string>();
        var outputPath = arguments["output_path"]?.Value<string>();

        var result = context.Bridge.RunOnRevitThread(app =>
        {
            var doc = McpHandlerHelpers.GetDoc(app);
            if (doc == null) return McpHandlerHelpers.NoDocument();
            var schedule = McpHandlerHelpers.FindScheduleByName(doc, scheduleName);
            if (schedule == null) return McpToolResult.Failure("Schedule not found.", "schedule_not_found");

            var path = McpHandlerHelpers.EnsureOutputPath(outputPath, McpHandlerHelpers.SanitizeFileName(schedule.Name), ".html");
            var rows = ScheduleMcpHelpers.ReadScheduleRows(schedule);
            var sb = new StringBuilder();
            sb.AppendLine("<html><head><meta charset=\"utf-8\"/><title>").Append(schedule.Name).AppendLine("</title></head><body>");
            sb.AppendLine($"<h1>{System.Net.WebUtility.HtmlEncode(schedule.Name)}</h1><table border=\"1\"><tbody>");
            var wroteHeader = false;
            foreach (JObject row in rows)
            {
                if (!wroteHeader)
                {
                    sb.Append("<tr>");
                    foreach (var prop in row.Properties())
                        sb.Append("<th>").Append(System.Net.WebUtility.HtmlEncode(prop.Name)).Append("</th>");
                    sb.AppendLine("</tr>");
                    wroteHeader = true;
                }
                sb.Append("<tr>");
                foreach (var prop in row.Properties())
                    sb.Append("<td>").Append(System.Net.WebUtility.HtmlEncode(prop.Value?.ToString())).Append("</td>");
                sb.AppendLine("</tr>");
            }
            sb.AppendLine("</tbody></table></body></html>");
            File.WriteAllText(path, sb.ToString());
            McpExportTracker.Record(path, new JObject { ["format"] = "html", ["schedule"] = schedule.Name });
            return McpToolResult.Success(new JObject { ["path"] = path, ["row_count"] = rows.Count });
        });
        return TaskResult(result);
    }
}

public sealed class ListSchedulesHandler : ToolHandlerBase
{
    public override McpToolDescriptor Descriptor => Desc(
        "aec.list_schedules",
        "List all schedules in the document with field names.",
        Schema.Object(),
        requiresRevit: true);

    public override Task<McpToolResult> InvokeAsync(JObject arguments, HostContext context)
    {
        var result = context.Bridge.RunOnRevitThread(app =>
        {
            var doc = McpHandlerHelpers.GetDoc(app);
            if (doc == null) return McpHandlerHelpers.NoDocument();

            var schedules = new FilteredElementCollector(doc).OfClass(typeof(ViewSchedule)).Cast<ViewSchedule>()
                .Select(s => new JObject
                {
                    ["schedule_id"] = s.Id.Value,
                    ["name"] = s.Name,
                    ["fields"] = ScheduleMcpHelpers.ListScheduleFields(s),
                }).ToList();

            return McpToolResult.Success(new JObject
            {
                ["count"] = schedules.Count,
                ["schedules"] = new JArray(schedules),
            });
        });
        return TaskResult(result);
    }
}

public sealed class DuplicateScheduleHandler : ToolHandlerBase
{
    public override McpToolDescriptor Descriptor => Desc(
        "aec.duplicate_schedule",
        "Duplicate an existing schedule.",
        Schema.ObjectWithRequired(
            [
                ("schedule", Schema.String("Source schedule name")),
                ("name", Schema.String("Optional new schedule name")),
            ],
            "schedule"),
        ToolRiskClass.Write,
        requiresRevit: true);

    public override Task<McpToolResult> InvokeAsync(JObject arguments, HostContext context)
    {
        var scheduleName = arguments["schedule"]?.Value<string>();
        var newName = arguments["name"]?.Value<string>();

        var result = context.Bridge.RunOnRevitThread(app =>
        {
            var doc = McpHandlerHelpers.GetDoc(app);
            if (doc == null) return McpHandlerHelpers.NoDocument();
            var schedule = McpHandlerHelpers.FindScheduleByName(doc, scheduleName);
            if (schedule == null) return McpToolResult.Failure("Schedule not found.", "schedule_not_found");

            return McpHandlerHelpers.RunTransaction(doc, "MCP Duplicate Schedule", () =>
            {
                var dupId = schedule.Duplicate(ViewDuplicateOption.Duplicate);
                var dup = doc.GetElement(dupId) as ViewSchedule;
                if (!string.IsNullOrWhiteSpace(newName) && dup != null)
                    dup.Name = newName;
                return McpToolResult.Success(new JObject
                {
                    ["source_schedule"] = schedule.Name,
                    ["schedule_id"] = dup?.Id.Value,
                    ["name"] = dup?.Name,
                });
            });
        });
        return TaskResult(result);
    }
}

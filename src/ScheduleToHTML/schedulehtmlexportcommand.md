# ScheduleHtmlExportCommand

| Field | Value |
|-------|-------|
| **Sample** | ScheduleToHTML |
| **Class** | `ScheduleHtmlExportCommand` |
| **Source** | `src/ScheduleToHTML/ScheduleHTMLExportCommand.cs` |
| **MCP rating** | 5/5 |

Exports the active view schedule to an HTML file using `ScheduleHtmlExporter`.

## What it demonstrates

- Requiring `commandData.View` to be a `ViewSchedule`
- `ScheduleHtmlExporter.ExportToHtml` with interactive versus journal-playback mode
- Read-only export suitable for reporting and external viewing

## Prerequisites

- Active view must be a schedule (not a plan, 3D view, etc.)

## User interaction

- Interactive mode may prompt for save location; non-interactive during journal playback

## MCP notes

- Proposed tool: `revit_export_schedule_html`
- Parameters: `schedule_view_id`, `output_path`, optional `interactive` flag
- Returns: output file path and row/column counts if available
- MCP descriptor: `src/ScheduleToHTML/schedulehtmlexportcommand.json`

## See also

- MCP descriptor: `src/ScheduleToHTML/schedulehtmlexportcommand.json`
- Exporter: `src/ScheduleToHTML/ScheduleHTMLExporter.cs`

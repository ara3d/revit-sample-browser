# PanelScheduleExport

| Field | Value |
|-------|-------|
| **Sample** | PanelSchedule |
| **Class** | `PanelScheduleExport` |
| **Source** | `src/PanelSchedule/CS/PanelScheduleExport.cs` |
| **SDK ReadMe** | `src/PanelSchedule/CS/ReadMe_PanelSchedule.rtf` |
| **MCP rating** | 5/5 |

Exports every non-template `PanelScheduleView` in the document to CSV or HTML and opens the output file.

## What it demonstrates

- Collecting `PanelScheduleView` elements with `FilteredElementCollector` and skipping templates via `IsPanelScheduleTemplate`
- `CsvTranslator` and `HtmlTranslator` implementing a shared `Translator.Export` pipeline
- Per-view `TaskDialog` choosing export format; `Process.Start` to open the result

## Prerequisites

- At least one panel schedule instance view in the project

## User interaction

- Task dialog per schedule for CSV vs HTML; warns if no schedules exist
- Export translators are callable without dialogs if format is specified

## MCP notes

- Proposed tool: `revit_export_panel_schedules`
- Parameters: `format` (`csv` | `html`), optional `schedule_view_ids[]`, `output_directory`
- Returns: list of exported file paths
- MCP descriptor: `docs/mcp/PanelSchedule/panelscheduleexport.json`

## See also

- MCP descriptor: `docs/mcp/PanelSchedule/panelscheduleexport.json`

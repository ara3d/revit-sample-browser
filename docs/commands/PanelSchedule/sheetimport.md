# SheetImport

| Field | Value |
|-------|-------|
| **Sample** | PanelSchedule |
| **Class** | `SheetImport` |
| **Source** | `src/PanelSchedule/CS/SheetImport.cs` |
| **SDK ReadMe** | `src/PanelSchedule/CS/ReadMe_PanelSchedule.rtf` |
| **MCP rating** | 4/5 |

Places all panel schedule instance views onto the active sheet, spacing them horizontally by each instance width.

## What it demonstrates

- Requiring `doc.ActiveView` as a `ViewSheet`
- `PanelScheduleSheetInstance.Create` with incremental `Origin` along X using bounding box width
- Skipping template schedule views with `IsPanelScheduleTemplate`

## Prerequisites

- Active view must be a sheet
- Panel schedule instance views exist in the document

## User interaction

- No dialog; fails with a message if the active view is not a sheet
- Fully automatable with `sheet_view_id` parameter instead of active view

## MCP notes

- Proposed tool: `revit_place_panel_schedules_on_sheet`
- Parameters: `sheet_view_id`, optional `schedule_view_ids[]`, optional `start_origin`
- Returns: created `PanelScheduleSheetInstance` element ids and positions
- MCP descriptor: `docs/mcp/PanelSchedule/sheetimport.json`

## See also

- MCP descriptor: `docs/mcp/PanelSchedule/sheetimport.json`

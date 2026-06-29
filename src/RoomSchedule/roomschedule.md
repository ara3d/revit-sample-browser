# Command

| Field | Value |
|-------|-------|
| **Sample** | RoomSchedule |
| **Class** | `Command` |
| **Source** | `src/RoomSchedule/CS/Command.cs` |
| **MCP rating** | 4/5 |

Compares rooms in the Revit model with rows from an external spreadsheet and creates or updates rooms from the data source.

## What it demonstrates

- `RoomScheduleForm` loading Excel data via `RoomsData` and ODBC-style table access
- Level and phase binding for new room placement
- Side-by-side comparison of spreadsheet rows versus existing `Room` elements

## Prerequisites

- Spreadsheet data source configured in the form (sample uses .xls workflow)
- Levels and phases in the document matching import data

## User interaction

- Modal `RoomScheduleForm` with grid comparison and import actions
- Import logic would need file path and mapping parameters for MCP

## MCP notes

- Proposed tool: `revit_import_rooms_from_spreadsheet`
- Parameters: `file_path`, `worksheet_name`, `level_id`, `phase_id`, column mapping
- Returns: created/updated room element ids and mismatch report
- MCP descriptor: `src/RoomSchedule/roomschedule.json`

## See also

- MCP descriptor: `src/RoomSchedule/roomschedule.json`

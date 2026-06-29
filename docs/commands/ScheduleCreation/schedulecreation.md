# Command

| Field | Value |
|-------|-------|
| **Sample** | ScheduleCreation |
| **Class** | `Command` |
| **Source** | `src/ScheduleCreation/CS/Command.cs` |
| **MCP rating** | 4/5 |

Creates a wall category view schedule with fields, filters, and sorting, then places it on a new sheet.

## What it demonstrates

- `ScheduleCreationUtility.CreateAndAddSchedules` inside a `TransactionGroup`
- `ViewSchedule.CreateSchedule` for `OST_Walls` with programmatic field, filter, and sort setup
- `ScheduleSheetInstance.Create` on a new sheet with title block placement

## Prerequisites

- Project with wall types/instances and at least one title block family loaded

## User interaction

- No dialog; runs the full create-and-sheet workflow automatically

## MCP notes

- Proposed tool: `revit_create_wall_schedule_on_sheet`
- Parameters: optional `category_id`, field list, filter rules, sheet name
- Returns: schedule view id and sheet id
- MCP descriptor: `docs/mcp/ScheduleCreation/schedulecreation.json`

## See also

- MCP descriptor: `docs/mcp/ScheduleCreation/schedulecreation.json`
- Utility: `src/ScheduleCreation/CS/ScheduleCreationUtility.cs`

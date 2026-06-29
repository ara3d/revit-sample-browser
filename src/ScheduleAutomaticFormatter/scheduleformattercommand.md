# ScheduleFormatterCommand

| Field | Value |
|-------|-------|
| **Sample** | ScheduleAutomaticFormatter |
| **Class** | `ScheduleFormatterCommand` |
| **Source** | `src/ScheduleAutomaticFormatter/CS/ScheduleFormatterCommand.cs` |
| **MCP rating** | 4/5 |

Formats columns on the active schedule and registers an updater so formatting is reapplied when the schedule changes.

## What it demonstrates

- `ScheduleFormatter.FormatScheduleColumns` adjusting active `ViewSchedule` layout
- Extensible Storage schema `ScheduleFormatterFlag` marking formatted schedules
- `UpdaterRegistry` with `ExtensibleStorageFilter` trigger on `ViewSchedule` changes

## Prerequisites

- Command run with active view set to a `ViewSchedule`

## User interaction

- No dialog; operates on the current schedule view
- Updater persists for the session after first run

## MCP notes

- Proposed tool: `revit_format_schedule_columns`
- Parameters: `schedule_view_id`, optional column width or alignment rules
- Returns: confirmation and updater registration status
- MCP descriptor: `src/ScheduleAutomaticFormatter/scheduleformattercommand.json`

## See also

- MCP descriptor: `src/ScheduleAutomaticFormatter/scheduleformattercommand.json`

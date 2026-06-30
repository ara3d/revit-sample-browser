# WriteDoorMarksCommand

| Field | Value |
|-------|-------|
| **Sample** | PathOfTravelDoors |
| **Class** | `WriteDoorMarksCommand` |
| **Source** | `src/PathOfTravelDoors/WriteDoorMarksCommand.cs` |
| **Origin** | [jeremytammik/PathOfTravelDoors](https://github.com/jeremytammik/PathOfTravelDoors) (MIT) |
| **MCP rating** | 4/5 |

Writes comma-separated door marks crossed by each selected path of travel into that path element's Comments parameter (`ALL_MODEL_INSTANCE_COMMENTS`).

## What it demonstrates

- Reuses `PathOfTravelDoorFinder` from the list-doors workflow
- Batch update of path-of-travel instance parameters inside a single transaction
- Practical workaround for path-of-travel schedule tables that omit intermediate doors

## Prerequisites

- Same as `Command` (plan view, 3D view, path-of-travel elements)
- Path-of-travel Comments parameter must be writable

## User interaction

- Uses pre-selection or interactive pick of path(s) of travel
- Shows a summary `TaskDialog` with the number of updated paths

## MCP notes

- Proposed tool: `revit_write_path_of_travel_door_marks`
- Parameters: `path_of_travel_ids` (array of integers)
- Returns: updated path ids and written mark strings
- MCP descriptor: `src/PathOfTravelDoors/write-door-marks.json`

## See also

- MCP descriptor: `src/PathOfTravelDoors/write-door-marks.json`
- `Command` — read-only door mark report

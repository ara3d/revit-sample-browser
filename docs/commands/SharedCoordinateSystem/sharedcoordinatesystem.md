# Command

| Field | Value |
|-------|-------|
| **Sample** | SharedCoordinateSystem |
| **Class** | `Command` |
| **Source** | `src/SharedCoordinateSystem/CS/Command.cs` |
| **SDK ReadMe** | `src/SharedCoordinateSystem/CS/ReadMe_SharedCoordinateSystem.rtf` |
| **MCP rating** | 5/5 |

Reads and edits shared project location data—offsets, angle to true north, elevation, and named locations—for the active Revit project.

## What it demonstrates

- Enumerating `ProjectLocations` and reading `ProjectPosition` via `GetProjectPosition`
- Setting position with `SetProjectPosition`, duplicating locations with `ProjectLocation.Duplicate`
- Switching `Document.ActiveProjectLocation` and binding city/time-zone data in `CoordinateSystemDataForm`
- Unit conversion helpers in `UnitConversion` and `PlaceInfo`

## Prerequisites

- Project document with at least one project location

## User interaction

- `CoordinateSystemDataForm` and `DuplicateForm` drive all edits; core `CoordinateSystemData` methods are separable for automation

## MCP notes

- Proposed tools: `revit_list_project_locations` (read) and `revit_set_project_location` (write)
- Parameters: `location_name`, optional `east_west`, `north_south`, `elevation`, `angle_from_north`
- Returns: location names and offset values
- MCP descriptor: `docs/mcp/SharedCoordinateSystem/sharedcoordinatesystem.json`

## See also

- MCP descriptor: `docs/mcp/SharedCoordinateSystem/sharedcoordinatesystem.json`

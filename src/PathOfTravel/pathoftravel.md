# Command

| Field | Value |
|-------|-------|
| **Sample** | PathOfTravel |
| **Class** | `Command` |
| **Source** | `src/PathOfTravel/Command.cs` |
| **MCP rating** | 3/5 |

Creates egress `PathOfTravel` elements on a floor plan from room points to doors using bulk or single-room workflows.

## What it demonstrates

- `Autodesk.Revit.DB.Analysis.PathOfTravel.CreateMapped` and `CreateMultiple` on a `ViewPlan`
- Near-corner room point extraction by offsetting boundary segments 1.5 ft
- Selection filters for rooms and doors; `PathCreateOptions` chosen in `CreateForm`
- Performance timing and success/failure reporting via `PathOfTravelCalculationStatus`

## Prerequisites

- Active view must be a floor plan with rooms and doors on the level

## User interaction

- `CreateForm` picks one of three generation modes; some modes require room and door picks
- Bulk all-rooms/all-doors mode runs without further picks after dialog confirmation

## MCP notes

- Proposed tool: `revit_create_paths_of_travel`
- Parameters: `view_id`, `mode` (`room_corners_to_door`, `all_room_centers_to_door`, `all_corners_to_all_doors`), optional `room_ids[]`, `door_ids[]`
- Returns: created path ids, failure count, and elapsed milliseconds
- Refactor: replace `CreateForm` and interactive picks with explicit ids
- MCP descriptor: `src/PathOfTravel/pathoftravel.json`

## See also

- MCP descriptor: `src/PathOfTravel/pathoftravel.json`

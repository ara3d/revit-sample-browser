# Command

| Field | Value |
|-------|-------|
| **Sample** | CropViewToRoom |
| **Class** | `Command` |
| **Source** | `src/CropViewToRoom/Command.cs` |
| **Origin** | [jeremytammik/CropViewToRoom](https://github.com/jeremytammik/CropViewToRoom) (MIT) |
| **MCP rating** | 4/5 |

For each level, duplicates the associated plan view once per room and sets a crop region shaped to the room boundary offset outward by wall thickness.

## What it demonstrates

- `Level.FindAssociatedPlanViewId` to locate the default plan view per level
- `View.Duplicate(ViewDuplicateOption.AsDependent)` to create room-specific plan views
- `Room.GetBoundarySegments` and `CurveLoop.CreateViaOffset` for outward boundary offset
- `ViewCropRegionShapeManager.SetCropShape` with tessellated line loops
- Per-segment wall thickness heuristics for walls and room separation lines

## Prerequisites

- Project with placed, bounded rooms on levels that have associated plan views
- Rooms must expose a valid first boundary loop (holes and disjoint parts are ignored)

## User interaction

- Headless batch command; shows a summary `TaskDialog` with processed, created, and skipped counts
- Skips unbounded rooms, levels without associated plan views, and rooms whose crop shape is rejected by Revit

## MCP notes

- Proposed tool: `revit_create_room_cropped_plan_views`
- Optional filters: `level_ids`, `room_ids`; optional `wall_offset_factor` and `view_name_suffix`
- Returns created view element ids and skip reasons
- Re-running on the same day may fail on duplicate view names because the command appends the current date
- MCP descriptor: `src/CropViewToRoom/crop-view-to-room.json`

## See also

- MCP descriptor: `src/CropViewToRoom/crop-view-to-room.json`
- `TBC_CropToRoom` — crops the active **3D** view to one room's bounding box (different workflow)
- [ElementOutline companion add-in](https://github.com/jeremytammik/ElementOutline) for outer room outline geometry

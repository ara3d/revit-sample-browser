# Command

| Field | Value |
|-------|-------|
| **Sample** | SpatialElementGeometryCalculator |
| **Class** | `Command` |
| **Source** | `src/SpatialElementGeometryCalculator/Command.cs` |
| **Origin** | [jeremytammik/SpatialElementGeometryCalculator](https://github.com/jeremytammik/SpatialElementGeometryCalculator) (MIT) |
| **MCP rating** | 4/5 |

Computes net, opening, and gross wall area for every placed room using spatial element geometry and opening deductions.

## What it demonstrates

- `Autodesk.Revit.DB.SpatialElementGeometryCalculator` with finish-location boundary options
- `SpatialElementGeometryResults.GetBoundaryFaceInfo` for side subfaces bounding walls
- `HostObject.FindInserts` to locate doors, windows, and embedded walls
- `ExporterIFCUtils.GetInstanceCutoutFromWall` for family-instance opening cutout area
- Boolean solid intersection for wall-as-opening area in stacked and curtain scenarios
- Aggregation by room, wall type, and outer-layer material (square meters)

## Prerequisites

- Project with placed rooms that have non-zero area
- Rooms bounded by standard (non-curtain) walls for meaningful paint-area totals

## User interaction

- Shows three sequential `TaskDialog` summaries (by room, by wall type, by material)
- Read-only; no model changes unless debug DirectShape visualization is enabled
- Core calculation loop is headless-friendly; dialogs would be replaced for MCP use

## MCP notes

- Proposed tool: `revit_room_wall_net_area`
- Optional filters: `room_ids`, `level_id`
- Returns grouped net, opening, and gross wall areas in m²
- MCP descriptor: `src/SpatialElementGeometryCalculator/spatial-element-geometry-calculator.json`

## See also

- MCP descriptor: `src/SpatialElementGeometryCalculator/spatial-element-geometry-calculator.json`
- Related: `RoofsRooms` (spatial geometry for roofs), `TBC_RoomWallAdjacency` (boundary-segment approach)

# Command

| Field | Value |
|-------|-------|
| **Sample** | RoomVolumeDirectShape |
| **Class** | `Command` |
| **Source** | `src/RoomVolumeDirectShape/Command.cs` |
| **Origin** | [jeremytammik/RoomVolumeDirectShape](https://github.com/jeremytammik/RoomVolumeDirectShape) (MIT) |
| **MCP rating** | 4/5 |

Creates Generic Model `DirectShape` volumes for placed rooms so room geometry appears in 3D views and Forge exports, with all room parameters copied to the shape Comments field as JSON.

## What it demonstrates

- `Room.ClosedShell` to obtain the room volume solid
- `SolidUtils.TessellateSolidOrShell` and `TessellatedShapeBuilder` to rebuild geometry suitable for downstream viewers
- `DirectShape.CreateElement` with `OST_GenericModel` (rooms category cannot be used)
- Serializing element parameters to a JSON dictionary stored in `ALL_MODEL_INSTANCE_COMMENTS`
- Linking shapes to source rooms via `ApplicationDataId` (`UniqueId`)

## Prerequisites

- Project with placed rooms that have non-zero area and valid closed-shell geometry

## User interaction

- Headless batch command; shows a summary `TaskDialog` with created and skipped counts
- Skips unplaced rooms, zero-area rooms, and rooms whose closed shell cannot be tessellated

## MCP notes

- Proposed tool: `revit_create_room_volume_direct_shapes`
- Optional filters: `room_ids`, `level_id`
- Returns created DirectShape element ids and skip reasons
- Re-running creates additional shapes; deduplication by `ApplicationDataId` is a future enhancement
- MCP descriptor: `src/RoomVolumeDirectShape/room-volume-direct-shape.json`

## See also

- MCP descriptor: `src/RoomVolumeDirectShape/room-volume-direct-shape.json`
- [Building Coder: Generate DirectShape element to represent room volume](https://thebuildingcoder.typepad.com/blog/2019/05/generate-directshape-element-to-represent-room-volume.html)
- Related: `SpatialElementGeometryCalculator` (room boundary geometry analysis), `GeometryAPI/BRepBuilderExample/CreateCube` (DirectShape creation)

# CreateDirectShapeSimpleCommand

| Field | Value |
|-------|-------|
| **Sample** | DirectShapeFromFace |
| **Class** | `CreateDirectShapeSimpleCommand` |
| **Source** | `src/DirectShapeFromFace/CreateDirectShapeSimpleCommand.cs` |
| **Origin** | [jeremytammik/DirectShapeFromFace](https://github.com/jeremytammik/DirectShapeFromFace) (MIT) |
| **MCP rating** | 4/5 |

Creates a Generic Model `DirectShape` from a user-picked face. When the host is a `FamilyInstance`, mesh vertices are transformed with `GetTotalTransform()` before building the tessellated shape.

## What it demonstrates

- Face triangulation and `TessellatedShapeBuilder` for DirectShape geometry
- Applying `FamilyInstance.GetTotalTransform()` to face mesh coordinates
- Linking the shape to the source element via `ApplicationDataId` (`UniqueId`)

## Prerequisites

- Model element with a pickable face in the active view

## User interaction

- Prompts for a single face pick; cancellation returns `Cancelled`
- Shows a summary `TaskDialog` with the created DirectShape id

## MCP notes

- Proposed tool: `revit_create_direct_shape_from_face_simple`
- Best starting point among the three variants for typical family faces
- MCP descriptor: `src/DirectShapeFromFace/create-direct-shape-simple.json`

## See also

- MCP descriptor: `src/DirectShapeFromFace/create-direct-shape-simple.json`
- `CreateDirectShapeInitialCommand` — LocationPoint offset variant
- `CreateDirectShapeCommand` — advanced nested-family transform and sketch-plane reuse
- `RoomVolumeDirectShape` — batch DirectShape creation from room closed shells

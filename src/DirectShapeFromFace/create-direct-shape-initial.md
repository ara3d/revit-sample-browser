# CreateDirectShapeInitialCommand

| Field | Value |
|-------|-------|
| **Sample** | DirectShapeFromFace |
| **Class** | `CreateDirectShapeInitialCommand` |
| **Source** | `src/DirectShapeFromFace/CreateDirectShapeInitialCommand.cs` |
| **Origin** | [jeremytammik/DirectShapeFromFace](https://github.com/jeremytammik/DirectShapeFromFace) (MIT) |
| **MCP rating** | 3/5 |

Creates a Generic Model `DirectShape` from a user-picked element face using the original "initial" tessellated shape builder workflow. Face triangles are offset by the host element `LocationPoint` when present.

## What it demonstrates

- `PickObject(ObjectType.Face)` to obtain a face reference
- `Element.GetGeometryObjectFromReference` and `Face.Triangulate`
- Building a `TessellatedShapeBuilder` face set from mesh triangles
- `DirectShape.CreateElement` with `OST_GenericModel`

## Prerequisites

- Model element with a pickable face in the active view

## User interaction

- Prompts for a single face pick; cancellation returns `Cancelled`
- Shows a summary `TaskDialog` with the created DirectShape id

## MCP notes

- Proposed tool: `revit_create_direct_shape_from_face_initial`
- Requires refactoring for headless use (stable face reference input)
- MCP descriptor: `src/DirectShapeFromFace/create-direct-shape-initial.json`

## See also

- MCP descriptor: `src/DirectShapeFromFace/create-direct-shape-initial.json`
- `CreateDirectShapeSimpleCommand` — simpler family-instance transform handling
- `CreateDirectShapeCommand` — sketch-plane reuse and nested instance transforms
- [Building Coder: DirectShape from face and sketch plane reuse](https://thebuildingcoder.typepad.com/blog/2015/09/directshape-from-face-and-sketch-plane-reuse.html)

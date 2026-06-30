# CreateDirectShapeCommand

| Field | Value |
|-------|-------|
| **Sample** | DirectShapeFromFace |
| **Class** | `CreateDirectShapeCommand` |
| **Source** | `src/DirectShapeFromFace/CreateDirectShapeCommand.cs` |
| **Origin** | [jeremytammik/DirectShapeFromFace](https://github.com/jeremytammik/DirectShapeFromFace) (MIT) |
| **MCP rating** | 3/5 |

Creates a Generic Model `DirectShape` from a user-picked face using Jeremy Tammik's advanced workflow: geometry-instance transform stack resolution, sketch-plane reuse for triangles sharing a plane, and model-line loops drawn for each triangle.

## What it demonstrates

- Walking `GeometryInstance` hierarchies to build a transform stack matched by stable face representation
- Reusing existing sketch planes named `<not associated>` when origin and normal match
- Drawing closed `ModelCurve` loops per triangle in the reused sketch plane
- `TessellatedShapeBuilder` and `DirectShape.CreateElement`

## Prerequisites

- Model element with a pickable face; nested family instances benefit most from this variant

## User interaction

- Prompts for a single face pick; cancellation returns `Cancelled`
- Creates model lines in addition to the DirectShape
- Shows a summary `TaskDialog` with the created DirectShape id

## MCP notes

- Proposed tool: `revit_create_direct_shape_from_face`
- Side effect: creates model lines and sketch planes; not ideal for unattended MCP runs
- MCP descriptor: `src/DirectShapeFromFace/create-direct-shape-from-face.json`

## See also

- MCP descriptor: `src/DirectShapeFromFace/create-direct-shape-from-face.json`
- `CreateDirectShapeSimpleCommand` — simpler transform handling without model lines
- `CreateDirectShapeInitialCommand` — earliest experimental variant
- [Building Coder: DirectShape from face and sketch plane reuse](https://thebuildingcoder.typepad.com/blog/2015/09/directshape-from-face-and-sketch-plane-reuse.html)

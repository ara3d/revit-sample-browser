# CreateCube

| Field | Value |
|-------|-------|
| **Sample** | GeometryAPI/BRepBuilderExample |
| **Class** | `CreateCube` |
| **Source** | `src/GeometryAPI/BRepBuilderExample/CreateCube.cs` |
| **SDK ReadMe** | — |
| **MCP rating** | 4/5 |

Builds a 100×100×100 solid cube with `BRepBuilder` and places it in the model as a `DirectShape`.

## What it demonstrates

- `BRepBuilder` with `BRepType.Solid`, planar faces, edges, loops, and consistent outward orientation
- `BRepBuilder.GetResult()` producing a `Solid` assigned via `DirectShape.SetShape`
- `DirectShape.CreateElement` with application/data ids for external geometry tracking

## Prerequisites

- Any project document; creates geometry in the Walls category in the sample

## User interaction

- No dialog; fixed cube dimensions and placement

## MCP notes

- Proposed tool: `revit_create_brep_direct_shape` with box dimensions, origin, and category id. Generalize `CreateCubeImpl` to accept parameters.

## See also

- MCP descriptor: `src/GeometryAPI/BRepBuilderExample/createcube.json`

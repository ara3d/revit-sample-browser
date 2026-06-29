# CreatePeriodic

| Field | Value |
|-------|-------|
| **Sample** | GeometryAPI/BRepBuilderExample |
| **Class** | `CreatePeriodic` |
| **Source** | `src/GeometryAPI/BRepBuilderExample/CS/CreatePeriodic.cs` |
| **SDK ReadMe** | — |
| **MCP rating** | 4/5 |

Builds periodic cylindrical solids—a cylinder and a truncated cone—using `BRepBuilder` and publishes each as a `DirectShape`.

## What it demonstrates

- `CylindricalSurface.Create` with paired periodic faces on a solid
- Truncated cone construction with conical and planar caps
- `CreateDirectShapeElementFromBrepBuilderObject` committing each shape in its own transaction

## Prerequisites

- Project document

## User interaction

- No UI; creates two demonstration solids at fixed dimensions

## MCP notes

- Proposed tool: `revit_create_periodic_brep` with shape enum (`cylinder`, `truncated_cone`) and dimension parameters.

## See also

- MCP descriptor: `docs/mcp/GeometryAPI/BRepBuilderExample/createperiodic.json`

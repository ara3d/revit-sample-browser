# CreateNurbs

| Field | Value |
|-------|-------|
| **Sample** | GeometryAPI/BRepBuilderExample |
| **Class** | `CreateNurbs` |
| **Source** | `src/GeometryAPI/BRepBuilderExample/CS/CreateNURBS.cs` |
| **SDK ReadMe** | — |
| **MCP rating** | 4/5 |

Creates an open-shell NURBS surface with trimmed boundary edges and displays it as a generic-model `DirectShape`.

## What it demonstrates

- `BRepBuilderSurfaceGeometry.CreateNURBSSurface` with degree, knots, and control points
- Boundary NURBS curves via `NurbSpline.CreateCurve` and `BRepBuilderEdgeGeometry`
- `BRepType.OpenShell` topology (face plus four edge loops)

## Prerequisites

- Project document

## User interaction

- Runs immediately with hard-coded control-point grid

## MCP notes

- Proposed tool: `revit_create_nurbs_direct_shape` accepting control points, degrees, and knots. Sample values are fixed.

## See also

- MCP descriptor: `src/GeometryAPI/BRepBuilderExample/createnurbs.json`

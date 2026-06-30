# Command

| Field | Value |
|-------|-------|
| **Sample** | FindReferencesByDirection/RaytraceBounce |
| **Class** | `Command` |
| **Source** | `src/FindReferencesByDirection/RaytraceBounce/RayTraceBounce.cs` |
| **MCP rating** | 5/5 |

Interactive ray-trace visualization that bounces rays off model faces and draws connecting detail lines.

## What it demonstrates

- `ReferenceIntersector.Find` for successive ray-face intersections
- `RayTraceBounceForm` UI for origin, direction, bounce count, and line style
- Requires a `{3D}` view and a line style named `bounce` in the document

## Prerequisites

- Default 3D view `{3D}`; pre-created `bounce` line style

## User interaction

- Modal form drives all ray parameters; not headless in current form

## MCP notes

- Proposed tool: `revit_raytrace_bounce` with `origin`, `direction`, `max_bounces`, and `view_id`. Returns hit points and optional line element ids. Bypass the form for MCP.

## See also

- MCP descriptor: `src/FindReferencesByDirection/RaytraceBounce/raytracebounce.json`

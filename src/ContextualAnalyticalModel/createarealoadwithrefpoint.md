# CreateAreaLoadWithRefPoint

| Field | Value |
|-------|-------|
| **Sample** | ContextualAnalyticalModel |
| **Class** | `CreateAreaLoadWithRefPoint` |
| **Source** | `src/ContextualAnalyticalModel/CreateAreaLoadWithRefPoint.cs` |
| **MCP rating** | 4/5 |

Creates an area load on a selected analytical host with per-reference-point force vectors and curve-end bindings.

## What it demonstrates

- Rectangular `CurveLoop` built from two picked points (axis-aligned box)
- `AreaLoad.IsCurveLoopsInsideHostBoundaries` host validation
- `AreaLoad.Create` overload with `forceVector` list, `refPointsIndexes`, and `refPointsCurveEnds`
- Variable force at reference points (sample uses downward load at index 0)

## Prerequisites

- Analytical panel or other host that accepts area loads
- Picked load boundary must lie inside host boundaries

## User interaction

- Pick analytical host element, then pick start and end corners of the load rectangle
- Replace picks with host ID and corner coordinates for headless use

## MCP notes

- Proposed tool: `revit_create_area_load_with_ref_points`
- Parameters: `host_element_id`, `boundary_corners`, `force_vectors[]`, `ref_point_indexes[]`, `ref_point_curve_ends[]`
- MCP descriptor: `src/ContextualAnalyticalModel/createarealoadwithrefpoint.json`

## See also

- MCP descriptor: `src/ContextualAnalyticalModel/createarealoadwithrefpoint.json`
- Related: [createcustomareaload.md](createcustomareaload.md)

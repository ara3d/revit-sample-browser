# CreateCustomAreaLoad

| Field | Value |
|-------|-------|
| **Sample** | ContextualAnalyticalModel |
| **Class** | `CreateCustomAreaLoad` |
| **Source** | `src/ContextualAnalyticalModel/CustomAreaLoad.cs` |
| **SDK ReadMe** | `src/ContextualAnalyticalModel/ReadMe_ContextualAnalyticalModel.rtf` |
| **MCP rating** | 4/5 |

Creates a uniform area load on a selected analytical host from a user-defined rectangular boundary.

## What it demonstrates

- `CurveLoop` rectangle from two `PickPoint` corners
- `AreaLoad.IsCurveLoopsInsideHostBoundaries` before creation
- `AreaLoad.Create(Document, ElementId hostId, IList<CurveLoop> loops, XYZ force, ElementId loadCaseId)` with uniform `(1,0,0)` force

## Prerequisites

- Analytical host element that supports area loads
- Load loop must be inside host boundaries

## User interaction

- Pick host element, then start and end corners of the load area
- Force direction and magnitude are hard-coded in the sample

## MCP notes

- Proposed tool: `revit_create_area_load`
- Parameters: `host_element_id`, `boundary_corners` or `curve_loops`, `force_vector`, optional `load_case_id`
- MCP descriptor: `src/ContextualAnalyticalModel/createcustomareaload.json`

## See also

- MCP descriptor: `src/ContextualAnalyticalModel/createcustomareaload.json`
- Related: [createarealoadwithrefpoint.md](createarealoadwithrefpoint.md)

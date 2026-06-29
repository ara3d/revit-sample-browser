# CreateCustomPointLoad

| Field | Value |
|-------|-------|
| **Sample** | ContextualAnalyticalModel |
| **Class** | `CreateCustomPointLoad` |
| **Source** | `src/ContextualAnalyticalModel/CustomPointLoad.cs` |
| **SDK ReadMe** | `src/ContextualAnalyticalModel/ReadMe_ContextualAnalyticalModel.rtf` |
| **MCP rating** | 4/5 |

Places a point load at a picked location on a selected analytical host element.

## What it demonstrates

- `PointLoad.IsPointInsideHostBoundaries` host containment check
- `PointLoad.Create` with location XYZ and force/moment vectors `(1,0,0)`
- Element pick plus single point pick workflow

## Prerequisites

- Analytical host that supports point loads
- Load location must be inside host boundaries

## User interaction

- Pick analytical host, then pick load location in the model
- Automation would pass `host_element_id`, `location`, and force parameters directly

## MCP notes

- Proposed tool: `revit_create_point_load`
- Parameters: `host_element_id`, `location`, `force`, `moment`, optional `load_case_id`
- MCP descriptor: `src/ContextualAnalyticalModel/createcustompointload.json`

## See also

- MCP descriptor: `src/ContextualAnalyticalModel/createcustompointload.json`
- Related: [createcustomlineload.md](createcustomlineload.md)

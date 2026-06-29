# CreateCustomLineLoad

| Field | Value |
|-------|-------|
| **Sample** | ContextualAnalyticalModel |
| **Class** | `CreateCustomLineLoad` |
| **Source** | `src/ContextualAnalyticalModel/CS/CustomLineLoad.cs` |
| **SDK ReadMe** | `src/ContextualAnalyticalModel/CS/ReadMe_ContextualAnalyticalModel.rtf` |
| **MCP rating** | 4/5 |

Creates a line load along a user-picked segment on a selected analytical host element.

## What it demonstrates

- `LineLoad.IsCurveInsideHostBoundaries` validation
- `LineLoad.Create` with start/end force vectors `(1,0,0)` at both ends
- `Line.CreateBound` from two picked points inside a transaction

## Prerequisites

- Analytical member or panel host that accepts line loads
- Load line must lie within host boundaries

## User interaction

- Pick host element, then pick start and end points of the load line
- Headless variant would accept `host_element_id`, `start`, `end`, and force vectors

## MCP notes

- Proposed tool: `revit_create_line_load`
- Parameters: `host_element_id`, `start_point`, `end_point`, `force_start`, `force_end`, optional `load_case_id`
- MCP descriptor: `src/ContextualAnalyticalModel/createcustomlineload.json`

## See also

- MCP descriptor: `src/ContextualAnalyticalModel/createcustomlineload.json`
- Related: [createcustompointload.md](createcustompointload.md), [createcustomareaload.md](createcustomareaload.md)

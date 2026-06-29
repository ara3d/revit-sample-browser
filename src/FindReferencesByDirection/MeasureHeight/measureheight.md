# Command

| Field | Value |
|-------|-------|
| **Sample** | FindReferencesByDirection/MeasureHeight |
| **Class** | `Command` |
| **Source** | `src/FindReferencesByDirection/MeasureHeight/CS/MeasureHeight.cs` |
| **SDK ReadMe** | — |
| **MCP rating** | 5/5 |

Measures vertical distance from a selected roof-hosted window (skylight) down to a floor face using ray casting.

## What it demonstrates

- `ReferenceIntersector` scoped to a floor element id with `FindReferenceTarget.Face`
- Casting a -Z ray from the skylight bounding-box center and choosing the closest hit
- Creating a temporary `ModelCurve` and showing the length in a `TaskDialog`

## Prerequisites

- Exactly one skylight selected (window category, hosted by a roof); floor element id is hard-coded in the sample

## User interaction

- Requires one pre-selected skylight; displays distance dialog and draws a model line

## MCP notes

- Proposed tool: `revit_measure_vertical_distance` with `source_element_id`, `target_element_id`, and optional `direction`. Replace hard-coded floor id with a parameter.

## See also

- MCP descriptor: `src/FindReferencesByDirection/MeasureHeight/measureheight.json`

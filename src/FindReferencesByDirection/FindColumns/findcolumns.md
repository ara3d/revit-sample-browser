# Command

| Field | Value |
|-------|-------|
| **Sample** | FindReferencesByDirection/FindColumns |
| **Class** | `Command` |
| **Source** | `src/FindReferencesByDirection/FindColumns/FindColumns.cs` |
| **MCP rating** | 5/5 |

Ray-traces from wall faces to find structural columns embedded alongside or within walls, then selects those columns.

## What it demonstrates

- `ReferenceIntersector` and `Find` along wall tangents at configurable elevation offsets
- Linear vs curved wall handling with normalized parameter stepping (`WallIncrement`)
- Building a wall-to-columns map and filtering `FamilyInstance` by `OST_Columns` / `OST_StructuralColumns`

## Prerequisites

- 3D view named `{3D}`; walls in the model (selected walls or all walls if nothing is selected)

## User interaction

- Optional pre-selection of walls; results applied by updating the active selection

## MCP notes

- Proposed tool: `revit_find_columns_near_walls` with optional `wall_ids` and `search_distance_feet`. Returns column ids grouped by wall. Core logic is already headless.

## See also

- MCP descriptor: `src/FindReferencesByDirection/FindColumns/findcolumns.json`

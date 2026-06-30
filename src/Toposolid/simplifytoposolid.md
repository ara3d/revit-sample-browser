# SimplifyToposolid

| Field | Value |
|-------|-------|
| **Sample** | Toposolid |
| **Class** | `SimplifyToposolid` |
| **Source** | `src/Toposolid/Command.cs` |
| **MCP rating** | 2/5 |

Reduces inner vertices of a user-picked toposolid using `Toposolid.Simplify`.

## What it demonstrates

- `Toposolid.Simplify(double)` with a tolerance of 0.6
- `ToposolidFilter` selection filter for `ISelectionFilter`
- In-place mesh simplification on an existing element

## Prerequisites

- At least one `Toposolid` element in the model

## User interaction

- Requires picking one toposolid in the Revit UI
- Headless use would need an element id parameter instead of `PickObject`

## MCP notes

- Could be parameterized with `element_id` and `tolerance`, but simplification is a niche editing operation.

## See also

- [SplitToposolid](splittoposolid.md)
- [ToposolidCreation](toposolidcreation.md)

# Command

| Field | Value |
|-------|-------|
| **Sample** | CreateDimensions |
| **Class** | `Command` |
| **Source** | `src/CreateDimensions/Command.cs` |
| **MCP rating** | 4/5 |

Adds linear dimensions to selected basic walls in the active 2D view, referencing analytical panel contour geometry associated with each wall.

## What it demonstrates

- Rejecting `View3D` and `ViewSheet` as dimension targets
- Resolving `AnalyticalPanel` via `AnalyticalToPhysicalAssociationManager`
- `AnalyticalModelSelector` with `AnalyticalCurveSelector.StartPoint` for vertical edge references
- `Document.Create.NewDimension(view, line, referenceArray)` offset from the wall run

## Prerequisites

- Active plan, section, or elevation view (not 3D or sheet)
- Pre-selected basic walls with associated analytical panels exposing a horizontal contour edge

## User interaction

- Uses current selection only; no dialog
- Offset line is fixed at +5 internal units from the analytical edge

## MCP notes

- Proposed tool: `revit_dimension_walls`
- Parameters: `wall_ids[]`, optional `offset`, optional `view_id`
- Returns: created dimension element ids
- MCP descriptor: `src/CreateDimensions/createdimensions.json`

## See also

- MCP descriptor: `src/CreateDimensions/createdimensions.json`

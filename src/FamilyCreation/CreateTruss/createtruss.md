# Command

| Field | Value |
|-------|-------|
| **Sample** | FamilyCreation/CreateTruss |
| **Class** | `Command` |
| **Source** | `src/FamilyCreation/CreateTruss/CreateTruss.cs` |
| **SDK ReadMe** | `src/FamilyCreation/CreateTruss/ReadMe_CreateTruss.rtf` |
| **MCP rating** | 2/5 |

Creates a mono truss in a truss family using named reference planes, model curves, and locked alignments.

## What it demonstrates

- `FamilyItemFactory.NewModelCurve` with `TrussCurveType` (top/bottom chord, web)
- Resolving reference planes and `Level 1` view by name; geometry via line intersections
- `NewAlignment` and locked `NewAngularDimension` to stabilize parametric behavior

## Prerequisites

- Truss family document with reference planes named Top, Bottom, Left, Right, Center and a Level 1 plan view

## User interaction

- No dialog; fails if required reference planes or views are missing

## MCP notes

- Family-editor tutorial with fixed layout; automating truss creation would need template-specific parameterization

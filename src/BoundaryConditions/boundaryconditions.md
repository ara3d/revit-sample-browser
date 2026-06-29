# Command

| Field | Value |
|-------|-------|
| **Sample** | BoundaryConditions |
| **Class** | `Command` |
| **Source** | `src/BoundaryConditions/Command.cs` |
| **SDK ReadMe** | `src/BoundaryConditions/ReadMe_BoundaryConditions.rtf` |
| **MCP rating** | 4/5 |

Creates or inspects structural boundary conditions on a single selected structural element.

## What it demonstrates

- Validating selection is one structural element (column, brace, beam, wall, foundation, slab)
- `BoundaryConditionsData` preparing BC state for a property grid
- Creating or updating `BoundaryConditions` on the element

## Prerequisites

- Exactly one supported structural element pre-selected

## User interaction

- `BoundaryConditionsForm` with property grid for translation/rotation states
- Element-type-specific BC options via `BCProperties` enums

## MCP notes

- Proposed tool: `revit_set_boundary_conditions`
- Parameters: `element_id`, BC state fields (translations/rotations per axis)
- Returns: boundary condition element id or updated state summary
- MCP descriptor: `src/BoundaryConditions/boundaryconditions.json`

## See also

- MCP descriptor: `src/BoundaryConditions/boundaryconditions.json`

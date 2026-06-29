# Command

| Field | Value |
|-------|-------|
| **Sample** | CreateBeamsColumnsBraces |
| **Class** | `Command` |
| **Source** | `src/CreateBeamsColumnsBraces/CreateBeamsColumnsBraces.cs` |
| **SDK ReadMe** | `src/CreateBeamsColumnsBraces/ReadMe_CreateBeamsColumnsBraces.rtf` |
| **MCP rating** | 4/5 |

Builds a rectilinear grid of structural columns, beams, and diagonal braces across consecutive levels from user-specified family types and bay spacing.

## What it demonstrates

- Collecting `Level`, `FamilySymbol` for columns and framing via `FilteredElementCollector`
- `Document.Create.NewFamilyInstance` for columns (`StructuralType.Column`), beams, and braces
- Brace pairs formed at mid-elevation between adjacent grid points
- Column base/top level parameters via `BuiltInParameter.FAMILY_BASE_LEVEL_PARAM` and related offsets

## Prerequisites

- At least two levels and loaded structural column and framing families

## User interaction

- `CreateBeamsColumnsBracesForm` collects grid size (X/Y count, spacing), floor count, and symbol choices
- Core placement logic in `AddInstance` is separable from the dialog

## MCP notes

- Proposed tool: `revit_create_structural_grid`
- Parameters: `column_type_id`, `beam_type_id`, `brace_type_id`, `x_count`, `y_count`, `spacing`, `floor_count`, optional origin `UV`
- Returns: created element ids grouped by category
- MCP descriptor: `src/CreateBeamsColumnsBraces/createbeamscolumnsbraces.json`

## See also

- MCP descriptor: `src/CreateBeamsColumnsBraces/createbeamscolumnsbraces.json`

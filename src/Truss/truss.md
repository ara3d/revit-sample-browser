# Command

| Field | Value |
|-------|-------|
| **Sample** | Truss |
| **Class** | `Command` |
| **Source** | `src/Truss/Command.cs` |
| **MCP rating** | 4/5 |

Launches a WinForms UI to create, edit, and visualize structural trusses from two columns or an existing truss.

## What it demonstrates

- `Truss.Create` between two structural columns on a selected `ViewPlan` level
- `TrussGeometry` profile editing (top chord, bottom chord, web members) with `LineTool` sketching
- Changing truss member `FamilySymbol` types and `TrussType` assignment
- GDI+ picture-box preview of truss layout before commit

## Prerequisites

- Pre-select either one existing truss or two structural columns
- Loaded `TrussType` and structural framing `FamilySymbol` types
- At least one non-template `ViewPlan`

## User interaction

- `TrussForm` modal dialog for type, view, profile, and member editing
- Automation would replace the form with truss type id, view id, column ids, and profile curve data

## MCP notes

- Proposed tool: `revit_create_truss`
- Parameters: `column_ids` (two) or `truss_id` (edit), `truss_type_id`, `view_id`, optional member profile curves and beam type ids
- Returns: created or updated truss element id
- MCP descriptor: `src/Truss/truss.json`

## See also

- MCP descriptor: `src/Truss/truss.json`
- [FamilyCreation/CreateTruss/createtruss.md](../FamilyCreation/CreateTruss/createtruss.md)

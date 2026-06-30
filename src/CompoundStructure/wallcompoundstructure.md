# WallCompoundStructure

| Field | Value |
|-------|-------|
| **Sample** | CompoundStructure |
| **Class** | `WallCompoundStructure` |
| **Source** | `src/CompoundStructure/Command.cs` |
| **MCP rating** | 4/5 |

Builds a vertical compound wall structure (layers, materials, functions) and assigns it to selected walls.

## What it demonstrates

- Reading and cloning `CompoundStructure` from a wall type
- Adding layers with `CompoundStructureLayer`, materials, and width offsets
- Applying the modified structure back to wall type or instance per sample logic

## Prerequisites

- At least one wall selected; valid wall types and materials in the project

## User interaction

- Uses current selection; shows `TaskDialog` on empty selection
- Layer recipe is largely hard-coded in the command body

## MCP notes

- Proposed tool: `revit_set_wall_compound_structure`
- Parameters: `wall_ids[]` or `wall_type_id`, layer definitions array `{ function, material_id, width }`
- MCP descriptor: `src/CompoundStructure/wallcompoundstructure.json`

## See also

- MCP descriptor: `src/CompoundStructure/wallcompoundstructure.json`

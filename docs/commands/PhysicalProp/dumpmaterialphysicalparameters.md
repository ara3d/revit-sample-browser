# DumpMaterialPhysicalParameters

| Field | Value |
|-------|-------|
| **Sample** | PhysicalProp |
| **Class** | `DumpMaterialPhysicalParameters` |
| **Source** | `src/PhysicalProp/CS/Command.cs` |
| **SDK ReadMe** | `src/PhysicalProp/CS/ReadMe_PhysicalProp.rtf` |
| **MCP rating** | 5/5 |

Reads structural physical properties from the material assigned to a single selected structural family instance and displays them in a task dialog.

## What it demonstrates

- Resolving a `Material` from the **Structural Material** instance parameter on a `FamilyInstance`
- Reading `BuiltInParameter.PHY_MATERIAL_PARAM_*` values for generic, concrete, and steel types
- Branching output by `PHY_MATERIAL_PARAM_TYPE` (generic, concrete, steel)
- Read-only inspection via `TransactionMode.ReadOnly`

## Prerequisites

- Exactly one selected `FamilyInstance` with a structural material (column, beam, or brace)
- Material must expose physical properties (type value greater than generic)

## User interaction

- Requires a pre-selected element; results appear in `TaskDialog.Show`
- No modal form; suitable for headless use if output is redirected from the dialog

## MCP notes

- Proposed tool: `revit_get_material_physical_properties`
- Parameters: `element_id` of a structural family instance
- Returns: material type, Young's modulus, Poisson ratio, shear modulus, thermal expansion, unit weight, behavior, and type-specific concrete or steel fields
- MCP descriptor: `docs/mcp/PhysicalProp/dumpmaterialphysicalparameters.json`

## See also

- MCP descriptor: `docs/mcp/PhysicalProp/dumpmaterialphysicalparameters.json`

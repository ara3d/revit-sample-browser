# MaterialProperties

| Field | Value |
|-------|-------|
| **Sample** | MaterialProperties |
| **Class** | `MaterialProperties` |
| **Source** | `src/MaterialProperties/MaterialProperties.cs` |
| **MCP rating** | 5/5 |

Displays physical material properties for a selected structural beam, column, or brace and lets the user assign a different material or override unit weight.

## What it demonstrates

- Reading `Structural Material` on `FamilyInstance` structural members
- Building a property table from `BuiltInParameter.PHY_MATERIAL_PARAM_*` (Young's modulus, Poisson, shear, expansion, steel/concrete-specific fields)
- Classifying materials via `StructuralAssetClass` from linked `PropertySetElement`
- `Parameter.Set(ElementId)` to swap structural material; `ChangeUnitWeight` sets concrete unit weight

## Prerequisites

- Exactly one selected beam, brace, or column in a project with materials loaded

## User interaction

- `MaterialPropertiesForm` shows property grids and material pick lists
- `GetParameterTable`, `SetMaterial`, and `ChangeUnitWeight` are callable without the form

## MCP notes

- Proposed tools: `revit_get_material_properties` and `revit_set_structural_material`
- Parameters: `element_id`, optional `material_id`, optional `unit_weight`
- Returns: material name, asset class, and physical property name/value pairs
- MCP descriptor: `src/MaterialProperties/materialproperties.json`

## See also

- MCP descriptor: `src/MaterialProperties/materialproperties.json`

# Command

| Field | Value |
|-------|-------|
| **Sample** | SlabProperties |
| **Class** | `Command` |
| **Source** | `src/SlabProperties/CS/Command.cs` |
| **SDK ReadMe** | `src/SlabProperties/CS/ReadMe_SlabProperties.rtf` |
| **MCP rating** | 4/5 |

Displays structural and material properties of a single selected floor slab, including span direction and per-layer thickness and Young's modulus.

## What it demonstrates

- Reading `Floor.LevelId`, `FloorType`, and `BuiltInParameter.FLOOR_PARAM_SPAN_DIRECTION` (radians to degrees)
- `CompoundStructure.GetLayers()` iteration with `CompoundStructureLayer` width and material
- Material Young's modulus via `PHY_MATERIAL_PARAM_YOUNG_MOD1/2/3`
- `SlabPropertiesForm` layer browser calling `SetLayer`

## Prerequisites

- Exactly one floor/slab pre-selected

## User interaction

- Form is read-only display; selection must exist before the command runs

## MCP notes

- Proposed tool: `revit_get_slab_properties`
- Parameters: `floor_id`
- Returns: level, type name, span direction, per-layer thickness, material names, and Young's modulus values
- MCP descriptor: `src/SlabProperties/slabproperties.json`

## See also

- MCP descriptor: `src/SlabProperties/slabproperties.json`

# Command

| Field | Value |
|-------|-------|
| **Sample** | StructuralLayerFunction |
| **Class** | `Command` |
| **Source** | `src/StructuralLayerFunction/Command.cs` |
| **MCP rating** | 3/5 |

Lists each compound-structure layer function for a selected floor, outside to inside, in a dialog.

## What it demonstrates

- `FloorType.GetCompoundStructure().GetLayers()` and `CompoundStructureLayer.Function`
- Mapping zero function to "No function" vs. `MaterialFunctionAssignment` enum text
- `StructuralLayerFunctionForm` as a simple read-only viewer

## Prerequisites

- Exactly one floor selected

## User interaction

- Dialog display only; no edits

## MCP notes

- Proposed tool: `revit_get_floor_layer_functions`
- Parameters: `floor_id`
- Returns: ordered list of layer function names
- MCP descriptor: `src/StructuralLayerFunction/structurallayerfunction.json`

## See also

- MCP descriptor: `src/StructuralLayerFunction/structurallayerfunction.json`

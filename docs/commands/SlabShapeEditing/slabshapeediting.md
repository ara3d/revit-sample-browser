# Command

| Field | Value |
|-------|-------|
| **Sample** | SlabShapeEditing |
| **Class** | `Command` |
| **Source** | `src/SlabShapeEditing/CS/Command.cs` |
| **SDK ReadMe** | `src/SlabShapeEditing/CS/ReadMe_SlabShapeEditing.rtf` |
| **MCP rating** | 4/5 |

Edits a selected floor's shape by adding, moving, and modifying slab shape vertices and creases through a 2D profile editor.

## What it demonstrates

- `Floor.GetSlabShapeEditor()` and `SlabShapeVertex` / `SlabShapeCrease` manipulation
- `SlabProfile` projecting slab geometry to a 2D canvas with transform matrices
- Interactive drawing tools (`LineTool`) for vertices and creases in `SlabShapeEditingForm`
- Committing shape edits back to the host floor in model transactions

## Prerequisites

- Exactly one floor/slab selected before run

## User interaction

- `SlabShapeEditingForm` is fully graphical; MCP would need curve/point inputs instead of mouse drawing

## MCP notes

- Proposed tool: `revit_edit_slab_shape`
- Parameters: `floor_id`, `vertices[]`, `creases[]`, optional move/rotate operations
- Returns: updated vertex and crease counts
- MCP descriptor: `docs/mcp/SlabShapeEditing/slabshapeediting.json`

## See also

- MCP descriptor: `docs/mcp/SlabShapeEditing/slabshapeediting.json`

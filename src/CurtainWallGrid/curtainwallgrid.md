# Command

| Field | Value |
|-------|-------|
| **Sample** | CurtainWallGrid |
| **Class** | `Command` |
| **Source** | `src/CurtainWallGrid/Command.cs` |
| **MCP rating** | 4/5 |

Launches an interactive editor for adding, moving, and deleting curtain wall grid lines on a selected curtain wall.

## What it demonstrates

- `MyDocument` wrapping the active curtain wall and its `CurtainGrid`
- `GridGeometry` operations on U/V `CurtainGridLine` collections
- `GridDrawing` 2D preview and `GridForm` for grid layout properties (`GridProperties`, `CurtainGridAlign`)
- Grid line add/remove via curtain grid API

## Prerequisites

- Pre-selected curtain wall with an editable curtain grid

## User interaction

- `GridForm` is fully interactive — selection, preview, and property edits require the dialog

## MCP notes

- Proposed tool: `revit_edit_curtain_grid`
- Parameters: `wall_id`, `u_grid_offsets[]`, `v_grid_offsets[]`, optional alignment settings
- Returns: updated grid line ids
- MCP descriptor: `src/CurtainWallGrid/curtainwallgrid.json`

## See also

- MCP descriptor: `src/CurtainWallGrid/curtainwallgrid.json`

# CreateWall

| Field | Value |
|-------|-------|
| **Sample** | Ribbon |
| **Class** | `CreateWall` |
| **Source** | `src/Ribbon/CS/AddInCommand.cs` |
| **MCP rating** | 1/5 |

Creates a non-structural wall using curve geometry and options read from custom ribbon controls.

## What it demonstrates

- Reading `RadioButtonGroup`, `ComboBox`, and `TextBox` ribbon items by name (`WallTypeSelector`, `LevelsSelector`, `WallShapeComboBox`, `WallMark`)
- `Wall.Create` with curve loops for rectangle, square, circle, and triangle shapes
- Tracking created walls in static `CreatedWalls` for companion ribbon commands

## Prerequisites

- Ribbon sample panel loaded with wall type, level, shape, and mark controls
- Wall types and levels present in the document

## User interaction

- Entirely driven by ribbon UI state, not a command dialog

## MCP notes

Demonstrates ribbon integration, not a parameterized API suitable for MCP; use direct `Wall.Create` instead.

## See also

- Structural variant: `CreateStructureWall` in the same file

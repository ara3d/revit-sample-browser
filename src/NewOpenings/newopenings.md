# Command

| Field | Value |
|-------|-------|
| **Sample** | NewOpenings |
| **Class** | `Command` |
| **Source** | `src/NewOpenings/command.cs` |
| **MCP rating** | 4/5 |

Creates sketch-based openings in a selected wall or floor through an interactive profile editor.

## What it demonstrates

- Host validation for a single `Wall` or `Floor` selection
- `ProfileWall` / `ProfileFloor` adapters wrapping host geometry for sketch tools
- `NewOpeningsForm` with line, arc, and circle tools to define opening boundaries and call opening creation APIs

## Prerequisites

- Exactly one wall or floor selected before running the command

## User interaction

- Modal sketch dialog; user draws the opening profile interactively
- Profile helper classes separate host setup from UI for partial automation

## MCP notes

- Proposed tool: `revit_create_opening`
- Parameters: `host_element_id`, closed profile as point/curve loop or bounding box
- Returns: new `Opening` element id
- Refactor: accept profile coordinates instead of `NewOpeningsForm` sketch tools
- MCP descriptor: `src/NewOpenings/newopenings.json`

## See also

- MCP descriptor: `src/NewOpenings/newopenings.json`

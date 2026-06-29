# YMoveWalls

| Field | Value |
|-------|-------|
| **Sample** | Ribbon |
| **Class** | `YMoveWalls` |
| **Source** | `src/Ribbon/CS/AddInCommand.cs` |
| **MCP rating** | 1/5 |

Moves all ribbon-created walls 12 feet in the positive Y direction.

## What it demonstrates

- Same `CreatedWalls` iteration pattern as `XMoveWalls` with `new XYZ(0, 12, 0)` translation
- `Wall.Location.Move` inside a named transaction

## Prerequisites

- Session walls tracked by `CreateWall.CreatedWalls`

## User interaction

- Ribbon-triggered with hard-coded offset

## MCP notes

Companion to `XMoveWalls` for ribbon UI demonstration only.

# XMoveWalls

| Field | Value |
|-------|-------|
| **Sample** | Ribbon |
| **Class** | `XMoveWalls` |
| **Source** | `src/Ribbon/CS/AddInCommand.cs` |
| **MCP rating** | 1/5 |

Moves all ribbon-created walls 12 feet in the positive X direction.

## What it demonstrates

- `Location.Move` on walls stored in `CreateWall.CreatedWalls`
- Transaction-wrapped batch translation tied to demo session state

## Prerequisites

- Walls in `CreatedWalls` from prior `CreateWall` runs

## User interaction

- Ribbon button; fixed 12 ft offset

## MCP notes

Demo motion helper; not a parameterized move-elements tool.

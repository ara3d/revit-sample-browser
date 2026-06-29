# DeleteWalls

| Field | Value |
|-------|-------|
| **Sample** | Ribbon |
| **Class** | `DeleteWalls` |
| **Source** | `src/Ribbon/AddInCommand.cs` |
| **MCP rating** | 1/5 |

Deletes all walls recorded in `CreateWall.CreatedWalls` and clears the static tracking set.

## What it demonstrates

- Companion cleanup for the Ribbon wall demo
- `Document.Delete` over the static `ElementSet` populated by `CreateWall`

## Prerequisites

- Walls previously created by `CreateWall` or `CreateStructureWall` in the same session

## User interaction

- Ribbon button only

## MCP notes

Session-scoped demo helper tied to ribbon sample state; not a general delete-walls tool.

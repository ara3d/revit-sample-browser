# Command

| Field | Value |
|-------|-------|
| **Sample** | NewPathReinforcement |
| **Class** | `Command` |
| **Source** | `src/NewPathReinforcement/CS/Command.cs` |
| **SDK ReadMe** | `src/NewPathReinforcement/CS/ReadMe_NewPathReinforcement.rtf` |
| **MCP rating** | 2/5 |

Creates path reinforcement on a selected structural wall or floor using an interactive sketch form.

## What it demonstrates

- Host selection validation for one `Wall` or `Floor`
- `ProfileWall` / `ProfileFloor` geometry helpers shared with the Openings sample pattern
- `NewPathReinforcementForm` to sketch reinforcement paths and invoke creation APIs

## Prerequisites

- One structural wall or slab (floor) selected

## User interaction

- Modal sketch UI; user defines path reinforcement geometry manually

## MCP notes

- Reinforcement sketching is inherently interactive; MCP would need curve loops and bar type ids as inputs.

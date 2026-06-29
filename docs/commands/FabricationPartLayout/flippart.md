# FlipPart

| Field | Value |
|-------|-------|
| **Sample** | FabricationPartLayout |
| **Class** | `FlipPart` |
| **Source** | `src/FabricationPartLayout/CS/FlipPart.cs` |
| **SDK ReadMe** | `src/FabricationPartLayout/CS/Readme_FabricationPartLayout.rtf` |
| **MCP rating** | 2/5 |

Flips a user-picked fabrication part when `CanFlipPart` returns true.

## What it demonstrates

- `FabricationPart.CanFlipPart` and `FabricationPart.Flip`

## User interaction

- Single element pick

## MCP notes

- Trivial mutation with pick dependency; MCP would accept `element_id`

# DetachRods

| Field | Value |
|-------|-------|
| **Sample** | FabricationPartLayout |
| **Class** | `DetachRods` |
| **Source** | `src/FabricationPartLayout/CS/HangerRods.cs` |
| **SDK ReadMe** | `src/FabricationPartLayout/CS/Readme_FabricationPartLayout.rtf` |
| **MCP rating** | 2/5 |

Detaches hanger rods from their structural host on a user-picked fabrication hanger part.

## What it demonstrates

- `FabricationRodInfo.CanRodsBeHosted = false` to detach rods
- `FabricationPart.IsAHanger` validation

## User interaction

- Single element pick

## MCP notes

- Simple mutation but requires pick; pass `element_id` for headless use

# IncreaseRodStructureExtension

| Field | Value |
|-------|-------|
| **Sample** | FabricationPartLayout |
| **Class** | `IncreaseRodStructureExtension` |
| **Source** | `src/FabricationPartLayout/CS/HangerRods.cs` |
| **SDK ReadMe** | `src/FabricationPartLayout/CS/Readme_FabricationPartLayout.rtf` |
| **MCP rating** | 2/5 |

Increases each hanger rod's structure extension by one foot on a picked hanger attached to structure.

## What it demonstrates

- Structure extension getters/setters on `FabricationRodInfo`
- Requires rods to be attached (`IsAttachedToStructure`)

## User interaction

- Single hanger pick

## MCP notes

- Pair with `DecreaseRodStructureExtension`; needs element id and delta parameters for MCP

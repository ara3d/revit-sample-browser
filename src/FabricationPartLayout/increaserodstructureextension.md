# IncreaseRodStructureExtension

| Field | Value |
|-------|-------|
| **Sample** | FabricationPartLayout |
| **Class** | `IncreaseRodStructureExtension` |
| **Source** | `src/FabricationPartLayout/HangerRods.cs` |
| **MCP rating** | 2/5 |

Increases each hanger rod's structure extension by one foot on a picked hanger attached to structure.

## What it demonstrates

- Structure extension getters/setters on `FabricationRodInfo`
- Requires rods to be attached (`IsAttachedToStructure`)

## User interaction

- Single hanger pick

## MCP notes

- Pair with `DecreaseRodStructureExtension`; needs element id and delta parameters for MCP

# DecreaseRodStructureExtension

| Field | Value |
|-------|-------|
| **Sample** | FabricationPartLayout |
| **Class** | `DecreaseRodStructureExtension` |
| **Source** | `src/FabricationPartLayout/HangerRods.cs` |
| **MCP rating** | 2/5 |

Decreases each hanger rod's structure extension by one foot on a picked fabrication hanger that is attached to structure.

## What it demonstrates

- `FabricationPart.GetRodInfo`, `GetRodStructureExtension`, and `SetRodStructureExtension`
- Validation that rods are attached before adjusting extensions

## User interaction

- Single hanger pick required

## MCP notes

- Interactive pick and fixed delta; parameterize element id and offset for automation

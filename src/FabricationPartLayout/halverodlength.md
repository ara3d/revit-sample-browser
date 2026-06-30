# HalveRodLength

| Field | Value |
|-------|-------|
| **Sample** | FabricationPartLayout |
| **Class** | `HalveRodLength` |
| **Source** | `src/FabricationPartLayout/HangerRods.cs` |
| **MCP rating** | 2/5 |

Halves each rod length on a picked fabrication hanger when rods are detached from structure.

## What it demonstrates

- Rod length iteration via `FabricationRodInfo`
- Precondition check on `IsAttachedToStructure`

## User interaction

- Single hanger pick

## MCP notes

- Mirror of `DoubleRodLength`; parameterize element id and scale for automation

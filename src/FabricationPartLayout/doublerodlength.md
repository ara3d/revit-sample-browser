# DoubleRodLength

| Field | Value |
|-------|-------|
| **Sample** | FabricationPartLayout |
| **Class** | `DoubleRodLength` |
| **Source** | `src/FabricationPartLayout/HangerRods.cs` |
| **MCP rating** | 2/5 |

Doubles the length of every rod on a picked fabrication hanger after verifying rods are detached from structure.

## What it demonstrates

- Iterating `RodCount` with `GetRodLength` / `SetRodLength`
- Guard when `IsAttachedToStructure` is true

## User interaction

- Single hanger pick

## MCP notes

- Fixed multiplier demo; MCP would take element id and scale factor

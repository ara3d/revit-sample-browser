# ContourSettingModification

| Field | Value |
|-------|-------|
| **Sample** | Toposolid |
| **Class** | `ContourSettingModification` |
| **Source** | `src/Toposolid/Command.cs` |
| **SDK ReadMe** | `src/Toposolid/ReadMe_Toposolid.rtf` |
| **MCP rating** | 2/5 |

Disables the first contour item on the first `ToposolidType` after verifying four contour settings exist.

## What it demonstrates

- `ContourSetting.GetContourSettingItems()` enumeration
- `ContourSetting.DisableItem` (with commented alternatives for `EnableItem` and `RemoveItem`)
- Validation of expected contour item count before modification

## Prerequisites

- A `ToposolidType` whose contour setting already has exactly four items (typically after running ContourSettingCreation)

## User interaction

- Headless; shows an error dialog if the contour item count is not four

## MCP notes

- Depends on prior contour setup; low value as an isolated automation tool.

## See also

- [ContourSettingCreation](contoursettingcreation.md)

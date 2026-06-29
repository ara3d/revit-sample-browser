# ResetSetting

| Field | Value |
|-------|-------|
| **Sample** | Ribbon |
| **Class** | `ResetSetting` |
| **Source** | `src/Ribbon/CS/AddInCommand.cs` |
| **MCP rating** | 1/5 |

Resets ribbon wall-creation controls to their default values (wall type, level, shape, and mark text).

## What it demonstrates

- Programmatic ribbon item lookup via `GetRibbonItemByName`
- Resetting `RadioButtonGroup.Current`, `ComboBox.Current`, and `TextBox.Value`

## Prerequisites

- Ribbon sample panel with named controls installed

## User interaction

- Ribbon-only; no document transaction

## MCP notes

UI state reset for the ribbon demo, not a document operation.

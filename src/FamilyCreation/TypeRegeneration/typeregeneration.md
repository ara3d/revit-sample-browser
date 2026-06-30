# Command

| Field | Value |
|-------|-------|
| **Sample** | FamilyCreation/TypeRegeneration |
| **Class** | `Command` |
| **Source** | `src/FamilyCreation/TypeRegeneration/Command.cs` |
| **MCP rating** | 2/5 |

Iterates every family type, switches `FamilyManager.CurrentType` to test regeneration, and logs results to file and a WinForms message window.

## What it demonstrates

- Walking `FamilyManager.Types` and assigning `CurrentType` to force model rebuild per type
- Writing `RegenerationLog.txt` beside the add-in; interactive `MessageForm` progress display
- Exception handling per type to distinguish successful vs failed regeneration

## Prerequisites

- Open family document with one or more named family types

## User interaction

- Modal WinForms dialogs advance per type; not suitable for unattended batch runs without refactoring

## MCP notes

- Regeneration audit could return a JSON list of type names and pass/fail, but the sample is UI-driven

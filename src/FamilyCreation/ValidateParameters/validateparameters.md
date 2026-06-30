# Command

| Field | Value |
|-------|-------|
| **Sample** | FamilyCreation/ValidateParameters |
| **Class** | `Command` |
| **Source** | `src/FamilyCreation/ValidateParameters/Command.cs` |
| **MCP rating** | 2/5 |

Validates that each family type can read every parameter value according to its storage type and reports failures.

## What it demonstrates

- `FamilyType.HasValue` and typed accessors (`AsDouble`, `AsInteger`, `AsString`, `AsElementId`)
- Static `ValidateParameters(FamilyManager)` returning error strings per type/parameter pair
- Displaying results in a modal `MessageForm`

## Prerequisites

- Open family document

## User interaction

- Shows a dialog with validation errors; no model changes

## MCP notes

- Validation logic is headless-friendly, but the command only surfaces results in a form

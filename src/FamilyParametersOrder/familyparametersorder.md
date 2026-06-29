# Command

| Field | Value |
|-------|-------|
| **Sample** | FamilyParametersOrder |
| **Class** | `Command` |
| **Source** | `src/FamilyParametersOrder/Command.cs` |
| **SDK ReadMe** | — |
| **MCP rating** | 2/5 |

Opens a dialog to sort parameters in loaded family files according to user-defined ordering rules.

## What it demonstrates

- `SortFamilyFilesParamsForm` driven by `UIApplication` for batch family parameter reordering
- Static `SortDialogIsOpened` flag to suppress auto-open on document load
- Standard external-command error handling around modal UI

## Prerequisites

- Families available for sorting via the form’s workflow (typically loaded or selected through the UI)

## User interaction

- Entire command is a modal dialog; no headless path

## MCP notes

- Parameter ordering is a niche family-management task tied to the sample’s WinForms UI

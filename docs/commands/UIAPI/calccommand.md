# CalcCommand

| Field | Value |
|-------|-------|
| **Sample** | UIAPI |
| **Class** | `CalcCommand` |
| **Source** | `src/UIAPI/CS/ExternalApplication.cs` |
| **MCP rating** | 1/5 |

Placeholder ribbon command that shows a task dialog; used to demonstrate contextual help on ribbon buttons.

## What it demonstrates

- `IExternalCommand` as a dummy target for `PushButtonData` entries in `ExternalApp.CreateRibbonButton`
- `ContextualHelp` types: `ContextId` (wiki), `Url`, and `ChmFile` with topic URL
- `ApplicationAvailabilityClass` controlling button availability

## Prerequisites

- `ExternalApp` must be loaded so ribbon buttons are registered

## User interaction

- Shows `TaskDialog.Show("Dummy command", ...)` with no model changes
- Primary value is ribbon integration, not command logic

## MCP notes

- No document operations; not an MCP candidate.

## See also

- [DragAndDropCommand](draganddropcommand.md)
- [PreviewCommand](previewcommand.md)

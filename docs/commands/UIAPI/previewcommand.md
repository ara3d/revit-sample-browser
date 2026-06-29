# PreviewCommand

| Field | Value |
|-------|-------|
| **Sample** | UIAPI |
| **Class** | `PreviewCommand` |
| **Source** | `src/UIAPI/CS/PreviewControl/PreviewCommand.cs` |
| **MCP rating** | 1/5 |

Opens a dialog that previews all views in the document using the WPF `PreviewControl` API inside a rolled-back transaction group.

## What it demonstrates

- `PreviewModel` form hosting Revit `PreviewControl` for view thumbnails
- `TransactionGroup` started before preview and always rolled back in `finally` to avoid persisting changes
- Ribbon registration via `ExternalApp` as "Preview all views"

## Prerequisites

- Active document with at least one view

## User interaction

- Modal `PreviewModel` dialog; read-only preview with no lasting model edits

## MCP notes

- UI-only preview widget; no meaningful headless or MCP surface.

## See also

- [CalcCommand](calccommand.md)

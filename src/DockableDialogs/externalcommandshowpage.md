# ExternalCommandShowPage

| Field | Value |
|-------|-------|
| **Sample** | DockableDialogs |
| **Class** | `ExternalCommandShowPage` |
| **Source** | `src/DockableDialogs/CS/TopLevelCommands/ExternalCommandShowPage.cs` |
| **SDK ReadMe** | `src/DockableDialogs/CS/ReadMe_DockableDialogs.rtf` |
| **MCP rating** | 1/5 |

Shows the previously registered dockable pane when a document is open.

## What it demonstrates

- `SetWindowVisibility(application, true)` on the shared `ThisApplication` instance
- Requires prior `RegisterDockableWindow` success
- `IExternalCommandAvailability` gated on open document

## Prerequisites

- Registered dockable pane and an active `UIDocument`

## User interaction

- Ribbon toggle to display the custom pane

## MCP notes

- Revit UI framework demo — not exposed as a document MCP tool.

## See also

- Related: [externalcommandregisterpage.md](externalcommandregisterpage.md), [externalcommandhidepage.md](externalcommandhidepage.md)

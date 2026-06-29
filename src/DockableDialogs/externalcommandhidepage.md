# ExternalCommandHidePage

| Field | Value |
|-------|-------|
| **Sample** | DockableDialogs |
| **Class** | `ExternalCommandHidePage` |
| **Source** | `src/DockableDialogs/TopLevelCommands/ExternalCommandHidePage.cs` |
| **SDK ReadMe** | `src/DockableDialogs/ReadMe_DockableDialogs.rtf` |
| **MCP rating** | 1/5 |

Hides the registered dockable pane in Revit when a document is open.

## What it demonstrates

- `ThisApplication.ThisApp.SetWindowVisibility(application, false)`
- `IExternalCommandAvailability` requiring `ActiveUIDocument != null`
- Graceful `TaskDialog` if the pane was never registered

## Prerequisites

- Dockable page previously registered via `ExternalCommandRegisterPage`

## User interaction

- Ribbon command only; toggles pane visibility

## MCP notes

- IDE shell integration sample — not a document operation suitable for MCP.

## See also

- Related: [externalcommandregisterpage.md](externalcommandregisterpage.md), [externalcommandshowpage.md](externalcommandshowpage.md)

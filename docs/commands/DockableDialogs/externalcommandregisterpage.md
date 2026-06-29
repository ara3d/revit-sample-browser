# ExternalCommandRegisterPage

| Field | Value |
|-------|-------|
| **Sample** | DockableDialogs |
| **Class** | `ExternalCommandRegisterPage` |
| **Source** | `src/DockableDialogs/CS/TopLevelCommands/ExternalCommandRegisterPage.cs` |
| **SDK ReadMe** | `src/DockableDialogs/CS/ReadMe_DockableDialogs.rtf` |
| **MCP rating** | 1/5 |

Creates and registers a dockable WPF pane with user-chosen initial docking geometry when no document is open.

## What it demonstrates

- `IDockablePaneProvider` registration via `RegisterDockableWindow`
- `DockingSetupDialog` for float position, dock side, and target pane GUID
- `IExternalCommandAvailability` when `ActiveUIDocument == null` (register before opening a project)

## Prerequisites

- Revit session without an active document (per availability rule)

## User interaction

- `DockingSetupDialog` collects docking parameters; cancel leaves pane created but unregistered

## MCP notes

- Add-in UI plumbing only — no MCP automation value.

## See also

- Related: [externalcommandshowpage.md](externalcommandshowpage.md), [externalcommandhidepage.md](externalcommandhidepage.md)

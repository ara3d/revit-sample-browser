# Command

| Field | Value |
|-------|-------|
| **Sample** | Events / ProgressNotifier |
| **Class** | `Command` |
| **Source** | `src/Events/ProgressNotifier/Command.cs` |
| **SDK ReadMe** | `src/Events/ProgressNotifier/ReadMe_ProgressNotifier.rtf` |
| **MCP rating** | 1/5 |

Opens a WPF window that subscribes to Revit progress-related events and displays a live stack of progress notifications.

## What it demonstrates

- `MainWindow` hosting progress event handlers against `Autodesk.Revit.ApplicationServices.Application`
- `ProgressStack` and `ProgressItem` for tracking nested progress callbacks

## User interaction

- Shows modal WPF dialog (`MainWindow.ShowDialog`); user dismisses when finished observing

## MCP notes

- UI-only event observer; no document mutation and no automation value for MCP

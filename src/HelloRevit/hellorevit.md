# Command

| Field | Value |
|-------|-------|
| **Sample** | HelloRevit |
| **Class** | `Command` |
| **Source** | `src/HelloRevit/CS/Command.cs` |
| **SDK ReadMe** | — |
| **MCP rating** | 2/5 |

Minimal starter command that demonstrates `TaskDialog` usage with command links for Revit and document information.

## What it demonstrates

- `TaskDialog` with `MainInstruction`, command links, footer hyperlink, and `CommonButtons`
- Branching on `TaskDialogResult` to show version info (`Application.VersionName`, etc.) or active document title/view
- No model modifications

## Prerequisites

- Any open document

## User interaction

- Sequence of task dialogs only; entirely informational

## MCP notes

- Onboarding sample only; version/document info is better served by dedicated query tools

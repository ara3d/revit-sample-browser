# Command

| Field | Value |
|-------|-------|
| **Sample** | DocumentChanged |
| **Class** | `Command` |
| **Source** | `src/DocumentChanged/CS/ChangesMonitor.cs` |
| **SDK ReadMe** | `src/DocumentChanged/CS/ReadMe_DocumentChanged.rtf` |
| **MCP rating** | 1/5 |

Re-shows the modeless changes log window that tracks `DocumentChanged` events registered by the sample's `ExternalApplication`.

## What it demonstrates

- `ExternalApplication` hooks `ControlledApplication.DocumentChanged` at startup
- `ChangesInformationForm` bound to a `DataTable` of added/deleted/modified element metadata
- Command recreates or shows `InfoForm` if closed

## Prerequisites

- `ExternalApplication` loaded so the event handler and table exist

## User interaction

- Modeless grid window updated live as the model changes

## MCP notes

- Event monitoring UI sample — agents would subscribe to events directly rather than this command.

## See also

- `ExternalApplication` in the same source file

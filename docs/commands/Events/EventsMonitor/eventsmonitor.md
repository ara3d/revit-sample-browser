# Command

| Field | Value |
|-------|-------|
| **Sample** | Events / EventsMonitor |
| **Class** | `Command` |
| **Source** | `src/Events/EventsMonitor/CS/Command.cs` |
| **SDK ReadMe** | `src/Events/EventsMonitor/CS/ReadMe_EventsMonitor.rtf` |
| **MCP rating** | 1/5 |

Opens a settings dialog so the user can choose which application-level Revit events to subscribe to, then displays live event data in an info window.

## What it demonstrates

- Application-level event registration through `ExternalApplication.AppEventMgr.Update`
- `EventsSettingForm` for selecting events; `EventsInfoWindows` for monitoring output
- Journal replay support (`JournalProcessor`) for automated regression runs in Release builds

## User interaction

- Requires `EventsSettingForm` dialog and floating info window
- Event wiring lives in `ExternalApplication`, not in the command itself

## MCP notes

- Event monitoring and UI selection are developer-debugging tools, not candidates for MCP exposure

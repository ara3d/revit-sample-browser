# Command

| Field | Value |
|-------|-------|
| **Sample** | ModelessDialog/ModelessForm_IdlingEvent |
| **Class** | `Command` |
| **Source** | `src/ModelessDialog/ModelessForm_IdlingEvent/Command.cs` |
| **SDK ReadMe** | `src/ModelessDialog/ModelessForm_IdlingEvent/Readme_ModelessForm_IdlingEvent.rtf` |
| **MCP rating** | 1/5 |

Shows a modeless dialog that drives Revit operations through the `Idling` event instead of `ExternalEvent`.

## What it demonstrates

- Subscribing to `UIApplication.Idling` to process queued requests when Revit is idle
- Same request/handler pattern as the ExternalEvent variant but with idling-based dispatch
- `Application` lifecycle managing form visibility on shutdown

## Prerequisites

- External application registration for the IdlingEvent sample

## User interaction

- Modeless UI only; command launches the form
- Requires an interactive Revit session with idling callbacks

## MCP notes

- Educational UI-threading sample; not an automation or MCP candidate.

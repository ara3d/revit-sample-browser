# Command

| Field | Value |
|-------|-------|
| **Sample** | ModelessDialog/ModelessForm_ExternalEvent |
| **Class** | `Command` |
| **Source** | `src/ModelessDialog/ModelessForm_ExternalEvent/Command.cs` |
| **SDK ReadMe** | `src/ModelessDialog/ModelessForm_ExternalEvent/Readme_ModelessForm_ExternalEvent.rtf` |
| **MCP rating** | 1/5 |

Opens a modeless WinForms dialog that posts Revit API work through an `ExternalEvent` and `RequestHandler`, demonstrating safe UI threading.

## What it demonstrates

- `IExternalApplication` (`Application`) holding a singleton modeless form
- `ExternalEvent.Create` to marshal document changes from the dialog back to Revit's API context
- `Request` / `RequestHandler` queue pattern for pick, delete, modify, and transaction operations

## Prerequisites

- Companion `Application` class registered as an external application in the sample

## User interaction

- Entirely UI-driven modeless form; command only calls `Application.ThisApp.ShowForm`
- Not meaningful for headless or MCP automation

## MCP notes

- API-pattern sample for add-in authors; no document query or modification surface suitable for MCP tools.

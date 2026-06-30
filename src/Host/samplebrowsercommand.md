# SampleBrowserCommand

| Field | Value |
|-------|-------|
| **Sample** | Host |
| **Class** | `SampleBrowserCommand` |
| **Source** | `src/SampleBrowserCommand.cs` |
| **MCP rating** | 1/5 |

Opens the Revit Sample Browser WinForms shell that lists and runs other sample commands from the assembly.

## What it demonstrates

- Reflecting `IExternalCommand` and `IExternalApplication` types into `SampleData` entries
- `SampleBrowserMainForm` with `RevitActionEvent` (from Common `RevitToolkitEvents`) for safe Revit API execution from UI threads
- `TraceListener` routing debug output to a RichTextBox log pane

## Prerequisites

- Sample browser add-in loaded in Revit

## User interaction

- Entire product UI; not a document operation

## MCP notes

- Host/infrastructure command; agents invoke individual sample commands directly, not through this browser

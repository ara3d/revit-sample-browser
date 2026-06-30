# N3P_ExternalEvent

| Field | Value |
|-------|-------|
| **Sample** | N3P_ExternalEvent |
| **Class** | `N3P_ExternalEvent` |
| **Source** | src/N3P_ExternalEvent/N3P_ExternalEvent.cs |
| **MCP rating** | 3/5 |

Opens a modeless dialog that queues Revit API work through `RevitActionEvent` from Common.

## What it demonstrates

- [Nice3point RevitToolkit](https://github.com/Nice3point/RevitToolkit) — `ExternalEvent` via `RevitToolkitEvents.RevitActionEvent`
- Modeless WinForms pattern without manual `IExternalEventHandler` boilerplate

## Prerequisites

- Revit 2025 with an open project document and elements selected before clicking the dialog button

## User interaction

Shows a modeless form with one button. Select elements, click the button, and comments are updated on up to ten selected elements.

## MCP notes

- Proposed tool: revit_n3p_external_event
- MCP descriptor: src/N3P_ExternalEvent/n3p-external-event.json

## See also

- MCP descriptor: src/N3P_ExternalEvent/n3p-external-event.json
- Package: [Nice3point.Revit.Toolkit on NuGet](https://www.nuget.org/packages/Nice3point.Revit.Toolkit)

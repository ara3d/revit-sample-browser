# N3P_View

| Field | Value |
|-------|-------|
| **Sample** | N3P_View |
| **Class** | N3P_View |
| **Source** | src/N3P_View/N3P_View.cs |
| **MCP rating** | 5/5 |

Counts elements visible/selectable in the active view and checks spatial field managers.

## What it demonstrates

- [Nice3point RevitExtensions](https://github.com/Nice3point/RevitExtensions) — View section
- Read-only command; output is written to the sample browser Debug log via Debug.WriteLine
- Uses the Nice3point.Revit.Extensions NuGet package (2025.*)

## Prerequisites

- Revit 2025 with a model containing relevant elements (walls for most samples; pipes or structural content for discipline demos)

## User interaction

No dialog. Uses current selection when applicable, otherwise queries the document. Fully headless aside from requiring an open document.

## MCP notes

- Proposed tool: revit_n3p_view
- MCP descriptor: src/N3P_View/n3p-view.json

## See also

- MCP descriptor: src/N3P_View/n3p-view.json
- Package: [Nice3point.Revit.Extensions on NuGet](https://www.nuget.org/packages/Nice3point.Revit.Extensions)

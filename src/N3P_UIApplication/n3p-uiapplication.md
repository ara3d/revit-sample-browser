# N3P_UIApplication

| Field | Value |
|-------|-------|
| **Sample** | N3P_UIApplication |
| **Class** | N3P_UIApplication |
| **Source** | src/N3P_UIApplication/N3P_UIApplication.cs |
| **MCP rating** | 4/5 |

Lists ribbon panels and notes ribbon authoring patterns for IExternalApplication.

## What it demonstrates

- [Nice3point RevitExtensions](https://github.com/Nice3point/RevitExtensions) — UIApplication section
- Read-only command; output is written to the sample browser Debug log via Debug.WriteLine
- Uses the Nice3point.Revit.Extensions NuGet package (2025.*)

## Prerequisites

- Revit 2025 with a model containing relevant elements (walls for most samples; pipes or structural content for discipline demos)

## User interaction

No dialog. Uses current selection when applicable, otherwise queries the document. Fully headless aside from requiring an open document.

## MCP notes

- Proposed tool: revit_n3p_uiapplication
- MCP descriptor: src/N3P_UIApplication/n3p-uiapplication.json

## See also

- MCP descriptor: src/N3P_UIApplication/n3p-uiapplication.json
- Package: [Nice3point.Revit.Extensions on NuGet](https://www.nuget.org/packages/Nice3point.Revit.Extensions)

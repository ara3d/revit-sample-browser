# N3P_Application

| Field | Value |
|-------|-------|
| **Sample** | N3P_Application |
| **Class** | N3P_Application |
| **Source** | src/N3P_Application/N3P_Application.cs |
| **MCP rating** | 5/5 |

Logs optional Revit module availability flags and AsControlledApplication.

## What it demonstrates

- [Nice3point RevitExtensions](https://github.com/Nice3point/RevitExtensions) — Application section
- Read-only command; output is written to the sample browser Debug log via Debug.WriteLine
- Uses the Nice3point.Revit.Extensions NuGet package (2025.*)

## Prerequisites

- Revit 2025 with a model containing relevant elements (walls for most samples; pipes or structural content for discipline demos)

## User interaction

No dialog. Uses current selection when applicable, otherwise queries the document. Fully headless aside from requiring an open document.

## MCP notes

- Proposed tool: revit_n3p_application
- MCP descriptor: src/N3P_Application/n3p-application.json

## See also

- MCP descriptor: src/N3P_Application/n3p-application.json
- Package: [Nice3point.Revit.Extensions on NuGet](https://www.nuget.org/packages/Nice3point.Revit.Extensions)

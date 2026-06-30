# N3P_Structure

| Field | Value |
|-------|-------|
| **Sample** | N3P_Structure |
| **Class** | N3P_Structure |
| **Source** | src/N3P_Structure/N3P_Structure.cs |
| **MCP rating** | 4/5 |

Inspects structural framing join flags or rebar shape parameters when present.

## What it demonstrates

- [Nice3point RevitExtensions](https://github.com/Nice3point/RevitExtensions) — Disciplines / Structure section
- Read-only command; output is written to the sample browser Debug log via Debug.WriteLine
- Uses the Nice3point.Revit.Extensions NuGet package (2025.*)

## Prerequisites

- Revit 2025 with a model containing relevant elements (walls for most samples; pipes or structural content for discipline demos)

## User interaction

No dialog. Uses current selection when applicable, otherwise queries the document. Fully headless aside from requiring an open document.

## MCP notes

- Proposed tool: revit_n3p_structure
- MCP descriptor: src/N3P_Structure/n3p-structure.json

## See also

- MCP descriptor: src/N3P_Structure/n3p-structure.json
- Package: [Nice3point.Revit.Extensions on NuGet](https://www.nuget.org/packages/Nice3point.Revit.Extensions)

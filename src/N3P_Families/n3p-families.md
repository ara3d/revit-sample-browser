# N3P_Families

| Field | Value |
|-------|-------|
| **Sample** | N3P_Families |
| **Class** | N3P_Families |
| **Source** | src/N3P_Families/N3P_Families.cs |
| **MCP rating** | 4/5 |

Queries family symbols/instances and wall join relationships using modeling extensions.

## What it demonstrates

- [Nice3point RevitExtensions](https://github.com/Nice3point/RevitExtensions) — Families and modeling section
- Read-only command; output is written to the sample browser Debug log via Debug.WriteLine
- Uses the Nice3point.Revit.Extensions NuGet package (2025.*)

## Prerequisites

- Revit 2025 with a model containing relevant elements (walls for most samples; pipes or structural content for discipline demos)

## User interaction

No dialog. Uses current selection when applicable, otherwise queries the document. Fully headless aside from requiring an open document.

## MCP notes

- Proposed tool: revit_n3p_families
- MCP descriptor: src/N3P_Families/n3p-families.json

## See also

- MCP descriptor: src/N3P_Families/n3p-families.json
- Package: [Nice3point.Revit.Extensions on NuGet](https://www.nuget.org/packages/Nice3point.Revit.Extensions)

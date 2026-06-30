# N3P_Display

| Field | Value |
|-------|-------|
| **Sample** | N3P_Display |
| **Class** | N3P_Display |
| **Source** | src/N3P_Display/N3P_Display.cs |
| **MCP rating** | 5/5 |

Formats ForgeTypeId labels and converts Revit Color values to hex/RGB strings.

## What it demonstrates

- [Nice3point RevitExtensions](https://github.com/Nice3point/RevitExtensions) — ForgeTypeId, Label, Color section
- Read-only command; output is written to the sample browser Debug log via Debug.WriteLine
- Uses the Nice3point.Revit.Extensions NuGet package (2025.*)

## Prerequisites

- Revit 2025 with a model containing relevant elements (walls for most samples; pipes or structural content for discipline demos)

## User interaction

No dialog. Uses current selection when applicable, otherwise queries the document. Fully headless aside from requiring an open document.

## MCP notes

- Proposed tool: revit_n3p_display
- MCP descriptor: src/N3P_Display/n3p-display.json

## See also

- MCP descriptor: src/N3P_Display/n3p-display.json
- Package: [Nice3point.Revit.Extensions on NuGet](https://www.nuget.org/packages/Nice3point.Revit.Extensions)

# N3P_ExternalCommand

| Field | Value |
|-------|-------|
| **Sample** | N3P_ExternalCommand |
| **Class** | `N3P_ExternalCommand` |
| **Source** | src/N3P_ExternalCommand/N3P_ExternalCommand.cs |
| **MCP rating** | 5/5 |

Read-only command using the toolkit `ExternalCommand` base class and `RevitContext` from Common.

## What it demonstrates

- [Nice3point RevitToolkit](https://github.com/Nice3point/RevitToolkit) — simplified `Execute()` override
- `RevitContext.IsRevitInApiMode`, `ActiveDocument`, and `ActiveView`

## Prerequisites

- Revit 2025 with an open project document

## User interaction

No dialog. Output is written to the sample browser Debug log.

## MCP notes

- Proposed tool: revit_n3p_external_command
- MCP descriptor: src/N3P_ExternalCommand/n3p-external-command.json

## See also

- MCP descriptor: src/N3P_ExternalCommand/n3p-external-command.json
- Package: [Nice3point.Revit.Toolkit on NuGet](https://www.nuget.org/packages/Nice3point.Revit.Toolkit)

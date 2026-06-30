# N3P_Scopes

| Field | Value |
|-------|-------|
| **Sample** | N3P_Scopes |
| **Class** | `N3P_Scopes` |
| **Source** | src/N3P_Scopes/N3P_Scopes.cs |
| **MCP rating** | 4/5 |

Copies a wall with RevitToolkit failure and dialog suppression scopes from Common.

## What it demonstrates

- [Nice3point RevitToolkit](https://github.com/Nice3point/RevitToolkit) — `RevitApiContext` / `RevitContext` scopes via `RevitToolkitScopes`
- `RevitToolkitCopyPaste` for duplicate-type handling during copy
- `ExternalCommand` base class from the toolkit

## Prerequisites

- Revit 2025 with a model containing at least one wall

## User interaction

No dialog. Output is written to the sample browser Debug log.

## MCP notes

- Proposed tool: revit_n3p_scopes
- MCP descriptor: src/N3P_Scopes/n3p-scopes.json

## See also

- MCP descriptor: src/N3P_Scopes/n3p-scopes.json
- Package: [Nice3point.Revit.Toolkit on NuGet](https://www.nuget.org/packages/Nice3point.Revit.Toolkit)

# CmdCreateSharedParameters

| Field | Value |
|-------|-------|
| **Sample** | ExportCncFab |
| **Class** | `CmdCreateSharedParameters` |
| **Source** | `src/ExportCncFab/Commands.cs` |
| **MCP rating** | 4/5 |

Creates and binds CNC fabrication export-tracking shared parameters to the Parts category.

## What it demonstrates

- Shared parameter file initialization when none is configured
- `DefinitionGroup` and `ExternalDefinitionCreationOptions` for `CncFabIsExported`, `CncFabExportedFirst`, `CncFabExportedLast`
- Instance binding to `OST_Parts` via `ParameterBindings.Insert` with `GroupTypeId.General`
- `SpecTypeId.Boolean.YesNo` and `SpecTypeId.String.Text` parameter types

## Prerequisites

- Active project document

## User interaction

- No dialog; runs immediately and returns `Result.Succeeded`

## MCP notes

- Proposed tool: `revit_create_cnc_fab_shared_parameters`
- Parameters: none required
- Returns: confirmation that parameters were created or already exist
- Suitable for headless MCP execution without refactoring
- MCP descriptor: `src/ExportCncFab/cmdcreatesharedparameters.json`

## See also

- MCP descriptor: `src/ExportCncFab/cmdcreatesharedparameters.json`
- Related: [cmddxf.md](cmddxf.md), [cmdsat.md](cmdsat.md)
- Upstream: [jeremytammik/ExportCncFab](https://github.com/jeremytammik/ExportCncFab)

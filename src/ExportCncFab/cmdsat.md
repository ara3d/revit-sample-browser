# CmdSat

| Field | Value |
|-------|-------|
| **Sample** | ExportCncFab |
| **Class** | `CmdSat` |
| **Source** | `src/ExportCncFab/Commands.cs` |
| **MCP rating** | 4/5 |

Exports pre-selected or interactively picked gyp wallboard wall **Part** elements from the active 3D view to individual SAT files for CNC fabrication.

## What it demonstrates

- Same export workflow as `CmdDxf`, delegating to `CmdDxf.Execute2` with `exportToSatFormat: true`
- `Document.Export` with `SATExportOptions` instead of `DXFExportOptions`
- Shared export logic for DXF and SAT output formats

## Prerequisites

- Same as `CmdDxf`: 3D parts-only view, wall-derived parts, CNC shared parameters initialized

## User interaction

- Same as `CmdDxf`: pre-selection or `PickObjects`, folder browser, validation dialogs

## MCP notes

- Proposed tool: `revit_export_wall_parts_sat`
- Parameters: `part_ids`, `output_folder`, optional `view_id`
- Returns: list of exported SAT file paths
- Headless use requires the same refactoring as `CmdDxf`
- MCP descriptor: `src/ExportCncFab/cmdsat.json`

## See also

- MCP descriptor: `src/ExportCncFab/cmdsat.json`
- Related: [cmddxf.md](cmddxf.md), [cmdcreatesharedparameters.md](cmdcreatesharedparameters.md)
- Upstream: [jeremytammik/ExportCncFab](https://github.com/jeremytammik/ExportCncFab)

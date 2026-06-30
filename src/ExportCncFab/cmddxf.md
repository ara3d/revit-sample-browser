# CmdDxf

| Field | Value |
|-------|-------|
| **Sample** | ExportCncFab |
| **Class** | `CmdDxf` |
| **Source** | `src/ExportCncFab/Commands.cs` |
| **MCP rating** | 4/5 |

Exports pre-selected or interactively picked gyp wallboard wall **Part** elements from the active 3D view to individual DXF files for CNC fabrication.

## What it demonstrates

- Wall-part selection via pre-selection or `ISelectionFilter` (`WallPartSelectionFilter` checks `OST_Walls` source category)
- Temporary view isolation with `View.IsolateElementTemporary` inside a rolled-back `TransactionGroup`
- `Document.Export` with `DXFExportOptions` for per-part geometry output
- CNC export history tracking via `ExportParameters` shared parameters
- Auto-dismissal of the temp-view-mode export warning via `DialogBoxShowing`

## Prerequisites

- Active project document with a 3D view as the active view
- View `PartsVisibility` set to **Show Parts Only**
- Wall-derived parts (gyp wallboard); run `CmdCreateSharedParameters` first to bind tracking parameters

## User interaction

- Accepts pre-selected wall parts or prompts with `PickObjects`
- `FolderBrowserDialog` for output directory; cancel returns `Result.Cancelled`
- `TaskDialog` messages for validation failures

## MCP notes

- Proposed tool: `revit_export_wall_parts_dxf`
- Parameters: `part_ids` (array of element ids), `output_folder`, optional `view_id`
- Returns: list of exported file paths and updated export-history parameter values
- Headless use requires replacing `FolderBrowserDialog` and optional pick with supplied ids and folder path
- MCP descriptor: `src/ExportCncFab/cmddxf.json`

## See also

- MCP descriptor: `src/ExportCncFab/cmddxf.json`
- Related: [cmdsat.md](cmdsat.md), [cmdcreatesharedparameters.md](cmdcreatesharedparameters.md)
- Upstream: [jeremytammik/ExportCncFab](https://github.com/jeremytammik/ExportCncFab)

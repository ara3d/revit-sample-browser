# MeasurePanelArea

| Field | Value |
|-------|-------|
| **Sample** | Massing/MeasurePanelArea |
| **Class** | `MeasurePanelArea` |
| **Source** | `src/Massing/MeasurePanelArea/CS/Command.cs` |
| **SDK ReadMe** | `src/Massing/MeasurePanelArea/CS/ReadMe_MeasurePanelArea.rtf` |
| **MCP rating** | 5/5 |

Computes divided-surface panel areas, classifies panels into min/mid/max area buckets, optionally swaps panel types, and exports id/area pairs to a text file.

## What it demonstrates

- Walking `DividedSurface` grid nodes and reading panel geometry for area
- User-defined area thresholds and panel type reassignment on tile instances
- Optional selection filtering to specific divided surfaces
- Writing results to `_PanelArea.txt` beside the add-in assembly

## Prerequisites

- Mass family with `DividedSurface` and multiple panel types loaded
- Panel types listed in `FrmPanelArea` combo boxes

## User interaction

- `FrmPanelArea` collects min/max area values, type names, and triggers Compute
- `FrmPanelArea` logic in the same folder is separable for headless batch runs

## MCP notes

- Proposed tool: `revit_analyze_panel_areas`
- Parameters: `divided_surface_ids[]`, `min_area`, `max_area`, optional `panel_type_mapping`, `export_path`
- Returns: per-panel element id, computed area, and assigned category (below min, in range, above max)
- Refactor: extract `btnCompute_Click` logic; accept element ids instead of UI text fields
- MCP descriptor: `docs/mcp/Massing/MeasurePanelArea/measurepanelarea.json`

## See also

- MCP descriptor: `docs/mcp/Massing/MeasurePanelArea/measurepanelarea.json`

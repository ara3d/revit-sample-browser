# Command

| Field | Value |
|-------|-------|
| **Sample** | GetElementImage |
| **Class** | `Command` |
| **Source** | `src/GetElementImage/Command.cs` |
| **MCP rating** | 4/5 |
| **Upstream** | [jeremytammik/GetElementImage](https://github.com/jeremytammik/GetElementImage) (MIT) |

Exports PNG snapshots of selected element(s) from multiple predefined 3D view angles (Isometric, North, East, Top). Adapted from Jeremy Tammik's GetElementImage sample; temporary views are created in a transaction that is rolled back so the model is not modified.

## What it demonstrates

- Interactive or pre-selected element pick for any non-view element
- Temporary `View3D` creation with `ViewOrientation3D` yaw/pitch setup
- Element isolation via `View.HideElements` / `UnhideElements`
- Multi-view PNG export with `ImageExportOptions` and `Document.ExportImage`
- Transaction rollback to avoid persisting export views

## Prerequisites

- An active project document with at least one exportable element

## User interaction

- Uses pre-selection when elements are already selected; otherwise prompts for picks
- Writes PNG files to `%TEMP%\GetElementImage\` and opens them in the default viewer (skipped during journal playback)
- Rejects view elements in the selection filter

## MCP notes

- Proposed tool: `revit_export_element_images`
- Parameters: `element_ids` (array of integers), optional `output_dir`
- Returns: array of exported PNG file paths
- Current command requires UI for element selection; headless use would need `element_ids` and `output_dir` parameters
- MCP descriptor: `src/GetElementImage/get-element-image.json`

## See also

- MCP descriptor: `src/GetElementImage/get-element-image.json`
- Related: `TBC_ExportImage` exports a whole-document preview image, not per-element multi-angle PNGs

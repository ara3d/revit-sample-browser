# ExportToMaj

| Field | Value |
|-------|-------|
| **Sample** | FabricationPartLayout |
| **Class** | `ExportToMaj` |
| **Source** | `src/FabricationPartLayout/ExportToMAJ.cs` |
| **SDK ReadMe** | `src/FabricationPartLayout/Readme_FabricationPartLayout.rtf` |
| **MCP rating** | 5/5 |

Exports selected fabrication parts to a MAJ fabrication job file via a save dialog.

## What it demonstrates

- `FabricationPart.SaveAsFabricationJob` with `FabricationSaveJobOptions`
- Validating selection contains at least one `FabricationPart`
- `FileSaveDialog` for output path selection

## Prerequisites

- One or more fabrication parts in the current selection

## User interaction

- Requires `FileSaveDialog`; not headless without a supplied output path

## MCP notes

- Proposed tool: `revit_export_fabrication_maj`
- Parameters: `element_ids[]`, `output_path`
- Returns: count of parts written and file path
- MCP descriptor: `src/FabricationPartLayout/exporttomaj.json`

## See also

- MCP descriptor: `src/FabricationPartLayout/exporttomaj.json`

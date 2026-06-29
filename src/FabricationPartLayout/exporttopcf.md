# ExportToPcf

| Field | Value |
|-------|-------|
| **Sample** | FabricationPartLayout |
| **Class** | `ExportToPcf` |
| **Source** | `src/FabricationPartLayout/CS/ExportToPCF.cs` |
| **SDK ReadMe** | `src/FabricationPartLayout/CS/Readme_FabricationPartLayout.rtf` |
| **MCP rating** | 5/5 |

Sets a spool name on selected fabrication parts and exports them to a PCF file.

## What it demonstrates

- Assigning `FabricationPart.SpoolName` before export
- `FabricationUtils.ExportToPCF` with user-chosen output path

## Prerequisites

- Selected fabrication parts

## User interaction

- `FileSaveDialog` for destination; spool name hard-coded to **My Spool**

## MCP notes

- Proposed tool: `revit_export_fabrication_pcf`
- Parameters: `element_ids[]`, `output_path`, optional `spool_name`
- Returns: output file path
- MCP descriptor: `src/FabricationPartLayout/exporttopcf.json`

## See also

- MCP descriptor: `src/FabricationPartLayout/exporttopcf.json`

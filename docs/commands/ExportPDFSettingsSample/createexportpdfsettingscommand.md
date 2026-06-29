# CreateExportPdfSettingsCommand

| Field | Value |
|-------|-------|
| **Sample** | ExportPDFSettingsSample |
| **Class** | `CreateExportPdfSettingsCommand` |
| **Source** | `src/ExportPDFSettingsSample/CS/Application.cs` |
| **MCP rating** | 5/5 |

Creates a new `ExportPDFSettings` element in the document with default `PDFExportOptions` under the name "sample".

## What it demonstrates

- Instantiating `PDFExportOptions` and persisting them via `ExportPDFSettings.Create`
- Document-level storage of PDF export presets as `ExportPDFSettings` elements

## User interaction

- Ribbon button; no dialog; fixed preset name "sample"

## MCP notes

- Proposed tool: `revit_create_pdf_export_settings`
- Parameters: `name`, optional serialized `PDFExportOptions` fields (paper format, combine, naming rules)
- Returns: new `ExportPDFSettings` element id
- MCP descriptor: `docs/mcp/ExportPDFSettingsSample/createexportpdfsettingscommand.json`

## See also

- MCP descriptor: `docs/mcp/ExportPDFSettingsSample/createexportpdfsettingscommand.json`

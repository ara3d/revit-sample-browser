# ModifyExportPdfSettingsCommand

| Field | Value |
|-------|-------|
| **Sample** | ExportPDFSettingsSample |
| **Class** | `ModifyExportPdfSettingsCommand` |
| **Source** | `src/ExportPDFSettingsSample/Application.cs` |
| **MCP rating** | 5/5 |

Updates PDF export options on the "sample" `ExportPDFSettings`: paper format, crop boundary visibility, and combined-export flag.

## What it demonstrates

- `ExportPDFSettings.GetOptions` / `SetOptions` round-trip for `PDFExportOptions`
- Changing `ExportPaperFormat`, `HideCropBoundaries`, and `Combine` properties

## Prerequisites

- Existing "sample" settings created by `CreateExportPdfSettingsCommand`

## User interaction

- Ribbon button; applies fixed option values with no dialog

## MCP notes

- Proposed tool: `revit_modify_pdf_export_settings`
- Parameters: `settings_name`, optional fields for paper format, hide crop boundaries, combine, and other `PDFExportOptions` members
- Returns: updated settings element id
- MCP descriptor: `src/ExportPDFSettingsSample/modifyexportpdfsettingscommand.json`

## See also

- MCP descriptor: `src/ExportPDFSettingsSample/modifyexportpdfsettingscommand.json`

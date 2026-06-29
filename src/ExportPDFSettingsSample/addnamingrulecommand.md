# AddNamingRuleCommand

| Field | Value |
|-------|-------|
| **Sample** | ExportPDFSettingsSample |
| **Class** | `AddNamingRuleCommand` |
| **Source** | `src/ExportPDFSettingsSample/CS/Application.cs` |
| **MCP rating** | 4/5 |

Adds a sheet **Approved By** parameter rule to the PDF export naming rule on the document's `ExportPDFSettings` named "sample".

## What it demonstrates

- Locating settings with `ExportPDFSettings.FindByName`
- Reading and modifying naming rules via `PDFExportOptions.GetNamingRule` / `SetNamingRule`
- Building `TableCellCombinedParameterData` for `BuiltInParameter.SHEET_APPROVED_BY` on `OST_Sheets`

## Prerequisites

- Run `CreateExportPdfSettingsCommand` first to create the "sample" settings
- `PDFExportOptions.Combine` must be **false** (naming rules are ignored when combined export is enabled)

## User interaction

- Ribbon button; runs headlessly with fixed parameter choices

## MCP notes

- Proposed tool: `revit_add_pdf_export_naming_rule`
- Parameters: `settings_name`, rule entries (category, parameter id, prefix, separator)
- Returns: updated naming rule count
- MCP descriptor: `src/ExportPDFSettingsSample/addnamingrulecommand.json`

## See also

- MCP descriptor: `src/ExportPDFSettingsSample/addnamingrulecommand.json`

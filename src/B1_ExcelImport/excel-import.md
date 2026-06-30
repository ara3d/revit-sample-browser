# CmdExcelImport

| Field | Value |
|-------|-------|
| **Sample** | B1_ExcelImport |
| **Class** | `CmdExcelImport` |
| **Source** | `src/B1_ExcelImport/CmdExcelImport.cs` |
| **Origin** | [bimone/addins-excelexporterimporter](https://github.com/bimone/addins-excelexporterimporter) (MIT) |
| **MCP rating** | 4/5 |

Imports Excel data back into Revit schedules and editable standard tables that were previously exported in bidirectional mode.

## What it demonstrates

- Worksheet discovery by schedule `UniqueId` and standards GUID rows
- `ScheduleImporter` and `StandardsImporter` writing parameter values inside transactions
- Read-only parameter detection (gray locked cells in export are skipped on import)
- WPF MVVM import workflow with available vs unavailable worksheet lists

## Prerequisites

- Excel file produced by `CmdExcelExport` in **bidirectional** mode (layout-only exports are not importable)
- Matching schedules/standards must still exist in the active document

## User interaction

- WPF import dialog: browse for `.xlsx`, select importable worksheets, run import
- Progress window during import; summary dialog on completion or partial errors

## MCP notes

- Proposed tool: `revit_import_schedules_from_excel`
- Parameters: `input_path`, `worksheet_names`
- Import only updates existing export-generated rows/columns; adding rows or columns in Excel is not supported
- MCP descriptor: `src/B1_ExcelImport/excel-import.json`

## See also

- MCP descriptor: `src/B1_ExcelImport/excel-import.json`
- `B1_ExcelExport` — export schedules to Excel for round-trip editing

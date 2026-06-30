# CmdExcelExport

| Field | Value |
|-------|-------|
| **Sample** | B1_ExcelExport |
| **Class** | `CmdExcelExport` |
| **Source** | `src/B1_ExcelExport/CmdExcelExport.cs` |
| **Origin** | [bimone/addins-excelexporterimporter](https://github.com/bimone/addins-excelexporterimporter) (MIT) |
| **MCP rating** | 4/5 |

Exports Revit schedules and standard project tables to Excel in bidirectional (editable) or layout-only (basic) mode.

## What it demonstrates

- `ViewSchedule` enumeration and selective export via `ScheduleExporter`
- Standards export (line styles, object styles, project information, etc.) via `StandardsExporter`
- EPPlus workbook generation with read-only parameter locking and color legend sheets
- WPF MVVM dialog hosted from an `IExternalCommand`

## Prerequisites

- Open project document with schedules and/or exportable standards
- Write access to the target folder for `.xlsx` output

## User interaction

- WPF export dialog: pick schedules and/or standards, choose bidirectional vs layout-only mode, save file(s)
- Progress window during export; overwrite confirmation when target files exist

## MCP notes

- Proposed tool: `revit_export_schedules_to_excel`
- Parameters: `schedule_names`, `include_standards`, `export_mode` (`bidirectional` | `layout_only`), `output_path`
- Requires UI refactoring to separate file-picker and selection from the view model before headless use
- MCP descriptor: `src/B1_ExcelExport/excel-export.json`

## See also

- MCP descriptor: `src/B1_ExcelExport/excel-export.json`
- `B1_ExcelImport` — import changes from Excel files exported in bidirectional mode

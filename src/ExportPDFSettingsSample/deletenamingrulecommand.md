# DeleteNamingRuleCommand

| Field | Value |
|-------|-------|
| **Sample** | ExportPDFSettingsSample |
| **Class** | `DeleteNamingRuleCommand` |
| **Source** | `src/ExportPDFSettingsSample/CS/Application.cs` |
| **MCP rating** | 4/5 |

Removes the **Approved By** sheet parameter entry from the PDF export naming rule on the "sample" `ExportPDFSettings`.

## What it demonstrates

- Finding a specific naming rule entry by category and parameter id
- Updating options with `options.SetNamingRule` after `List.Remove`

## Prerequisites

- Existing "sample" `ExportPDFSettings` with a non-combined export configuration
- Prior `AddNamingRuleCommand` run (or equivalent rule present)

## User interaction

- Ribbon button; headless with hard-coded rule target

## MCP notes

- Proposed tool: `revit_delete_pdf_export_naming_rule`
- Parameters: `settings_name`, `category_id`, `parameter_id`
- Returns: success flag and remaining rule count
- MCP descriptor: `src/ExportPDFSettingsSample/deletenamingrulecommand.json`

## See also

- MCP descriptor: `src/ExportPDFSettingsSample/deletenamingrulecommand.json`

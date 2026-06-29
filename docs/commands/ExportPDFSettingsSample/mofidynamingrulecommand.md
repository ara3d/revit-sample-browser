# MofidyNamingRuleCommand

| Field | Value |
|-------|-------|
| **Sample** | ExportPDFSettingsSample |
| **Class** | `MofidyNamingRuleCommand` |
| **Source** | `src/ExportPDFSettingsSample/CS/Application.cs` |
| **MCP rating** | 2/5 |

Modifies the sample value on the **Approved By** naming rule entry in the "sample" PDF export settings and reorders rules by sample value.

## What it demonstrates

- Locating an existing `TableCellCombinedParameterData` rule and changing `SampleValue`
- Re-sorting the naming rule list before calling `SetNamingRule`

## Prerequisites

- "sample" settings with non-combined export and an existing Approved By rule

## User interaction

- Ribbon button labeled "Mofidy Naming Rule" (SDK typo preserved); headless

## MCP notes

- Same API surface as add/delete naming rule commands but hard-coded to one demo field; better covered by a generalized modify-naming-rule MCP tool

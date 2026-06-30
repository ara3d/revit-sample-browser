# Command

| Field | Value |
|-------|-------|
| **Sample** | AnalyticalSupportData_Info |
| **Class** | `Command` |
| **Source** | `src/AnalyticalSupportData_Info/AnalyticalSupportData_Info.cs` |
| **MCP rating** | 5/5 |

Reports analytical support information for the current selection by resolving associated `AnalyticalElement` data and showing it in a grid.

## What it demonstrates

- Mapping physical elements to analytical models via `AnalyticalToPhysicalAssociationManager`
- Reading `AnalyticalModelSupport` and `AnalyticalSupportType` for each support
- Building a `DataTable` of element id, type name, and support descriptions for UI display

## Prerequisites

- Structural model with analytically associated elements; user must pre-select elements

## User interaction

- Reads current selection (no pick prompt); shows `AnalyticalSupportDataInfoForm`
- Data extraction in `StoreInformationInDataTable` is UI-independent and ideal for MCP

## MCP notes

- Proposed tool: `revit_get_analytical_support_info`
- Parameters: `element_ids[]` (optional; defaults to current selection)
- Returns: array of `{ element_id, type_name, supports[] }` records
- MCP descriptor: `src/AnalyticalSupportData_Info/analyticalsupportdata-info.json`

## See also

- MCP descriptor: `src/AnalyticalSupportData_Info/analyticalsupportdata-info.json`

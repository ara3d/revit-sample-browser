# Command

| Field | Value |
|-------|-------|
| **Sample** | ElementFilterSample |
| **Class** | `Command` |
| **Source** | `src/ElementFilterSample/Command.cs` |
| **MCP rating** | 5/5 |

Opens a dialog to browse, create, edit, and apply view filters (`ParameterFilterElement`) in the active document.

## What it demonstrates

- Listing filters with `FiltersUtil.GetViewFilters` and `FilteredElementCollector`
- Building `FilterRule` trees from `ElementParameterFilter` and converting rules via `FiltersUtil.CreateFilterRuleBuilder`
- Creating filters in `NewFilterForm` and assigning them to views in `ViewFiltersForm`
- Supported rule types: string, double, integer, and element id parameters

## Prerequisites

- Any project document; sample works on current view filters and categories

## User interaction

- `ViewFiltersForm` and `NewFilterForm` provide full UI for filter management
- Core utility methods in `FiltersUtil` and `FilterData` are separable from the dialogs

## MCP notes

- Proposed tools: `revit_list_view_filters` (read-only) and `revit_create_or_update_view_filter`
- Parameters: filter name, category ids, rule definitions (parameter, evaluator, value), optional target view ids
- Returns: filter element id and affected view ids
- MCP descriptor: `src/ElementFilterSample/elementfiltersample.json`

## See also

- MCP descriptor: `src/ElementFilterSample/elementfiltersample.json`

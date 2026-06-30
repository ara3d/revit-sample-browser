# Loads

| Field | Value |
|-------|-------|
| **Sample** | Loads |
| **Class** | `Loads` |
| **Source** | `src/Loads/Loads.cs` |
| **MCP rating** | 4/5 |

Opens a structural settings-style dialog to view and edit load cases, load natures, load combinations, usages, and combination formulas in the current project.

## What it demonstrates

- Enumerating `LoadCase`, `LoadNature`, `LoadCombination`, and `LoadUsage` elements
- Creating and duplicating load cases via `LoadCaseDeal`
- Managing load combinations and formulas through `LoadCombinationDeal`
- Rolling back a wrapping transaction when the user cancels the form

## Prerequisites

- Structural project; familiarity with Revit structural load settings

## User interaction

- `LoadsForm` with tab pages for load cases and load combinations; all edits flow through modal UI
- Public methods on `Loads` (`NewLoadCombination`, `DeleteCombination`, `NewLoadUsage`, etc.) encapsulate API calls and could be called headlessly

## MCP notes

- Proposed tool: `revit_manage_structural_loads`
- Parameters: operation (`list`, `create_combination`, `delete_combination`, `add_usage`, …) plus names and indices
- Returns: tabular load case, nature, combination, and usage data
- Refactor: extract `PrepareData` and deal classes from WinForms; accept explicit element ids instead of grid indices
- MCP descriptor: `src/Loads/loads.json`

## See also

- MCP descriptor: `src/Loads/loads.json`

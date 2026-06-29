# RebarParas

| Field | Value |
|-------|-------|
| **Sample** | AreaReinParameters |
| **Class** | `RebarParas` |
| **Source** | `src/AreaReinParameters/AreaReinParameters.cs` |
| **SDK ReadMe** | `src/AreaReinParameters/ReadMe_AreaReinParameters.rtf` |
| **MCP rating** | 2/5 |

Dumps all parameter names and values for the first selected `Rebar` element to a task dialog.

## What it demonstrates

- Iterating `Rebar.Parameters` and reading values by `StorageType` (double, element id, integer, string)
- Resolving element-id parameters to element names

## Prerequisites

- Pre-select at least one rebar element

## User interaction

- No input dialog; results shown via `TaskDialog.Show`
- Processes only the first matching rebar in the selection

## MCP notes

- Pattern is useful for parameter inspection, but output is UI-only; a dedicated parameter-query tool would supersede this sample

## See also

- [`areareinparameters.md`](areareinparameters.md) — area reinforcement editor

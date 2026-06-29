# Command

| Field | Value |
|-------|-------|
| **Sample** | Units |
| **Class** | `Command` |
| **Source** | `src/Units/Command.cs` |
| **SDK ReadMe** | `src/Units/Readme_Units.rtf` |
| **MCP rating** | 5/5 |

Displays and edits project unit formatting (discipline specs, decimal symbols, digit grouping) then commits changes with `Document.SetUnits`.

## What it demonstrates

- `Document.GetUnits` / `SetUnits` within a named transaction
- `UnitUtils.GetAllDisciplines`, `GetAllMeasurableSpecs`, and per-spec `FormatOptions`
- `UnitsForm` grid editing with nested `FormatForm` for individual spec format options
- `DecimalSymbol`, `DigitGroupingAmount`, and `DigitGroupingSymbol` document-level settings

## Prerequisites

- Open project document

## User interaction

- `UnitsForm` modal dialog; changes apply only on OK
- Headless automation would read/write `Units` directly without the form

## MCP notes

- Proposed tools: `revit_get_units` (read) and `revit_set_units` (write)
- Parameters: optional discipline filter; format option overrides per `ForgeTypeId` spec
- Returns: current unit formatting settings as structured data
- MCP descriptor: `src/Units/units.json`

## See also

- MCP descriptor: `src/Units/units.json`

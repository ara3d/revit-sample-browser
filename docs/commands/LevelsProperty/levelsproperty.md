# Command

| Field | Value |
|-------|-------|
| **Sample** | LevelsProperty |
| **Class** | `Command` |
| **Source** | `src/LevelsProperty/CS/Command.cs` |
| **SDK ReadMe** | `src/LevelsProperty/CS/ReadMe_LevelsProperty.rtf` |
| **MCP rating** | 5/5 |

Lists every `Level` in the active document, displays name and elevation in a dialog, and supports creating, renaming, elevating, or deleting levels through the API.

## What it demonstrates

- Collecting levels with `FilteredElementCollector.OfClass(typeof(Level))`
- Reading `BuiltInParameter.LEVEL_ELEV` and converting display units via `Unit.CovertFromApi`
- Creating levels with `Level.Create` and updating name/elevation through parameters
- Deleting levels with `Document.Delete`

## Prerequisites

- Any project document with at least zero levels (empty projects still open the form)

## User interaction

- `LevelsForm` is modal; users edit the grid and commit changes through dialog buttons
- Core methods `SetLevel`, `CreateLevel`, and `DeleteLevel` on `Command` are separable for automation

## MCP notes

- Proposed tools: `revit_list_levels` (read-only) and `revit_manage_levels` (create/update/delete)
- Parameters: optional filters; for writes, arrays of `{ level_id?, name, elevation }` plus delete ids
- Returns: level id, name, elevation in project units
- Refactor: bypass `LevelsForm` and expose elevation in numeric API units instead of `SetValueString`
- MCP descriptor: `docs/mcp/LevelsProperty/levelsproperty.json`

## See also

- MCP descriptor: `docs/mcp/LevelsProperty/levelsproperty.json`

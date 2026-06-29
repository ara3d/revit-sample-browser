# Command

| Field | Value |
|-------|-------|
| **Sample** | NewRoof |
| **Class** | `Command` |
| **Source** | `src/NewRoof/CS/Command.cs` |
| **SDK ReadMe** | `src/NewRoof/CS/ReadMe_NewRoof.rtf` |
| **MCP rating** | 4/5 |

Provides a dialog to create and edit footprint or extrusion roofs, including sketch-based footprint definition and roof type properties.

## What it demonstrates

- `RoofsManager` coordinating footprint and extrusion roof managers
- `RoofForm` / `RoofEditorForm` for roof type, level, slope, and sketch editing
- `LevelConverter` binding level choices for roof placement

## Prerequisites

- Project with levels and roof types; active view stored in `Command.ActiveView` for sketch context

## User interaction

- Modal roof wizard with retry loop for window selection (`RoofsManager.WindowSelect`)
- Managers encapsulate API calls but still depend on form input

## MCP notes

- Proposed tool: `revit_create_roof`
- Parameters: `roof_kind` (`footprint` | `extrusion`), `level_id`, `footprint_points[]`, `slope`, `roof_type_id`
- Returns: new roof element id
- Refactor: bypass `RoofForm` and pass sketch geometry explicitly
- MCP descriptor: `src/NewRoof/newroof.json`

## See also

- MCP descriptor: `src/NewRoof/newroof.json`

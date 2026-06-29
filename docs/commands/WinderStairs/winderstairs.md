# Command

| Field | Value |
|-------|-------|
| **Sample** | WinderStairs |
| **Class** | `Command` |
| **Source** | `src/WinderStairs/CS/Command.cs` |
| **SDK ReadMe** | `src/WinderStairs/CS/Readme_WinderStairs.rtf` |
| **MCP rating** | 4/5 |

Creates L-shaped or U-shaped winder stair runs from two or three connected model lines or walls.

## What it demonstrates

- `WinderUtil.CalculateControlPoints` and `CalculateMaxStepsCount` from selected curve elements
- `LWinder` / `UWinder` geometry with `WinderUpdater` implementing `IUpdater` for dynamic regeneration
- `StairsEditScope` dummy stairs to read default `StairsType` run width and tread depth
- Options dialogs (`LWinderOptions`, `UWinderOptions`) for step counts, run width, center offsets, and sketch display

## Prerequisites

- Pre-select exactly two or three connected straight walls or model lines
- At least two levels in the project

## User interaction

- Winder options modal dialog after selection; optional DMU updater registration when enabled
- Automation needs curve element ids and numeric winder parameters instead of dialogs

## MCP notes

- Proposed tool: `revit_create_winder_stairs`
- Parameters: `curve_element_ids` (2 or 3), `winder_type` (`L` or `U`), step counts, `run_width`, `tread_depth`, `center_offsets`, `enable_dmu`
- Returns: created `StairsRun` element id
- MCP descriptor: `docs/mcp/WinderStairs/winderstairs.json`

## See also

- MCP descriptor: `docs/mcp/WinderStairs/winderstairs.json`

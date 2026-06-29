# Command

| Field | Value |
|-------|-------|
| **Sample** | StructSample |
| **Class** | `Command` |
| **Source** | `src/StructSample/CS/Command.cs` |
| **SDK ReadMe** | `src/StructSample/CS/ReadMe_StructSample.rtf` |
| **MCP rating** | 4/5 |

Places wood timber columns along selected walls at a fixed five-foot spacing, constrained to each wall's base and top levels.

## What it demonstrates

- Filtering selection for level-constrained `Wall` elements
- `FindFamilySymbol` for "M_Wood Timber Column" / "191 x 292mm"
- `FrameWall` walking wall curves and `PlaceColumn` with `NewFamilyInstance` on a level
- Column rotation to align with wall direction and setting `FAMILY_BASE_LEVEL_PARAM` / `FAMILY_TOP_LEVEL_PARAM`

## Prerequisites

- Loaded "M_Wood Timber Column" family with "191 x 292mm" type
- Walls with top and base level constraints selected

## User interaction

- Uses current selection only; shows task dialogs with wall count and spacing diagnostics

## MCP notes

- Proposed tool: `revit_frame_walls_with_columns`
- Parameters: `wall_ids[]`, `column_type_id`, `spacing_feet`
- Returns: created column element ids
- MCP descriptor: `docs/mcp/StructSample/structsample.json`

## See also

- MCP descriptor: `docs/mcp/StructSample/structsample.json`

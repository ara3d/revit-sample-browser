# Command

| Field | Value |
|-------|-------|
| **Sample** | ShaftHolePuncher |
| **Class** | `Command` |
| **Source** | `src/ShaftHolePuncher/CS/Command.cs` |
| **SDK ReadMe** | `src/ShaftHolePuncher/CS/ReadMe_ShaftHolePuncher.rtf` |
| **MCP rating** | 4/5 |

Creates shaft openings or host-specific openings (wall, floor, or structural beam) from a 2D profile drawn in a preview dialog.

## What it demonstrates

- Branching on pre-selection: `Wall`, `Floor`, structural `FamilyInstance` beam, or no selection for shaft mode
- `ProfileWall`, `ProfileFloor`, `ProfileBeam`, and `ProfileNull` projecting host geometry to a picture-box canvas
- `Opening` creation from user-drawn line or rectangle profiles via `Profile.CreateOpening`
- Coordinate transforms between Revit model space and UI drawing space (`Matrix4`, `MathTools`)

## Prerequisites

- For host openings: exactly one wall, floor, or beam selected before run
- For shaft openings: no selection; sample expects Level 1 and Level 2 in the project

## User interaction

- `ShaftHolePuncherForm` requires mouse drawing on a profile preview; not headless without accepting curve loops as parameters

## MCP notes

- Proposed tool: `revit_create_opening_from_profile`
- Parameters: `host_element_id` (optional), `profile_points[]` or `curve_loop`, `opening_type` (`host` | `shaft`), optional `level_ids` for shaft
- Returns: new opening element id
- MCP descriptor: `docs/mcp/ShaftHolePuncher/shaftholepuncher.json`

## See also

- MCP descriptor: `docs/mcp/ShaftHolePuncher/shaftholepuncher.json`

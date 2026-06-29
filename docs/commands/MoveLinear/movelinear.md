# Command

| Field | Value |
|-------|-------|
| **Sample** | MoveLinear |
| **Class** | `Command` |
| **Source** | `src/MoveLinear/CS/Command.cs` |
| **SDK ReadMe** | `src/MoveLinear/CS/ReadMe_MoveLinear.rtf` |
| **MCP rating** | 4/5 |

Moves a single linear-hosted element by replacing its `LocationCurve` with a new line offset 100 feet in X at the start and 100 feet in Y at the end.

## What it demonstrates

- Reading `LocationCurve` from a selected element
- `Line.CreateBound` with computed endpoints and assigning `lineLoc.Curve`
- Selection validation and user messaging via `TaskDialog`

## Prerequisites

- Exactly one selected element whose `Location` is `LocationCurve` (walls, beams, model lines, etc.)

## User interaction

- Uses current selection; shows task dialogs for zero, multiple, or non-linear picks
- Offset values are hard-coded; headless use needs element id and delta parameters

## MCP notes

- Proposed tool: `revit_move_linear_element`
- Parameters: `element_id`, `start_offset` and `end_offset` as `{x,y,z}` or a single translation vector
- Returns: updated curve endpoints
- MCP descriptor: `docs/mcp/MoveLinear/movelinear.json`

## See also

- MCP descriptor: `docs/mcp/MoveLinear/movelinear.json`

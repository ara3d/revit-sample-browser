# Command

| Field | Value |
|-------|-------|
| **Sample** | FrameBuilder |
| **Class** | `Command` |
| **Source** | `src/FrameBuilder/CS/Command.cs` |
| **SDK ReadMe** | — |
| **MCP rating** | 4/5 |

Builds a structural frame (columns, beams, braces) from dimensions and family types entered in a dialog.

## What it demonstrates

- `FrameData.CreateInstance` collecting document context for framing
- `CreateFrameForm` user input and `FrameBuilder.CreateFraming` execution
- Cancel vs OK flow with `Result.Cancelled` when the user dismisses the form

## Prerequisites

- Project with appropriate structural framing families and levels

## User interaction

- Modal framing dialog required before any geometry is created

## MCP notes

- Proposed tool: `revit_create_structural_frame` with grid dimensions, level ids, and family type ids. Refactor `FrameBuilder` to accept parameters instead of form state.

## See also

- MCP descriptor: `docs/mcp/FrameBuilder/framebuilder.json`

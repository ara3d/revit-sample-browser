# Command

| Field | Value |
|-------|-------|
| **Sample** | SpanDirection |
| **Class** | `Command` |
| **Source** | `src/SpanDirection/Command.cs` |
| **MCP rating** | 4/5 |

Reports a structural floor's span direction angle and the names of its span-direction symbol types.

## What it demonstrates

- `Floor.SpanDirectionAngle` (radians; throws if floor is non-structural)
- `Floor.GetSpanDirectionSymbolIds()` and resolving symbol `ElementType` names
- Selection-based workflow with `TaskDialog` output

## Prerequisites

- One structural floor/slab selected

## User interaction

- Results shown in a dialog; no model changes

## MCP notes

- Proposed tool: `revit_get_span_direction`
- Parameters: `floor_id`
- Returns: span angle in radians and list of span symbol type names
- MCP descriptor: `src/SpanDirection/spandirection.json`

## See also

- MCP descriptor: `src/SpanDirection/spandirection.json`

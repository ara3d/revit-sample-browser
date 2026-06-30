# MoveAnalyticalMemberUsingSetCurve

| Field | Value |
|-------|-------|
| **Sample** | ContextualAnalyticalModel |
| **Class** | `MoveAnalyticalMemberUsingSetCurve` |
| **Source** | `src/ContextualAnalyticalModel/MoveAnalyticalMemberUsingSetCurve.cs` |
| **MCP rating** | 2/5 |

Repositions an analytical member by assigning a new curve with `SetCurve`, which breaks connectivity at shared end nodes.

## What it demonstrates

- `analyticalMember.GetCurve()` and endpoint extraction
- `Line.CreateBound` with X offset (+15 ft) applied to both endpoints
- `analyticalMember.SetCurve(line)` — contrast with `ElementTransformUtils` move that keeps nodes connected

## Prerequisites

- Structural document; creates two convergent members then moves the first

## User interaction

- None — offset value and geometry are hard-coded
- Expected outcome: connection to the second member at the shared node is lost

## MCP notes

- `SetCurve` by member ID is parameterizable, but the sample is a fixed-offset demo without selection or structured output.

## See also

- Related: [moveanalyticalmemberusingelementtransformutils.md](moveanalyticalmemberusingelementtransformutils.md)

# MoveAnalyticalMemberUsingElementTransformUtils

| Field | Value |
|-------|-------|
| **Sample** | ContextualAnalyticalModel |
| **Class** | `MoveAnalyticalMemberUsingElementTransformUtils` |
| **Source** | `src/ContextualAnalyticalModel/CS/MoveAnalyticalMemberUsingElementTransformUtils.cs` |
| **SDK ReadMe** | `src/ContextualAnalyticalModel/CS/ReadMe_ContextualAnalyticalModel.rtf` |
| **MCP rating** | 2/5 |

Moves an analytical member with `ElementTransformUtils.MoveElement` while preserving connectivity to a convergent second member at the shared node.

## What it demonstrates

- Setup: `CreateAnalyticalMember.CreateMember` plus `CreateConvergentMember` sharing endpoint `(0,0,0)`
- `ElementTransformUtils.MoveElement(document, memberId, new XYZ(15, 0, 0))`
- Connected end nodes remain linked after rigid translation (per expected results comment)

## Prerequisites

- Structural document; command builds its own two-member topology

## User interaction

- None — offset and member geometry are fixed
- Illustrates connectivity-preserving move vs. `SetCurve` alternative

## MCP notes

- Moving elements by ID and translation is automatable, but this command is a fixed demo without parameters or return values.

## See also

- Related: [moveanalyticalmemberusingsetcurve.md](moveanalyticalmemberusingsetcurve.md), [createanalyticalmember.md](createanalyticalmember.md)

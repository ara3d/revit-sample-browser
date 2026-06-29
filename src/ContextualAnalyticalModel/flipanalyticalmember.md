# FlipAnalyticalMember

| Field | Value |
|-------|-------|
| **Sample** | ContextualAnalyticalModel |
| **Class** | `FlipAnalyticalMember` |
| **Source** | `src/ContextualAnalyticalModel/CS/FlipAnalyticalMember.cs` |
| **SDK ReadMe** | `src/ContextualAnalyticalModel/CS/ReadMe_ContextualAnalyticalModel.rtf` |
| **MCP rating** | 2/5 |

Creates a sample analytical member, then reverses its curve direction with `FlipCurve()` so end-node IDs swap.

## What it demonstrates

- `AnalyticalMember.FlipCurve()` to reverse member orientation
- Reuse of `CreateAnalyticalMember.CreateMember` for setup geometry
- Observing end-node ID changes after flip (per SDK comments)

## Prerequisites

- Structural document; command creates its own test member

## User interaction

- None — creates hard-coded member then flips it
- Demonstration-only; no selection or output dialog

## MCP notes

- Narrow use case: flipping curve direction on an existing member is automatable (`member_id`), but this command only exercises a self-created demo element without returning results.

## See also

- Related: [createanalyticalmember.md](createanalyticalmember.md)

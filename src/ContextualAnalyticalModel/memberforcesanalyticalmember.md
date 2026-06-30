# MemberForcesAnalyticalMember

| Field | Value |
|-------|-------|
| **Sample** | ContextualAnalyticalModel |
| **Class** | `MemberForcesAnalyticalMember` |
| **Source** | `src/ContextualAnalyticalModel/MemberForcesAnalyticalMember.cs` |
| **MCP rating** | 2/5 |

Reads and writes member end forces on a newly created analytical member using `GetMemberForces` and `SetMemberForces`.

## What it demonstrates

- `analyticalMember.GetMemberForces()` iteration over `MemberForces` (start flag, force, moment)
- `SetMemberForces(bool atStart, XYZ force, XYZ moment)` overload
- `SetMemberForces(MemberForces)` with a constructed `MemberForces` object
- Console output only — no UI feedback

## Prerequisites

- Structural document; command creates its own analytical member

## User interaction

- None; forces are written to hard-coded XYZ values
- Results printed to console, not returned to the caller

## MCP notes

- API is automatable (get/set forces by member ID), but this sample is a demo with console logging and no structured return — better as reference than as a direct MCP tool.

## See also

- Related: [releaseconditionsanalyticalmember.md](releaseconditionsanalyticalmember.md), [createanalyticalmember.md](createanalyticalmember.md)

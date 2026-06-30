# ReleaseConditionsAnalyticalMember

| Field | Value |
|-------|-------|
| **Sample** | ContextualAnalyticalModel |
| **Class** | `ReleaseConditionsAnalyticalMember` |
| **Source** | `src/ContextualAnalyticalModel/ReleaseConditionsAnalyticalMember.cs` |
| **MCP rating** | 2/5 |

Reads and updates end release conditions and release types on a sample analytical member.

## What it demonstrates

- `GetReleaseConditions()` and per-end `GetReleaseType(bool atStart)`
- `SetReleaseType(true, ReleaseType.UserDefined)` before custom releases
- `SetReleaseConditions(ReleaseConditions)` with Fx/Fy/Fz/Mx/My/Mz flags
- `InvalidOperationException` handling when release settings are incompatible

## Prerequisites

- Structural document; creates its own analytical member

## User interaction

- None; release flags are hard-coded
- Values logged to console only

## MCP notes

- Release get/set by member ID is useful structurally, but this command is a console demo without returning data to an agent.

## See also

- Related: [memberforcesanalyticalmember.md](memberforcesanalyticalmember.md)

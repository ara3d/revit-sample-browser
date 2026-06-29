# Command

| Field | Value |
|-------|-------|
| **Sample** | Massing/DividedSurfaceByIntersects |
| **Class** | `Command` |
| **Source** | `src/Massing/DividedSurfaceByIntersects/Command.cs` |
| **SDK ReadMe** | `src/Massing/DividedSurfaceByIntersects/ReadMe_DividedSurfaceByIntersects.rtf` |
| **MCP rating** | 2/5 |

Demonstrates adding and removing intersection elements on a `DividedSurface`, first with reference planes and levels, then with model lines, in a bundled mass family.

## What it demonstrates

- `DividedSurface.AddIntersectionElement`, `RemoveIntersectionElement`, and `CanBeIntersectionElement`
- `GetAllIntersectionElements` to clear prior intersections
- Hard-coded `ElementId` lookups for a sample family file

## Prerequisites

- Open the sample mass family from the DividedSurfaceByIntersects folder (divided surface id `31519` and preset plane/line ids)

## User interaction

- No UI; command fails with a message if the expected divided surface is missing
- Not portable to arbitrary projects without replacing hard-coded element ids

## MCP notes

- Educational sample only; automation would need dynamic element resolution instead of fixed ids.

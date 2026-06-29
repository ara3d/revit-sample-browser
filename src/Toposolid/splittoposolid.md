# SplitToposolid

| Field | Value |
|-------|-------|
| **Sample** | Toposolid |
| **Class** | `SplitToposolid` |
| **Source** | `src/Toposolid/Command.cs` |
| **SDK ReadMe** | `src/Toposolid/ReadMe_Toposolid.rtf` |
| **MCP rating** | 2/5 |

Splits a picked toposolid along model curves selected after the toposolid.

## What it demonstrates

- `Toposolid.Split` with a `CurveLoop` built from picked `ModelCurve` geometry
- `ToposolidFilter` and `ModelCurveFilter` selection filters
- Multi-step interactive selection (one toposolid, then one or more model curves)

## Prerequisites

- A toposolid and model curves that form a valid split boundary

## User interaction

- Two selection steps in the Revit UI; not headless without refactoring to accept element ids

## MCP notes

- Split geometry is highly interactive; poor standalone MCP candidate without scripted curve input.

## See also

- [SimplifyToposolid](simplifytoposolid.md)

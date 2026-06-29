# SineCurve

| Field | Value |
|-------|-------|
| **Sample** | Massing/PointCurveCreation |
| **Class** | `SineCurve` |
| **Source** | `src/Massing/PointCurveCreation/CS/Command.cs` |
| **SDK ReadMe** | `src/Massing/PointCurveCreation/CS/ReadMe_PointCurveCreation.rtf` |
| **MCP rating** | 2/5 |

Samples 500 points along `z = cos(x) * 10` and builds a `CurveByPoints` through the reference points.

## What it demonstrates

- Incremental parametric point placement (`xctr += 0.1`)
- `ReferencePointArray` collection and `FamilyCreate.NewCurveByPoints`
- Note: class name says Sine but the equation uses cosine

## Prerequisites

- Conceptual mass family document

## User interaction

- No UI

## MCP notes

- Illustrates curve-by-points workflow; not a practical MCP tool as written.

# PointsOnCurve

| Field | Value |
|-------|-------|
| **Sample** | Massing/PointCurveCreation |
| **Class** | `PointsOnCurve` |
| **Source** | `src/Massing/PointCurveCreation/Command.cs` |
| **MCP rating** | 2/5 |

Creates a diagonal model line, then places reference points along it at normalized parameters from 0.1 to 1.0 using `PointOnEdge`.

## What it demonstrates

- `Application.Create.NewPointOnEdge` with `PointLocationOnCurve` and normalized curve parameter
- `FamilyCreate.NewReferencePoint(PointOnEdge)` for hosted points
- Sketch-plane model line creation in a mass family

## Prerequisites

- Conceptual mass family document

## User interaction

- No UI; line endpoints and parameter step are fixed

## MCP notes

- Shows point-on-edge hosting; low priority for MCP unless curve id and parameter list are supplied externally.

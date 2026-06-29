# PointsParabola

| Field | Value |
|-------|-------|
| **Sample** | Massing/PointCurveCreation |
| **Class** | `PointsParabola` |
| **Source** | `src/Massing/PointCurveCreation/CS/Command.cs` |
| **SDK ReadMe** | `src/Massing/PointCurveCreation/CS/ReadMe_PointCurveCreation.rtf` |
| **MCP rating** | 2/5 |

Creates mirrored reference-point grids following power-law curves `z = x^power` for exponents between 1.2 and 1.4.

## What it demonstrates

- Nested loops generating `XYZ` from `Math.Pow` and offsetting rows along Y
- Mirroring positive X columns to negative X
- `FamilyCreate.NewReferencePoint` batch creation

## Prerequisites

- Conceptual mass family document

## User interaction

- No UI

## MCP notes

- Parametric point cloud demo only.

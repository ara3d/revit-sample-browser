# CyclicSurface

| Field | Value |
|-------|-------|
| **Sample** | Massing/PointCurveCreation |
| **Class** | `CyclicSurface` |
| **Source** | `src/Massing/PointCurveCreation/CS/Command.cs` |
| **SDK ReadMe** | `src/Massing/PointCurveCreation/CS/ReadMe_PointCurveCreation.rtf` |
| **MCP rating** | 2/5 |

Builds a lofted mass form from curves sampled on a `z = cos(x) + cos(y)` surface grid.

## What it demonstrates

- Nested loops creating `ReferencePoint` grids from a trigonometric height field
- `NewCurveByPoints` per U-direction row, collecting curve references into `ReferenceArrayArray`
- `FamilyCreate.NewLoftForm` across the generated profile curves

## Prerequisites

- Conceptual mass family document

## User interaction

- No UI; grid spacing and domain are hard-coded (800×800 at 40-unit steps)

## MCP notes

- Procedural surface tutorial; unsuitable for MCP without external surface definition inputs.

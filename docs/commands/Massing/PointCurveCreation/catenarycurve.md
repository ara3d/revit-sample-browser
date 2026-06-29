# CatenaryCurve

| Field | Value |
|-------|-------|
| **Sample** | Massing/PointCurveCreation |
| **Class** | `CatenaryCurve` |
| **Source** | `src/Massing/PointCurveCreation/CS/Command.cs` |
| **SDK ReadMe** | `src/Massing/PointCurveCreation/CS/ReadMe_PointCurveCreation.rtf` |
| **MCP rating** | 2/5 |

Places reference points along hyperbolic-cosine (catenary) curves for several scaling factors and connects each set with `NewCurveByPoints`.

## What it demonstrates

- Parametric point generation with `y = scalingFactor * Cosh(x / scalingFactor)`
- `FamilyCreate.NewReferencePoint` and `FamilyCreate.NewCurveByPoints`
- Multiple curve loops at different scaling factors

## Prerequisites

- Conceptual mass family document

## User interaction

- No UI; equation bounds and step size are fixed in code

## MCP notes

- Math demo for mass point/curve creation; not an automation-oriented command.

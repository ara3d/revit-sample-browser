# MakeRevolveForm

| Field | Value |
|-------|-------|
| **Sample** | Massing/NewForm |
| **Class** | `MakeRevolveForm` |
| **Source** | `src/Massing/NewForm/Command.cs` |
| **MCP rating** | 2/5 |

Revolves a triangular profile about a reference-line axis through π/4 radians to create a mass form.

## What it demonstrates

- `FamilyCreate.NewRevolveForms` with profile `ReferenceArray`, axis reference, start angle, and end angle
- `ModelCurve.ChangeToReferenceLine` to define the revolve axis
- Sketch-plane model lines on a Z-normal plane

## Prerequisites

- Conceptual mass family document

## User interaction

- No UI; fixed geometry and sweep angle

## MCP notes

- Revolve-form tutorial only; low automation value without parameterized profiles and axes.

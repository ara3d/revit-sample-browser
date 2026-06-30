# MakeExtrusionForm

| Field | Value |
|-------|-------|
| **Sample** | Massing/NewForm |
| **Class** | `MakeExtrusionForm` |
| **Source** | `src/Massing/NewForm/Command.cs` |
| **MCP rating** | 2/5 |

Creates an extrusion form from a triangular model-curve profile extruded along a fixed Z vector.

## What it demonstrates

- `FamilyCreate.NewExtrusionForm(true, refAr, direction)` with a `ReferenceArray` of model curve references
- `FormUtils.MakeLine` helper for sketch-plane model curves in mass families

## Prerequisites

- Conceptual mass family document

## User interaction

- No UI; hard-coded profile points and extrusion direction `(0, 0, 50)`

## MCP notes

- Educational massing sample; profile and direction would need parameters for automation use.

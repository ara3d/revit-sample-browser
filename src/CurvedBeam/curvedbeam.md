# Command

| Field | Value |
|-------|-------|
| **Sample** | CurvedBeam |
| **Class** | `Command` |
| **Source** | `src/CurvedBeam/CS/CurvedBeam.cs` |
| **SDK ReadMe** | `src/CurvedBeam/CS/ReadMe_CurvedBeam.rtf` |
| **MCP rating** | 4/5 |

Creates structural framing beams along hard-coded arc, partial ellipse, or NURBS spline curves at a user-chosen level and family type.

## What it demonstrates

- Collecting `Level` and structural framing `FamilySymbol` types
- `Arc.Create`, `Ellipse.CreateCurve`, and `NurbSpline.CreateCurve` for horizontal curves at a given Z
- `Document.Create.NewFamilyInstance(curve, symbol, level, StructuralType.Beam)`
- `CurvedBeamForm` for type and level selection

## Prerequisites

- Loaded structural framing families and at least one level

## User interaction

- Dialog picks beam type, level, and curve kind; curve geometry is fixed in code

## MCP notes

- Proposed tool: `revit_create_curved_beam`
- Parameters: `beam_type_id`, `level_id`, `curve_type` (arc/ellipse/nurbs), optional curve control data
- Returns: beam element id
- MCP descriptor: `src/CurvedBeam/curvedbeam.json`

## See also

- MCP descriptor: `src/CurvedBeam/curvedbeam.json`

# SetLengthAngleParams

| Field | Value |
|-------|-------|
| **Sample** | Massing/PanelEdgeLengthAngle |
| **Class** | `SetLengthAngleParams` |
| **Source** | `src/Massing/PanelEdgeLengthAngle/Command.cs` |
| **SDK ReadMe** | `src/Massing/PanelEdgeLengthAngle/ReadMe_PanelEdgeLengthAngle.rtf` |
| **MCP rating** | 2/5 |

Measures edge lengths and interior angles of divided-surface panel solids and pushes values into `Length1`–`Length4` and `Angle1`–`Angle4` instance parameters.

## What it demonstrates

- Enumerating panel `FamilyInstance` tiles from `DividedSurface`
- Extracting `EdgeArray` from panel solid geometry (direct solid or `GeometryInstance`)
- `Edge.ApproximateLength` and vector-based `AngleBetweenEdges` using `ComputeDerivatives`
- `LookupParameter` validation for required driving parameters

## Prerequisites

- Mass family with divided surfaces and panel types containing Length/Angle instance parameters

## User interaction

- No dialog; shows `TaskDialog` if required parameters are missing on a panel family

## MCP notes

- Domain-specific panel reporting; automation would need configurable parameter names and divided-surface scope.

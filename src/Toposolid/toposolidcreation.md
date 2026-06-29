# ToposolidCreation

| Field | Value |
|-------|-------|
| **Sample** | Toposolid |
| **Class** | `ToposolidCreation` |
| **Source** | `src/Toposolid/Command.cs` |
| **SDK ReadMe** | `src/Toposolid/ReadMe_Toposolid.rtf` |
| **MCP rating** | 2/5 |

Creates a toposolid from a rectangular profile, interior shape points, and an offset sub-division.

## What it demonstrates

- `Toposolid.Create` overload combining `CurveLoop` profile and point list
- `Toposolid.CreateSubDivision` with `CurveLoop.CreateViaOffset`
- Resolving the first `ToposolidType` and `Level` via `FilteredElementCollector`

## Prerequisites

- At least one `ToposolidType` and one `Level` in the project

## User interaction

- Headless with hard-coded geometry (100×100 ft footprint, two interior Z points)
- Commented code shows alternate create overloads (profile-only or points-only)

## MCP notes

- Geometry is sample-specific; real automation would need profile points and type/level ids as parameters.

## See also

- [ToposolidFromDwg](toposolidfromdwg.md)
- [ToposolidFromSurface](toposolidfromsurface.md)

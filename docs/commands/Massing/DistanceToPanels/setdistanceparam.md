# SetDistanceParam

| Field | Value |
|-------|-------|
| **Sample** | Massing/DistanceToPanels |
| **Class** | `SetDistanceParam` |
| **Source** | `src/Massing/DistanceToPanels/CS/Command.cs` |
| **SDK ReadMe** | `src/Massing/DistanceToPanels/CS/ReadMe_DistanceToPanels.rtf` |
| **MCP rating** | 2/5 |

Computes the 3D distance from a selected family instance to every divided-surface panel tile and writes the value into each panel's `Distance` instance parameter.

## What it demonstrates

- Iterating `DividedSurface` seed nodes with `GridNode`, `IsSeedNode`, and `GetTileFamilyInstance`
- Reading a host point from `LocationPoint` on a selected `FamilyInstance`
- Setting instance parameters on panel families via `LookupParameter("Distance")`

## Prerequisites

- Conceptual mass or adaptive family document with `DividedSurface` elements
- Panel families must include a `Distance` instance parameter
- Exactly one family instance selected as the measurement origin

## User interaction

- Requires a pre-selected reference point element; no dialog
- Headless execution is feasible if selection is replaced with an element id parameter

## MCP notes

- Niche massing workflow tied to divided surfaces and custom panel parameters; poor general automation fit without document-specific setup.

## See also

- Related: `Massing/MeasurePanelArea/measurepanelarea.md`

# FindSouthFacingWallsWithProjectLocation

| Field | Value |
|-------|-------|
| **Sample** | DirectionCalculation |
| **Class** | `FindSouthFacingWallsWithProjectLocation` |
| **Source** | `src/DirectionCalculation/Commands.cs` |
| **MCP rating** | 5/5 |

Finds exterior walls facing south after transforming wall normals by the active project location's north direction, then selects those walls.

## What it demonstrates

- `FindSouthFacingWalls.Execute(true)` with `TransformByProjectLocation`
- Same exterior wall collection and south-facing test as the without-location variant
- Read-only geometric query wrapped in a transaction for journaling consistency

## Prerequisites

- Project with exterior walls and a defined project location

## User interaction

- Replaces selection with matching walls; no dialog

## MCP notes

- Proposed tool: `revit_find_south_facing_walls` with `use_project_location: true`
- Returns: wall element ids facing south per project north
- MCP descriptor: `src/DirectionCalculation/findsouthfacingwallswithprojectlocation.json`

## See also

- MCP descriptor: `src/DirectionCalculation/findsouthfacingwallswithprojectlocation.json`
- Related: [findsouthfacingwallswithoutprojectlocation.md](findsouthfacingwallswithoutprojectlocation.md)

# FindSouthFacingWallsWithoutProjectLocation

| Field | Value |
|-------|-------|
| **Sample** | DirectionCalculation |
| **Class** | `FindSouthFacingWallsWithoutProjectLocation` |
| **Source** | `src/DirectionCalculation/CS/Commands.cs` |
| **SDK ReadMe** | `src/DirectionCalculation/CS/ReadMe_DirectionCalculation.rtf` |
| **MCP rating** | 5/5 |

Finds exterior walls whose outward normal faces south using the model Y-axis as north/south, then updates the selection to those walls.

## What it demonstrates

- `FindSouthFacingWalls.Execute(false)` — south = negative Y in project coordinates
- `CollectExteriorWalls` via `FilteredElementCollector` and exterior wall type function
- `GetExteriorWallDirection` and `IsSouthFacing` from `FindSouthFacingBase`
- `UIDocument.Selection.SetElementIds` to highlight results; optional log via `CloseFile`

## Prerequisites

- Project with exterior walls

## User interaction

- No dialog; replaces current selection with south-facing exterior walls

## MCP notes

- Proposed tool: `revit_find_south_facing_walls`
- Parameters: `use_project_location` (false for this command)
- Returns: matching wall element ids
- MCP descriptor: `src/DirectionCalculation/findsouthfacingwallswithoutprojectlocation.json`

## See also

- MCP descriptor: `src/DirectionCalculation/findsouthfacingwallswithoutprojectlocation.json`
- Related: [findsouthfacingwallswithprojectlocation.md](findsouthfacingwallswithprojectlocation.md)

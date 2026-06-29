# FindSouthFacingWindowsWithoutProjectLocation

| Field | Value |
|-------|-------|
| **Sample** | DirectionCalculation |
| **Class** | `FindSouthFacingWindowsWithoutProjectLocation` |
| **Source** | `src/DirectionCalculation/CS/Commands.cs` |
| **SDK ReadMe** | `src/DirectionCalculation/CS/ReadMe_DirectionCalculation.rtf` |
| **MCP rating** | 5/5 |

Finds windows whose facing direction is south using model Y-axis as north/south, then selects those instances.

## What it demonstrates

- `FindSouthFacingWindows.Execute(false)`
- `CollectWindows` and `GetWindowDirection` from family instance orientation
- `IsSouthFacing` angle test against south vector
- Selection update via `SetElementIds`

## Prerequisites

- Project containing window family instances

## User interaction

- No dialog; updates selection to south-facing windows

## MCP notes

- Proposed tool: `revit_find_south_facing_windows`
- Parameters: `use_project_location` (false)
- Returns: window element ids
- MCP descriptor: `docs/mcp/DirectionCalculation/findsouthfacingwindowswithoutprojectlocation.json`

## See also

- MCP descriptor: `docs/mcp/DirectionCalculation/findsouthfacingwindowswithoutprojectlocation.json`
- Related: [findsouthfacingwindowswithprojectlocation.md](findsouthfacingwindowswithprojectlocation.md)

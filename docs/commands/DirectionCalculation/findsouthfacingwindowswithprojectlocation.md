# FindSouthFacingWindowsWithProjectLocation

| Field | Value |
|-------|-------|
| **Sample** | DirectionCalculation |
| **Class** | `FindSouthFacingWindowsWithProjectLocation` |
| **Source** | `src/DirectionCalculation/CS/Commands.cs` |
| **SDK ReadMe** | `src/DirectionCalculation/CS/ReadMe_DirectionCalculation.rtf` |
| **MCP rating** | 5/5 |

Finds windows facing south relative to the active project location north, then selects those instances.

## What it demonstrates

- `FindSouthFacingWindows.Execute(true)` with project-location transform
- Window direction from instance transform / facing parameter logic in `FindSouthFacingWindows`
- Bulk selection of matching `FamilyInstance` windows

## Prerequisites

- Windows in the model and a configured project location

## User interaction

- Selection-only output; no user prompts

## MCP notes

- Proposed tool: `revit_find_south_facing_windows` with `use_project_location: true`
- Returns: south-facing window element ids
- MCP descriptor: `docs/mcp/DirectionCalculation/findsouthfacingwindowswithprojectlocation.json`

## See also

- MCP descriptor: `docs/mcp/DirectionCalculation/findsouthfacingwindowswithprojectlocation.json`
- Related: [findsouthfacingwindowswithoutprojectlocation.md](findsouthfacingwindowswithoutprojectlocation.md)

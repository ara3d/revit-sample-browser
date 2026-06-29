# SiteRaiseTerrainInRegionCommand

| Field | Value |
|-------|-------|
| **Sample** | Site |
| **Class** | `SiteRaiseTerrainInRegionCommand` |
| **Source** | `src/Site/SiteRaiseTerrainInRegionCommand.cs` |
| **SDK ReadMe** | `src/Site/Readme_Site.rtf` |
| **MCP rating** | 2/5 |

Raises all non-boundary topography points inside a picked subregion by three feet.

## What it demonstrates

- `SiteUiUtils.ChangeSubregionAndPointsElevation` with a positive delta
- `TopographySurface.MovePoints` within `TopographyEditScope`

## Prerequisites

- Topography subregion with editable interior points

## User interaction

- Subregion pick required; elevation delta is hardcoded to +3 feet

## MCP notes

Same pattern as lower terrain; fixed offset and interactive pick limit MCP usefulness without refactoring.

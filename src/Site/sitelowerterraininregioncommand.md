# SiteLowerTerrainInRegionCommand

| Field | Value |
|-------|-------|
| **Sample** | Site |
| **Class** | `SiteLowerTerrainInRegionCommand` |
| **Source** | `src/Site/SiteLowerTerrainInRegionCommand.cs` |
| **MCP rating** | 2/5 |

Lowers all non-boundary topography points inside a picked subregion by three feet.

## What it demonstrates

- `SiteUiUtils.ChangeSubregionAndPointsElevation` with a negative delta
- `TopographySurface.MovePoints` inside `TopographyEditScope`
- Shared site editing helpers in `SiteEditingUtils` and `SiteUiUtils`

## Prerequisites

- Topography subregion with editable interior points

## User interaction

- Subregion pick required; elevation delta is hardcoded to −3 feet

## MCP notes

Parameterized elevation change by subregion id would be automatable, but the sample is pick-driven with a fixed offset.

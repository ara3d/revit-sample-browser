# SiteNormalizeTerrainInRegionCommand

| Field | Value |
|-------|-------|
| **Sample** | Site |
| **Class** | `SiteNormalizeTerrainInRegionCommand` |
| **Source** | `src/Site/CS/SiteNormalizeTerrainInRegionCommand.cs` |
| **SDK ReadMe** | `src/Site/CS/Readme_Site.rtf` |
| **MCP rating** | 2/5 |

Flattens all points in a picked subregion to the average elevation of the entire host topography surface.

## What it demonstrates

- `TopographySurface.ChangePointsElevation` on points from `GetPointsFromSubregionExact`
- Average elevation via `SiteEditingUtils.GetAverageElevation`
- `TopographyEditScope` with failure preprocessor

## Prerequisites

- Topography subregion with points to normalize

## User interaction

- User picks the subregion interactively

## MCP notes

Elevation normalization is automatable with subregion id, but the sample requires a user pick.

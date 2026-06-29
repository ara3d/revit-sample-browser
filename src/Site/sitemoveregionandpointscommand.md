# SiteMoveRegionAndPointsCommand

| Field | Value |
|-------|-------|
| **Sample** | Site |
| **Class** | `SiteMoveRegionAndPointsCommand` |
| **Source** | `src/Site/CS/SiteMoveRegionAndPointsCommand.cs` |
| **SDK ReadMe** | `src/Site/CS/Readme_Site.rtf` |
| **MCP rating** | 2/5 |

Moves a topography subregion and its interior points to a new location on the host surface.

## What it demonstrates

- `ElementTransformUtils.MoveElement` on the subregion, then `TopographySurface.MovePoints` for interior data
- Elevation adjustment via `SiteEditingUtils.MoveXyzToElevation` after the planar move
- Deleting conflicting points at the destination before relocating source points
- `TransactionGroup` wrapping subregion and topography edits

## Prerequisites

- Existing subregion on a topography surface

## User interaction

- Picks subregion and target point on the surface; not headless without element ids and XYZ

## MCP notes

Useful pattern for site editing APIs, but interactive picks limit direct MCP use without refactoring.

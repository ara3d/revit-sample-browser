# SiteDeleteRegionAndPointsCommand

| Field | Value |
|-------|-------|
| **Sample** | Site |
| **Class** | `SiteDeleteRegionAndPointsCommand` |
| **Source** | `src/Site/SiteDeleteRegionAndPointsCommand.cs` |
| **MCP rating** | 4/5 |

Deletes a topography subregion and removes all non-boundary topography points contained within it.

## What it demonstrates

- Picking `SiteSubRegion` and resolving host surface via `SiteEditingUtils.GetTopographySurfaceHost`
- Separating boundary vs. interior points with `GetNonBoundaryPoints`
- `TopographyEditScope` for point deletion followed by `Document.Delete` on the subregion
- `TransactionGroup` for a single undo operation

## Prerequisites

- Topography surface with at least one subregion

## User interaction

- User must pick the subregion to delete (`SiteUiUtils.PickSubregion`)

## MCP notes

- Proposed tool: `revit_delete_topography_subregion`
- Parameters: `subregion_id`
- Returns: deleted subregion id and points removed count
- MCP descriptor: `src/Site/sitedeleteregionandpointscommand.json`

## See also

- MCP descriptor: `src/Site/sitedeleteregionandpointscommand.json`

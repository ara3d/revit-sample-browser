# SiteAddRetainingPondCommand

| Field | Value |
|-------|-------|
| **Sample** | Site |
| **Class** | `SiteAddRetainingPondCommand` |
| **Source** | `src/Site/SiteAddRetainingPondCommand.cs` |
| **SDK ReadMe** | `src/Site/Readme_Site.rtf` |
| **MCP rating** | 4/5 |

Adds a circular retaining pond subregion on a topography surface, clears interior points, and inserts graded pond topography points.

## What it demonstrates

- `SiteSubRegion.Create` with circular `CurveLoop` and optional Water material assignment
- `TopographyEditScope` with `TopographyEditFailuresPreprocessor` for point edits
- `TopographySurface.AddPoints`, `DeletePoints`, and `SiteEditingUtils.GeneratePondPointsSurrounding`
- `TransactionGroup` combining subregion and topography edits into one undo step

## Prerequisites

- Project with at least one `TopographySurface` (user picks surface when multiple exist)
- Optional "Water" material in the document

## User interaction

- Picks center point on the surface (`SiteUiUtils.PickPointNearToposurface`); pond radius is hardcoded to 32 feet

## MCP notes

- Proposed tool: `revit_add_retaining_pond`
- Parameters: `topography_surface_id`, `center_xyz`, `radius`, optional `material_name`
- Returns: subregion id and point count added
- MCP descriptor: `src/Site/siteaddretainingpondcommand.json`

## See also

- MCP descriptor: `src/Site/siteaddretainingpondcommand.json`

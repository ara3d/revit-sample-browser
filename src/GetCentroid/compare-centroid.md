# CompareCentroidCommand

| Field | Value |
|-------|-------|
| **Sample** | GetCentroid |
| **Class** | `CompareCentroidCommand` |
| **Source** | `src/GetCentroid/CompareCentroidCommand.cs` |
| **Origin** | [jeremytammik/GetCentroid](https://github.com/jeremytammik/GetCentroid) (MIT) |
| **MCP rating** | 4/5 |

Compares tessellation-based centroid and volume with Revit's native `Solid.ComputeCentroid` and `Solid.Volume` for each tessellatable solid in the selection.

## What it demonstrates

- Validation of the tessellation algorithm against Revit API results
- Per-solid centroid distance and volume delta reporting
- The same geometry traversal used by `Command`

## Prerequisites

- Same as `Command`

## User interaction

- Uses pre-selection when elements are already selected; otherwise prompts for picks
- Read-only command; shows centroid distance and volume difference per solid

## MCP notes

- Proposed tool: `revit_compare_element_centroid`
- Optional argument: `element_ids`
- Returns tessellated vs native centroid/volume and deltas per solid
- MCP descriptor: `src/GetCentroid/compare-centroid.json`

## See also

- MCP descriptor: `src/GetCentroid/compare-centroid.json`
- [Building Coder: Solid Centroid and Volume Calculation](https://thebuildingcoder.typepad.com/blog/2012/12/solid-centroid-and-volume-calculation.html)
- Related: `Command` (centroid report only)

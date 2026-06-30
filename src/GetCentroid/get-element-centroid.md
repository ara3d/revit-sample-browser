# Command

| Field | Value |
|-------|-------|
| **Sample** | GetCentroid |
| **Class** | `Command` |
| **Source** | `src/GetCentroid/Command.cs` |
| **Origin** | [jeremytammik/GetCentroid](https://github.com/jeremytammik/GetCentroid) (MIT) |
| **MCP rating** | 4/5 |

Calculates tessellation-based centroid and volume for selected or picked elements and reports the results in a summary dialog.

## What it demonstrates

- `SolidUtils.TessellateSolidOrShell` with `SolidOrShellTessellationControls.LevelOfDetail = 0`
- Volume and centroid accumulation from triangulated shell components
- Volume-weighted combination of multiple solids per element
- Family instance geometry with symbol-geometry fallback

## Prerequisites

- Elements with non-empty solids that pass `SolidUtils.IsValidForTessellation`
- A view that supports element picking when nothing is pre-selected

## User interaction

- Uses pre-selection when elements are already selected; otherwise prompts for picks
- Read-only command; shows a `TaskDialog` with centroid and volume per element

## MCP notes

- Proposed tool: `revit_get_element_centroid`
- Optional argument: `element_ids`
- Returns centroid XYZ and volume per element id
- MCP descriptor: `src/GetCentroid/get-element-centroid.json`

## See also

- MCP descriptor: `src/GetCentroid/get-element-centroid.json`
- [Building Coder: Solid Centroid and Volume Calculation](https://thebuildingcoder.typepad.com/blog/2012/12/solid-centroid-and-volume-calculation.html)
- Related: `CompareCentroidCommand` (tessellation vs native API), `CustomExporter/AdnMeshJsonExporter` (mesh export with centroid)

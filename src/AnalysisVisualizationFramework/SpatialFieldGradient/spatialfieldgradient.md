# SpatialFieldGradient

| Field | Value |
|-------|-------|
| **Sample** | AnalysisVisualizationFramework / SpatialFieldGradient |
| **Class** | `SpatialFieldGradient` |
| **Source** | `src/AnalysisVisualizationFramework/SpatialFieldGradient/Command.cs` |
| **SDK ReadMe** | `src/AnalysisVisualizationFramework/SpatialFieldGradient/ReadMe_SpatialFieldGradient.rtf` |
| **MCP rating** | 2/5 |

Paints a color gradient spatial field on picked faces using the Analysis Visualization Framework.

## What it demonstrates

- `SpatialFieldManager.GetSpatialFieldManager` / `CreateSpatialFieldManager` on the active view
- Face selection via `PickObjects(ObjectType.Face)`
- Building `ValueAtPoint` samples on a UV grid and registering a spatial field primitive

## Prerequisites

- Active view; model faces available for selection

## User interaction

- Multi-face pick loop; values incorporate `DateTime.Now.Second` as a demo gradient driver
- Visualization-only; no durable model change beyond the spatial field overlay

## MCP notes

- Low MCP value: requires face picks and produces transient view graphics, not queryable document data

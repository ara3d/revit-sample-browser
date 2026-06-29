# MultithreadedCalculation

| Field | Value |
|-------|-------|
| **Sample** | AnalysisVisualizationFramework / MultithreadedCalculation |
| **Class** | `MultithreadedCalculation` |
| **Source** | `src/AnalysisVisualizationFramework/MultithreadedCalculation/CS/MultithreadedCalculation.cs` |
| **SDK ReadMe** | `src/AnalysisVisualizationFramework/MultithreadedCalculation/CS/ReadMe_MultithreadedCalculation.rtf` |
| **MCP rating** | 2/5 |

Demonstrates multithreaded AVF (Analysis Visualization Framework) field updates on a user-picked element using `SpatialFieldManager` and an `IUpdater`.

Picks one element, sets up `SpatialFieldManager` on the active view, runs background calculation threads, and pushes results through a dynamic model updater. Tied to view-specific spatial field primitives and idling/update events.

## Prerequisites

- Active 3D or analysis-capable view; element with geometry suitable for AVF sampling

## User interaction

- Requires `PickObject` selection; uses static state across command invocations
- Not suitable for headless MCP without redesigning updater lifecycle and removing picks

## MCP notes

- Poor MCP candidate: interactive pick, view-bound visualization, and threaded updater coupling
- Useful as an API learning sample for `SpatialFieldManager` and AVF multithreading patterns

# Command

| Field | Value |
|-------|-------|
| **Sample** | GeometryAPI/GeometryCreation_BooleanOperation |
| **Class** | `Command` |
| **Source** | `src/GeometryAPI/GeometryCreation_BooleanOperation/Command.cs` |
| **MCP rating** | 2/5 |

Demonstrates constructive solid geometry by boolean-combining primitive solids and visualizing the result with the Analysis Visualization Framework.

## What it demonstrates

- `GeometryCreation` factory methods for box, sphere, and axis-aligned cylinders
- CSG tree combining solids with union, intersection, and difference operations
- `AnalysisVisualizationFramework` displaying results in a dedicated `CSGTree` view

## Prerequisites

- Document; sample switches the active view to `CSGTree` after creation

## User interaction

- No dialog; fixed primitive sizes and boolean tree

## MCP notes

- Boolean solid tutorials are useful for learning but not parameterized for agent-driven modeling

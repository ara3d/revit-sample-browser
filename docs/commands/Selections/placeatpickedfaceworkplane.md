# PlaceAtPickedFaceWorkplane

| Field | Value |
|-------|-------|
| **Sample** | Selections |
| **Class** | `PlaceAtPickedFaceWorkplane` |
| **Source** | `src/Selections/CS/Command.cs` |
| **MCP rating** | 2/5 |

Sets the active view work plane to a picked planar face, then creates a model circle at a picked point on that plane.

## What it demonstrates

- `PlanarFaceFilter` limiting face picks to planar geometry
- `SketchPlane.Create` from face normal and origin; assigning `ActiveView.SketchPlane`
- `PickPoint` with snap types and `NewModelCurve` with an arc circle
- Family versus project `ItemFactoryBase` selection

## Prerequisites

- Planar face available for pick in the active view

## User interaction

- Two picks: planar face, then point on the work plane

## MCP notes

Selection and work-plane setup are interactive teaching examples; automation would pass face reference and center point directly.

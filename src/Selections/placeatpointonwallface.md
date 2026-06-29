# PlaceAtPointOnWallFace

| Field | Value |
|-------|-------|
| **Sample** | Selections |
| **Class** | `PlaceAtPointOnWallFace` |
| **Source** | `src/Selections/CS/Command.cs` |
| **MCP rating** | 2/5 |

Picks a point on a wall face and places a fixed 36" x 48" window family instance at that location.

## What it demonstrates

- `PickObject` with `ObjectType.PointOnElement` and `WallFaceFilter`
- `Document.Create.NewFamilyInstance` at `Reference.GlobalPoint` hosted on the wall element
- `FilteredElementCollector` lookup of `FamilySymbol` by exact name

## Prerequisites

- Wall geometry in the model
- Window type named **36" x 48"** loaded in the project

## User interaction

- Single point-on-wall pick; cancel returns `Result.Cancelled`

## MCP notes

Hard-coded family symbol name and interactive pick limit MCP usefulness; parameterized host, symbol id, and UV point would be needed.

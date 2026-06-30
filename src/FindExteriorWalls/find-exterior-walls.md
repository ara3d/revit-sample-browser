# FindExteriorWallsCommand

| Field | Value |
|-------|-------|
| **Sample** | FindExteriorWalls |
| **Class** | `FindExteriorWallsCommand` |
| **Source** | `src/FindExteriorWalls/FindExteriorWallsCommand.cs` |
| **MCP rating** | 4/5 |

Identifies exterior walls from a rectangle-selected set in a floor plan using perpendicular ray casting and end-join propagation, then selects the result.

## What it demonstrates

- Rectangle wall selection with `ISelectionFilter` (`WallsSelectionFilter`)
- Analytical exterior-wall detection: cast rays perpendicular to wall location curves at three sample points (start half, full, end half)
- Classify a wall as exterior when one side has zero ray intersections and the other has at least one (skipping co-directional parallel walls)
- Second pass propagates exterior classification via `LocationCurve.get_ElementsAtJoin` and curve-end intersections
- Curve helpers in `FindExteriorWallsExtensions` for center points, Z-flattened intersections, and perpendicular rays (supports straight and arc walls)

## Prerequisites

- Active project document with a **floor plan** view active
- Walls with `LocationCurve` geometry in the selected set

## User interaction

- `TaskDialog` if the active view is not a floor plan
- Rectangle pick of candidate walls (`PickElementsByRectangle`)
- Selects identified exterior walls via `UIDocument.Selection.SetElementIds`
- `TaskDialog` on propagation overflow (1000 iterations)

## MCP notes

- Proposed tool: `revit_find_exterior_walls`
- Parameters: `wall_ids` (optional array, replaces rectangle pick), `view_id` (optional floor plan)
- Returns: list of exterior wall element ids
- Headless use would require accepting wall ids instead of interactive rectangle selection
- MCP descriptor: `src/FindExteriorWalls/find-exterior-walls.json`

## See also

- MCP descriptor: `src/FindExteriorWalls/find-exterior-walls.json`
- Related: [TBC_ExteriorWalls/exterior-walls.md](../TBC_ExteriorWalls/exterior-walls.md) (automatic room-boundary method, no user selection)
- Upstream algorithm: [RevitFindExteriorWalls](https://github.com/jeremytammik/RevitFindExteriorWalls) — [blog post](http://blog.modplus.org/index.php/11-revitapi/10-revit-find-external-walls-algorithm)

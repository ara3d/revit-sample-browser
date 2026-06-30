# CmdGeomVertices

| Field | Value |
|-------|-------|
| **Sample** | SetoutPoints |
| **Class** | `CmdGeomVertices` |
| **Source** | `src/SetoutPoints/CmdGeomVertices.cs` |
| **Origin** | [jeremytammik/SetoutPoints](https://github.com/jeremytammik/SetoutPoints) (MIT) |
| **MCP rating** | 4/5 |

Places a setout point family instance on every geometry corner of structural concrete elements in the active project.

## What it demonstrates

- Collecting concrete and precast concrete structural elements (columns, framing, floors, foundations, ramps, walls)
- Extracting solid geometry and deduplicating corner vertices with `GeomVertices`
- Loading the `SetoutPoint.rfa` family when it is not already in the project
- Creating non-structural `FamilyInstance` markers and populating shared parameters (X, Y, Z, Host_Type, Host_Id, Point_Number)
- Transforming insertion coordinates with the active project location

## Prerequisites

- Active project with structural concrete elements
- Shared parameters from `src/SetoutPoints/shared_parameters.txt` bound to the setout point family (see upstream [Building Coder article](http://thebuildingcoder.typepad.com/blog/2012/08/structural-concrete-setout-point-add-in.html))
- `SetoutPoint.rfa` in `src/SetoutPoints/test/` (loaded automatically if missing from the project)

## User interaction

- Non-interactive; processes all matching elements in one transaction
- Fails with a message if shared parameters are not bound to the family

## MCP notes

- Proposed tool: `revit_place_concrete_setout_points`
- Parameters: optional element filter (category ids, element ids), optional `dry_run`
- Returns: count of points placed, host element ids, and corner coordinates
- Headless use would skip family UI and accept a family path or assume the family is preloaded
- MCP descriptor: `src/SetoutPoints/geom-vertices.json`

## See also

- MCP descriptor: `src/SetoutPoints/geom-vertices.json`
- Companion command: [CmdRenumber](renumber.md)
- Upstream: [jeremytammik/SetoutPoints](https://github.com/jeremytammik/SetoutPoints)

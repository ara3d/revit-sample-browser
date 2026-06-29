# Command

| Field | Value |
|-------|-------|
| **Sample** | CreateSimpleAreaRein |
| **Class** | `Command` |
| **Source** | `src/CreateSimpleAreaRein/CS/CreateSimpleAreaRein.cs` |
| **SDK ReadMe** | `src/CreateSimpleAreaRein/CS/ReadMe_CreateSimpleAreaRein.rtf` |
| **MCP rating** | 2/5 |

Creates area reinforcement on a single selected basic wall or horizontal rectangular floor using default types and a parameter dialog.

## What it demonstrates

- Floor path: `GeomHelper.GetFloorGeom` and `AreaReinforcement.Create` with major direction from first boundary line
- Wall path: basic wall validation, `GeomHelper.GetWallGeom`, same `AreaReinforcement.Create` workflow
- `AreaReinDataOnFloor` / `AreaReinDataOnWall` and `FillIn` for curve-level parameters

## Prerequisites

- Pre-select exactly one basic wall or horizontal rectangular floor/slab

## User interaction

- `CreateSimpleAreaReinForm` for reinforcement settings; cancel rolls back

## MCP notes

- Similar to CreateComplexAreaRein — selection plus modal form make it a weak direct MCP tool without parameter injection.

## See also

- Related: [createcomplexarearein.md](../CreateComplexAreaRein/createcomplexarearein.md)

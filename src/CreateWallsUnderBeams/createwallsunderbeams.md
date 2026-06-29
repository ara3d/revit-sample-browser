# CreateWallsUnderBeams

| Field | Value |
|-------|-------|
| **Sample** | CreateWallsUnderBeams |
| **Class** | `CreateWallsUnderBeams` |
| **Source** | `src/CreateWallsUnderBeams/CreateWallsUnderBeams.cs` |
| **SDK ReadMe** | `src/CreateWallsUnderBeams/ReadMe_CreateWallsUnderBeams.rtf` |
| **MCP rating** | 4/5 |

Creates one wall along each selected horizontal beam's location curve, offset slightly below the beam elevation.

## What it demonstrates

- Filtering selection for `FamilyInstance` beams with horizontal `LocationCurve`
- `Wall.Create(document, curve, wallTypeId, levelId, height, offset, flip, structural)`
- Base offset derived from beam Z minus a fixed drop (~3000 mm in internal units)
- Per-beam reference level from `BuiltInParameter.INSTANCE_REFERENCE_LEVEL_PARAM`

## Prerequisites

- Pre-selected horizontal structural beams

## User interaction

- `CreateWallsUnderBeamsForm` chooses wall type and structural flag

## MCP notes

- Proposed tool: `revit_create_walls_under_beams`
- Parameters: `beam_ids[]`, `wall_type_id`, `is_structural`, optional `height`, optional `vertical_offset`
- Returns: created wall element ids
- MCP descriptor: `src/CreateWallsUnderBeams/createwallsunderbeams.json`

## See also

- MCP descriptor: `src/CreateWallsUnderBeams/createwallsunderbeams.json`
- Related: [createwallinbeamprofile.md](../CreateWallinBeamProfile/createwallinbeamprofile.md)

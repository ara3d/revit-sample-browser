# CreateWallinBeamProfile

| Field | Value |
|-------|-------|
| **Sample** | CreateWallinBeamProfile |
| **Class** | `CreateWallinBeamProfile` |
| **Source** | `src/CreateWallinBeamProfile/CS/CreateWallinBeamProfile.cs` |
| **SDK ReadMe** | `src/CreateWallinBeamProfile/CS/ReadMe_CreateWallinBeamProfile.rtf` |
| **MCP rating** | 4/5 |

Creates a single wall whose profile follows a closed loop formed by selected structural beam location curves lying in one vertical plane.

## What it demonstrates

- Collecting beam `LocationCurve` geometry and validating a single closed vertical profile
- Ordering curves head-to-tail into a `List<Curve>` for `Wall.Create`
- Base/top constraints from lowest and highest beam elevations relative to a reference level
- `CreateWallinBeamProfileForm` for wall type and structural vs architectural choice

## Prerequisites

- Pre-selected beams (`StructuralType.Beam`) forming one closed coplanar vertical loop

## User interaction

- Dialog selects `WallType` and structural flag; beams must be selected before running

## MCP notes

- Proposed tool: `revit_create_wall_from_beam_profile`
- Parameters: `beam_ids[]`, `wall_type_id`, `level_id`, `is_structural`
- Returns: created wall element id
- MCP descriptor: `src/CreateWallinBeamProfile/createwallinbeamprofile.json`

## See also

- MCP descriptor: `src/CreateWallinBeamProfile/createwallinbeamprofile.json`
- Related: [createwallsunderbeams.md](../CreateWallsUnderBeams/createwallsunderbeams.md)

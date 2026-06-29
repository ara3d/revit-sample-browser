# Command

| Field | Value |
|-------|-------|
| **Sample** | AddSpaceAndZone |
| **Class** | `Command` |
| **Source** | `src/AddSpaceAndZone/CS/Command.cs` |
| **SDK ReadMe** | `src/AddSpaceAndZone/CS/ReadMe_AddSpaceAndZone.rtf` |
| **MCP rating** | 4/5 |

Opens a dialog to query, create, and manage MEP **Space** and **Zone** elements by level and phase.

## What it demonstrates

- Collecting `Space` and `Zone` elements per level via `DataManager`, `SpaceManager`, and `ZoneManager`
- Creating spaces from closed wall loops or space separations
- Creating zones and adding or removing spaces from a zone
- Wrapping changes in a manual document transaction committed only on OK

## Prerequisites

- MEP-enabled project with levels, phases, and enclosing geometry suitable for space creation

## User interaction

- Modal `MainForm` drives all operations; cancel rolls back the transaction
- Headless use would require extracting `DataManager` logic and replacing the form with parameters (level id, phase id, create flags)

## MCP notes

- Proposed tool: `revit_manage_spaces_zones` with actions `list`, `create_spaces`, `create_zone`, `assign_spaces`
- Parameters: `level_id`, `phase_id`, `zone_name`, `space_ids[]`
- Returns: created element ids and counts per level
- MCP descriptor: `docs/mcp/AddSpaceAndZone/addspaceandzone.json`

## See also

- MCP descriptor: `docs/mcp/AddSpaceAndZone/addspaceandzone.json`

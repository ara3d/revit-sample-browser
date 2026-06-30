# LoadAndPlaceNextItemFile

| Field | Value |
|-------|-------|
| **Sample** | FabricationPartLayout |
| **Class** | `LoadAndPlaceNextItemFile` |
| **Source** | `src/FabricationPartLayout/ItemFile.cs` |
| **MCP rating** | 2/5 |

Reloads the fabrication configuration, loads the next unloaded item file from the item folder tree, and places a part on Level 1.

## What it demonstrates

- Walking `FabricationItemFolder` hierarchy with `GetNextUnloadedItemFile`
- `FabricationConfiguration.LoadItemFiles` and `FabricationPart.Create` from `FabricationItemFile`

## Prerequisites

- Fabrication configuration with unloaded valid item files; **Level 1** present

## User interaction

- Selects and zooms to the new part; no file picker

## MCP notes

- Useful pattern for item-file management but depends on config-specific folder contents

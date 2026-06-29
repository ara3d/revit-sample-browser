# UnloadUnusedItemFiles

| Field | Value |
|-------|-------|
| **Sample** | FabricationPartLayout |
| **Class** | `UnloadUnusedItemFiles` |
| **Source** | `src/FabricationPartLayout/ItemFile.cs` |
| **SDK ReadMe** | `src/FabricationPartLayout/Readme_FabricationPartLayout.rtf` |
| **MCP rating** | 2/5 |

Unloads fabrication item files that are loaded in the configuration but not referenced by any placed parts.

## What it demonstrates

- `FabricationConfiguration.GetFabricationConfiguration`, `ReloadConfiguration`, and `GetAllLoadedItemFiles`
- Filtering with `IsUsed == false`, checking `CanUnloadItemFiles`, then `UnloadItemFiles`
- Reporting unloaded identifiers in a `TaskDialog`

## Prerequisites

- Project using a fabrication configuration with at least one loaded, unused item file

## User interaction

- No picks; shows an information dialog listing unloaded file names on success

## MCP notes

- Configuration housekeeping is useful but niche; a headless wrapper could return unloaded file names instead of a dialog

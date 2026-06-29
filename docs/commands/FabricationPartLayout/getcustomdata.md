# GetCustomData

| Field | Value |
|-------|-------|
| **Sample** | FabricationPartLayout |
| **Class** | `GetCustomData` |
| **Source** | `src/FabricationPartLayout/CS/CustomData.cs` |
| **SDK ReadMe** | `src/FabricationPartLayout/CS/Readme_FabricationPartLayout.rtf` |
| **MCP rating** | 2/5 |

Reads and displays all fabrication custom data fields defined in the loaded configuration for a picked fabrication part.

## What it demonstrates

- `FabricationConfiguration.GetAllPartCustomData` and type/name lookup
- `FabricationPart.HasCustomData` with getters for text, integer, and real custom data types

## User interaction

- Single part pick; read-only `TaskDialog` report via `CustomDataHelper`

## MCP notes

- Read-only query suitable for MCP with `element_id`; current command requires interactive pick

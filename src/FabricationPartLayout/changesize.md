# ChangeSize

| Field | Value |
|-------|-------|
| **Sample** | FabricationPartLayout |
| **Class** | `ChangeSize` |
| **Source** | `src/FabricationPartLayout/ChangeService.cs` |
| **SDK ReadMe** | `src/FabricationPartLayout/Readme_FabricationPartLayout.rtf` |
| **MCP rating** | 2/5 |

Remaps rectangular duct sizes on selected fabrication parts using predefined `FabricationPartSizeMap` entries.

## What it demonstrates

- Building `FabricationPartSizeMap` objects with source and mapped width/depth
- `FabricationNetworkChangeService.ChangeSize` and `GetElementsThatFailed` error reporting

## Prerequisites

- Selected parts compatible with the hard-coded 12×12 and 18×18 mappings

## User interaction

- Uses current selection only

## MCP notes

- Size mappings are sample constants; MCP tool would accept a size map array

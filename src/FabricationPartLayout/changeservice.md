# ChangeService

| Field | Value |
|-------|-------|
| **Sample** | FabricationPartLayout |
| **Class** | `ChangeService` |
| **Source** | `src/FabricationPartLayout/CS/ChangeService.cs` |
| **SDK ReadMe** | `src/FabricationPartLayout/CS/Readme_FabricationPartLayout.rtf` |
| **MCP rating** | 2/5 |

Changes the fabrication service and palette for all selected fabrication parts to the first loaded service (palette index 0).

## What it demonstrates

- `FabricationNetworkChangeService.ChangeService` with `FabricationNetworkChangeServiceResult` handling
- `FabricationConfiguration.GetAllLoadedServices`

## Prerequisites

- Selected fabrication parts and a loaded fabrication configuration

## User interaction

- Requires pre-selection; fails if nothing is selected

## MCP notes

- Service/palette indices are hard-coded; needs parameters for real automation

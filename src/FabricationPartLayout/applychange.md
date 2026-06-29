# ApplyChange

| Field | Value |
|-------|-------|
| **Sample** | FabricationPartLayout |
| **Class** | `ApplyChange` |
| **Source** | `src/FabricationPartLayout/ChangeService.cs` |
| **SDK ReadMe** | `src/FabricationPartLayout/Readme_FabricationPartLayout.rtf` |
| **MCP rating** | 2/5 |

Applies a staged service change, palette swap, straight size mapping, and in-line part type remapping to selected fabrication parts using `FabricationNetworkChangeService`.

## What it demonstrates

- Multi-step `FabricationNetworkChangeService` workflow: `SetSelection`, `SetServiceId`, `SetPaletteId`, size maps, in-line type swaps, `ApplyChange`
- `GetMapOfAllSizesForStraights` and `GetInLinePartTypes` for discovery before applying changes

## Prerequisites

- Fabrication configuration with at least two loaded services
- Pre-selected fabrication network elements

## User interaction

- Uses current selection; no dialog

## MCP notes

- Powerful but demo-specific mappings; needs parameterization before MCP use

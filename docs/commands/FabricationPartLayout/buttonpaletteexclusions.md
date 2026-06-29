# ButtonPaletteExclusions

| Field | Value |
|-------|-------|
| **Sample** | FabricationPartLayout |
| **Class** | `ButtonPaletteExclusions` |
| **Source** | `src/FabricationPartLayout/CS/ButtonPaletteExclusions.cs` |
| **SDK ReadMe** | `src/FabricationPartLayout/CS/Readme_FabricationPartLayout.rtf` |
| **MCP rating** | 2/5 |

Excludes a specific service button and an entire palette from the **ADSK - HVAC: Supply Air** fabrication service.

## What it demonstrates

- `FabricationService.OverrideServiceButtonExclusion` for individual buttons
- `FabricationService.SetServicePaletteExclusions` to hide whole palettes

## Prerequisites

- Fabrication configuration with the named HVAC supply air service and expected palette/button names

## User interaction

- Runs headlessly; confirms success in `TaskDialog`

## MCP notes

- Configuration-specific demo; general MCP tool would need service/palette/button identifiers as parameters

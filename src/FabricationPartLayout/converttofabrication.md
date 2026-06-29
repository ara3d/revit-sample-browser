# ConvertToFabrication

| Field | Value |
|-------|-------|
| **Sample** | FabricationPartLayout |
| **Class** | `ConvertToFabrication` |
| **Source** | `src/FabricationPartLayout/ConvertToFabrication.cs` |
| **SDK ReadMe** | `src/FabricationPartLayout/Readme_FabricationPartLayout.rtf` |
| **MCP rating** | 2/5 |

Converts selected design MEP elements to fabrication parts on the first loaded service, optionally mapping a rectangular fire damper family to a fabrication part type.

## What it demonstrates

- `DesignToFabricationConverter` with `SetMapForFamilySymbolToFabricationPartType` and `Convert`
- Resolving `FamilySymbol` and `FabricationPartType` by name for inline component swap

## Prerequisites

- Fabrication configuration loaded; selection of convertable design elements
- Optional damper family **Fire Damper - Rectangular - Simple** and part type **Rect FD - Flange**

## User interaction

- Requires pre-selection

## MCP notes

- Family-to-part-type map is demo-specific; MCP would need explicit mapping tables

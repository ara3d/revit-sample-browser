# Command

| Field | Value |
|-------|-------|
| **Sample** | DeckProperties |
| **Class** | `Command` |
| **Source** | `src/DeckProperties/CS/Command.cs` |
| **SDK ReadMe** | `src/DeckProperties/CS/ReadMe_DeckProperties.rtf` |
| **MCP rating** | 4/5 |

Reads and displays compound structure layer data for selected floors, emphasizing structural deck layers, materials, and deck profile parameters.

## What it demonstrates

- Iterating `FloorType.GetCompoundStructure().GetLayers()` by `MaterialFunctionAssignment`
- Deck layers: `DeckProfileId` as `FamilySymbol`, material name, and parameter dump via `DumpParameters`
- Non-deck layers: material name and `CompoundStructureLayer.Width`
- `DeckPropertyForm` as a read-only log window

## Prerequisites

- Pre-selected floor/slab elements

## User interaction

- Form displays dumped text; no editing — selection required before run

## MCP notes

- Proposed tool: `revit_get_deck_properties`
- Parameters: `floor_ids[]`
- Returns: per-floor layer list with deck profile names and parameters
- MCP descriptor: `src/DeckProperties/deckproperties.json`

## See also

- MCP descriptor: `src/DeckProperties/deckproperties.json`

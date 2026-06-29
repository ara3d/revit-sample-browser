# Command

| Field | Value |
|-------|-------|
| **Sample** | PlacementOptions |
| **Class** | `Command` |
| **Source** | `src/PlacementOptions/CS/Command.cs` |
| **SDK ReadMe** | `src/PlacementOptions/CS/Readme_PlacementOptions.rtf` |
| **MCP rating** | 4/5 |

Demonstrates programmatic family placement using `UIDocument.PromptForFamilyInstancePlacement` with custom `FamilyPlacementOptions` for face-based and sketch-based families.

## What it demonstrates

- `PlacementOptionsEnum` switching between face-based (`OST_GenericModel`) and sketch-based (`OST_StructuralFraming`) flows
- `FilteredElementCollector` with `ElementCategoryFilter` to find eligible `FamilySymbol` types
- `FacebasedForm` configuring `FaceBasedPlacementType`; `SketchbasedForm` configuring sketch gallery options
- `PromptForFamilyInstancePlacement` as the Revit-hosted placement UI

## Prerequisites

- At least one face-based generic model family symbol, or a sketch-based structural framing symbol, depending on the chosen mode

## User interaction

- `OptionsForm` plus mode-specific sub-dialogs, then Revit's built-in placement prompt
- Automation would replace dialogs with explicit symbol id and `FamilyPlacementOptions`

## MCP notes

- Proposed tool: `revit_place_family_with_options`
- Parameters: `family_symbol_id`, `placement_mode` (`face_based` or `sketch_based`), optional `FaceBasedPlacementType` or sketch options
- Returns: placed instance element id(s) from the placement session
- MCP descriptor: `src/PlacementOptions/placementoptions.json`

## See also

- MCP descriptor: `src/PlacementOptions/placementoptions.json`

# CreateDraftingView

| Field | Value |
|-------|-------|
| **Sample** | CreateViewSection |
| **Class** | `CreateDraftingView` |
| **Source** | `src/CreateViewSection/Command.cs` |
| **MCP rating** | 4/5 |

Creates a new empty drafting view using the first available `ViewFamilyType` with `ViewFamily.Drafting`.

## What it demonstrates

- `FilteredElementCollector` for `ViewFamilyType` elements
- `ViewDrafting.Create(document, viewFamilyTypeId)`
- Minimal transaction wrapping with success confirmation dialog

## Prerequisites

- Project containing at least one drafting `ViewFamilyType`

## User interaction

- No selection or input — uses first drafting view type found
- Shows `TaskDialog` on success

## MCP notes

- Proposed tool: `revit_create_drafting_view`
- Parameters: optional `view_family_type_id`, optional `view_name`
- Returns: new drafting view element id
- MCP descriptor: `src/CreateViewSection/createdraftingview.json`

## See also

- MCP descriptor: `src/CreateViewSection/createdraftingview.json`
- Related: [createviewsection.md](createviewsection.md)

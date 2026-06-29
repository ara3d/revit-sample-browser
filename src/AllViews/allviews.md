# Command

| Field | Value |
|-------|-------|
| **Sample** | AllViews |
| **Class** | `Command` |
| **Source** | `src/AllViews/AllViews.cs` |
| **SDK ReadMe** | `src/AllViews/ReadMe_AllViews.rtf` |
| **MCP rating** | 5/5 |

Lists project views, lets the user pick a subset, and generates a sheet with viewports arranged using a golden-ratio layout.

## What it demonstrates

- Enumerating printable views with `ViewsMgr.GetAllViews`
- Collecting title block `FamilySymbol` types
- Creating a sheet and placing `Viewport` elements via `ViewsMgr.GenerateSheet`
- Adjusting viewport position, size, and title visibility on the sheet

## Prerequisites

- Project with views and at least one title block family loaded

## User interaction

- `AllViewsForm` provides tree selection of views, title block choice, and sheet name
- Core `ViewsMgr` logic is separable for automation without the dialog

## MCP notes

- Proposed tools: `revit_list_views` (read-only) and `revit_create_sheet_with_views`
- Parameters: `view_ids[]`, `title_block_type_id`, `sheet_name`, optional layout options
- Returns: new sheet element id and placed viewport ids
- MCP descriptor: `src/AllViews/allviews.json`

## See also

- MCP descriptor: `src/AllViews/allviews.json`

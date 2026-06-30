# HideCategoryCommand

| Field | Value |
|-------|-------|
| **Sample** | HideCategory |
| **Class** | `HideCategoryCommand` |
| **Source** | `src/HideCategory/HideCategoryCommand.cs` |
| **MCP rating** | 4/5 |

Hides the category of a user-picked element in every view of the active project.

## What it demonstrates

- Picking an element to determine its `Category`
- Iterating all `View` elements with `FilteredElementCollector.OfClass`
- `View.SetCategoryHidden` to hide a category per view inside a single transaction
- Handling views where the category cannot be hidden (`ArgumentException`) and other per-view failures

## Prerequisites

- Active project document with at least one view

## User interaction

- Prompts the user to pick one element in the model
- `TaskDialog` summarizes how many views were updated, skipped, or failed

## MCP notes

- Proposed tool: `revit_hide_category_in_all_views`
- Parameters: `category_id` or `element_id` (to resolve category), optional `dry_run`
- Returns: counts and view names for success, skipped, and failed views
- Headless use would replace `PickObject` with a supplied category or element id
- MCP descriptor: `src/HideCategory/hide-category.json`

## See also

- MCP descriptor: `src/HideCategory/hide-category.json`
- Related: [VisibilityControl/visibilitycontrol.md](../VisibilityControl/visibilitycontrol.md) (toggle category visibility in the active view)
- Upstream: [jeremytammik/HideCategory](https://github.com/jeremytammik/HideCategory)

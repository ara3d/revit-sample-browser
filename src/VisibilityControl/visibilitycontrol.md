# Command

| Field | Value |
|-------|-------|
| **Sample** | VisibilityControl |
| **Class** | `Command` |
| **Source** | `src/VisibilityControl/Command.cs` |
| **MCP rating** | 4/5 |

Provides a dialog to toggle category visibility in the active view or isolate elements by category.

## What it demonstrates

- `VisibilityCtrl` enumerating categories with `get_AllowsVisibilityControl` and current visibility state
- `View.SetCategoryHidden` for per-category show/hide in the active view
- Isolate mode: hide all categories then show only categories of picked or window-selected elements
- Transaction commit on OK/Yes, rollback on cancel

## Prerequisites

- Active view that supports category visibility control

## User interaction

- `VisibilityCtrlForm` checklist for categories; optional pick/window isolate (Yes button)
- Headless automation would accept category id list and visibility booleans

## MCP notes

- Proposed tool: `revit_set_category_visibility`
- Parameters: `view_id` (default active), `category_visibility` map of category name/id to visible boolean, optional `isolate_element_ids`
- Returns: updated visibility state per category
- MCP descriptor: `src/VisibilityControl/visibilitycontrol.json`

## See also

- MCP descriptor: `src/VisibilityControl/visibilitycontrol.json`

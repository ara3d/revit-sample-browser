# Command

| Field | Value |
|-------|-------|
| **Sample** | ColorFill |
| **Class** | `Command` |
| **Source** | `src/ColorFill/Command.cs` |
| **MCP rating** | 4/5 |

Applies color fill schemes to views through a manager/form that edits `ColorFill` placement on categories or filters.

## What it demonstrates

- `ColorFillMgr` coordinating view, document, and color fill scheme assignment
- UI-driven selection of schemes and targets via `ColorFillForm`

## Prerequisites

- Project with color fill schemes and views that support them

## User interaction

- `ColorFillForm` modal dialog required

## MCP notes

- Proposed tool: `revit_apply_color_fill`
- Parameters: `view_id`, `scheme_id`, optional category/filter targets
- MCP descriptor: `src/ColorFill/colorfill.json`

## See also

- MCP descriptor: `src/ColorFill/colorfill.json`

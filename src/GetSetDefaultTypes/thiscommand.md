# ThisCommand

| Field | Value |
|-------|-------|
| **Sample** | GetSetDefaultTypes |
| **Class** | `ThisCommand` |
| **Source** | `src/GetSetDefaultTypes/CS/ThisCommand.cs` |
| **SDK ReadMe** | — |
| **MCP rating** | 4/5 |

Shows dockable panes for viewing and editing default family and element types in the active document.

## What it demonstrates

- `DockablePane.PaneExists` and `UIApplication.GetDockablePane` with registered pane ids
- `DefaultFamilyTypesPane` / `DefaultElementTypesPane` `SetDocument` on show
- Integration with `ThisApplication` registered dockable pane providers

## Prerequisites

- Dockable panes registered at application startup (`DefaultFamilyTypes`, `DefaultElementTypes`)

## User interaction

- Shows WPF dockable panes; user edits types interactively in the UI

## MCP notes

- Proposed tool: `revit_get_default_types` / `revit_set_default_type` with category or class key and type id. Extract read/write from pane view models instead of showing UI.

## See also

- MCP descriptor: `src/GetSetDefaultTypes/thiscommand.json`

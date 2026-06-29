# DragAndDropCommand

| Field | Value |
|-------|-------|
| **Sample** | UIAPI |
| **Class** | `DragAndDropCommand` |
| **Source** | `src/UIAPI/DragAndDrop/DragAndDropCommand.cs` |
| **MCP rating** | 1/5 |

Opens a modeless form for dragging furniture family types from a list into the active Revit view.

## What it demonstrates

- `FurnitureFamilyDragAndDropForm` WinForms drag-and-drop onto the Revit canvas
- `FilteredElementCollector` population of furniture `FamilySymbol` types
- Modeless form pattern (`Show()` rather than `ShowDialog()`)

## Prerequisites

- Furniture families loaded in the project

## User interaction

- Modeless palette window; user drags symbols to place instances interactively
- Not automatable without replacing drag-and-drop with programmatic placement

## MCP notes

- Pure UI integration sample; family placement is better served by dedicated placement tools.

## See also

- [CalcCommand](calccommand.md)
- [PlacementOptions](../PlacementOptions/placementoptions.md)

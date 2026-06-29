# SelectionDialog

| Field | Value |
|-------|-------|
| **Sample** | Selections |
| **Class** | `SelectionDialog` |
| **Source** | `src/Selections/CS/Command.cs` |
| **MCP rating** | 2/5 |

Opens a modeless-style workflow that combines interactive element or point picking with a WinForms dialog to move the picked element to a new location.

## What it demonstrates

- `SelectionManager` coordinating `PickObject` / `PickPoint` with stored selection state
- Moving elements via `ElementTransformUtils.MoveElement` using the vector from pick point to target
- Dialog loop with `DialogResult.Retry` to re-run picks without closing the form

## User interaction

- `SelectionForm` lets the user choose element vs. point mode, pick objects, then pick a destination point
- Fully interactive; headless use would require replacing the form with explicit element id and XYZ parameters

## MCP notes

Element move by dialog-driven picks is a poor MCP fit; agents that know source and target coordinates should call move APIs directly.

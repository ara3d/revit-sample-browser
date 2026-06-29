# Command

| Field | Value |
|-------|-------|
| **Sample** | DeleteObject |
| **Class** | `Command` |
| **Source** | `src/DeleteObject/CS/DeleteObject.cs` |
| **SDK ReadMe** | `src/DeleteObject/CS/ReadMe_DeleteObject.rtf` |
| **MCP rating** | 4/5 |

Deletes all elements in the current selection, rolling back if any deletion fails.

## What it demonstrates

- `Document.Delete` for each selected `Element`
- Transaction with rollback on exception; failed elements added to `ElementSet` for highlighting
- `TaskDialog` on delete failure

## Prerequisites

- Pre-selected deletable elements

## User interaction

- Selection-only; no confirmation dialog beyond error handling

## MCP notes

- Proposed tool: `revit_delete_elements`
- Parameters: `element_ids[]`
- Returns: deleted ids or error with blocking element ids
- MCP descriptor: `src/DeleteObject/deleteobject.json`

## See also

- MCP descriptor: `src/DeleteObject/deleteobject.json`

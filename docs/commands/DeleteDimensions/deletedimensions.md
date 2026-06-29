# Command

| Field | Value |
|-------|-------|
| **Sample** | DeleteDimensions |
| **Class** | `Command` |
| **Source** | `src/DeleteDimensions/CS/DeleteDimesions.cs` |
| **SDK ReadMe** | `src/DeleteDimensions/CS/ReadMe_DeleteDimensions.rtf` |
| **MCP rating** | 4/5 |

Deletes all unpinned `Dimension` elements in the current selection.

## What it demonstrates

- Filtering selection for `Dimension` with `Pinned == false`
- `Document.Delete(elementId)` inside a named transaction
- Error messages when selection is empty or contains no unpinned dimensions

## Prerequisites

- Pre-selected dimensions (pinned dimensions are skipped)

## User interaction

- Selection-only; no dialog

## MCP notes

- Proposed tool: `revit_delete_unpinned_dimensions`
- Parameters: `dimension_ids[]` (optional filter) or `delete_all_unpinned_in_selection`
- Returns: deleted element ids
- MCP descriptor: `docs/mcp/DeleteDimensions/deletedimensions.json`

## See also

- MCP descriptor: `docs/mcp/DeleteDimensions/deletedimensions.json`

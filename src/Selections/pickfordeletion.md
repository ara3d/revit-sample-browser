# PickforDeletion

| Field | Value |
|-------|-------|
| **Sample** | Selections |
| **Class** | `PickforDeletion` |
| **Source** | `src/Selections/Command.cs` |
| **MCP rating** | 2/5 |

Lets the user pick multiple elements in the graphics view and deletes them in one transaction.

## What it demonstrates

- `UIDocument.Selection.PickObjects` with `ObjectType.Element` and finish/cancel on the selection bar
- Batch `Document.Delete` from collected element ids
- `OperationCanceledException` handling with transaction rollback

## Prerequisites

- Active document with deletable elements

## User interaction

- Interactive multi-pick required; ESC cancels without deleting

## MCP notes

Deletion by interactive pick is redundant for MCP agents that already know element ids; use `revit_delete_elements` pattern instead.

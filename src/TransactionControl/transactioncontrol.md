# Command

| Field | Value |
|-------|-------|
| **Sample** | TransactionControl |
| **Class** | `Command` |
| **Source** | `src/TransactionControl/Command.cs` |
| **MCP rating** | 2/5 |

Opens an interactive form for experimenting with `Transaction`, `TransactionGroup`, and wall create/move/delete operations.

## What it demonstrates

- Nested `TransactionGroup` and `Transaction` start, commit, and rollback
- Visual tree tracking of document changes with color-coded node states
- Wall creation, movement, and deletion within controlled transaction scopes (`CreateWallForm`, `TransactionForm`)

## Prerequisites

- An open project document with levels and wall types for create operations

## User interaction

- Entirely driven by `TransactionForm` modal dialog; no headless path
- User manually starts, commits, or rolls back transactions and performs wall edits

## MCP notes

- Educational sample for transaction semantics; not suitable as an MCP tool because behavior is exploratory UI, not a fixed API operation.

## See also

- [Journaling](../Journaling/journaling.md)

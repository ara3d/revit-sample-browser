# SetReadonlyCost2

| Field | Value |
|-------|-------|
| **Sample** | ReadonlySharedParameters |
| **Class** | `SetReadonlyCost2` |
| **Source** | `src/ReadonlySharedParameters/ReadonlySharedParametersCommands.cs` |
| **MCP rating** | 2/5 |

Populates **ReadonlyCost** on applicable element types using a simple incrementing seed formula.

## What it demonstrates

- Same collector and filter pattern as `SetReadonlyCost1`
- `GetReadonlyCostFromIncrements` assigning `seed * 100.0 + 0.88` per type
- Transaction-named batch update **Apply ReadonlyCost**

## Prerequisites

- **ReadonlyCost** shared parameter bound to element types

## User interaction

- Headless; no picks or dialogs

## MCP notes

Alternate demo strategy for the same read-only parameter; not intended as production pricing automation.

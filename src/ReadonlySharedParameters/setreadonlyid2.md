# SetReadonlyId2

| Field | Value |
|-------|-------|
| **Sample** | ReadonlySharedParameters |
| **Class** | `SetReadonlyId2` |
| **Source** | `src/ReadonlySharedParameters/CS/ReadonlySharedParametersCommands.cs` |
| **MCP rating** | 2/5 |

Sets **ReadonlyId** on filtered elements using a composite of type name prefix and element id.

## What it demonstrates

- `GetReadonlyIdFromElementId` building `typeName.Substring(0,2) + element.Id`
- Same applicability filter and transaction pattern as `SetReadonlyId1`
- Alternative ID generation for external content coordination demos

## Prerequisites

- **ReadonlyId** instance parameter bound in the document

## User interaction

- No dialog

## MCP notes

Companion to `SetReadonlyId1`; illustrates alternate ID schemes rather than a reusable MCP operation.

# SetReadonlyId1

| Field | Value |
|-------|-------|
| **Sample** | ReadonlySharedParameters |
| **Class** | `SetReadonlyId1` |
| **Source** | `src/ReadonlySharedParameters/CS/ReadonlySharedParametersCommands.cs` |
| **MCP rating** | 2/5 |

Sets the **ReadonlyId** instance parameter on all elements that expose it to each element's `UniqueId`.

## What it demonstrates

- `ParameterFilterRuleFactory.CreateSharedParameterApplicableRule("ReadonlyId")`
- `ReadonlyIdSetter.SetReadonlyIds1` using `element.UniqueId` as the value
- Instance-level shared parameter updates across filtered elements

## Prerequisites

- **ReadonlyId** bound to instances (via bind command)

## User interaction

- Runs without UI

## MCP notes

Shows coordination-id stamping pattern; demo-specific parameter name limits MCP generality.

# SetReadonlyCost1

| Field | Value |
|-------|-------|
| **Sample** | ReadonlySharedParameters |
| **Class** | `SetReadonlyCost1` |
| **Source** | `src/ReadonlySharedParameters/CS/ReadonlySharedParametersCommands.cs` |
| **MCP rating** | 2/5 |

Populates the **ReadonlyCost** type parameter on all element types that expose it, using a formula based on element id.

## What it demonstrates

- `FilteredElementCollector` with `WhereElementIsElementType` and `ElementParameterFilter` for shared parameter applicability
- `ReadonlyCostSetter.SetReadonlyCosts1` computing `(elementId % 100) * 100 + 0.99`
- Batch parameter writes inside a single transaction

## Prerequisites

- **ReadonlyCost** bound to types (run `BindNewReadonlySharedParametersToDocument` first)

## User interaction

- No UI; updates all matching types automatically

## MCP notes

Demonstrates programmatic writes to read-only shared parameters via API; niche demo logic, not a general cost-update tool.

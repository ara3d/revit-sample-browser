# Command

| Field | Value |
|-------|-------|
| **Sample** | GeometryAPI/ComputedSymbolGeometry |
| **Class** | `Command` |
| **Source** | `src/GeometryAPI/ComputedSymbolGeometry/CS/Command.cs` |
| **SDK ReadMe** | — |
| **MCP rating** | 2/5 |

Extracts and displays computed symbol geometry for family instances in the active document.

## What it demonstrates

- `ComputedSymbolGeometry` helper class walking instances and reading `get_Geometry` with computed-symbol options
- Transaction wrapper around geometry inspection (read-oriented workflow)
- Presenting geometry results through the sample’s viewer layer

## Prerequisites

- Project with family instances whose symbol geometry can be computed

## User interaction

- No picks; output is shown via the sample’s display mechanism after `GetInstanceGeometry`

## MCP notes

- Geometry introspection is educational; output format is viewer-specific rather than machine-friendly JSON

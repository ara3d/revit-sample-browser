# Command

| Field | Value |
|-------|-------|
| **Sample** | ParameterUtils |
| **Class** | `Command` |
| **Source** | `src/ParameterUtils/CS/Command.cs` |
| **SDK ReadMe** | `src/ParameterUtils/CS/ReadMe_ParameterUtils.rtf` |
| **MCP rating** | 5/5 |

Shows a property-grid style list of every parameter on a single selected element, including name, storage type, and value.

## What it demonstrates

- Iterating `element.Parameters` on the sole selection
- Reading values by `StorageType` (`Double`, `Integer`, `String`, `ElementId`, `None` via `AsValueString`)
- Resolving `ElementId` parameters to element names with `Document.GetElement`
- Read-only transaction mode (`TransactionMode.ReadOnly`)

## Prerequisites

- Exactly one element selected

## User interaction

- `PropertiesForm` displays tab-separated parameter rows
- Parameter enumeration logic is entirely headless

## MCP notes

- Proposed tool: `revit_get_element_parameters`
- Parameters: `element_id`, optional `include_read_only`
- Returns: array of `{ name, storage_type, value, value_string }`
- MCP descriptor: `src/ParameterUtils/parameterutils.json`

## See also

- MCP descriptor: `src/ParameterUtils/parameterutils.json`

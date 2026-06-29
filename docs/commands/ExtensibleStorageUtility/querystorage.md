# QueryStorage

| Field | Value |
|-------|-------|
| **Sample** | ExtensibleStorageUtility |
| **Class** | `QueryStorage` |
| **Source** | `src/ExtensibleStorageUtility/CS/QueryStorage.cs` |
| **SDK ReadMe** | `src/ExtensibleStorageUtility/CS/Readme_ExtensibleStorageUtility.rtf` |
| **MCP rating** | 5/5 |

Reports all extensible storage schemas in the document and lists elements that carry storage for each schema.

## What it demonstrates

- Read-only inspection via `StorageUtility.GetElementsWithAllSchemas`
- `Schema.ListSchemas` and per-schema element lookup in `StorageUtility`

## User interaction

- Displays results in a `TaskDialog`; no picks or edits

## MCP notes

- Proposed tool: `revit_query_extensible_storage`
- Parameters: optional `schema_guid` to filter one schema
- Returns: structured list of schema guids, field names, and hosting element ids
- MCP descriptor: `docs/mcp/ExtensibleStorageUtility/querystorage.json`

## See also

- MCP descriptor: `docs/mcp/ExtensibleStorageUtility/querystorage.json`

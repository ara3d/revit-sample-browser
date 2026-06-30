# DeleteStorage

| Field | Value |
|-------|-------|
| **Sample** | ExtensibleStorageUtility |
| **Class** | `DeleteStorage` |
| **Source** | `src/ExtensibleStorageUtility/DeleteStorage.cs` |
| **MCP rating** | 4/5 |

Erases all extensible storage schemas and entities from the active document, or reports when no storage exists.

## What it demonstrates

- Detecting storage with `StorageUtility.DoesAnyStorageExist`
- Iterating `Schema.ListSchemas()` and calling `Document.EraseSchemaAndAllEntities`
- Transaction-wrapped bulk deletion with user feedback via `TaskDialog`

## User interaction

- Runs immediately; shows result message in a dialog
- Document must be saved after deletion to persist changes

## MCP notes

- Proposed tool: `revit_delete_extensible_storage`
- Parameters: optional `schema_guids[]` to limit deletion; default all schemas
- Returns: count of schemas erased and whether document had storage
- MCP descriptor: `src/ExtensibleStorageUtility/deletestorage.json`

## See also

- MCP descriptor: `src/ExtensibleStorageUtility/deletestorage.json`

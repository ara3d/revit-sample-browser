# Command

| Field | Value |
|-------|-------|
| **Sample** | ExtensibleStorageManager / ExtensibleStorageManager |
| **Class** | `Command` |
| **Source** | `src/ExtensibleStorageManager/ExtensibleStorageManager/Application/Command.cs` |
| **MCP rating** | 2/5 |

Opens a WPF dialog for browsing, creating, and editing extensible storage schemas and entity data on document elements.

## What it demonstrates

- Launching `UiCommand` WPF UI bound to the active document and add-in GUID
- Schema wrapper tools (`SchemaWrapper`, `SchemaDataWrapper`) for typed field access
- XML schema definitions under `schemas/` for sample data structures

## User interaction

- Full modal WPF workflow; all read/write operations go through the UI

## MCP notes

- Useful reference for extensible storage UI patterns; automation would need to bypass the dialog and call `StorageCommand` logic directly

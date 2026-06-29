# AddParameterToFamilies

| Field | Value |
|-------|-------|
| **Sample** | FamilyCreation/AutoParameter |
| **Class** | `AddParameterToFamilies` |
| **Source** | `src/FamilyCreation/AutoParameter/CS/Command.cs` |
| **SDK ReadMe** | `src/FamilyCreation/AutoParameter/CS/ReadMe_AutoParameter.rtf` |
| **MCP rating** | 4/5 |

Batch-opens `.rfa` files from a fixed folder, adds parameters from text files, saves, and closes each family.

## What it demonstrates

- `Application.OpenDocumentFile`, per-family transactions, and `Document.Save` / `Close`
- Scanning `MyDocuments\AutoParameter_Families` for writable `.rfa` files
- Reusing `FamilyParameterAssigner` for the same parameter-file format as the single-family command

## Prerequisites

- Folder `AutoParameter_Families` under the user Documents directory containing family files; parameter txt files beside the add-in

## User interaction

- No UI; writes errors to `MessageManager` when folders or files are missing

## MCP notes

- Proposed tool: `revit_batch_add_family_parameters` with `family_folder` and `parameter_definitions`. Replace hard-coded MyDocuments path with caller-supplied directory.

## See also

- MCP descriptor: `docs/mcp/FamilyCreation/AutoParameter/addparametertofamilies.json`
- Related: [addparametertofamily](addparametertofamily.md)

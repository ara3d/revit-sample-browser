# Command

| Field | Value |
|-------|-------|
| **Sample** | FamilyCreation/DWGFamilyCreation |
| **Class** | `Command` |
| **Source** | `src/FamilyCreation/DWGFamilyCreation/Command.cs` |
| **SDK ReadMe** | `src/FamilyCreation/DWGFamilyCreation/ReadMe_DWGFamilyCreation.rtf` |
| **MCP rating** | 2/5 |

Imports a bundled `Desk.dwg` into a family floor plan and adds type parameters recording the file name and import date.

## What it demonstrates

- `Document.Import` with `DWGImportOptions` (`ImportPlacement.Origin`, `OrientToView`)
- `FamilyManager.NewType`, `AddParameter`, and `Set` for string instance/type data
- Locating the `Ref. Level` floor plan view in a family document

## Prerequisites

- Family document from the provided template; `Desk.dwg` beside the add-in assembly

## User interaction

- No picks; uses fixed DWG path and view name

## MCP notes

- DWG import is automatable with file path and view id, but the sample is a single hard-coded demo

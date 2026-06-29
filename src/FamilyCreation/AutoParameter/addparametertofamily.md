# AddParameterToFamily

| Field | Value |
|-------|-------|
| **Sample** | FamilyCreation/AutoParameter |
| **Class** | `AddParameterToFamily` |
| **Source** | `src/FamilyCreation/AutoParameter/CS/Command.cs` |
| **SDK ReadMe** | `src/FamilyCreation/AutoParameter/CS/ReadMe_AutoParameter.rtf` |
| **MCP rating** | 4/5 |

Adds family and shared parameters defined in text files to the active family document.

## What it demonstrates

- `FamilyParameterAssigner` loading `familyParameter.txt` and `sharedParameter.txt` beside the add-in
- `FamilyManager.AddParameter` / shared-parameter binding inside a transaction
- Validating `Document.IsFamilyDocument` before modification

## Prerequisites

- Open family document; parameter definition files in the add-in assembly folder

## User interaction

- Runs immediately with no dialog; errors accumulate in `MessageManager.MessageBuff`

## MCP notes

- Proposed tool: `revit_add_family_parameters` with `parameter_definitions` (inline or file path) and optional `family_path` when not active. Refactor file I/O to accept structured input.

## See also

- MCP descriptor: `src/FamilyCreation/AutoParameter/addparametertofamily.json`
- Related: [addparametertofamilies](addparametertofamilies.md)

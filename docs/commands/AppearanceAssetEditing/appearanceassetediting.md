# Command

| Field | Value |
|-------|-------|
| **Sample** | AppearanceAssetEditing |
| **Class** | `Command` |
| **Source** | `src/AppearanceAssetEditing/CS/Command.cs` |
| **SDK ReadMe** | `src/AppearanceAssetEditing/CS/Readme_AppearanceAssetEditing.rtf` |
| **MCP rating** | 2/5 |

Launches a modeless/form-based UI for editing rendering appearance assets on materials.

## What it demonstrates

- Delegating to `Application.ThisApp.ShowForm` for appearance asset property editing
- Pattern for hosting a persistent add-in form from an external command

## User interaction

- Entire workflow is UI-driven through the sample's custom form
- No document mutation logic in `Command.Execute` itself

## MCP notes

- Poor automation fit: visual asset editing is inherently interactive and not easily parameterized

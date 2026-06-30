# Command

| Field | Value |
|-------|-------|
| **Sample** | InvisibleParam |
| **Class** | `Command` |
| **Source** | `src/InvisibleParam/Command.cs` |
| **MCP rating** | 2/5 |

Creates visible and invisible shared parameters on the Walls category using a temporary shared parameter file.

## What it demonstrates

- Resetting `Application.SharedParametersFilename` and opening a new `DefinitionFile`
- `ExternalDefinitionCreationOptions` with visibility flag; `InstanceBinding` to `OST_Walls`
- `ParameterBindings.Insert` for project-level shared parameter registration

## Prerequisites

- Project document; write access to create `RevitParameters.txt` beside the add-in

## User interaction

- No dialog; writes a fresh parameter file each run

## MCP notes

- Demonstrates invisible parameter definitions; automatable but tied to a demo file path and fixed parameter names

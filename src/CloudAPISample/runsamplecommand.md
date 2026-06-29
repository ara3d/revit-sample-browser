# RunSampleCommand

| Field | Value |
|-------|-------|
| **Sample** | CloudAPISample |
| **Class** | `RunSampleCommand` |
| **Source** | `src/CloudAPISample/Application.cs` |
| **SDK ReadMe** | `src/CloudAPISample/ReadMe_CloudAPISample.rtf` |
| **MCP rating** | 1/5 |

Launches the Cloud API tutorial menu via `SampleEngine`, registering BIM 360 migration samples behind a ribbon button.

## What it demonstrates

- `IExternalApplication` ribbon setup with `PushButtonData`
- `SampleEngine` pattern for registering and running cloud workflow samples
- `IExternalCommandAvailability` on the run command

## User interaction

- Opens interactive cloud tutorial UI (`engine.Run()`); requires Autodesk cloud context and credentials

## MCP notes

- Infrastructure/demo only; cloud workflows are OAuth-bound and not suitable as generic Revit document MCP tools

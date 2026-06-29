# Command

| Field | Value |
|-------|-------|
| **Sample** | BeamAndSlabNewParameter |
| **Class** | `Command` |
| **Source** | `src/BeamAndSlabNewParameter/BeamAndSlabNewParameter.cs` |
| **SDK ReadMe** | `src/BeamAndSlabNewParameter/ReadMe_BeamAndSlabNewParameter.rtf` |
| **MCP rating** | 2/5 |

Adds a shared instance parameter to beams and slabs, stores GUID values, and finds elements by that parameter.

## What it demonstrates

- Opening and navigating a shared parameter file (`DefinitionFile`, `DefinitionGroup`)
- Binding instance parameters to structural categories with `InstanceBinding`
- Writing GUID values and searching elements by parameter value

## Prerequisites

- Valid shared parameter file path configured in Revit; beams and/or slabs in the model

## User interaction

- `BeamAndSlabParametersForm` drives bind, assign, and find operations

## MCP notes

- Shared-parameter workflows are automatable, but this sample mixes file paths, category binding, and UI in one form

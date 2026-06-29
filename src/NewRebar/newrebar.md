# Command

| Field | Value |
|-------|-------|
| **Sample** | NewRebar |
| **Class** | `Command` |
| **Source** | `src/NewRebar/Command.cs` |
| **SDK ReadMe** | `src/NewRebar/ReadMe_NewRebar.rtf` |
| **MCP rating** | 2/5 |

Walks through creating a new rebar shape and placing rebar in a selected concrete beam or column via a multi-step dialog workflow.

## What it demonstrates

- `RebarCreator` validating concrete beam/column hosts with structural filters
- Custom rebar shape definition (`RebarShapeDef`, parameters, constraints) through `NewRebarForm`
- `Rebar.Create` placement driven by `GeometrySupport` extracted from the host

## Prerequisites

- Selected concrete beam or column family instance

## User interaction

- Series of WinForms (`NewRebarForm`, shape and parameter dialogs); fully interactive

## MCP notes

- Shape authoring UI is not MCP-friendly; would need predefined `RebarShapeType` and placement curves as parameters.

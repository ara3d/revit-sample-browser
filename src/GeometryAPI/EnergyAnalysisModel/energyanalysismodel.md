# Command

| Field | Value |
|-------|-------|
| **Sample** | GeometryAPI/EnergyAnalysisModel |
| **Class** | `Command` |
| **Source** | `src/GeometryAPI/EnergyAnalysisModel/Command.cs` |
| **SDK ReadMe** | — |
| **MCP rating** | 2/5 |

Initializes energy analysis model data and lets the user browse analysis options and results in a dialog.

## What it demonstrates

- `EnergyAnalysisModel.Initialize` preparing analytical geometry for the current document
- `OptionsAndAnalysisForm` for user configuration and visualization
- Transaction rollback on dialog cancel

## Prerequisites

- Model suitable for energy analysis (per Revit energy settings)

## User interaction

- Modal options/analysis form; cancel rolls back the transaction

## MCP notes

- Analysis export could be an MCP tool, but this sample focuses on interactive browsing

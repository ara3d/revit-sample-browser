# Command

| Field | Value |
|-------|-------|
| **Sample** | Journaling |
| **Class** | `Command` |
| **Source** | `src/Journaling/CS/Command.cs` |
| **SDK ReadMe** | — |
| **MCP rating** | 1/5 |

Demonstrates journaling modes by running scripted document operations that replay through Revit’s journal mechanism.

## What it demonstrates

- `[Journaling(JournalingMode.UsingCommandData)]` attribute on the external command
- `Journaling` helper class performing the main work inside a transaction
- Patterns for journal-compatible command data serialization

## Prerequisites

- Journal replay environment when testing playback (interactive run uses live document)

## User interaction

- No end-user dialog; intended for developers testing journal capture/replay

## MCP notes

- Infrastructure/testing sample; not an agent-facing document operation

# Command

| Field | Value |
|-------|-------|
| **Sample** | AvoidObstruction |
| **Class** | `Command` |
| **Source** | `src/AvoidObstruction/Command.cs` |
| **MCP rating** | 2/5 |

Resolves duct/pipe routing obstructions by adjusting fitting placement using the sample's `Resolver` engine.

## What it demonstrates

- Wrapping obstruction resolution in a single transaction
- Delegating geometry and connector logic to `Resolver` with `ExternalCommandData` context

## Prerequisites

- MEP model with clashing or obstructed routing elements as expected by the sample resolver

## User interaction

- No dialog in `Command`; resolver may prompt or use selection internally

## MCP notes

- Specialized routing demo; obstruction resolution is context-heavy and not a clean read/query or batch-edit API

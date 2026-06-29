# Command

| Field | Value |
|-------|-------|
| **Sample** | AutoRoute |
| **Class** | `Command` |
| **Source** | `src/AutoRoute/CS/Command.cs` |
| **SDK ReadMe** | `src/AutoRoute/CS/ReadMe_AutoRoute.rtf` |
| **MCP rating** | 2/5 |

Automatically routes ductwork and fittings between a supply air handler and two terminals, creating a mechanical system.

## What it demonstrates

- Locating mechanical equipment and terminals by category/symbol
- Creating ducts with `Document.Create.NewDuct` and fittings with `NewElbowFitting` / `NewTeeFitting`
- Connecting connectors via `Connector.ConnectTo` with layout heuristics and bounding-box fallbacks

## Prerequisites

- MEP template with placed air handler and two air terminals; equipment positions affect routing geometry

## User interaction

- Runs immediately with no dialog; uses hard-coded offsets and document content
- Layout is brittle to model changes

## MCP notes

- Demonstrates routing API patterns but is not generalized enough for a reusable MCP tool without major refactoring

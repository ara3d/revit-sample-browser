# ExternalCommandCreateWall

| Field | Value |
|-------|-------|
| **Sample** | ExternalCommand |
| **Class** | `ExternalCommandCreateWall` |
| **Source** | `src/ExternalCommand/CS/ExternalCommandRegistration/ExternalCommandClass.cs` |
| **MCP rating** | 1/5 |

Creates a rectangular wall loop from four bound lines to demonstrate a registered external command that modifies the model.

## What it demonstrates

- `Wall.Create` with a closed `Curve` loop (60×40 ft rectangle in the sample coordinates)
- Manual transaction wrapping a single geometry creation

## User interaction

- Runs immediately with hard-coded geometry; no picks

## MCP notes

- Illustrates add-in registration wiring rather than reusable wall-creation automation

# Dummy

| Field | Value |
|-------|-------|
| **Sample** | Ribbon |
| **Class** | `Dummy` |
| **Source** | `src/Ribbon/CS/AddInCommand.cs` |
| **MCP rating** | 1/5 |

No-op external command required because ribbon `ToggleButton` items must bind to an `IExternalCommand`.

## What it demonstrates

- Placeholder command returning `Result.Succeeded` immediately
- Ribbon UI pattern where toggle state is visual only

## User interaction

- Toggle button invokes this command but performs no document changes

## MCP notes

Not applicable; exists only to satisfy ribbon toggle binding requirements.

# Command

| Field | Value |
|-------|-------|
| **Sample** | Events / SelectionChanged |
| **Class** | `Command` |
| **Source** | `src/Events/SelectionChanged/Command.cs` |
| **SDK ReadMe** | `src/Events/SelectionChanged/ReadMe_SelectionChanged.rtf` |
| **MCP rating** | 1/5 |

Shows or focuses a WPF info window that logs `SelectionChanged` UI events as the user changes the active selection.

## What it demonstrates

- Subscribing to selection-changed events in `SelectionChanged` static handler
- `InfoWindow` WPF UI for displaying selection details via `LogManager`

## User interaction

- Opens a modeless info window on first run; subsequent runs focus the existing window

## MCP notes

- Debugging aid for selection events; not suitable for MCP tooling

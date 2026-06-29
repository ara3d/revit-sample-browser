# Command

| Field | Value |
|-------|-------|
| **Sample** | RoutingPreferenceTools |
| **Class** | `Command` |
| **Source** | `src/RoutingPreferenceTools/RoutingPreferenceAnalysis/Command.cs` |
| **MCP rating** | 2/5 |

Opens a WPF window to analyze routing preferences for pipe types in an MEP document.

## What it demonstrates

- `Validation.ValidateMep` and `ValidatePipesDefined` guards before analysis UI
- `MainWindow` presenting routing preference data from the active document
- Read-only transaction mode for inspection

## Prerequisites

- Revit MEP with at least one pipe type defined

## User interaction

- Modal WPF `MainWindow`; no document changes from the command itself

## MCP notes

Analysis viewer tied to WPF UI; export-oriented sibling commands are better MCP candidates.

## See also

- Import: [commandreadpreferences.md](commandreadpreferences.md)
- Export: [commandwritepreferences.md](commandwritepreferences.md)

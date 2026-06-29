# SsePointVisibility

| Field | Value |
|-------|-------|
| **Sample** | Toposolid |
| **Class** | `SsePointVisibility` |
| **Source** | `src/Toposolid/Command.cs` |
| **SDK ReadMe** | `src/Toposolid/ReadMe_Toposolid.rtf` |
| **MCP rating** | 2/5 |

Hides SSE (shape-editing) points for the toposolid category in the active document.

## What it demonstrates

- `SSEPointVisibilitySettings.SetVisibility` and `GetVisibility` for `OST_Toposolid`
- Document-level display setting changed inside a transaction

## Prerequisites

- None beyond an open project document

## User interaction

- Fully headless; sets visibility to `false` with no user input

## MCP notes

- Trivial boolean toggle; could be a parameter on a view-display tool but not worth a dedicated MCP entry.

## See also

- [ToposolidCreation](toposolidcreation.md)

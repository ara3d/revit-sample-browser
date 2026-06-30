# CommandClearExternalGraphics

| Field | Value |
|-------|-------|
| **Sample** | DuplicateGraphics |
| **Class** | `CommandClearExternalGraphics` |
| **Source** | `src/DuplicateGraphics/Command.cs` |
| **MCP rating** | 2/5 |

Unregisters all DirectContext3D drawing servers created by the DuplicateGraphics external application.

## What it demonstrates

- `Application.ProcessCommandClearExternalGraphics(document)` 
- `UnregisterServers` removing `RevitElementDrawingServer` instances from `DirectContext3DService`
- Paired cleanup for `CommandDuplicateGraphics` overlay graphics

## Prerequisites

- `DuplicateGraphics` `IExternalApplication` loaded (registers document-close cleanup)

## User interaction

- Single-click clear; no picks

## MCP notes

- Clearing transient graphics overlays is possible headlessly but low value without the duplicate command workflow.

## See also

- Related: [commandduplicategraphics.md](commandduplicategraphics.md)

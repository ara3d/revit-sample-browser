# CommandDuplicateGraphics

| Field | Value |
|-------|-------|
| **Sample** | DuplicateGraphics |
| **Class** | `CommandDuplicateGraphics` |
| **Source** | `src/DuplicateGraphics/Command.cs` |
| **MCP rating** | 4/5 |

Duplicates graphics of user-picked elements as DirectContext3D overlays offset from the originals without creating new Revit elements.

## What it demonstrates

- `IDirectContext3DServer` implementation in `RevitElementDrawingServer`
- `ExternalServiceRegistry` / `MultiServerService` server registration and activation
- `PickObjects` multi-select feeding per-element servers with fixed `XYZ` offset (0,0,45)
- `UIDocument.UpdateAllOpenViews` to refresh display

## Prerequisites

- DuplicateGraphics `Application` OnStartup registered

## User interaction

- Requires picking one or more elements interactively

## MCP notes

- Proposed tool: `revit_duplicate_graphics_overlay`
- Parameters: `element_ids[]`, optional `offset` {x,y,z}
- Returns: registered server ids (transient, not element ids)
- MCP descriptor: `src/DuplicateGraphics/commandduplicategraphics.json`

## See also

- MCP descriptor: `src/DuplicateGraphics/commandduplicategraphics.json`
- Related: [commandclearexternalgraphics.md](commandclearexternalgraphics.md)

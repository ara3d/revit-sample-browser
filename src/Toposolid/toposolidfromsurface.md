# ToposolidFromSurface

| Field | Value |
|-------|-------|
| **Sample** | Toposolid |
| **Class** | `ToposolidFromSurface` |
| **Source** | `src/Toposolid/Command.cs` |
| **SDK ReadMe** | `src/Toposolid/ReadMe_Toposolid.rtf` |
| **MCP rating** | 2/5 |

Converts a picked legacy topography surface into a new toposolid element.

## What it demonstrates

- `Toposolid.CreateFromTopographySurface` migration API
- `TopographySurfaceFilter` for element selection
- Post-creation inspection of `GetSubDivisionIds`

## Prerequisites

- A `TopographySurface` element in the model
- `ToposolidType` and `Level` available

## User interaction

- Requires picking one topography surface
- Could be headless with a `topography_surface_id` parameter

## MCP notes

- One-time migration helper; useful in batch upgrades but narrow scope for general MCP tooling.

## See also

- [ToposolidCreation](toposolidcreation.md)

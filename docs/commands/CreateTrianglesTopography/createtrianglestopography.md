# Command

| Field | Value |
|-------|-------|
| **Sample** | CreateTrianglesTopography |
| **Class** | `Command` |
| **Source** | `src/CreateTrianglesTopography/CS/Command.cs` |
| **SDK ReadMe** | `src/CreateTrianglesTopography/CS/ReadMe_CreateTrianglesTopography.rtf` |
| **MCP rating** | 3/5 |

Builds a topography surface from triangle mesh data loaded from an embedded JSON file beside the add-in assembly.

## What it demonstrates

- `TrianglesData.Load()` parsing `TrianglesData.json` into `XYZ` points and facet index lists
- `PolymeshFacet` construction and `TopographySurface.Create(document, points, facets)`
- Setting the surface name via `BuiltInParameter.ROOM_NAME`

## Prerequisites

- `TrianglesData.json` deployed next to the add-in DLL

## User interaction

- None — mesh data is fixed at build/deploy time

## MCP notes

- Proposed tool: `revit_create_topography_mesh`
- Parameters: `points[]`, `facets[]` (triangle vertex index triples), optional `name`
- Returns: topography surface element id
- MCP descriptor: `docs/mcp/CreateTrianglesTopography/createtrianglestopography.json`

## See also

- MCP descriptor: `docs/mcp/CreateTrianglesTopography/createtrianglestopography.json`

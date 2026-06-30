# StlImportCommand

| Field | Value |
|-------|-------|
| **Sample** | StlImport |
| **Class** | `StlImportCommand` |
| **Source** | `src/StlImport/StlImportCommand.cs` |
| **Origin** | [jeremytammik/StlImport](https://github.com/jeremytammik/StlImport) (MIT) |
| **MCP rating** | 3/5 |

Imports an STL file into a Generic Model `DirectShape` element. Written by Scott Conover (Autodesk) to demonstrate `DirectShape` with externally sourced mesh geometry.

## What it demonstrates

- Reading binary and ASCII STL via the [QuantumConcepts.Formats.STL](https://www.nuget.org/packages/QuantumConcepts.Formats.STL) package
- Building Revit geometry with `TessellatedShapeBuilder` and `DirectShape.CreateElement`
- Configurable builder target, fallback, and graphics style through shared `StlImportProperties`

## Prerequisites

- Active project document
- Valid STL file on disk

## User interaction

- `OpenFileDialog` prompts for an STL file; cancel returns `Cancelled`
- `TaskDialog` reports the created DirectShape id and facet count on success

## Import options

Run one of the property setter commands before import to change defaults:

| Command | Effect |
|---------|--------|
| `SetToSolidCommand` | Solid target, abort on failure |
| `SetToAnyGeometryCommand` | Any geometry target, mesh fallback (default) |
| `SetToPolymeshCommand` | Mesh target, salvage fallback |
| `SetStyleToNoneCommand` | No graphics style (default) |
| `SetStyleToSketchCommand` | Use the `<Sketch>` graphics style when present |
| `SetDataTypeToBinaryCommand` | Read binary STL (default) |
| `SetDataTypeToAsciiCommand` | Read ASCII STL |

## MCP notes

- Proposed tool: `revit_import_stl`
- Headless use would require a supplied `file_path` instead of `OpenFileDialog`
- MCP descriptor: `src/StlImport/stl-import.json`

## See also

- MCP descriptor: `src/StlImport/stl-import.json`
- Property commands: `src/StlImport/StlImportPropertiesCommands.cs`
- Related: [DirectShapeFromFace/create-direct-shape-from-face.md](../DirectShapeFromFace/create-direct-shape-from-face.md)

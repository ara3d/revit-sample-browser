# Command

| Field | Value |
|-------|-------|
| **Sample** | CustomExporter/Custom2DExporter |
| **Class** | `Command` |
| **Source** | `src/CustomExporter/Custom2DExporter/CS/Command.cs` |
| **SDK ReadMe** | `src/CustomExporter/Custom2DExporter/CS/ReadMe_Custom2DExporter.rtf` |
| **MCP rating** | 5/5 |

Exports tessellated 2D geometry and text from the active printable plan, section, or elevation view using a custom `IExportContext`.

## What it demonstrates

- `CustomExporter` with `TessellatedGeomAndText2DExportContext` implementing `IExportContext2D`
- Options: `IncludeGeometricObjects`, `Export2DIncludingAnnotationObjects`, `Export2DGeometricObjectsIncludingPatternLines`
- `GetExportableViewTypes()` filtering printable non-template views
- Collecting tessellated `XYZ` points and text node summaries for display

## Prerequisites

- Active view that is printable and one of: floor/ceiling/area/engineering plan, section, elevation, or detail

## User interaction

- `Export2DView` dialog toggles annotation and pattern-line export; results shown in `TaskDialog` and optional graphics preview

## MCP notes

- Proposed tool: `revit_export_view_2d_geometry`
- Parameters: `view_id`, `include_annotations`, `include_pattern_lines`, `display_style`
- Returns: element count, text nodes, tessellated point data (or file path if serialized)
- MCP descriptor: `src/CustomExporter/Custom2DExporter/custom2dexporter.json`

## See also

- MCP descriptor: `src/CustomExporter/Custom2DExporter/custom2dexporter.json`

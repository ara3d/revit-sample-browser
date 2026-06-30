# Command

| Field | Value |
|-------|-------|
| **Sample** | CustomExporter/AdnMeshJsonExporter |
| **Class** | `Command` |
| **Source** | `src/CustomExporter/AdnMeshJsonExporter/Command.cs` |
| **MCP rating** | 5/5 |

Exports tessellated 3D mesh geometry from the active 3D view to ADN mesh JSON format using a custom `IExportContext`.

## What it demonstrates

- `Autodesk.Revit.DB.CustomExporter` with `ExportContextAdnMesh` implementing `IExportContext`
- Per-element polymesh capture: vertices, triangle indices, normals, centroid, material color, and element `UniqueId`
- `IncludeGeometricObjects = false` to export tessellated meshes without face-level callbacks
- Manual JSON serialization via `AdnMeshData.ToJson()` for WebGL viewer consumption
- Vertex and normal deduplication with `VertexLookupInt` and `NormalLookupXyz`

## Prerequisites

- Active project document with a 3D view as the active view

## User interaction

- `SaveFileDialog` prompts for output JSON path; cancel returns `Result.Cancelled`
- `TaskDialog` reports mesh count and saved file path on success

## MCP notes

- Proposed tool: `revit_export_view_3d_mesh_json`
- Parameters: `view_id` (optional, default active view), `output_path` (optional)
- Returns: mesh count and output file path
- Headless use would require replacing `SaveFileDialog` with a supplied `output_path`
- MCP descriptor: `src/CustomExporter/AdnMeshJsonExporter/adnmeshjsonexporter.json`

## See also

- MCP descriptor: `src/CustomExporter/AdnMeshJsonExporter/adnmeshjsonexporter.json`
- Related: [CustomExporter/Custom2DExporter/custom2dexporter.md](../Custom2DExporter/custom2dexporter.md) (2D export via `IExportContext2D`)
- Upstream: [jeremytammik/CustomExporterAdnMeshJson](https://github.com/jeremytammik/CustomExporterAdnMeshJson)

# SerializePredefinedPointCloud

| Field | Value |
|-------|-------|
| **Sample** | PointCloudEngine |
| **Class** | `SerializePredefinedPointCloud` |
| **Source** | `src/PointCloudEngine/CS/PointCloudEngineSample.cs` |
| **MCP rating** | 2/5 |

Utility command that serializes a dummy predefined point cloud to a fixed XML file on disk.

## What it demonstrates

- `PredefinedPointCloud.SerializeObjectData` into an `XDocument` root element
- Writing point cloud XML suitable for the file-based engine (`xml` registration)
- Read-only transaction mode while exporting data

## Prerequisites

- Write access to `c:\serializedPC.xml` (hard-coded output path)

## User interaction

- No Revit UI; writes XML silently on execution

## MCP notes

Developer utility for authoring point cloud XML, not a document-query or modeling tool. Hard-coded path limits automation value.

## See also

- Engine registration: `PointCloudTestApplication` in the same source file

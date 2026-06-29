# AddRandomizedInstanceCommand

| Field | Value |
|-------|-------|
| **Sample** | PointCloudEngine |
| **Class** | `AddRandomizedInstanceCommand` |
| **Source** | `src/PointCloudEngine/PointCloudEngineSample.cs` |
| **MCP rating** | 4/5 |

Adds a point cloud instance using the sample engine that randomizes points at cell borders (`apipcr`).

## What it demonstrates

- Using a second registered engine (`apipcr`, `PointCloudEngineType.RandomizedPoints`)
- Same `AddInstance` pipeline as the predefined command with a different engine identifier
- Identity transform placement at document origin

## Prerequisites

- `PointCloudTestApplication` registered the `apipcr` engine on startup

## User interaction

- Ribbon-triggered, no user picks or dialogs

## MCP notes

- Proposed tool: `revit_add_point_cloud_instance` with `engine_type: apipcr`
- Parameters: optional `identifier`, optional `transform` (defaults to identity)
- Returns: new point cloud instance element id
- MCP descriptor: `src/PointCloudEngine/addrandomizedinstancecommand.json`

## See also

- MCP descriptor: `src/PointCloudEngine/addrandomizedinstancecommand.json`

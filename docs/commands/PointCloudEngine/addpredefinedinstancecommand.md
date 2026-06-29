# AddPredefinedInstanceCommand

| Field | Value |
|-------|-------|
| **Sample** | PointCloudEngine |
| **Class** | `AddPredefinedInstanceCommand` |
| **Source** | `src/PointCloudEngine/CS/PointCloudEngineSample.cs` |
| **MCP rating** | 4/5 |

Adds a point cloud instance to the active document using the sample's predefined (non-randomized) custom engine.

## What it demonstrates

- Custom `IPointCloudEngine` registration via `PointCloudEngineRegistry.RegisterPointCloudEngine` (engine id `apipc`)
- `PointCloudType.Create` and `PointCloudInstance.Create` inside a transaction
- `AddInstanceCommandBase.AddInstance` as shared placement logic

## Prerequisites

- `PointCloudTestApplication` must have registered the `apipc` engine on add-in startup
- Active view document available from `commandData.View.Document`

## User interaction

- No dialog; runs immediately from the ribbon button
- Suitable for headless automation with engine id and transform parameters

## MCP notes

- Proposed tool: `revit_add_point_cloud_instance`
- Parameters: `engine_type` (e.g. `apipc`), optional `identifier`, optional `transform`
- Returns: new `PointCloudInstance` element id
- MCP descriptor: `docs/mcp/PointCloudEngine/addpredefinedinstancecommand.json`

## See also

- MCP descriptor: `docs/mcp/PointCloudEngine/addpredefinedinstancecommand.json`
- Related: `AddRandomizedInstanceCommand`, `AddTransformedInstanceCommand`

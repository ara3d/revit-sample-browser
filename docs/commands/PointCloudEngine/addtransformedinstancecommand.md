# AddTransformedInstanceCommand

| Field | Value |
|-------|-------|
| **Sample** | PointCloudEngine |
| **Class** | `AddTransformedInstanceCommand` |
| **Source** | `src/PointCloudEngine/CS/PointCloudEngineSample.cs` |
| **MCP rating** | 4/5 |

Adds a randomized point cloud instance with a non-identity transform (30° rotation about Z at a fixed point).

## What it demonstrates

- `Transform.CreateRotationAtPoint` applied when creating a `PointCloudInstance`
- Combining custom engine data (`apipcr`) with explicit placement transform
- Transaction-wrapped `PointCloudType.Create` / `PointCloudInstance.Create`

## Prerequisites

- `apipcr` engine registered by the sample application

## User interaction

- No UI; transform is hard-coded in the command

## MCP notes

- Proposed tool: `revit_add_point_cloud_instance` with required `transform` (origin, basis, or rotation parameters)
- Parameters: `engine_type`, `transform`, optional `identifier`
- Returns: new instance element id
- MCP descriptor: `docs/mcp/PointCloudEngine/addtransformedinstancecommand.json`

## See also

- MCP descriptor: `docs/mcp/PointCloudEngine/addtransformedinstancecommand.json`

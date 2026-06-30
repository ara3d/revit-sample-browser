# Command

| Field | Value |
|-------|-------|
| **Sample** | GeometryAPI/ProximityDetection_WallJoinControl |
| **Class** | `Command` |
| **Source** | `src/GeometryAPI/ProximityDetection_WallJoinControl/Command.cs` |
| **MCP rating** | 2/5 |

Detects proximate walls and demonstrates join/disjoin control through an interactive form.

## What it demonstrates

- `ProximityDetection.GetInstance` finding walls within tolerance
- `WallJoinControl` enabling or disabling joins at wall ends
- `ProximityDetectionAndWallJoinControlForm` presenting operations and results

## Prerequisites

- Project with multiple walls that can be joined or separated

## User interaction

- Modal form selects proximity/join operations; cancel returns `Result.Cancelled`

## MCP notes

- Wall join utilities could be exposed with wall ids and join end enums, but the sample is UI-centric

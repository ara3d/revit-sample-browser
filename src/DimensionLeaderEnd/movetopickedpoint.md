# MoveToPickedPoint

| Field | Value |
|-------|-------|
| **Sample** | DimensionLeaderEnd |
| **Class** | `MoveToPickedPoint` |
| **Source** | `src/DimensionLeaderEnd/Command.cs` |
| **MCP rating** | 3/5 |

Sets dimension leader end positions to a user-picked point, staggering multi-segment leaders by segment spacing.

## What it demonstrates

- `UIDocument.Selection.PickPoint` for leader placement
- Single-segment: `dim.LeaderEndPosition = point`
- Multi-segment: offset each `DimensionSegment.LeaderEndPosition` by cumulative segment origin delta

## Prerequisites

- Pre-selected linear dimensions
- Active view suitable for point picking

## User interaction

- Requires interactive point pick per dimension batch — not headless as written

## MCP notes

- Proposed tool: `revit_set_dimension_leader_point`
- Parameters: `dimension_ids[]`, `leader_point` {x,y,z}
- Returns: updated dimension ids
- MCP descriptor: `src/DimensionLeaderEnd/movetopickedpoint.json`

## See also

- MCP descriptor: `src/DimensionLeaderEnd/movetopickedpoint.json`
- Related: [movehorizontally.md](movehorizontally.md)

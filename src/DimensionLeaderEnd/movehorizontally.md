# MoveHorizontally

| Field | Value |
|-------|-------|
| **Sample** | DimensionLeaderEnd |
| **Class** | `MoveHorizontally` |
| **Source** | `src/DimensionLeaderEnd/Command.cs` |
| **MCP rating** | 3/5 |

Offsets dimension leader end positions along the dimension line direction by a fixed negative delta for selected dimensions.

## What it demonstrates

- `Dimension.LeaderEndPosition` on single-segment dimensions
- Per-segment `DimensionSegment.LeaderEndPosition` when `dim.Segments` is non-empty
- `ComputeLeaderPosition` using dimension line direction times `m_delta` (-10) added to origin

## Prerequisites

- Pre-selected linear `Dimension` elements

## User interaction

- Selection-only; shows dialog if nothing selected

## MCP notes

- Proposed tool: `revit_move_dimension_leader`
- Parameters: `dimension_ids[]`, `offset` (signed distance along dimension direction)
- Returns: updated dimension ids
- MCP descriptor: `src/DimensionLeaderEnd/movehorizontally.json`

## See also

- MCP descriptor: `src/DimensionLeaderEnd/movehorizontally.json`
- Related: [movetopickedpoint.md](movetopickedpoint.md)

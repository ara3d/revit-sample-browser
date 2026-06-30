# Command

| Field | Value |
|-------|-------|
| **Sample** | PathOfTravelDoors |
| **Class** | `Command` |
| **Source** | `src/PathOfTravelDoors/Command.cs` |
| **Origin** | [jeremytammik/PathOfTravelDoors](https://github.com/jeremytammik/PathOfTravelDoors) (MIT) |
| **MCP rating** | 4/5 |

Reports door marks crossed by selected path-of-travel elements in traversal order. The upstream GitHub repository documents the approach; this sample implements it with `PathOfTravel.GetCurves`, bidirectional `ReferenceIntersector` rays per segment, and a geometric door-opening fallback.

## What it demonstrates

- `PathOfTravel.GetCurves` to obtain path line segments
- `ReferenceIntersector` with `ElementCategoryFilter` for `OST_Doors` and `FindNearest`
- Bidirectional ray agreement from segment start and end to reduce false positives
- Geometric fallback: door opening lines from `ExporterIFCUtils.GetInstanceCutoutFromWall` or hand orientation
- Ordered, de-duplicated door list along the path

## Prerequisites

- Floor plan view containing path-of-travel element(s)
- At least one non-template 3D view for `ReferenceIntersector` evaluation
- Doors with marks (or names) in the model

## User interaction

- Uses pre-selection when path(s) of travel are already selected; otherwise prompts for picks
- Read-only command; shows a `TaskDialog` listing door marks per path
- See also `WriteDoorMarksCommand` to store marks on the path Comments parameter

## MCP notes

- Proposed tool: `revit_list_path_of_travel_doors`
- Parameters: `path_of_travel_ids` (array of integers)
- Returns: ordered door marks per path element id
- MCP descriptor: `src/PathOfTravelDoors/path-of-travel-doors.json`

## See also

- MCP descriptor: `src/PathOfTravelDoors/path-of-travel-doors.json`
- `WriteDoorMarksCommand` — writes door marks to each path Comments parameter
- `PathOfTravel` — creates path-of-travel elements from rooms to doors
- [The Building Coder: doors traversed by path of travel](https://jeremytammik.github.io/tbc/a/2030_travel_doors.html)

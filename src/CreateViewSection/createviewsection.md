# Command

| Field | Value |
|-------|-------|
| **Sample** | CreateViewSection |
| **Class** | `Command` |
| **Source** | `src/CreateViewSection/Command.cs` |
| **SDK ReadMe** | `src/CreateViewSection/ReadMe_CreateViewSection.rtf` |
| **MCP rating** | 3/5 |

Creates a detail section view centered on a selected linear wall, horizontal beam, or structural floor using a computed `BoundingBoxXYZ` transform.

## What it demonstrates

- Selection rules for `Wall` (linear), `FamilyInstance` beam, or structural `Floor`
- `BoundingBoxXYZ` with `Transform` basis vectors from element geometry (`XyzMath` helpers)
- `ViewSection.CreateDetail` with a `ViewFamilyType` of `ViewFamily.Detail`
- Floor sections use associated `AnalyticalPanel` outer contour for orientation

## Prerequisites

- Pre-select exactly one wall, horizontal beam, or structural floor

## User interaction

- Selection-driven; shows success `TaskDialog` on completion
- Box half-size (10 ft) and height (5 ft) are constants

## MCP notes

- Proposed tool: `revit_create_detail_section`
- Parameters: `element_id`, optional `box_half_size`, `box_height`, optional `detail_view_type_id`
- Returns: new `ViewSection` element id
- MCP descriptor: `src/CreateViewSection/createviewsection.json`

## See also

- MCP descriptor: `src/CreateViewSection/createviewsection.json`
- Related: [createdraftingview.md](createdraftingview.md)

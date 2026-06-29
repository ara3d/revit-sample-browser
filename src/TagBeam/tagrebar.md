# TagRebar

| Field | Value |
|-------|-------|
| **Sample** | TagBeam |
| **Class** | `TagRebar` |
| **Source** | `src/TagBeam/CS/Command.cs` |
| **SDK ReadMe** | `src/TagBeam/CS/ReadMe_TagBeam.rtf` |
| **MCP rating** | 4/5 |

Creates a category tag on the first subelement of a selected rebar at the start of its centerline.

## What it demonstrates

- `Rebar.GetSubelements` and tagging via subelement `Reference`
- `IndependentTag.Create` with `TagMode.TM_ADDBY_CATEGORY` and horizontal orientation
- Selection loop over `Rebar` elements in the active view

## Prerequisites

- Active view and a selected `Rebar` element

## User interaction

- Selection-only; no dialog

## MCP notes

- Proposed tool: `revit_tag_rebar`
- Parameters: `rebar_id`, optional `tag_type_id`, `has_leader`, `orientation`
- Returns: new tag element id
- MCP descriptor: `src/TagBeam/tagrebar.json`

## See also

- MCP descriptor: `src/TagBeam/tagrebar.json`

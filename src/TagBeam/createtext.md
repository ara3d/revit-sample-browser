# CreateText

| Field | Value |
|-------|-------|
| **Sample** | TagBeam |
| **Class** | `CreateText` |
| **Source** | `src/TagBeam/Command.cs` |
| **SDK ReadMe** | `src/TagBeam/ReadMe_TagBeam.rtf` |
| **MCP rating** | 4/5 |

Creates a text note beside the first end of a selected rebar's centerline, labeling its category and name.

## What it demonstrates

- `Rebar.GetCenterlineCurves` with `MultiplanarOption.IncludeAllMultiplanarCurves`
- `TextNote.Create` with default `TextNoteType` from `GetDefaultElementTypeId`
- Offsetting text origin from the rebar endpoint along the curve length

## Prerequisites

- Active view and a selected `Rebar` element

## User interaction

- Selection-only; no dialog

## MCP notes

- Proposed tool: `revit_create_rebar_text_note`
- Parameters: `rebar_id`, optional `text`, `offset_xyz`
- Returns: new text note element id
- MCP descriptor: `src/TagBeam/createtext.json`

## See also

- MCP descriptor: `src/TagBeam/createtext.json`

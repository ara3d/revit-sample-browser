# AttachedDetailGroupHideAllCommand

| Field | Value |
|-------|-------|
| **Sample** | AttachedDetailGroup |
| **Class** | `AttachedDetailGroupHideAllCommand` |
| **Source** | `src/AttachedDetailGroup/CS/AttachedDetailGroupHideAllCommand.cs` |
| **SDK ReadMe** | `src/AttachedDetailGroup/CS/Readme_AttachedDetailGroup.rtf` |
| **MCP rating** | 4/5 |

Hides all attached detail groups on the selected model group in the active view.

## What it demonstrates

- Validating selection is a model `Group` via `GroupHelper.GetSelectedModelGroup`
- Calling `Group.HideAllAttachedDetailGroups(view)` inside a transaction

## Prerequisites

- Active view; user must select exactly one model group with attached detail groups

## User interaction

- Selection-based only; no dialog
- Straightforward to parameterize with `group_id` and `view_id`

## MCP notes

- Proposed tool: `revit_hide_attached_detail_groups`
- Parameters: `group_id`, optional `view_id` (defaults to active view)
- Returns: success flag and group id
- MCP descriptor: `docs/mcp/AttachedDetailGroup/attacheddetailgrouphideallcommand.json`

## See also

- [`attacheddetailgroupshowallcommand.md`](attacheddetailgroupshowallcommand.md)

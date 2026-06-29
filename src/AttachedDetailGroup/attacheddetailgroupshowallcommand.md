# AttachedDetailGroupShowAllCommand

| Field | Value |
|-------|-------|
| **Sample** | AttachedDetailGroup |
| **Class** | `AttachedDetailGroupShowAllCommand` |
| **Source** | `src/AttachedDetailGroup/CS/AttachedDetailGroupShowAllCommand.cs` |
| **SDK ReadMe** | `src/AttachedDetailGroup/CS/Readme_AttachedDetailGroup.rtf` |
| **MCP rating** | 4/5 |

Shows all attached detail groups on the selected model group in the active view.

## What it demonstrates

- `Group.ShowAllAttachedDetailGroups(view)` within a named transaction
- Shared `GroupHelper` selection validation with the hide command

## Prerequisites

- Active view; one model group selected

## User interaction

- Selection-based; no modal UI

## MCP notes

- Proposed tool: `revit_show_attached_detail_groups`
- Parameters: `group_id`, optional `view_id`
- MCP descriptor: `src/AttachedDetailGroup/attacheddetailgroupshowallcommand.json`

## See also

- [`attacheddetailgrouphideallcommand.md`](attacheddetailgrouphideallcommand.md)

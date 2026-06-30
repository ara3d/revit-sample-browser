# CmdUnhostedElements

| Field | Value |
|-------|-------|
| **Sample** | AdnRme |
| **Class** | `CmdUnhostedElements` |
| **Source** | `src/AdnRme/CmdUnhostedElements.cs` |
| **MCP rating** | 3/5 |

Lists MEP family instances whose host parameter indicates they are not associated with a valid host.

## What it demonstrates

- Filtering `FamilyInstance` elements by category
- Reading `INSTANCE_FREE_HOST_PARAM` to detect unhosted placement
- Debug output of element descriptions and ids

## Prerequisites

- Any project document with MEP family instances

## User interaction

- Read-only; no dialog. Results appear in debug/trace output.

## MCP notes

- Proposed tool: `revit_list_unhosted_mep_elements`
- Could return `{ element_id, category, description }[]` instead of debug output
- MCP descriptor: `src/AdnRme/cmdunhostedelements.json`

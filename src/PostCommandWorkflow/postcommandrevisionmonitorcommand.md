# PostCommandRevisionMonitorCommand

| Field | Value |
|-------|-------|
| **Sample** | PostCommandWorkflow |
| **Class** | `PostCommandRevisionMonitorCommand` |
| **Source** | `src/PostCommandWorkflow/PostCommandRevisionMonitorCommand.cs` |
| **MCP rating** | 1/5 |

Toggles a post-command revision monitor on or off for the active document and updates the ribbon button label.

## What it demonstrates

- Static `PostCommandRevisionMonitor` lifecycle (`Activate` / `Deactivate`)
- `SetPushButton` coordination so the command text flips between setup and remove states
- Post-command workflow hooks for revision-related automation

## Prerequisites

- Sample `Application` wires the push button via `SetPushButton`

## User interaction

- Single ribbon click toggles monitoring; no property dialog

## MCP notes

Infrastructure sample for post-command events, not a document query or edit tool suitable for MCP exposure.

## See also

- Monitor implementation: `src/PostCommandWorkflow/PostCommandRevisionMonitor.cs`

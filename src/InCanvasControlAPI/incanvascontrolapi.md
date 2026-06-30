# Command

| Field | Value |
|-------|-------|
| **Sample** | InCanvasControlAPI |
| **Class** | `Command` |
| **Source** | `src/InCanvasControlAPI/Command.cs` |
| **MCP rating** | 1/5 |

Places an in-canvas issue marker control on a user-picked element and registers it for tracking.

## What it demonstrates

- `IssueMarker.Create` attached to an element id
- `IssueMarkerTrackingManager` subscribing markers per document
- `PickObject` selection with duplicate-marker guard

## Prerequisites

- Element eligible for marker attachment; no existing marker on the same element

## User interaction

- Requires one interactive element pick; marker rendering is viewport-specific UI

## MCP notes

- In-canvas UI is inherently interactive; poor fit for headless MCP automation

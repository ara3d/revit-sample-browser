# Command

| Field | Value |
|-------|-------|
| **Sample** | InPlaceMembers |
| **Class** | `Command` |
| **Source** | `src/InPlaceMembers/Command.cs` |
| **SDK ReadMe** | — |
| **MCP rating** | 2/5 |

Reads properties and analytical model graphics from a selected in-place structural family instance and displays them in a form.

## What it demonstrates

- Selecting one in-place `FamilyInstance` with an `AnalyticalElement` model
- `GraphicsDataFactory.CreateGraphicsData` for 2D profile rendering on a `PictureBox`
- `Properties` wrapper exposing instance parameters to a WinForms viewer

## Prerequisites

- Exactly one in-place member selected with a valid analytical model

## User interaction

- Requires selection; shows `InPlaceMembersForm` with read-only property and graphics panes

## MCP notes

- Property extraction could return JSON, but the sample is built around a visualization form

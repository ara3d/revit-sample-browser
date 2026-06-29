# UiCommand

| Field | Value |
|-------|-------|
| **Sample** | PerformanceAdviserControl |
| **Class** | `UiCommand` |
| **Source** | `src/PerformanceAdviserControl/UICommand.cs` |
| **SDK ReadMe** | `src/PerformanceAdviserControl/ReadMe_PerformanceAdviserControl.rtf` |
| **MCP rating** | 1/5 |

Opens a dialog that lists all Performance Adviser rules and lets the user choose which to run against the active document.

## What it demonstrates

- Querying `PerformanceAdviser.GetPerformanceAdviser()` for registered rule IDs
- Reading rule metadata with `GetRuleName`, `GetRuleDescription`, and `IsRuleEnabled`
- Distinguishing built-in rules from a custom API rule (`FlippedDoorCheck`)
- Driving `TestDisplayDialog` to execute selected rules interactively

## Prerequisites

- The sample's `Application` registers `FlippedDoorCheck` as a custom `IPerformanceAdviserRule` on startup
- Any project document where Performance Adviser rules apply

## User interaction

- Modal `TestDisplayDialog` is required; the command only collects rule metadata and shows the UI
- Rule execution and result display happen inside the dialog, not in `UiCommand.Execute`

## MCP notes

Poor automation candidate: the value is in interactive rule selection and Performance Adviser UI integration, not parameterized document changes.

## See also

- Custom rule implementation: `src/PerformanceAdviserControl/FlippedDoorCheck.cs`

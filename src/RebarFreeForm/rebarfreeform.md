# Command

| Field | Value |
|-------|-------|
| **Sample** | RebarFreeForm |
| **Class** | `Command` |
| **Source** | `src/RebarFreeForm/CS/Command.cs` |
| **MCP rating** | 2/5 |

Creates a free-form rebar hosted on a picked structural element, constrained by user-selected faces and optional model-curve data.

## What it demonstrates

- `Rebar.CreateFreeForm` with `RebarUpdateServer.SampleGuid` external server
- `RebarConstraintsManager` handle iteration and `RebarConstraint.Create` from picked faces
- Linking shared parameters **Updated** and **CurveElementId** via `FreeFormAccessor.AddUpdatingSharedParameter`
- Interactive host, curve, and per-handle face picks

## Prerequisites

- At least one `RebarBarType` in the document
- Run `AddSharedParams` first to bind regeneration dependency parameters
- Structural host element and optional model curve

## User interaction

- Multiple `Selection.PickObject` calls for host, optional curve, and constraint faces
- Not headless without supplying references and handle constraints programmatically

## MCP notes

Free-form rebar with custom update servers is highly interactive and server-dependent; poor generic MCP candidate.

## See also

- Setup command: [addsharedparams.md](addsharedparams.md)

# Command

| Field | Value |
|-------|-------|
| **Sample** | NewHostedSweep |
| **Class** | `Command` |
| **Source** | `src/NewHostedSweep/Command.cs` |
| **MCP rating** | 2/5 |

Launches a dialog-driven workflow to create or modify hosted sweeps (fascia, gutter, slab edge) on picked roof or slab edges.

## What it demonstrates

- `CreationMgr` coordinating `FasciaCreator`, `GutterCreator`, and `SlabEdgeCreator`
- Hosted sweep creation data (`CreationData`, `ModificationData`) and edge-fetch UI
- Interactive edge selection and type assignment through `MainForm`

## Prerequisites

- Project with roofs or floors exposing sweepable edges and loaded hosted sweep types

## User interaction

- Multi-step WinForms UI (`MainForm`, `EdgeFetchForm`, modify dialogs); not headless

## MCP notes

- Complex hosted-sweep UI sample; automation would require edge references and sweep type ids instead of interactive edge picking.

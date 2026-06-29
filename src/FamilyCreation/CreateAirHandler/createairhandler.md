# Command

| Field | Value |
|-------|-------|
| **Sample** | FamilyCreation/CreateAirHandler |
| **Class** | `Command` |
| **Source** | `src/FamilyCreation/CreateAirHandler/Command.cs` |
| **SDK ReadMe** | `src/FamilyCreation/CreateAirHandler/ReadMe_CreateAirHandler.rtf` |
| **MCP rating** | 2/5 |

Builds a mechanical-equipment family air handler from five extrusions, then adds duct and pipe connectors and combines the solids.

## What it demonstrates

- `FamilyItemFactory.NewExtrusion` with rectangular and arc profiles, offsets, and solid/void flags
- `ConnectorElement.CreateDuctConnector` / `CreatePipeConnector` on planar face references
- `Document.CombineElements` and connector flow/dimension parameters via `BuiltInParameter`

## Prerequisites

- Mechanical Equipment family template open in the family editor

## User interaction

- Runs immediately with hard-coded geometry; no picks or dialogs

## MCP notes

- Illustrates connector placement patterns but geometry is fixed; not a reusable parameterized MCP tool without redesign

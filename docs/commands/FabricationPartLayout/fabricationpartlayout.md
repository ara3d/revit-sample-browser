# FabricationPartLayout

| Field | Value |
|-------|-------|
| **Sample** | FabricationPartLayout |
| **Class** | `FabricationPartLayout` |
| **Source** | `src/FabricationPartLayout/CS/FabricationPartLayout.cs` |
| **SDK ReadMe** | `src/FabricationPartLayout/CS/Readme_FabricationPartLayout.rtf` |
| **MCP rating** | 2/5 |

Builds a large demo HVAC and plumbing fabrication layout from an AHU connector, exercising creation, connection, taps, hangers, materials, and pipe insertion.

## What it demonstrates

- End-to-end `FabricationPart.Create`, `CreateHanger`, `PlaceAsTap`, `AlignPartByConnectors`, `ConnectAndCouple`
- Setting dimensions via `SetDimensionValue`, product entries, materials, insulation specs, and connector overrides
- `AlignPartByInsertionPoint`, `AlignPartByInsertionPointAndCutInToStraight`, and sloped pipe connections

## Prerequisites

- Sample model: **Level 1**, single AHU family with 40×40 in rectangular unused connector, loaded fabrication services (HVAC and Plumbing)

## User interaction

- Runs automatically with no picks; fails if model preconditions are not met

## MCP notes

- Comprehensive API tour but tightly coupled to the SDK sample model; not a parameterized layout tool without major refactoring

# SplitStraight

| Field | Value |
|-------|-------|
| **Sample** | FabricationPartLayout |
| **Class** | `SplitStraight` |
| **Source** | `src/FabricationPartLayout/SplitStraight.cs` |
| **MCP rating** | 2/5 |

Splits a selected fabrication straight duct or pipe at its midpoint into two connected parts.

## What it demonstrates

- Picking a `FabricationPart` and validating `IsAStraight()`
- Reading end connectors from `ConnectorManager` and computing a split point
- Pre-checking with `CanSplitStraight` before calling `SplitStraight` inside a transaction

## Prerequisites

- Active project with fabrication parts; the picked element must be a straight with exactly two end connectors

## User interaction

- Requires one interactive pick via `PickObject`; split location is always the connector midpoint, not user-specified

## MCP notes

- Could be parameterized with an element id and XYZ split point, but the sample is pick-driven and MEP-fabrication specific

# Command

| Field | Value |
|-------|-------|
| **Sample** | GenericStructuralConnection |
| **Class** | `Command` |
| **Source** | `src/GenericStructuralConnection/Command.cs` |
| **MCP rating** | 2/5 |

Demonstrates creating, reading, updating, deleting, and managing detailed structural connections via a menu-driven dialog.

## What it demonstrates

- `StructuralConnectionForm` dispatching to `GenericStructuralConnectionOps` and `DetailedStructuralConnectionOps`
- CRUD workflows for generic connections plus copy, match properties, and reset for detailed connections
- Selection-dependent operations inside the ops classes (invoked after UI choice)

## Prerequisites

- Structural model with connectable framing elements; operations vary by chosen menu item

## User interaction

- Modal form selects the operation; many sub-operations require further picks in the ops layer

## MCP notes

- Connection APIs are useful but buried in multi-step UI; would need per-operation MCP tools with element id parameters

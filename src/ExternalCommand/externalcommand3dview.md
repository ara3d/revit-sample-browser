# ExternalCommand3DView

| Field | Value |
|-------|-------|
| **Sample** | ExternalCommand |
| **Class** | `ExternalCommand3DView` |
| **Source** | `src/ExternalCommand/CS/ExternalCommandRegistration/ExternalCommandClass.cs` |
| **MCP rating** | 1/5 |

Shows a hello message when invoked from a 3D view context, demonstrating external command registration and availability classes.

## What it demonstrates

- Minimal `IExternalCommand` with read-only transaction mode
- Paired with `AvailabilityClass` (separate file) for view-type-specific ribbon visibility

## User interaction

- Single `TaskDialog`; no document changes

## MCP notes

- Registration/availability demo only; no document operations to expose via MCP

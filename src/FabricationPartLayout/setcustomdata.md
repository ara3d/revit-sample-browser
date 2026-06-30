# SetCustomData

| Field | Value |
|-------|-------|
| **Sample** | FabricationPartLayout |
| **Class** | `SetCustomData` |
| **Source** | `src/FabricationPartLayout/CustomData.cs` |
| **MCP rating** | 2/5 |

Writes sample custom data values (install timestamp, hours, cost) to a picked fabrication part and reports before/after values.

## What it demonstrates

- `SetPartCustomDataText`, `SetPartCustomDataInteger`, and `SetPartCustomDataReal`
- Shared `CustomDataHelper.ReportCustomData` with read vs. write modes inside a transaction

## User interaction

- Single part pick; random demo values for numeric fields

## MCP notes

- MCP would need explicit custom data id/value pairs instead of random demo assignments

# CmdPopulateCfmPerSf

| Field | Value |
|-------|-------|
| **Sample** | AdnRme |
| **Class** | `CmdPopulateCfmPerSf` |
| **Source** | `src/AdnRme/CmdPopulateCfmPerSf.cs` |
| **MCP rating** | 3/5 |

Populates the CFM-per-SF parameter on every MEP space from actual supply airflow and area.

## What it demonstrates

- Reading space built-in parameters (`ROOM_ACTUAL_SUPPLY_AIRFLOW_PARAM`, `ROOM_AREA`)
- Writing a named instance parameter (`CFM per SF`) via `Util.GetSpaceParameter`
- Batch space iteration with `Util.GetSpaces`

## Prerequisites

- MEP project with placed spaces that have supply airflow and area values

## User interaction

- No dialog; runs on all spaces with a progress form

## MCP notes

- Proposed tool: `revit_populate_cfm_per_sf`
- No arguments required; returns count of spaces updated
- MCP descriptor: `src/AdnRme/cmdpopulatecfmpersf.json`

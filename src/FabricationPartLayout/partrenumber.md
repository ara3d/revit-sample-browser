# PartRenumber

| Field | Value |
|-------|-------|
| **Sample** | FabricationPartLayout |
| **Class** | `PartRenumber` |
| **Source** | `src/FabricationPartLayout/PartRenumber.cs` |
| **MCP rating** | 2/5 |

Clears and reassigns item numbers on selected fabrication parts, grouping identical parts via `IsSameAs`.

## What it demonstrates

- Category-based numbering prefixes (duct, pipe, hanger, misc) and coupling detection by CID
- `FabricationPartCompareType` ignore list for notes, order number, and service when comparing parts

## Prerequisites

- Selected fabrication parts

## User interaction

- Uses current selection; shows success dialog

## MCP notes

- Logic is reusable with `element_ids[]`; numbering rules are sample-specific strings

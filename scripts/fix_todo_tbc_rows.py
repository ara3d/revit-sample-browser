#!/usr/bin/env python3
"""Remove stale/duplicate TBC rows from todo.md and refresh progress counts."""

from __future__ import annotations

import re
from pathlib import Path

REPO = Path(__file__).resolve().parent.parent
TODO_PATH = REPO / "todo.md"

# Planned import names that were never created or duplicate an existing on-disk sample.
REMOVE_SOURCES = {
    "TBC_AlignTwoViews/CmdViewsShowingElements.cs",
    "TBC_BrokenCommand/CmdDeleteUnusedRefPlanes.cs",
    "TBC_ChangeFloorSlope/CmdCreateSlopedSlab.cs",
    "TBC_CmdDimensionInstanceOrigin_Command/CmdDimensionInstanceOrigin.cs",
    "TBC_CollectorPerformance_RevitCommand/CmdCollectorPerformance.cs",
    "TBC_Commands/CmdCollectorPerformance.cs",
    "TBC_CreateFillledRegionDimensionsCommand/CmdDimensionWallsIterateFaces.cs",
    "TBC_DeleteUnusedRefPlanes/CmdDeleteUnusedRefPlanes.cs",
    "TBC_DeleteUnusedRefPlanes_2014/CmdDeleteUnusedRefPlanes.cs",
    "TBC_ElevationWatcherUpdater/CmdElevationWatcher.cs",
    "TBC_GetColumnGeometry1/CmdColumnRound.cs",
    "TBC_MiroTest2/CmdSheetToModel.cs",
    "TBC_MirrorListAdded/CmdMirror.cs",
    "TBC_ParamFilterTest/CmdCollectorPerformance.cs",
    "TBC_RetrievingMaterialCommand/CmdGetMaterials.cs",
    "TBC_RevitCommand/CmdCollectorPerformance.cs",
    "TBC_SimpleUpdaterExample/CmdElevationWatcher.cs",
    "TBC_TestWall/CmdSlopedWall.cs",
}

ROW = re.compile(
    r"^\| \[[ x~]\] \| [^\|]* \| `[^`]+` \| \[[^\]]+\]\([^)]+\) \| [^\|]* \| `([^`]+)` \|"
)


def main() -> None:
    text = TODO_PATH.read_text(encoding="utf-8", errors="replace")
    lines = text.splitlines(keepends=True)
    out: list[str] = []
    removed = 0
    for line in lines:
        m = ROW.match(line.rstrip("\n"))
        if m and m.group(1) in REMOVE_SOURCES:
            removed += 1
            continue
        out.append(line)

    new_text = "".join(out)
    new_text = re.sub(
        r"Imported \d+ commands from The Building Coder\.",
        f"Imported {170 - removed} commands from The Building Coder.",
        new_text,
        count=1,
    )

    doc_rows = len(re.findall(r"^\| \[[ x~]\] \|", new_text, re.MULTILINE))
    mcp_rows = len(
        re.findall(
            r"^\| \[[ x~]\] \| \[[ x~]\] \|",
            new_text,
            re.MULTILINE,
        )
    )
    new_text = re.sub(
        r"\| Total commands \| \d+ \|",
        f"| Total commands | {doc_rows} |",
        new_text,
        count=1,
    )
    new_text = re.sub(
        r"\| Command docs created \| \d+ / \d+ \|",
        f"| Command docs created | {doc_rows} / {doc_rows} |",
        new_text,
        count=1,
    )
    new_text = re.sub(
        r"\| MCP descriptors created \| \d+ / \d+ \|",
        f"| MCP descriptors created | {mcp_rows} / {mcp_rows} |",
        new_text,
        count=1,
    )

    TODO_PATH.write_text(new_text, encoding="utf-8")
    print(f"Removed {removed} stale TBC rows")
    print(f"Inventory rows: {doc_rows}, MCP JSON rows: {mcp_rows}")


if __name__ == "__main__":
    main()

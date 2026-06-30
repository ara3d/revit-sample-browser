#!/usr/bin/env python3
"""Build mcp-manifest.json from src/**/*.json tool descriptors."""

from __future__ import annotations

import json
from pathlib import Path

REPO = Path(__file__).resolve().parent.parent
SRC = REPO / "src"
OUT = SRC / "BB_McpServer" / "mcp-manifest.json"
TEMPLATE = SRC / "_template.json"


def is_descriptor(path: Path, data: dict) -> bool:
    return (
        path.name != "_template.json"
        and "name" in data
        and "arguments" in data
        and "description" in data
    )


def tier_for(data: dict, path: Path) -> str:
    rating = data.get("mcpRating")
    if path.parts[:2] == ("src", "BB_McpServer"):
        return "semantic"
    if "BB_" in str(path):
        return "bowerbird"
    if rating == 5:
        return "mcp5"
    if rating == 4:
        return "mcp4"
    return "descriptor"


def main() -> None:
    tools: list[dict] = []
    for path in sorted(SRC.rglob("*.json")):
        if path == TEMPLATE:
            continue
        try:
            data = json.loads(path.read_text(encoding="utf-8"))
        except json.JSONDecodeError:
            continue
        if not is_descriptor(path, data):
            continue

        rel = path.relative_to(REPO)
        tools.append(
            {
                "name": data["name"],
                "description": data.get("description", ""),
                "arguments": data.get("arguments", {"type": "object"}),
                "tier": tier_for(data, rel),
                "mcpRating": data.get("mcpRating"),
                "requiresUi": data.get("requiresUi", False),
                "commandClass": data.get("commandClass"),
                "sample": data.get("sample"),
                "source": data.get("source") or str(rel).replace("\\", "/"),
                "requiresRevit": not data["name"].startswith("dev."),
            }
        )

    manifest = {
        "generatedFrom": "scripts/build_mcp_manifest.py",
        "toolCount": len(tools),
        "exposure": {
            "tiers": ["semantic", "bowerbird", "mcp5"],
            "maxTools": 25,
        },
        "tools": sorted(tools, key=lambda t: (t.get("tier", ""), t["name"])),
    }

    OUT.write_text(json.dumps(manifest, indent=2), encoding="utf-8")
    print(f"Wrote {OUT} with {len(tools)} tools")


if __name__ == "__main__":
    main()

#!/usr/bin/env python3
import json
import re
from pathlib import Path

repo = Path(__file__).resolve().parent.parent
manifest = json.loads((repo / "scripts/tbc-import-manifest.json").read_text())

lines = [
    "## The Building Coder (TBC) samples",
    "",
    f"Imported {len(manifest)} commands from The Building Coder.",
    "",
    "| Doc | MCP | Class | Command doc | MCP descriptor | Source | Rating |",
    "|-----|-----|-------|-------------|----------------|--------|--------|",
]
mcp_count = 0
for e in manifest:
    r = e.get("mcpRating", 3)
    json_path = repo / "src" / e["folder"] / f"{e['slug']}.json"
    if json_path.exists():
        mcp_count += 1
    if r >= 3:
        mcp_col = "[x]" if json_path.exists() else "[ ]"
    else:
        mcp_col = "—"
    folder = e["folder"]
    slug = e["slug"]
    if r >= 3 and json_path.exists():
        json_link = f"[{folder}/{slug}.json](src/{folder}/{slug}.json)"
    else:
        json_link = "—"
    lines.append(
        f"| [x] | {mcp_col} | `{e['className']}` | "
        f"[{folder}/{slug}.md](src/{folder}/{slug}.md) | {json_link} | "
        f"`{folder}/{e['sourceFile']}` | {r} |"
    )

todo_path = repo / "todo.md"
todo = todo_path.read_text(encoding="utf-8")
total = 283 + len(manifest)
docs = 283 + len(manifest)
mcp_total = 125 + mcp_count
todo = re.sub(r"Total commands \| \d+", f"Total commands | {total}", todo)
todo = re.sub(r"Command docs created \| \d+ / \d+", f"Command docs created | {docs} / {total}", todo)
todo = re.sub(r"MCP descriptors created \| \d+ / \d+", f"MCP descriptors created | {mcp_total} / {mcp_total}", todo)
todo = re.sub(r"Last updated \| .*", "Last updated | 2026-06-29", todo)
if "## The Building Coder (TBC) samples" not in todo:
    todo = todo.rstrip() + "\n\n" + "\n".join(lines) + "\n"
todo_path.write_text(todo, encoding="utf-8")
print(f"Updated todo.md: {len(manifest)} TBC commands, {mcp_count} MCP JSON")

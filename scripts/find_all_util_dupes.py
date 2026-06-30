#!/usr/bin/env python3
"""Find duplicate method bodies across all Util.cs and _utils.cs files."""
from __future__ import annotations

import hashlib
import re
from collections import defaultdict
from pathlib import Path

SRC = Path(__file__).resolve().parents[1] / "src"

METHOD_START = re.compile(
    r"^\s*(?:public|private|internal|protected)\s+(?:static\s+)?[\w<>,\[\]\s]+\s+(\w+)\s*\(",
    re.MULTILINE,
)


def extract_methods(text: str) -> list[tuple[str, str, int]]:
    results = []
    lines = text.splitlines()
    i = 0
    while i < len(lines):
        m = METHOD_START.match(lines[i])
        if not m:
            i += 1
            continue
        name = m.group(1)
        if name in ("if", "for", "while", "switch", "catch", "using"):
            i += 1
            continue
        start = i
        brace_depth = 0
        started = False
        body_lines = []
        while i < len(lines):
            line = lines[i]
            body_lines.append(line)
            brace_depth += line.count("{") - line.count("}")
            if "{" in line:
                started = True
            if started and brace_depth == 0:
                i += 1
                break
            i += 1
        body = "\n".join(body_lines)
        results.append((name, body, start + 1))
    return results


def normalize(body: str) -> str:
    body = re.sub(r"//.*", "", body)
    body = re.sub(r"/\*.*?\*/", "", body, flags=re.DOTALL)
    body = re.sub(r"\s+", "", body)
    return body


def main() -> None:
    util_files = sorted(SRC.rglob("Util.cs"))
    util_files += sorted(SRC.rglob("_utils.cs"))
    util_files = sorted(set(util_files))

    clusters: dict[str, list[tuple[str, str, int]]] = defaultdict(list)
    name_map: dict[str, list[tuple[str, str, int]]] = defaultdict(list)

    for util in util_files:
        rel = str(util.relative_to(SRC))
        text = util.read_text(encoding="utf-8", errors="replace")
        for name, body, line in extract_methods(text):
            key = hashlib.md5(normalize(body).encode()).hexdigest()
            clusters[key].append((rel, name, line))
            name_map[name].append((rel, line))

    dup_clusters = [(k, v) for k, v in clusters.items() if len(v) >= 2]
    dup_clusters.sort(key=lambda x: -len(x[1]))

    print(f"Scanned {len(util_files)} util files")
    print(f"Exact duplicate body groups: {len(dup_clusters)}")
    excess = sum(len(v) - 1 for _, v in dup_clusters)
    print(f"Excess duplicate copies: {excess}\n")

    print("=== EXACT DUPLICATE BODIES (2+ locations) ===\n")
    for i, (_, spans) in enumerate(dup_clusters[:50], 1):
        names = sorted({s[1] for s in spans})
        print(f"Cluster {i} ({len(spans)} copies, names: {names})")
        for path, name, line in spans:
            print(f"  {path}:{line} {name}")
        print()

    print("\n=== SAME METHOD NAME IN 3+ UTIL FILES ===\n")
    for name, spans in sorted(name_map.items(), key=lambda x: (-len(x[1]), x[0])):
        folders = {s[0].split("\\")[0].split("/")[0] for s in spans}
        if len(folders) < 3:
            continue
        print(f"{name} ({len(spans)} defs in {len(folders)} folders)")
        for path, line in spans[:8]:
            print(f"  {path}:{line}")
        if len(spans) > 8:
            print(f"  ... and {len(spans) - 8} more")
        print()


if __name__ == "__main__":
    main()

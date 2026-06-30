#!/usr/bin/env python3
"""Find duplicate method bodies across TBC_*/Util.cs partial classes."""
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
    """Return (name, body, start_line) for each method-like block."""
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


def main():
    clusters: dict[str, list[tuple[str, str, int]]] = defaultdict(list)
    name_map: dict[str, list[tuple[str, str, int]]] = defaultdict(list)

    for util in sorted(SRC.glob("TBC_*/Util.cs")):
        if util.parent.name == "TBC_Shared":
            continue
        text = util.read_text(encoding="utf-8", errors="replace")
        for name, body, line in extract_methods(text):
            key = hashlib.md5(normalize(body).encode()).hexdigest()
            clusters[key].append((str(util.relative_to(SRC)), name, line))
            name_map[name].append((str(util.relative_to(SRC)), line))

    print("=== EXACT DUPLICATE BODIES (2+ locations) ===\n")
    count = 0
    for key, spans in sorted(clusters.items(), key=lambda x: -len(x[1])):
        if len(spans) < 2:
            continue
        count += 1
        names = {s[1] for s in spans}
        print(f"Cluster {count} ({len(spans)} copies, names: {sorted(names)})")
        for path, name, line in spans:
            print(f"  {path}:{line} {name}")
        print()

    print("\n=== SAME METHOD NAME IN MULTIPLE UTIL FILES ===\n")
    for name, spans in sorted(name_map.items(), key=lambda x: (-len(x[1]), x[0])):
        folders = {s[0].split("\\")[0] for s in spans}
        if len(folders) < 2:
            continue
        print(f"{name} ({len(spans)} defs in {len(folders)} folders)")
        for path, line in spans:
            print(f"  {path}:{line}")
        print()


if __name__ == "__main__":
    main()

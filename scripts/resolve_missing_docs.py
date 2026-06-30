#!/usr/bin/env python3
"""Resolve todo.md doc paths to actual on-disk markdown files."""

from __future__ import annotations

import re
from pathlib import Path

REPO = Path(__file__).resolve().parent.parent
SRC = REPO / "src"
TODO = (REPO / "todo.md").read_text(encoding="utf-8", errors="replace")

ROW = re.compile(
    r"\| \[[ x~]\] \| [^\|]* \| `([^`]+)` \| \[([^\]]+)\]\((src/[^)]+\.md)\) \| [^\|]* \| `([^`]+)`"
)


def main() -> None:
    missing = []
    wrong_path = []
    for m in ROW.finditer(TODO):
        cls, _label, doc_path, source = m.group(1), m.group(2), m.group(3), m.group(4)
        doc = REPO / doc_path
        src = SRC / source.replace("/", "\\") if "\\" not in source else SRC / source
        if not doc.exists():
            missing.append((cls, doc_path, source))
            # find source on disk
            alt = list(SRC.rglob(Path(source).name))
            if alt:
                folder = alt[0].parent
                mds = list(folder.glob("*.md"))
                wrong_path.append((doc_path, mds[0].relative_to(REPO).as_posix() if mds else None, source))

    print(f"Missing docs: {len(missing)}")
    for item in missing:
        print(f"  class={item[0]}")
        print(f"    expected: {item[1]}")
        print(f"    source:   {item[2]}")

    print("\nAlternate locations:")
    for expected, actual, source in wrong_path:
        print(f"  {expected}")
        print(f"    -> {actual}  (source {source})")


if __name__ == "__main__":
    main()

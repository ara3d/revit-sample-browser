#!/usr/bin/env python3
"""Audit command markdown docs and RTF documentation files."""

from __future__ import annotations

import re
from pathlib import Path

REPO = Path(__file__).resolve().parent.parent
SRC = REPO / "src"
TODO = (REPO / "todo.md").read_text(encoding="utf-8", errors="replace")

ROW = re.compile(
    r"\| \[[ x~]\] \| [^\|]* \| `([^`]+)` \| \[([^\]]+)\]\((src/[^)]+\.md)\)"
)

IEC = re.compile(
    r"class\s+(\w+)\s*(?::\s*|\r?\n\s*:\s*)IExternalCommand\b"
)

SKIP_CLASSES = {"SampleBrowserCommand"}


def main() -> None:
    expected: dict[str, str] = {}
    for m in ROW.finditer(TODO):
        cls, _label, path = m.group(1), m.group(2), m.group(3)
        expected[path] = cls

    missing = sorted(p for p in expected if not (REPO / p).exists())
    print(f"Expected docs from todo: {len(expected)}")
    print(f"Missing on disk: {len(missing)}")
    for p in missing:
        print(f"  {p}")

    readme_rtfs = sorted(
        f
        for f in SRC.rglob("*.rtf")
        if re.search(r"ReadMe_|Readme_|help_text", f.name, re.I)
    )
    print(f"\nReadMe RTF files: {len(readme_rtfs)}")

    doc_txts = [SRC / "help.txt", SRC / "StringSearch" / "help_text.rtf"]
    doc_txts = [p for p in doc_txts if p.exists()]
    print(f"Other doc TXT/RTF: {[str(p.relative_to(REPO)) for p in doc_txts]}")


if __name__ == "__main__":
    main()

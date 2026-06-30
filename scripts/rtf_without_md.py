#!/usr/bin/env python3
"""Find command folders with RTF readme but no markdown doc."""

from __future__ import annotations

import re
from pathlib import Path

SRC = Path(__file__).resolve().parent.parent / "src"

IEC = re.compile(r"class\s+(\w+)\s*(?::\s*|\r?\n\s*:\s*)IExternalCommand\b")


def main() -> None:
    issues = []
    for cs in sorted(SRC.rglob("*.cs")):
        if "TBC_Shared" in str(cs) or "obj" in cs.parts:
            continue
        if not IEC.search(cs.read_text(encoding="utf-8", errors="replace")):
            continue
        folder = cs.parent
        rtfs = [f for f in folder.glob("*.rtf") if re.search(r"ReadMe_|Readme_|help_text", f.name, re.I)]
        mds = list(folder.glob("*.md"))
        if rtfs and not mds:
            issues.append((str(folder.relative_to(SRC)), [f.name for f in rtfs]))

    print(f"Command folders with RTF but no MD: {len(issues)}")
    for folder, rtfs in issues:
        print(f"  {folder}: {rtfs}")


if __name__ == "__main__":
    main()

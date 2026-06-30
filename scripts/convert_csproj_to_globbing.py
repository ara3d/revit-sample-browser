#!/usr/bin/env python3
"""Convert explicit Compile Include entries to SDK-style globbing in the csproj."""
from __future__ import annotations

import re
import xml.etree.ElementTree as ET
from pathlib import Path

ROOT = Path(__file__).resolve().parents[1]
CSPROJ = ROOT / "src" / "Ara3D.RevitSampleBrowser.csproj"

# Already globbed — do not duplicate
EXISTING_GLOB_PREFIXES = (
    "Common\\",
    "TBC_",
    "N3P_",
    "BB_",
    "B1_",
)

ROOT_FILES = {"Revit2025Aliases.cs", "Extensions.cs", "SampleBrowserApplication.cs",
              "SampleBrowserCommand.cs", "SampleBrowserMainForm.cs",
              "SampleBrowserMainForm.Designer.cs", "SampleData.cs"}


def is_glob_pattern(path: str) -> bool:
    return "*" in path or "?" in path


def should_skip_explicit(path: str) -> bool:
    if is_glob_pattern(path):
        return True
    for prefix in EXISTING_GLOB_PREFIXES:
        if path.startswith(prefix):
            return True
    if path in ROOT_FILES:
        return True
    return False


def parse_compile_entries(text: str) -> tuple[list[dict], list[tuple[str, str]]]:
    """Return explicit compile paths and Compile Update metadata (path, subtype)."""
    compiles: list[dict] = []
    updates: list[tuple[str, str, str | None]] = []  # path, subtype, dependent

    compile_block = re.compile(
        r'<Compile Include="([^"]+)"(?:\s*/>|>(.*?)</Compile>)',
        re.DOTALL,
    )
    for m in compile_block.finditer(text):
        path = m.group(1).replace("/", "\\")
        inner = m.group(2) or ""
        if should_skip_explicit(path):
            continue
        subtype = None
        dependent = None
        sm = re.search(r"<SubType>([^<]+)</SubType>", inner)
        if sm:
            subtype = sm.group(1)
        dm = re.search(r"<DependentUpon>([^<]+)</DependentUpon>", inner)
        if dm:
            dependent = dm.group(1)
        compiles.append({"path": path, "subtype": subtype, "dependent": dependent})
        if subtype or dependent:
            updates.append((path, subtype, dependent))

    return compiles, updates


def main() -> None:
    text = CSPROJ.read_text(encoding="utf-8")
    compiles, updates = parse_compile_entries(text)

    if not compiles:
        print("No explicit Compile entries to convert.")
        return

    print(f"Found {len(compiles)} explicit Compile entries to replace with glob")
    print(f"Preserving {len(updates)} Compile Update metadata entries")

    # Remove explicit Compile Include lines (non-glob, non-already-globbed)
    def remove_compile(m: re.Match) -> str:
        path = m.group(1).replace("/", "\\")
        if should_skip_explicit(path):
            return m.group(0)
        return ""

    new_text = compile_block.sub(remove_compile, text) if False else text

    compile_block = re.compile(
        r'\s*<Compile Include="([^"]+)"(?:\s*/>|>\s*(?:<SubType>[^<]+</SubType>\s*)?(?:<DependentUpon>[^<]+</DependentUpon>\s*)?</Compile>)\s*\n?',
        re.DOTALL,
    )

    removed = 0

    def repl(m: re.Match) -> str:
        nonlocal removed
        path = m.group(1).replace("/", "\\")
        if should_skip_explicit(path):
            return m.group(0)
        removed += 1
        return ""

    new_text = compile_block.sub(repl, text)

    # Insert glob + Compile Update block after existing glob section
    glob_marker = '<Compile Include="Revit2025Aliases.cs" />'
    glob_section = """
    <Compile Include="**\\*.cs"
              Exclude="Common\\**\\*.cs;TBC_*\\**\\*.cs;N3P_*\\**\\*.cs;BB_*\\**\\*.cs;B1_*\\**\\*.cs;bin\\**;obj\\**" />
"""

    update_lines = []
    seen = set()
    for path, subtype, dependent in updates:
        if path in seen:
            continue
        seen.add(path)
        inner = ""
        if subtype:
            inner += f"\n      <SubType>{subtype}</SubType>"
        if dependent:
            inner += f"\n      <DependentUpon>{dependent}</DependentUpon>"
        if inner:
            update_lines.append(
                f'    <Compile Update="{path}">{inner}\n    </Compile>'
            )

    insert_block = glob_section
    if update_lines:
        insert_block += "\n" + "\n".join(update_lines) + "\n"

    if glob_marker in new_text and '<Compile Include="**\\*.cs"' not in new_text:
        new_text = new_text.replace(glob_marker, glob_marker + insert_block, 1)
    else:
        print("Warning: could not insert glob pattern; manual edit needed")

    # Clean up excessive blank lines
    new_text = re.sub(r"\n{4,}", "\n\n\n", new_text)

    CSPROJ.write_text(new_text, encoding="utf-8")
    print(f"Removed {removed} explicit Compile entries")
    print(f"Added glob + {len(update_lines)} Compile Update entries")
    print(f"Updated {CSPROJ}")


if __name__ == "__main__":
    main()

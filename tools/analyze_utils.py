#!/usr/bin/env python3
"""Analyze all _utils.cs files for methods, duplicates, and usage."""

import re
import hashlib
import difflib
from pathlib import Path
from collections import defaultdict
from dataclasses import dataclass, field

ROOT = Path(__file__).resolve().parent.parent / "src"
NEAR_THRESHOLD = 0.85


def strip_csharp_comments(text: str) -> str:
    result = []
    i = 0
    n = len(text)
    while i < n:
        if text[i:i+3] == '///':
            while i < n and text[i] != '\n':
                i += 1
            continue
        if text[i:i+2] == '//':
            while i < n and text[i] != '\n':
                i += 1
            continue
        if text[i:i+2] == '/*':
            end = text.find('*/', i + 2)
            i = end + 2 if end != -1 else n
            continue
        if text[i] in ('"', "'"):
            q = text[i]
            result.append(q)
            i += 1
            while i < n:
                if text[i] == '\\':
                    result.append(text[i:i+2])
                    i += 2
                    continue
                result.append(text[i])
                if text[i] == q:
                    i += 1
                    break
                i += 1
            continue
        result.append(text[i])
        i += 1
    return ''.join(result)


def find_matching_brace(text: str, open_idx: int) -> int:
    depth = 0
    in_string = None
    i = open_idx
    while i < len(text):
        c = text[i]
        if in_string:
            if c == '\\':
                i += 2
                continue
            if c == in_string:
                in_string = None
            i += 1
            continue
        if c in ('"', "'"):
            in_string = c
            i += 1
            continue
        if c == '{':
            depth += 1
        elif c == '}':
            depth -= 1
            if depth == 0:
                return i
        i += 1
    return -1


CLASS_RE = re.compile(
    r'(?:^|[;\}\s])(?:public|private|protected|internal)?\s*'
    r'(?:static\s+)?(?:partial\s+)?(?:sealed\s+)?(?:abstract\s+)?'
    r'class\s+(\w+)\s*(?:<[^>]+>)?\s*(?::|\{)',
    re.MULTILINE,
)
NAMESPACE_RE = re.compile(r'^\s*namespace\s+([\w\.]+)\s*\{', re.MULTILINE)
DELEGATE_RE = re.compile(
    r'^\s*public\s+delegate\s+(.+?)\s+(\w+)\s*\([^;]*\)\s*;',
    re.MULTILINE,
)
METHOD_RE = re.compile(
    r'^(?P<indent>\s*)'
    r'(?P<prefix>(?:\[[^\]]+\]\s*)*)'
    r'(?P<mods>(?:(?:public|private|protected|internal)\s+)?'
    r'(?:(?:static|virtual|override|sealed|new|readonly|async|unsafe|partial|extern)\s+)*)'
    r'(?P<ret>(?:[\w<>\[\],\.\?\s]+?))\s+'
    r'(?P<name>(?:~?\w+(?:<[^>]*>)?|operator\s+[\+\-\*/%&\|~!=<>]+))\s*'
    r'\(',
    re.MULTILINE,
)


@dataclass
class MethodInfo:
    file: str
    sample: str
    namespace: str
    containing_type: str
    name: str
    signature: str
    body: str
    normalized_body: str
    is_public: bool
    body_hash: str = field(default='')

    def __post_init__(self):
        self.body_hash = hashlib.md5(self.normalized_body.encode()).hexdigest() if self.normalized_body else ''

    @property
    def key(self):
        return f"{self.file}::{self.containing_type}.{self.name}"


def normalize_body(body: str) -> str:
    s = strip_csharp_comments(body).strip()
    s = re.sub(r'\s+', ' ', s).strip()
    return s


def parse_method_at(text: str, match: re.Match) -> tuple[str, str, str, bool] | None:
    name = match.group('name').strip()
    if name in {'if', 'for', 'while', 'switch', 'catch', 'using', 'lock', 'foreach', 'return'}:
        return None

    start = match.start()
    paren = text.index('(', start)
    depth = 0
    i = paren
    while i < len(text):
        if text[i] == '(':
            depth += 1
        elif text[i] == ')':
            depth -= 1
            if depth == 0:
                break
        i += 1
    sig_end = i + 1
    signature = text[start:sig_end].strip()
    signature = re.sub(r'\s+', ' ', signature)

    mods = match.group('mods') or ''
    is_public = 'public' in mods

    j = sig_end
    while j < len(text) and text[j] in ' \t\r\n':
        j += 1

    if j + 1 < len(text) and text[j:j+2] == '=>':
        semi = text.find(';', j)
        body = text[j:semi + 1].strip() if semi != -1 else text[j:].strip()
    elif j < len(text) and text[j] == '{':
        end = find_matching_brace(text, j)
        body = text[j:end + 1] if end != -1 else ''
    elif j < len(text) and text[j] == ';':
        body = ';'
    else:
        body = ''

    return name, signature, body, is_public


def parse_utils_file(path: Path) -> list[MethodInfo]:
    raw = path.read_text(encoding='utf-8-sig')
    text = strip_csharp_comments(raw)
    rel = path.relative_to(ROOT)
    sample = rel.parts[0] if len(rel.parts) > 1 else '(root)'
    ns_match = NAMESPACE_RE.search(text)
    namespace = ns_match.group(1) if ns_match else ''

    methods: list[MethodInfo] = []

    for m in DELEGATE_RE.finditer(text):
        ret, name = m.group(1).strip(), m.group(2)
        sig = f"delegate {ret} {name}(...)"
        methods.append(MethodInfo(
            file=str(rel).replace('\\', '/'), sample=sample, namespace=namespace,
            containing_type='_Utils', name=name, signature=sig, body='',
            normalized_body='', is_public=True,
        ))

    class_spans = []
    for m in CLASS_RE.finditer(text):
        class_name = m.group(1)
        brace = text.find('{', m.end() - 1)
        if brace == -1:
            continue
        end = find_matching_brace(text, brace)
        if end == -1:
            continue
        class_spans.append((class_name, brace + 1, end))

    for class_name, start, end in class_spans:
        chunk = text[start:end]
        for m in METHOD_RE.finditer(chunk):
            line_start = chunk.rfind('\n', 0, m.start()) + 1
            line_prefix = chunk[line_start:m.start()].strip()
            if line_prefix.startswith(('return ', 'throw ', 'new ', 'var ', 'if ', 'for ', 'while ')):
                continue
            mods = m.group('mods') or ''
            name = m.group('name')
            has_access = bool(re.search(r'\b(public|private|protected|internal)\b', mods))
            is_ctor = name == class_name
            is_operator = name.startswith('operator')
            if not (has_access or is_ctor or is_operator):
                continue
            parsed = parse_method_at(chunk, m)
            if not parsed:
                continue
            name, signature, body, is_public = parsed
            norm = normalize_body(body) if body and body != ';' else ''
            methods.append(MethodInfo(
                file=str(rel).replace('\\', '/'), sample=sample, namespace=namespace,
                containing_type=class_name, name=name, signature=signature, body=body,
                normalized_body=norm, is_public=is_public,
            ))

    # dedupe same method parsed twice
    seen = set()
    unique = []
    for m in methods:
        k = (m.file, m.containing_type, m.name, m.signature)
        if k in seen:
            continue
        seen.add(k)
        unique.append(m)
    return unique


def find_command_classes(sample_dir: Path) -> dict[str, str]:
    """file -> comma-separated command class names."""
    cmd_re = re.compile(
        r'class\s+(\w+)\s*:\s*[^,{]*\b(?:IExternalCommand|IExternalApplication)\b'
    )
    result = {}
    if not sample_dir.exists():
        return result
    for cs in sample_dir.rglob('*.cs'):
        if cs.name == '_utils.cs':
            continue
        try:
            content = cs.read_text(encoding='utf-8-sig')
        except OSError:
            continue
        names = [m.group(1) for m in cmd_re.finditer(content)]
        if names:
            result[cs.relative_to(ROOT).as_posix()] = ','.join(names)
    return result


def find_usages(all_methods: list[MethodInfo]) -> dict[str, set[str]]:
    usage: dict[str, set[str]] = defaultdict(set)

    by_sample: dict[str, list[MethodInfo]] = defaultdict(list)
    for m in all_methods:
        by_sample[m.sample].append(m)

    cmd_re = re.compile(
        r'class\s+(\w+)\s*:\s*[^,{]*\b(?:IExternalCommand|IExternalApplication)\b'
    )

    for cs in ROOT.rglob('*.cs'):
        if cs.name == '_utils.cs':
            continue
        try:
            content = cs.read_text(encoding='utf-8-sig')
        except OSError:
            continue
        if '_Utils.' not in content:
            continue

        rel = cs.relative_to(ROOT)
        sample = rel.parts[0] if len(rel.parts) > 1 else '(root)'
        cmd_classes = [m.group(1) for m in cmd_re.finditer(content)]
        if cmd_classes:
            caller = f"{rel.as_posix()} ({', '.join(cmd_classes)})"
        else:
            caller = rel.as_posix()

        for m in by_sample.get(sample, []):
            if m.containing_type == '_Utils' and re.search(rf'\b_Utils\.{re.escape(m.name)}\b', content):
                usage[m.key].add(caller)
            elif m.containing_type != '_Utils':
                # enum/const/nested type member references
                if re.search(rf'\b_Utils\.{re.escape(m.containing_type)}\b', content) or \
                   re.search(rf'\b{re.escape(m.containing_type)}\.{re.escape(m.name)}\b', content):
                    usage[m.key].add(caller)

    return usage


def count_exact_duplicates(with_body: list[MethodInfo]) -> tuple[int, int, dict]:
    groups: dict[str, list[MethodInfo]] = defaultdict(list)
    for m in with_body:
        if m.normalized_body:
            groups[m.body_hash].append(m)
    dup_groups = {h: g for h, g in groups.items() if len(g) > 1}
    excess = sum(len(g) - 1 for g in dup_groups.values())
    return len(dup_groups), excess, dup_groups


def find_near_duplicates(with_body: list[MethodInfo]) -> list[tuple]:
    pairs = []
    seen = set()
    by_name: dict[str, list[MethodInfo]] = defaultdict(list)
    for m in with_body:
        if m.normalized_body:
            by_name[m.name].append(m)

    candidates = []
    for group in by_name.values():
        if len(group) < 2:
            continue
        for i in range(len(group)):
            for j in range(i + 1, len(group)):
                if group[i].body_hash != group[j].body_hash:
                    candidates.append((group[i], group[j]))

    for m1, m2 in candidates:
        pair_id = tuple(sorted([m1.key, m2.key]))
        if pair_id in seen:
            continue
        ratio = difflib.SequenceMatcher(None, m1.normalized_body, m2.normalized_body).ratio()
        if ratio >= NEAR_THRESHOLD:
            seen.add(pair_id)
            pairs.append((m1, m2, ratio))
    return pairs


def main():
    utils_files = sorted(ROOT.rglob('_utils.cs'))
    all_methods: list[MethodInfo] = []
    for f in utils_files:
        all_methods.extend(parse_utils_file(f))

    with_body = [m for m in all_methods if m.normalized_body]
    dup_group_count, dup_excess, dup_groups = count_exact_duplicates(with_body)
    near_pairs = find_near_duplicates(with_body)
    usage = find_usages(all_methods)

    single_cmd, multi_cmd, unused = [], [], []
    for m in all_methods:
        callers = usage.get(m.key, set())
        if not callers:
            unused.append(m)
        elif len(callers) == 1:
            single_cmd.append((m, callers))
        else:
            multi_cmd.append((m, callers))

    lines = [
        "# _utils.cs Analysis Report",
        "",
        f"Analysis of **{len(utils_files)}** `_utils.cs` files across the Revit sample browser.",
        "",
        "## Summary",
        "",
        "| Metric | Count |",
        "|--------|------:|",
        f"| `_utils.cs` files | {len(utils_files)} |",
        f"| Total methods (all types in `_utils.cs`) | {len(all_methods)} |",
        f"| `_Utils` static helper methods | {sum(1 for m in all_methods if m.containing_type == '_Utils')} |",
        f"| Methods with analyzable body | {len(with_body)} |",
        f"| Exact duplicate body groups | {dup_group_count} |",
        f"| Duplicate method bodies (excess copies) | {dup_excess} |",
        f"| Near-duplicate pairs (≥{NEAR_THRESHOLD:.0%} similarity) | {len(near_pairs)} |",
        f"| Methods used by exactly one command/caller | {len(single_cmd)} |",
        f"| Methods used by multiple commands/callers | {len(multi_cmd)} |",
        f"| Methods with no detected `_Utils.` usage | {len(unused)} |",
        "",
        "> **Notes:** Duplicate detection compares normalized method bodies (whitespace/comments stripped). "
        "Near-duplicates share ≥85% body similarity but are not identical. "
        "Usage is detected via `_Utils.<Method>` references from non-`_utils` `.cs` files within the same sample folder. "
        "A *command* is a class implementing `IExternalCommand` or `IExternalApplication`; when absent, the calling file path is listed.",
        "",
    ]

    lines += ["## Exact Duplicate Method Bodies", ""]
    if not dup_groups:
        lines.append("_None found._")
    else:
        for idx, group in enumerate(sorted(dup_groups.values(), key=lambda g: (-len(g), g[0].name)), 1):
            lines.append(f"### Group {idx} — `{group[0].name}` ({len(group)} copies)")
            lines.append("")
            for m in sorted(group, key=lambda x: x.file):
                lines.append(f"- `{m.namespace}.{m.containing_type}.{m.name}` — `{m.file}`")
            preview = group[0].normalized_body
            if len(preview) > 300:
                preview = preview[:297] + '...'
            lines += ["", "```csharp", preview, "```", ""]

    lines += [
        "## Near-Duplicate Method Bodies",
        "",
        f"Method pairs with ≥{NEAR_THRESHOLD:.0%} normalized body similarity but different hashes.",
        "",
    ]
    if not near_pairs:
        lines.append("_None found._")
    else:
        lines += [
            "| Similarity | Method A | Method B |",
            "|-----------:|----------|----------|",
        ]
        for m1, m2, ratio in sorted(near_pairs, key=lambda x: -x[2]):
            a = f"`{m1.sample}/{m1.containing_type}.{m1.name}`"
            b = f"`{m2.sample}/{m2.containing_type}.{m2.name}`"
            lines.append(f"| {ratio:.1%} | {a} | {b} |")
    lines.append("")

    lines += [
        "## Usage: Multiple Commands vs Single Command",
        "",
        f"### Used by multiple commands/callers ({len(multi_cmd)})",
        "",
    ]
    if multi_cmd:
        lines += ["| Method | Namespace | Callers |", "|--------|-----------|---------|"]
        for m, callers in sorted(multi_cmd, key=lambda x: (-len(x[1]), x[0].file, x[0].name)):
            cl = '<br>'.join(sorted(callers))
            lines.append(f"| `{m.containing_type}.{m.name}` | `{m.namespace}` | {cl} |")
    else:
        lines.append("_None._")
    lines.append("")

    lines += [f"### Used by exactly one command/caller ({len(single_cmd)})", ""]
    if single_cmd:
        lines += ["| Method | Namespace | Caller |", "|--------|-----------|--------|"]
        for m, callers in sorted(single_cmd, key=lambda x: (x[0].file, x[0].name)):
            lines.append(f"| `{m.containing_type}.{m.name}` | `{m.namespace}` | {list(callers)[0]} |")
    lines.append("")

    lines += [f"### No detected usage ({len(unused)})", ""]
    if unused:
        for m in sorted(unused, key=lambda x: (x.file, x.containing_type, x.name)):
            vis = "public" if m.is_public else "non-public"
            lines.append(f"- `{m.containing_type}.{m.name}` ({vis}) — `{m.file}`")
    lines.append("")

    lines += ["## All _utils.cs Files", ""]
    by_file: dict[str, list[MethodInfo]] = defaultdict(list)
    for m in all_methods:
        by_file[m.file].append(m)

    for file_path in sorted(by_file):
        ms = by_file[file_path]
        ns = ms[0].namespace
        types = sorted({m.containing_type for m in ms})
        lines += [
            f"### `{file_path}`",
            "",
            f"- **Namespace:** `{ns}`",
            f"- **Types:** {', '.join(f'`{t}`' for t in types)}",
            f"- **Method count:** {len(ms)}",
            "",
            "| Type | Method | Signature |",
            "|------|--------|-----------|",
        ]
        for m in sorted(ms, key=lambda x: (x.containing_type, x.name)):
            sig = m.signature.replace('|', '\\|')
            if len(sig) > 100:
                sig = sig[:97] + '...'
            lines.append(f"| `{m.containing_type}` | `{m.name}` | `{sig}` |")
        lines.append("")

    out = ROOT.parent / "utils-report.md"
    out.write_text('\n'.join(lines), encoding='utf-8')
    print(f"Wrote {out}")
    print(f"Files={len(utils_files)} Methods={len(all_methods)} DupGroups={dup_group_count} NearPairs={len(near_pairs)}")


if __name__ == '__main__':
    main()

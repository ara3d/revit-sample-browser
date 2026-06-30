using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace Ara3D.Bowerbird.RevitSamples.AecAgent;

public sealed class SampleCatalogEntry
{
    public string Name { get; set; }
    public string Sample { get; set; }
    public string Description { get; set; }
    public string Category { get; set; }
    public string DocPath { get; set; }
    public string DescriptorPath { get; set; }
    public int? McpRating { get; set; }
    public bool RequiresUi { get; set; }
    public string SearchText { get; set; }
}

public sealed class SampleCatalogIndex
{
    readonly List<SampleCatalogEntry> _entries = [];
    bool _loaded;

    public IReadOnlyList<SampleCatalogEntry> Entries => _entries;

    public void EnsureLoaded(string repoRoot)
    {
        if (_loaded) return;
        _loaded = true;
        if (string.IsNullOrEmpty(repoRoot)) return;

        LoadCommandsMd(Path.Combine(repoRoot, "COMMANDS.md"), repoRoot);
        LoadDescriptors(repoRoot);
    }

    void LoadCommandsMd(string path, string repoRoot)
    {
        if (!File.Exists(path)) return;
        foreach (var line in File.ReadAllLines(path))
        {
            var match = Regex.Match(line, @"\| `([^`]+)` \| ([^|]+) \| ([^|]+) \| ([^|]+) \| \[([^\]]+)\]\((src/[^)]+)\) \|");
            if (!match.Success) continue;

            _entries.Add(new SampleCatalogEntry
            {
                Name = match.Groups[1].Value.Trim(),
                Sample = match.Groups[2].Value.Trim(),
                Description = match.Groups[3].Value.Trim(),
                Category = match.Groups[4].Value.Trim(),
                DocPath = Path.Combine(repoRoot, match.Groups[6].Value.Replace('/', Path.DirectorySeparatorChar)),
            });
        }
    }

    void LoadDescriptors(string repoRoot)
    {
        var src = Path.Combine(repoRoot, "src");
        if (!Directory.Exists(src)) return;

        foreach (var file in Directory.EnumerateFiles(src, "*.json", SearchOption.AllDirectories))
        {
            if (file.Contains($"{Path.DirectorySeparatorChar}_template.json", StringComparison.OrdinalIgnoreCase))
                continue;

            try
            {
                var json = JObject.Parse(File.ReadAllText(file));
                if (json["name"] == null || json["arguments"] == null) continue;

                var toolName = json["name"]?.Value<string>();
                var sample = json["sample"]?.Value<string>();
                var entry = _entries.FirstOrDefault(e =>
                    string.Equals(e.Sample, sample, StringComparison.OrdinalIgnoreCase)
                    && (e.DescriptorPath == null || e.Name == json["commandClass"]?.Value<string>()));

                if (entry == null)
                {
                    entry = new SampleCatalogEntry
                    {
                        Name = json["commandClass"]?.Value<string>() ?? toolName,
                        Sample = sample ?? Path.GetFileName(Path.GetDirectoryName(file)),
                        Description = json["description"]?.Value<string>(),
                    };
                    _entries.Add(entry);
                }

                entry.DescriptorPath = file;
                entry.McpRating = json["mcpRating"]?.Value<int?>();
                entry.RequiresUi = json["requiresUi"]?.Value<bool?>() ?? false;
                entry.SearchText = string.Join(' ',
                    toolName, sample, entry.Description, json["notes"]?.Value<string>(), entry.Category);
            }
            catch
            {
                // Skip invalid descriptor files.
            }
        }

        foreach (var entry in _entries)
        {
            entry.SearchText ??= string.Join(' ', entry.Name, entry.Sample, entry.Description, entry.Category);
            if (!string.IsNullOrEmpty(entry.DocPath) && File.Exists(entry.DocPath))
            {
                try
                {
                    var body = File.ReadAllText(entry.DocPath);
                    entry.SearchText += " " + body;
                }
                catch { }
            }
        }
    }

    public IReadOnlyList<SampleCatalogEntry> Search(string query, int limit = 20)
    {
        if (string.IsNullOrWhiteSpace(query))
            return _entries.Take(limit).ToList();

        var terms = query.Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        return _entries
            .Select(e => new { Entry = e, Score = Score(e, terms) })
            .Where(x => x.Score > 0)
            .OrderByDescending(x => x.Score)
            .ThenBy(x => x.Entry.Sample)
            .Take(limit)
            .Select(x => x.Entry)
            .ToList();
    }

    static int Score(SampleCatalogEntry entry, string[] terms)
    {
        var hay = (entry.SearchText ?? "").ToLowerInvariant();
        var score = 0;
        foreach (var term in terms)
        {
            if (hay.Contains(term.ToLowerInvariant()))
                score += term.Length;
        }
        if (entry.McpRating >= 5) score += 2;
        if (!entry.RequiresUi) score += 1;
        return score;
    }
}

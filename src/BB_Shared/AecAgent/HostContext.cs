using Autodesk.Revit.UI;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;

namespace Ara3D.Bowerbird.RevitSamples.AecAgent;

public sealed class HostContext
{
    public string HostName { get; init; } = "revit";
    public UIApplication UiApp { get; init; }
    public IRevitBridge Bridge { get; init; }
    public AgentAuditLog Audit { get; init; }
    public string RepoRoot { get; init; }
    public ChangeSetSession ChangeSetSession { get; init; }
    public SampleCatalogIndex Catalog { get; init; }
    public int ToolCount { get; init; }
    public string[] ToolNames { get; init; } = [];
}

public sealed class AgentAuditLog
{
    readonly List<JObject> _entries = [];
    readonly object _lock = new();

    public void Record(string kind, string name, bool ok, JObject details = null)
    {
        lock (_lock)
        {
            _entries.Add(new JObject
            {
                ["timestampUtc"] = DateTime.UtcNow.ToString("o"),
                ["kind"] = kind,
                ["name"] = name,
                ["ok"] = ok,
                ["details"] = details ?? new JObject(),
            });
        }
    }

    public JArray GetEntries(int max = 100)
    {
        lock (_lock)
        {
            var slice = _entries.Count <= max
                ? _entries
                : _entries.GetRange(_entries.Count - max, max);
            return new JArray(slice);
        }
    }
}

using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Ara3D.Bowerbird.RevitSamples.AecAgent;

public enum ChangeOpKind
{
    SetParameter,
    SetViewOverride,
    DeleteElements,
    CreateElements,
}

public sealed class ChangeOp
{
    public string Id { get; set; } = Guid.NewGuid().ToString("N");
    public ChangeOpKind Kind { get; set; }
    public JObject Payload { get; set; } = [];
}

public sealed class ChangeSet
{
    public string Id { get; set; } = Guid.NewGuid().ToString("N");
    public string Intent { get; set; }
    public List<ChangeOp> Operations { get; set; } = [];
    public string RiskLevel { get; set; } = "low";
    public bool RequiresApproval { get; set; }
}

public sealed class ChangeSetSession
{
    readonly Dictionary<string, ChangeSet> _sets = new(StringComparer.OrdinalIgnoreCase);
    ChangeSet _lastApplied;
    string _lastApprovalToken;

    public ChangeSet Create(string intent, IEnumerable<ChangeOp> ops)
    {
        var set = new ChangeSet
        {
            Intent = intent,
            Operations = ops?.ToList() ?? [],
            RiskLevel = ClassifyRisk(ops),
            RequiresApproval = ClassifyRisk(ops) is "high" or "medium",
        };
        _sets[set.Id] = set;
        return set;
    }

    public bool TryGet(string id, out ChangeSet set) => _sets.TryGetValue(id, out set);

    public void MarkApplied(ChangeSet set) => _lastApplied = set;

    public ChangeSet GetLastApplied() => _lastApplied;

    public string CreateApprovalToken(string changeSetId)
    {
        _lastApprovalToken = $"{changeSetId}:{Guid.NewGuid():N}";
        return _lastApprovalToken;
    }

    public bool IsApproved(string token, string changeSetId)
        => !string.IsNullOrEmpty(token)
           && token.StartsWith(changeSetId + ":", StringComparison.Ordinal);

    static string ClassifyRisk(IEnumerable<ChangeOp> ops)
    {
        if (ops == null) return "low";
        if (ops.Any(o => o.Kind == ChangeOpKind.DeleteElements)) return "high";
        if (ops.Any(o => o.Kind == ChangeOpKind.CreateElements)) return "medium";
        if (ops.Any(o => o.Kind == ChangeOpKind.SetViewOverride)) return "low";
        return "low";
    }
}

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
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
    public bool Applied { get; set; }
    public DateTime? AppliedAtUtc { get; set; }
    public string LastTransactionId { get; set; }
}

public sealed class AppliedTransaction
{
    public string Id { get; set; } = Guid.NewGuid().ToString("N");
    public string ChangeSetId { get; set; }
    public string Intent { get; set; }
    public DateTime AppliedAtUtc { get; set; } = DateTime.UtcNow;
    public int AppliedOperations { get; set; }
    public List<long> ChangedElementIds { get; set; } = [];
    public string Status { get; set; } = "committed";
    public JObject Details { get; set; } = [];
}

public sealed class ChangeSetSession
{
    readonly Dictionary<string, ChangeSet> _sets = new(StringComparer.OrdinalIgnoreCase);
    readonly Dictionary<string, AppliedTransaction> _transactions = new(StringComparer.OrdinalIgnoreCase);
    readonly List<string> _order = [];
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
        _order.Add(set.Id);
        return set;
    }

    public bool TryGet(string id, out ChangeSet set) => _sets.TryGetValue(id, out set);

    public ChangeSet Get(string id) => _sets.GetValueOrDefault(id);

    public IEnumerable<ChangeSet> ListChangeSets()
        => _order.Select(id => _sets.GetValueOrDefault(id)).Where(s => s != null);

    public void MarkApplied(ChangeSet set, AppliedTransaction transaction)
    {
        set.Applied = true;
        set.AppliedAtUtc = transaction.AppliedAtUtc;
        set.LastTransactionId = transaction.Id;
        _lastApplied = set;
        _transactions[transaction.Id] = transaction;
    }

    public ChangeSet GetLastApplied() => _lastApplied;

    public AppliedTransaction GetTransaction(string id)
        => string.IsNullOrEmpty(id) ? null : _transactions.GetValueOrDefault(id);

    public string CreateApprovalToken(string changeSetId)
    {
        _lastApprovalToken = $"{changeSetId}:{Guid.NewGuid():N}";
        return _lastApprovalToken;
    }

    public bool IsApproved(string token, string changeSetId)
        => !string.IsNullOrEmpty(token)
           && token.StartsWith(changeSetId + ":", StringComparison.Ordinal);

    public JObject ToAuditJson(ChangeSet set)
    {
        if (set == null) return null;
        return new JObject
        {
            ["change_set_id"] = set.Id,
            ["intent"] = set.Intent,
            ["risk_level"] = set.RiskLevel,
            ["requires_approval"] = set.RequiresApproval,
            ["operation_count"] = set.Operations.Count,
            ["created_at_utc"] = set.CreatedAtUtc.ToString("o"),
            ["applied"] = set.Applied,
            ["applied_at_utc"] = set.AppliedAtUtc?.ToString("o"),
            ["last_transaction_id"] = set.LastTransactionId,
            ["operations"] = new JArray(set.Operations.Select(o => new JObject
            {
                ["id"] = o.Id,
                ["kind"] = o.Kind.ToString(),
                ["payload"] = o.Payload,
            })),
        };
    }

    public JObject TransactionToAuditJson(AppliedTransaction tx)
    {
        if (tx == null) return null;
        return new JObject
        {
            ["transaction_id"] = tx.Id,
            ["change_set_id"] = tx.ChangeSetId,
            ["intent"] = tx.Intent,
            ["applied_at_utc"] = tx.AppliedAtUtc.ToString("o"),
            ["applied_operations"] = tx.AppliedOperations,
            ["changed_element_ids"] = new JArray(tx.ChangedElementIds),
            ["status"] = tx.Status,
            ["details"] = tx.Details ?? new JObject(),
        };
    }

    static string ClassifyRisk(IEnumerable<ChangeOp> ops)
    {
        if (ops == null) return "low";
        if (ops.Any(o => o.Kind == ChangeOpKind.DeleteElements)) return "high";
        if (ops.Any(o => o.Kind == ChangeOpKind.CreateElements)) return "medium";
        if (ops.Any(o => o.Kind == ChangeOpKind.SetParameter)) return "medium";
        if (ops.Any(o => o.Kind == ChangeOpKind.SetViewOverride)) return "low";
        return "low";
    }
}

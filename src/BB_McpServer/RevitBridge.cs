using Autodesk.Revit.UI;
using Newtonsoft.Json.Linq;
using System;
using System.Threading;

namespace Ara3D.Bowerbird.RevitSamples;

public sealed class RevitBridge : AecAgent.IRevitBridge, IExternalEventHandler
{
    readonly ManualResetEventSlim _done = new(false);
    readonly object _lock = new();
    readonly ExternalEvent _event;
    Func<UIApplication, AecAgent.McpToolResult> _work;
    AecAgent.McpToolResult _result;
    Exception _error;

    public RevitBridge()
    {
        _event = ExternalEvent.Create(this);
    }

    public AecAgent.McpToolResult RunOnRevitThread(Func<UIApplication, AecAgent.McpToolResult> work)
        => RunOnRevitThread(30_000, work);

    public AecAgent.McpToolResult RunOnRevitThread(int timeoutMs, Func<UIApplication, AecAgent.McpToolResult> work)
    {
        lock (_lock)
        {
            _work = work;
            _result = null;
            _error = null;
            _done.Reset();
            _event.Raise();
        }

        if (!_done.Wait(timeoutMs))
            return AecAgent.McpToolResult.Failure("Timed out waiting for Revit.", "timeout");

        lock (_lock)
        {
            if (_error != null)
                return AecAgent.McpToolResult.Failure(_error.Message, "revit_exception");
            return _result ?? AecAgent.McpToolResult.Failure("No result from Revit bridge.", "empty_result");
        }
    }

    public void Execute(UIApplication app)
    {
        try
        {
            Func<UIApplication, AecAgent.McpToolResult> work;
            lock (_lock)
                work = _work;

            _result = work?.Invoke(app);
        }
        catch (Exception ex)
        {
            _error = ex;
        }
        finally
        {
            _done.Set();
        }
    }

    public string GetName() => "MCP Revit Bridge";
}

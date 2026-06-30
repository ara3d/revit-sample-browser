using Autodesk.Revit.UI;
using Newtonsoft.Json.Linq;
using System;

namespace Ara3D.Bowerbird.RevitSamples.AecAgent;

public interface IRevitBridge
{
    McpToolResult RunOnRevitThread(int timeoutMs, Func<UIApplication, McpToolResult> work);
    McpToolResult RunOnRevitThread(Func<UIApplication, McpToolResult> work);
}

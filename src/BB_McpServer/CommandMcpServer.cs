using Ara3D.Bowerbird.RevitSamples.AecAgent;
using Ara3D.Utils;
using Autodesk.Revit.UI;
using System.Windows.Forms;

namespace Ara3D.Bowerbird.RevitSamples;

/// <summary>
/// Starts a localhost MCP server (Streamable HTTP) for Cursor.
/// </summary>
public class CommandMcpServer : NamedCommand
{
    const int Port = 8765;
    const string McpPath = "/mcp";

    static WebServer _server;
    static RevitBridge _bridge;
    static McpToolRegistry _registry;
    static McpProtocolHandler _protocol;

    public override string Name => "Start MCP Server";

    public override void Execute(object arg)
    {
        if (_server?.Active == true)
        {
            MessageBox.Show($"MCP server is already running at {McpUrl}");
            return;
        }

        _bridge = new RevitBridge();
        _registry = new McpToolRegistry();
        _registry.Initialize(_bridge, RepoPaths.FindRepoRoot());
        _protocol = new McpProtocolHandler(_registry);
        _server = new WebServer(context => _protocol.HandleRequest(context), Port);

        _server.Start();

        var config = $@"Add to %USERPROFILE%\.cursor\mcp.json:

{{
  ""mcpServers"": {{
    ""bowerbird-revit"": {{
      ""url"": ""{McpUrl}""
    }}
  }}
}}

Reload MCP in Cursor (Settings → Tools & MCP), then try:
- aec.get_host_context
- aec.query_elements
- dev.search_examples
- inspect_current_context prompt";

        MessageBox.Show($"MCP server started at {McpUrl}\n\nRegistered tools: {_registry.Context.ToolCount}\n\n{config}", "Bowerbird MCP Server");
    }

    static string McpUrl => $"http://127.0.0.1:{Port}{McpPath}";
}

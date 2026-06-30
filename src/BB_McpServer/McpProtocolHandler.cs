using Newtonsoft.Json.Linq;
using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Ara3D.Bowerbird.RevitSamples;

public sealed class McpProtocolHandler
{
    readonly McpToolRegistry _registry;

    public McpProtocolHandler(McpToolRegistry registry) => _registry = registry;

    public void HandleRequest(HttpListenerContext context)
    {
        var request = context.Request;
        var response = context.Response;

        if (!IsAllowedOrigin(request))
        {
            response.StatusCode = 403;
            return;
        }

        var path = request.Url?.AbsolutePath ?? "";
        if (!path.Equals("/mcp", StringComparison.OrdinalIgnoreCase))
        {
            response.StatusCode = 404;
            return;
        }

        if (request.HttpMethod == "GET")
        {
            response.StatusCode = 405;
            return;
        }

        if (request.HttpMethod != "POST")
        {
            response.StatusCode = 405;
            return;
        }

        string body;
        using (var reader = new StreamReader(request.InputStream, request.ContentEncoding))
            body = reader.ReadToEnd();

        if (string.IsNullOrWhiteSpace(body))
        {
            response.StatusCode = 400;
            return;
        }

        var message = JToken.Parse(body);
        if (message is JArray batch)
        {
            HandleBatch(batch, response);
            return;
        }

        HandleMessage(message as JObject, response);
    }

    void HandleBatch(JArray batch, HttpListenerResponse response)
    {
        var requests = batch.OfType<JObject>().Where(IsRequest).ToList();
        if (requests.Count == 0)
        {
            response.StatusCode = 202;
            return;
        }

        var responses = requests.Select(HandleRequestObject).Where(r => r != null).ToList();
        WriteJson(response, responses.Count == 1 ? responses[0]! : new JArray(responses));
    }

    void HandleMessage(JObject message, HttpListenerResponse response)
    {
        if (message == null)
        {
            response.StatusCode = 400;
            return;
        }

        if (!IsRequest(message))
        {
            response.StatusCode = 202;
            return;
        }

        WriteJson(response, HandleRequestObject(message)!);
    }

    JObject HandleRequestObject(JObject request)
    {
        var id = request["id"];
        var method = request["method"]?.Value<string>() ?? "";

        try
        {
            var result = method switch
            {
                "initialize" => HandleInitialize(),
                "tools/list" => _registry.ListTools(),
                "tools/call" => HandleToolsCall(request["params"] as JObject).GetAwaiter().GetResult(),
                "resources/list" => _registry.Resources.ListResources(),
                "resources/read" => _registry.Resources.ReadResource(request["params"]?["uri"]?.Value<string>()),
                "prompts/list" => _registry.Prompts.ListPrompts(),
                "prompts/get" => _registry.Prompts.GetPrompt(
                    request["params"]?["name"]?.Value<string>(),
                    request["params"]?["arguments"] as JObject),
                "ping" => new JObject(),
                _ => throw new McpProtocolException(-32601, $"Method not found: {method}"),
            };

            return id == null || id.Type == JTokenType.Null
                ? null
                : new JObject
                {
                    ["jsonrpc"] = "2.0",
                    ["id"] = id,
                    ["result"] = result,
                };
        }
        catch (McpProtocolException ex)
        {
            return Error(id, ex.Code, ex.Message);
        }
        catch (Exception ex)
        {
            return Error(id, -32603, ex.Message);
        }
    }

    static JObject HandleInitialize()
        => new()
        {
            ["protocolVersion"] = "2025-03-26",
            ["capabilities"] = new JObject
            {
                ["tools"] = new JObject(),
                ["resources"] = new JObject(),
                ["prompts"] = new JObject(),
            },
            ["serverInfo"] = new JObject
            {
                ["name"] = "bowerbird-revit-mcp",
                ["version"] = "2.0.0",
            },
        };

    async Task<JObject> HandleToolsCall(JObject parameters)
    {
        var name = parameters?["name"]?.Value<string>();
        var arguments = parameters?["arguments"] as JObject ?? new JObject();
        return await _registry.CallToolAsync(name, arguments);
    }

    static bool IsRequest(JObject message) => message["method"] != null && message["id"] != null;

    static JObject Error(JToken id, int code, string message)
        => id == null || id.Type == JTokenType.Null
            ? null
            : new JObject
            {
                ["jsonrpc"] = "2.0",
                ["id"] = id,
                ["error"] = new JObject
                {
                    ["code"] = code,
                    ["message"] = message,
                },
            };

    static bool IsAllowedOrigin(HttpListenerRequest request)
    {
        var origin = request.Headers["Origin"];
        return string.IsNullOrEmpty(origin)
               || Uri.TryCreate(origin, UriKind.Absolute, out var uri)
               && uri.Host is "localhost" or "127.0.0.1";
    }

    static void WriteJson(HttpListenerResponse response, JToken payload)
    {
        var json = payload.ToString(Newtonsoft.Json.Formatting.None);
        var bytes = Encoding.UTF8.GetBytes(json);
        response.StatusCode = 200;
        response.ContentType = "application/json; charset=utf-8";
        response.ContentLength64 = bytes.Length;
        response.OutputStream.Write(bytes, 0, bytes.Length);
    }
}

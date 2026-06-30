using Ara3D.Bowerbird.RevitSamples.AecAgent;
using Newtonsoft.Json.Linq;
using System;

namespace Ara3D.Bowerbird.RevitSamples;

public sealed class McpProtocolException : Exception
{
    public int Code { get; }

    public McpProtocolException(int code, string message) : base(message)
    {
        Code = code;
    }
}

public static class McpJson
{
    public static JObject ToolCallResult(McpToolResult result, bool structured = true)
    {
        var text = structured
            ? result.ToEnvelope().ToString(Newtonsoft.Json.Formatting.Indented)
            : result.ToDisplayText();

        return new JObject
        {
            ["content"] = new JArray
            {
                new JObject
                {
                    ["type"] = "text",
                    ["text"] = text,
                },
            },
            ["isError"] = !result.Ok,
        };
    }
}

using Newtonsoft.Json.Linq;
using System.Collections.Generic;

namespace Ara3D.Bowerbird.RevitSamples.AecAgent;

public sealed class QuerySpec
{
    public List<long> ElementIds { get; set; }
    public string CategoryName { get; set; }
    public string ClassName { get; set; }
    public string LevelName { get; set; }
    public string NameContains { get; set; }
    public int Limit { get; set; } = 200;

    public static QuerySpec FromArgs(JObject args)
    {
        var spec = new QuerySpec
        {
            CategoryName = args?["category"]?.Value<string>(),
            ClassName = args?["class_name"]?.Value<string>(),
            LevelName = args?["level"]?.Value<string>(),
            NameContains = args?["name_contains"]?.Value<string>(),
            Limit = args?["limit"]?.Value<int?>() ?? 200,
        };

        if (args?["element_ids"] is JArray ids)
        {
            spec.ElementIds = [];
            foreach (var id in ids)
            {
                if (id.Type == JTokenType.Integer)
                    spec.ElementIds.Add(id.Value<long>());
            }
        }

        return spec;
    }
}

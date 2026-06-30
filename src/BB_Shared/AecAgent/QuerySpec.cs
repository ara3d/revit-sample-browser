using Newtonsoft.Json.Linq;
using System.Collections.Generic;

namespace Ara3D.Bowerbird.RevitSamples.AecAgent;

public sealed class QuerySpec
{
    public List<long> ElementIds { get; set; }
    public string CategoryName { get; set; }
    public string ClassName { get; set; }
    public string TypeName { get; set; }
    public string LevelName { get; set; }
    public string NameContains { get; set; }
    public string ParameterName { get; set; }
    public string ParameterValue { get; set; }
    public bool ParameterRegex { get; set; }
    public bool VisibleInActiveView { get; set; }
    public int Limit { get; set; } = 200;

    public static QuerySpec FromArgs(JObject args)
    {
        var spec = new QuerySpec
        {
            CategoryName = args?["category"]?.Value<string>(),
            ClassName = args?["class_name"]?.Value<string>(),
            TypeName = args?["type_name"]?.Value<string>(),
            LevelName = args?["level"]?.Value<string>(),
            NameContains = args?["name_contains"]?.Value<string>(),
            ParameterName = args?["parameter_name"]?.Value<string>(),
            ParameterValue = args?["parameter_value"]?.Value<string>(),
            ParameterRegex = args?["parameter_regex"]?.Value<bool?>() ?? false,
            VisibleInActiveView = args?["visible_in_active_view"]?.Value<bool?>() ?? false,
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

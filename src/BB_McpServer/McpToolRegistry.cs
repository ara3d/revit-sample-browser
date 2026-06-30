using Ara3D.Bowerbird.RevitSamples.AecAgent;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Ara3D.Bowerbird.RevitSamples;

public sealed class McpToolRegistry
{
    readonly Dictionary<string, IToolHandler> _handlers = new(StringComparer.OrdinalIgnoreCase);
    readonly List<McpToolDescriptor> _descriptorTools = [];

    public HostContext Context { get; private set; }
    public SampleCatalogIndex Catalog { get; } = new();
    public McpResourceProvider Resources { get; }
    public McpPromptProvider Prompts { get; }

    public McpToolRegistry()
    {
        Resources = new McpResourceProvider(this);
        Prompts = new McpPromptProvider(this);
    }

    public void Initialize(RevitBridge bridge, string repoRoot)
    {
        var audit = new AgentAuditLog();
        var changeSetSession = new ChangeSetSession();
        Catalog.EnsureLoaded(repoRoot);

        Context = new HostContext
        {
            HostName = "revit",
            Bridge = bridge,
            Audit = audit,
            RepoRoot = repoRoot,
            ChangeSetSession = changeSetSession,
            Catalog = Catalog,
        };

        RegisterBuiltInHandlers();
        RegisterDescriptorTools(repoRoot);

        Context = new HostContext
        {
            HostName = "revit",
            Bridge = bridge,
            Audit = audit,
            RepoRoot = repoRoot,
            ChangeSetSession = changeSetSession,
            Catalog = Catalog,
            ToolCount = _handlers.Count,
            ToolNames = _handlers.Keys.OrderBy(k => k).ToArray(),
        };
    }

    void RegisterBuiltInHandlers()
    {
        Register(new EchoHandler());
        Register(new RevitDocumentInfoHandler());
        Register(new GetHostContextHandler());
        Register(new GetActiveViewHandler());
        Register(new ListCapabilitiesHandler());
        Register(new GetSelectionHandler());
        Register(new QueryElementsHandler());
        Register(new ResolveElementRefsHandler());
        Register(new ReadElementsHandler());
        Register(new ReadParametersHandler());
        Register(new GetModelStatisticsHandler());
        Register(new CaptureViewImageHandler());
        Register(new ColorByParameterHandler());
        Register(new ReportStatusHandler());
        Register(new SearchExamplesHandler());
        Register(new StandardsSearchHandler());
        Register(new CreateChangeSetHandler());
        Register(new ValidateChangeSetHandler());
        Register(new PreviewChangesHandler());
        Register(new ApplyChangesHandler());
        Register(new UndoLastAgentChangeHandler());
        Register(new ClassifyOperationRiskHandler());
        Register(new GenerateToolSpecHandler());
        Register(new GenerateBowerbirdScriptHandler());
        Register(new CompileScriptHandler());
        Register(new ReviewToolForSafetyHandler());
        Register(new CreateMcpToolFromScriptHandler());
        Register(new ExportAllJsonHandler());
        Register(new ListRoomsHandler());

        // Geometry
        Register(new CreateDirectShapeBoxHandler());
        Register(new CreateDirectShapeCylinderHandler());
        Register(new CreateDirectShapeExtrusionHandler());
        Register(new GetElementGeometryJsonHandler());
        Register(new ComputeElementBoundingBoxHandler());
        Register(new ComputeMaterialQuantitiesHandler());
        Register(new FindIntersectingElementsHandler());
        Register(new ComputeWallFaceAreasHandler());

        // Massing
        Register(new CreateMassBoxHandler());
        Register(new CreateMassFromProfileHandler());
        Register(new ApplyMassFloorsHandler());
        Register(new ComputeMassPropertiesHandler());
        Register(new CreateCurtainSystemOnFacesHandler());
        Register(new CreateDividedSurfaceHandler());

        // Building creation
        Register(new CreateWallHandler());
        Register(new CreateFloorHandler());
        Register(new CreateCeilingHandler());
        Register(new CreateRoomHandler());
        Register(new CreateColumnHandler());
        Register(new CreateBeamHandler());
        Register(new CreateLevelHandler());
        Register(new CreateGridLineHandler());
        Register(new PlaceFamilyInstanceHandler());
        Register(new CreateRectangularFloorPlanHandler());

        // Views and sheets
        Register(new Create3dViewHandler());
        Register(new CreatePlanViewHandler());
        Register(new CreateSectionViewHandler());
        Register(new CreateSheetHandler());
        Register(new PlaceViewOnSheetHandler());
        Register(new ApplyViewTemplateHandler());
        Register(new ListViewsAndSheetsHandler());
        Register(new IsolateCategoryInViewHandler());

        // Schedules
        Register(new CreateScheduleHandler());
        Register(new ReadScheduleDataHandler());
        Register(new AddScheduleFilterHandler());
        Register(new AddScheduleSortHandler());
        Register(new ExportScheduleCsvHandler());
        Register(new ExportScheduleHtmlHandler());
        Register(new ListSchedulesHandler());
        Register(new DuplicateScheduleHandler());

        // Export / import
        Register(new ExportDwgHandler());
        Register(new ExportIfcHandler());
        Register(new ExportPdfHandler());
        Register(new ExportGbxmlHandler());
        Register(new Export3dMeshJsonHandler());
        Register(new ExportColladaHandler());
        Register(new ExportAllElementsJsonHandler());
        Register(new ExportRoomsJsonHandler());
        Register(new ExportScheduleExcelHandler());

        // Validation
        Register(new GetModelWarningsDetailedHandler());
        Register(new FindUnplacedRoomsHandler());
        Register(new FindElementsWithoutParameterHandler());
        Register(new ValidateNamingConventionHandler());
        Register(new FindLevelLessElementsHandler());
        Register(new CheckRoomDoorRelationshipsHandler());
        Register(new FindOverlappingRoomsHandler());
        Register(new AuditModelHealthHandler());
        Register(new CheckMepSystemCompletenessHandler());

        // Parameters
        Register(new SetParametersBulkHandler());
        Register(new CopyParametersBetweenElementsHandler());
        Register(new ExportParametersCsvHandler());
        Register(new ImportParametersCsvHandler());
        Register(new ListProjectParametersHandler());
        Register(new FindElementsByParameterValueHandler());
        Register(new ComputeParameterStatisticsHandler());
    }

    void RegisterDescriptorTools(string repoRoot)
    {
        _descriptorTools.Clear();
        var manifestPath = FindManifestPath(repoRoot);
        if (string.IsNullOrEmpty(manifestPath) || !System.IO.File.Exists(manifestPath))
            return;

        var manifest = JObject.Parse(System.IO.File.ReadAllText(manifestPath));
        var exposure = manifest["exposure"] as JObject ?? new JObject();
        var maxTools = exposure["maxTools"]?.Value<int?>() ?? 25;
        var tiers = exposure["tiers"] as JArray ?? new JArray("semantic", "bowerbird", "mcp5");
        var tierSet = new HashSet<string>(tiers.Select(t => t.Value<string>()), StringComparer.OrdinalIgnoreCase);

        var added = 0;
        foreach (var item in manifest["tools"] as JArray ?? [])
        {
            if (added >= maxTools) break;
            var tier = item["tier"]?.Value<string>() ?? "descriptor";
            if (!tierSet.Contains(tier)) continue;
            var name = item["name"]?.Value<string>();
            if (string.IsNullOrEmpty(name) || _handlers.ContainsKey(name)) continue;

            var descriptor = DescriptorToolAdapter.CreateDescriptor(item as JObject);
            _descriptorTools.Add(descriptor);
            Register(new DescriptorToolAdapter(descriptor, item as JObject));
            added++;
        }
    }

    static string FindManifestPath(string repoRoot)
    {
        if (!string.IsNullOrEmpty(repoRoot))
        {
            var repoManifest = System.IO.Path.Combine(repoRoot, "src", "BB_McpServer", "mcp-manifest.json");
            if (System.IO.File.Exists(repoManifest))
                return repoManifest;
        }

        var asmDir = System.IO.Path.GetDirectoryName(typeof(McpToolRegistry).Assembly.Location);
        var local = System.IO.Path.Combine(asmDir ?? "", "mcp-manifest.json");
        return System.IO.File.Exists(local) ? local : null;
    }

    public void Register(IToolHandler handler)
    {
        _handlers[handler.Descriptor.Name] = handler;
    }

    public JObject ListTools()
    {
        var tools = _handlers.Values
            .Select(h => new JObject
            {
                ["name"] = h.Descriptor.Name,
                ["description"] = h.Descriptor.Description,
                ["inputSchema"] = h.Descriptor.InputSchema ?? new JObject { ["type"] = "object" },
            })
            .OrderBy(t => t["name"]?.Value<string>())
            .ToList();

        return new JObject { ["tools"] = new JArray(tools) };
    }

    public async Task<JObject> CallToolAsync(string name, JObject arguments)
    {
        if (!_handlers.TryGetValue(name, out var handler))
            throw new McpProtocolException(-32602, $"Unknown tool: {name}");

        McpToolResult result;
        try
        {
            result = await handler.InvokeAsync(arguments ?? new JObject(), Context);
        }
        catch (McpProtocolException)
        {
            throw;
        }
        catch (Exception ex)
        {
            result = McpToolResult.Failure(ex.Message, "handler_exception");
        }

        Context?.Audit.Record("tool", name, result.Ok, new JObject
        {
            ["risk"] = handler.Descriptor.RiskClass.ToString(),
            ["tier"] = handler.Descriptor.Tier,
        });

        return McpJson.ToolCallResult(result);
    }
}

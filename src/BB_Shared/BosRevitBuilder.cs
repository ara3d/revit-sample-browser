using Ara3D.BimOpenSchema;
using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;
using Document = Autodesk.Revit.DB.Document;
using FilePath = Ara3D.Utils.FilePath;

namespace Ara3D.Bowerbird.RevitSamples;

public class BosRevitBuilder
{
    public BosRevitBuilder(
        Options options,
        BimOpenSchemaExportSettings settings,
        Document initialDoc,
        Func<Element, bool> elementFilter)
    {
        InitialDocument = initialDoc;
        BimDataBuilder = new BimDataBuilder();
        Options = options;
        Settings = settings;

        MeshGatherer = new(this);

        // TEMP
        BimDataBuilder.Manifest = new()
        {
            GeneratorApplication = "Revit 2025 BIM Open Schema Parquet Exporter",
            GeneratorVersion = "0.9.5",
            ExportOptions = Settings
        };
        CreateCommonDescriptors();

        BosDocumentContext context = new(this, InitialDocument);

        DocumentContexts = settings.IncludeLinks
            ? context.GatherLinkedDocuments()
            : [context];

        DocumentBuilders = [];
        foreach (var dc in DocumentContexts)
        {
            BosDocumentBuilder db = new(this, dc, elementFilter);
            DocumentBuilders.Add(db);
        }
    }

    public FilePath InputPath
        => new(InitialDocument.PathName);

    public void CreateCommonDescriptors()
    {
        foreach (var p in CommonRevitParameters.GetParameters())
        {
            var desc = BimDataBuilder.AddDescriptor(p.Name, "", "RevitAPI", p.Type);
            DescriptorLookup.Add(p.Name, desc);
        }
    }

    public Document InitialDocument { get; }
    public List<BosDocumentContext> DocumentContexts { get; }
    public List<BosDocumentBuilder> DocumentBuilders { get; }
    public BimDataBuilder BimDataBuilder { get; }
    public BimOpenSchemaExportSettings Settings { get; }
    public Options Options { get; }
    public Dictionary<string, DescriptorIndex> DescriptorLookup = [];
    public BimGeometryBuilder GeometryBuilder { get; } = new();
    public BosMeshGatherer MeshGatherer { get; }
}
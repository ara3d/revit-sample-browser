using Ara3D.Geometry;
using Ara3D.IO.PLY;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.DirectContext3D;
using Autodesk.Revit.DB.ExternalService;
using Autodesk.Revit.UI;
using System;
using System.Diagnostics;
using System.Linq;
using System.Windows.Forms;
using View = Autodesk.Revit.DB.View;

namespace Ara3D.Bowerbird.RevitSamples;

public sealed class CommandDirectContextPlyLoader : NamedCommand, IDirectContext3DServer
{
    public override string Name => "Direct Context Demo - Ply Loader";

    public Guid Guid { get; } = Guid.NewGuid();

    private Outline _boundingBox = new(new XYZ(0, 0, 0), new XYZ(1, 1, 1));

    public override void Execute(object argument)
    {
        var app = (UIApplication)argument;

        LoadPlyFile();
        if (Mesh is null)
            return;

        _boundingBox = Mesh.Bounds;
        
        var svc = ExternalServiceRegistry.GetService(ExternalServices.BuiltInExternalServices.DirectContext3DService);
        svc.AddServer(this);

        if (svc is not MultiServerService ms)
            throw new InvalidOperationException("Expected DirectContext3DService to be a MultiServerService.");

        var ids = ms.GetActiveServerIds();
        if (!ids.Contains(GetServerId()))
            ids.Add(GetServerId());

        ms.SetActiveServers(ids);

        app.ActiveUIDocument?.UpdateAllOpenViews();
    }

    public Guid GetServerId() => Guid;
    public ExternalServiceId GetServiceId() => ExternalServices.BuiltInExternalServices.DirectContext3DService;
    public string GetName() => Name;
    public string GetVendorId() => "Ara 3D Inc.";
    public string GetDescription() => "Demonstrates using the DirectContext3D API";
    public bool CanExecute(View dBView) => dBView.ViewType == ViewType.ThreeD;
    public string GetApplicationId() => "Bowerbird";
    public string GetSourceId() => "";
    public bool UsesHandles() => false;
    public Outline GetBoundingBox(View dBView) => _boundingBox;
    public bool UseInTransparentPass(View dBView) => true;

    public RenderMesh? Mesh { get; private set; }
    private BufferStorage? _faceBuffers;

    public void RenderScene(View dBView, DisplayStyle displayStyle)
    {
        if (Mesh is null) return;

        _faceBuffers ??= new BufferStorage(Mesh);
        _faceBuffers.Render();
    }

    private OpenFileDialog? _plyOpenFileDialog;

    private void LoadPlyFile()
    {
        _plyOpenFileDialog ??= new OpenFileDialog
        {
            DefaultExt = ".ply",
            Filter = "PLY Files (*.ply)|*.ply|All Files (*.*)|*.*",
            Title = "Open PLY File"
        };

        if (_plyOpenFileDialog.ShowDialog() != DialogResult.OK)
            return;

        var plyFile = _plyOpenFileDialog.FileName;
        //var plyFile = @"C:\Users\cdigg\git\draco\testdata\bun_zipper.ply";
        
        var mesh = PlyImporter.LoadMesh(plyFile);

        // Assuming your extension exists:
        Mesh = mesh.ToRenderMesh(new Color32());
        

        Debug.Assert(Mesh.VertexCount > 0);
        Debug.Assert(Mesh.IndexCount > 0);
        Debug.Assert(Mesh.IndexCount % 3 == 0);
    }
}

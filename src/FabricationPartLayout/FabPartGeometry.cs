// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;

namespace Ara3D.RevitSampleBrowser.FabricationPartLayout.CS
{
    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    public class FabPartGeometry : IExternalCommand
    {
        public virtual Result Execute(ExternalCommandData commandData
            , ref string message, ElementSet elements)
        {
            try
            {
                var uidoc = commandData.Application.ActiveUIDocument;
                var doc = uidoc.Document;

                ISet<ElementId> parts = null;

                using (Transaction tr = new(doc, "Optimise Preselection"))
                {
                    tr.Start();
                    var selElems = uidoc.Selection.GetElementIds();
                    if (selElems.Count > 0) parts = new HashSet<ElementId>(selElems);

                    tr.Commit();
                }

                if (parts == null)
                {
                    MessageBox.Show("Select parts to export.");
                    return Result.Failed;
                }

                var callingFolder = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

                FileSaveDialog saveAsDlg = new("CSV Files (*.csv)|*.csv")
                {
                    InitialFileName = $"{callingFolder}\\geomExport",
                    Title = "Save Part Geometry As"
                };
                var result = saveAsDlg.Show();

                if (result == ItemSelectionDialogResult.Canceled)
                    return Result.Cancelled;

                var filename = ModelPathUtils.ConvertModelPathToUserVisiblePath(saveAsDlg.GetSelectedModelPath());
                var ext = Path.GetExtension(filename);
                filename = Path.GetFileNameWithoutExtension(filename);

                int partcount = 1, exported = 0;

                foreach (var eid in parts)
                {
                    if (doc.GetElement(eid) is FabricationPart part)
                    {
                        Options options = new()
                        {
                            DetailLevel = ViewDetailLevel.Coarse
                        };

                        var main = GetMeshes(part.get_Geometry(options));
                        var ins = GetMeshes(part.GetInsulationLiningGeometry());

                        var mlp = 0;
                        foreach (var mesh in main)
                        {
                            var file = $"{filename}{partcount}-main-{++mlp}{ext}";
                            if (ExportMesh(file, mesh))
                                exported++;
                        }

                        var ilp = 0;
                        foreach (var mesh in ins)
                        {
                            var file = $"{filename}{partcount}-ins-{++ilp}{ext}";
                            if (ExportMesh(file, mesh))
                                exported++;
                        }
                    }

                    partcount++;
                }

                var res = exported > 0 ? "Export was successful" : "Nothing was exported";
                var manywritten = $"{exported} Parts were exported";

                TaskDialog td = new("Export Part Mesh Geometry")
                {
                    MainIcon = TaskDialogIcon.TaskDialogIconInformation,
                    TitleAutoPrefix = false,
                    MainInstruction = res,
                    MainContent = manywritten,
                    AllowCancellation = false,
                    CommonButtons = TaskDialogCommonButtons.Ok
                };

                td.Show();

                return Result.Succeeded;
            }
            catch (Exception ex)
            {
                message = ex.Message;
                return Result.Failed;
            }
        }

        private IList<Mesh> GetMeshes(GeometryElement ge)
        {
            IList<Mesh> rv = [];
            if (ge != null)
                foreach (var g in ge)
                {
                    var i = g as GeometryInstance;
                    if (i != null)
                    {
                        var ge2 = i.GetInstanceGeometry();
                        if (ge2 != null)
                            rv = rv.Concat(GetMeshes(ge2)).ToList();
                    }
                    else
                    {
                        var mesh = g as Mesh;
                        if (mesh != null && mesh.Vertices.Count > 0)
                            rv.Add(mesh);
                    }
                }

            return rv;
        }

        private bool ExportMesh(string filename, Mesh mesh)
        {
            var bSaved = false;

            try
            {
                StreamWriter sout = new(filename, false);
                sout.WriteLine("P1X, P1Y, P1Z, P2X, P2Y, P2Z, P3X, P3Y, P3Z");
                for (var tlp = 0; tlp < mesh.NumTriangles; tlp++)
                {
                    var tri = mesh.get_Triangle(tlp);

                    var p1 = mesh.Vertices[(int)tri.get_Index(0)];
                    var p2 = mesh.Vertices[(int)tri.get_Index(1)];
                    var p3 = mesh.Vertices[(int)tri.get_Index(2)];

                    var tstr =
                        $"{p1.X:0.000}, {p1.Y:0.000}, {p1.Z:0.000}, {p2.X:0.000}, {p2.Y:0.000}, {p2.Z:0.000}, {p3.X:0.000}, {p3.Y:0.000}, {p3.Z:0.000}";
                    sout.WriteLine(tstr);
                }

                sout.Close();

                bSaved = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Unable to write file", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            return bSaved;
        }
    }
}

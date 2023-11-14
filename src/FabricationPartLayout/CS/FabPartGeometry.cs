//
// (C) Copyright 2003-2019 by Autodesk, Inc.
//
// Permission to use, copy, modify, and distribute this software in
// object code form for any purpose and without fee is hereby granted,
// provided that the above copyright notice appears in all copies and
// that both that copyright notice and the limited warranty and
// restricted rights notice below appear in all supporting
// documentation.
//
// AUTODESK PROVIDES THIS PROGRAM "AS IS" AND WITH ALL FAULTS.
// AUTODESK SPECIFICALLY DISCLAIMS ANY IMPLIED WARRANTY OF
// MERCHANTABILITY OR FITNESS FOR A PARTICULAR USE. AUTODESK, INC.
// DOES NOT WARRANT THAT THE OPERATION OF THE PROGRAM WILL BE
// UNINTERRUPTED OR ERROR FREE.
//
// Use, duplication, or disclosure by the U.S. Government is subject to
// restrictions set forth in FAR 52.227-19 (Commercial Computer
// Software - Restricted Rights) and DFAR 252.227-7013(c)(1)(ii)
// (Rights in Technical Data and Computer Software), as applicable.
//

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace Revit.SDK.Samples.FabricationPartLayout.CS
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
                // check user selection
                var uidoc = commandData.Application.ActiveUIDocument;
                var doc = uidoc.Document;

                ISet<ElementId> parts = null;

                using (var tr = new Transaction(doc, "Optimise Preselection"))
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

                var saveAsDlg = new FileSaveDialog("CSV Files (*.csv)|*.csv");

                saveAsDlg.InitialFileName = callingFolder + "\\geomExport";
                saveAsDlg.Title = "Save Part Geometry As";
                var result = saveAsDlg.Show();

                if (result == ItemSelectionDialogResult.Canceled)
                    return Result.Cancelled;

                var filename = ModelPathUtils.ConvertModelPathToUserVisiblePath(saveAsDlg.GetSelectedModelPath());
                var ext = Path.GetExtension(filename);
                filename = Path.GetFileNameWithoutExtension(filename);

                int partcount = 1, exported = 0;

                foreach (var eid in parts)
                {
                    // get all rods and kist with rods 
                    var part = doc.GetElement(eid) as FabricationPart;
                    if (part != null)
                    {
                        var options = new Options();
                        options.DetailLevel = ViewDetailLevel.Coarse;

                        var main = getMeshes(part.get_Geometry(options));
                        var ins = getMeshes(part.GetInsulationLiningGeometry());

                        var mlp = 0;
                        foreach (var mesh in main)
                        {
                            var file = string.Concat(filename, partcount.ToString(), "-main-", (++mlp).ToString(), ext);
                            if (exportMesh(file, mesh))
                                exported++;
                        }

                        var ilp = 0;
                        foreach (var mesh in ins)
                        {
                            var file = string.Concat(filename, partcount.ToString(), "-ins-", (++ilp).ToString(), ext);
                            if (exportMesh(file, mesh))
                                exported++;
                        }
                    }

                    partcount++;
                }

                var res = exported > 0 ? "Export was successful" : "Nothing was exported";
                var manywritten = string.Format("{0} Parts were exported", exported);

                var td = new TaskDialog("Export Part Mesh Geometry")
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

        private IList<Mesh> getMeshes(GeometryElement ge)
        {
            IList<Mesh> rv = new List<Mesh>();
            if (ge != null)
                foreach (var g in ge)
                {
                    var i = g as GeometryInstance;
                    if (i != null)
                    {
                        var ge2 = i.GetInstanceGeometry();
                        if (ge2 != null)
                            rv = rv.Concat(getMeshes(ge2)).ToList();
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

        private bool exportMesh(string filename, Mesh mesh)
        {
            var bSaved = false;

            try
            {
                var sout = new StreamWriter(filename, false);
                sout.WriteLine("P1X, P1Y, P1Z, P2X, P2Y, P2Z, P3X, P3Y, P3Z");
                for (var tlp = 0; tlp < mesh.NumTriangles; tlp++)
                {
                    var tri = mesh.get_Triangle(tlp);

                    var p1 = mesh.Vertices[(int)tri.get_Index(0)];
                    var p2 = mesh.Vertices[(int)tri.get_Index(1)];
                    var p3 = mesh.Vertices[(int)tri.get_Index(2)];

                    var tstr = string.Format(
                        "{0:0.000}, {1:0.000}, {2:0.000}, {3:0.000}, {4:0.000}, {5:0.000}, {6:0.000}, {7:0.000}, {8:0.000}",
                        p1.X, p1.Y, p1.Z, p2.X, p2.Y, p2.Z, p3.X, p3.Y, p3.Z);
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
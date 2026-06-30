// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt
// Adapted from CustomExporterAdnMeshJson by Jeremy Tammik (MIT).
// https://github.com/jeremytammik/CustomExporterAdnMeshJson

using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;

namespace Ara3D.RevitSampleBrowser.CustomExporter.AdnMeshJsonExporter.CS
{
    [Transaction(TransactionMode.ReadOnly)]
    [Regeneration(RegenerationOption.Manual)]
    [Journaling(JournalingMode.NoCommandData)]
    public class Command : IExternalCommand
    {
        public Result Execute(
            ExternalCommandData commandData,
            ref string message,
            ElementSet elements)
        {
            var uiapp = commandData.Application;
            var uidoc = uiapp.ActiveUIDocument;

            if (null == uidoc)
            {
                message = "Please run this command in an active project document.";
                return Result.Failed;
            }

            var doc = uidoc.Document;

            if (doc.ActiveView is not View3D view)
            {
                message = "Please run this command in a 3D view.";
                return Result.Failed;
            }

            ExportContextAdnMesh context = new(doc);

            using (Autodesk.Revit.DB.CustomExporter exporter = new(doc, context))
            {
                exporter.IncludeGeometricObjects = false;

                try
                {
                    exporter.Export(view);
                }
                catch (Autodesk.Revit.Exceptions.ExternalApplicationException ex)
                {
                    Debug.Print("ExternalApplicationException " + ex.Message);
                    message = ex.Message;
                    return Result.Failed;
                }
            }

            var meshData = context.MeshData;

            using SaveFileDialog saveDialog = new();
            saveDialog.Filter = "JSON files (*.json)|*.json";
            saveDialog.FileName = "adn_mesh_export.json";
            saveDialog.InitialDirectory = Environment.GetFolderPath(
                Environment.SpecialFolder.MyDocuments);

            if (saveDialog.ShowDialog() != DialogResult.OK)
            {
                return Result.Cancelled;
            }

            WriteMeshJson(saveDialog.FileName, meshData);

            TaskDialog td = new("ADN Mesh JSON Export")
            {
                MainInstruction = "Export complete.",
                MainContent = string.Format(
                    "Exported {0} mesh object{1} to:\n{2}",
                    meshData.Length,
                    meshData.Length == 1 ? "" : "s",
                    saveDialog.FileName)
            };
            td.Show();

            return Result.Succeeded;
        }

        static void WriteMeshJson(string path, AdnMeshData[] meshData)
        {
            using StreamWriter writer = new(path);
            writer.Write("[");

            for (var i = 0; i < meshData.Length; ++i)
            {
                if (0 < i)
                {
                    writer.Write(',');
                }

                writer.Write(meshData[i].ToJson());
            }

            writer.Write("\n]\n");
        }
    }
}

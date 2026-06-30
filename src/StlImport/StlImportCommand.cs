// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt
//
// Adapted from StlImport by Scott Conover, Autodesk Inc. (MIT).
// https://github.com/jeremytammik/StlImport

using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using QuantumConcepts.Formats.StereoLithography;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;

namespace Ara3D.RevitSampleBrowser.StlImport.CS
{
    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    [Journaling(JournalingMode.NoCommandData)]
    public class StlImportCommand : IExternalCommand
    {
        static readonly ElementId CategoryForDirectShape
            = new(BuiltInCategory.OST_GenericModel);

        public Result Execute(
            ExternalCommandData commandData,
            ref string message,
            ElementSet elements)
        {
            var uidoc = commandData.Application.ActiveUIDocument;
            if (uidoc == null)
            {
                message = "Please run this command in an active project document.";
                return Result.Failed;
            }

            var doc = uidoc.Document;
            var applicationId = commandData.Application.ActiveAddInId
                .GetGUID().ToString();

            var filename = SelectStlFile(GetInitialStlDirectory());
            if (filename == null)
            {
                return Result.Cancelled;
            }

            var documentName = Path.GetFileNameWithoutExtension(filename);
            var properties = StlImportProperties.GetProperties();

            STLDocument stlDocument;
            try
            {
                stlDocument = ReadStlDocument(filename, properties.Binary);
            }
            catch (Exception ex)
            {
                message = $"Failed to read STL file: {ex.Message}";
                return Result.Failed;
            }

            DirectShape directShape;
            try
            {
                directShape = ImportStlDocument(
                    stlDocument,
                    doc,
                    documentName,
                    applicationId,
                    properties);
            }
            catch (Exception ex)
            {
                message = $"Failed to import STL geometry: {ex.Message}";
                return Result.Failed;
            }

            TaskDialog.Show(
                "STL Import",
                $"Created DirectShape element {directShape.Id.Value} "
                + $"from {Path.GetFileName(filename)} "
                + $"({stlDocument.Facets.Count} facets).");

            return Result.Succeeded;
        }

        static string GetInitialStlDirectory()
        {
            return Environment.GetFolderPath(
                Environment.SpecialFolder.MyDocuments);
        }

        static string SelectStlFile(string initialDirectory)
        {
            using var dialog = new OpenFileDialog
            {
                Filter = "STL files (*.stl)|*.stl|All files (*.*)|*.*",
                InitialDirectory = initialDirectory,
                Title = "Select STL file"
            };

            return dialog.ShowDialog() == DialogResult.OK
                ? dialog.FileName
                : null;
        }

        static STLDocument ReadStlDocument(string filename, bool binary)
        {
            if (binary)
            {
                using var stream = File.OpenRead(filename);
                using var reader = new BinaryReader(stream);
                return STLDocument.Read(reader);
            }

            using var textStream = File.OpenText(filename);
            return STLDocument.Read(textStream);
        }

        static DirectShape ImportStlDocument(
            STLDocument stlDocument,
            Document doc,
            string stlDocumentName,
            string applicationId,
            StlImportProperties properties)
        {
            using var transaction = new Transaction(doc, "Import STL");
            transaction.Start();

            var builder = new TessellatedShapeBuilder();
            builder.OpenConnectedFaceSet(false);

            foreach (var facet in stlDocument.Facets)
            {
                builder.AddFace(FromFacet(facet));
            }

            builder.CloseConnectedFaceSet();
            builder.Target = properties.Target;
            builder.Fallback = properties.Fallback;
            builder.GraphicsStyleId = properties.GraphicsStyleId;
            builder.Build();

            var result = builder.GetBuildResult();
            var directShape = DirectShape.CreateElement(
                doc, CategoryForDirectShape);
            directShape.ApplicationId = applicationId;
            directShape.ApplicationDataId = stlDocumentName;
            directShape.SetShape(result.GetGeometricalObjects());
            directShape.Name = stlDocumentName;

            transaction.Commit();
            return directShape;
        }

        static TessellatedFace FromFacet(Facet facet)
        {
            var vertices = new List<XYZ>(3);
            foreach (var vertex in facet.Vertices)
            {
                vertices.Add(new XYZ(
                    (double)vertex.X,
                    (double)vertex.Y,
                    (double)vertex.Z));
            }

            return new TessellatedFace(
                vertices, ElementId.InvalidElementId);
        }
    }
}

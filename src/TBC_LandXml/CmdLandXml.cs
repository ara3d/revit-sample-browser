#region Header

//
// CmdLandXml.cs - import LandXML data and create TopographySurface
//
// Copyright (C) 2010-2021 by Jeremy Tammik,
// Autodesk Inc. All rights reserved.
//
// Keywords: The Building Coder Revit API C# .NET add-in.
//

#endregion // Header

#region Namespaces

using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Xml;
using W = System.Windows.Forms;

#endregion // Namespaces

namespace BuildingCoder
{
    [Transaction(TransactionMode.Manual)]
    internal class CmdLandXml : IExternalCommand
    {
        public Result Execute(
            ExternalCommandData commandData,
            ref string message,
            ElementSet elements)
        {
            var app = commandData.Application;
            var doc = app.ActiveUIDocument.Document;

            OpenFileDialog dlg = new()
            {
                // select file to open

                Filter = "LandXML files (*.xml)|*.xml",

                Title = "Import LandXML and "
                            + "Create TopographySurface"
            };

            if (dlg.ShowDialog() != W.DialogResult.OK) return Result.Cancelled;

            XmlDocument xmlDoc = new();
            xmlDoc.Load(dlg.FileName);

            var pts = Util.ParseLandXmlPoints(xmlDoc);

            using Transaction t = new(doc);
            t.Start("Create Topography Surface");

            //TopographySurface surface = doc.Create.NewTopographySurface( pntList );

            //TopographySurface surface = doc.Create.NewTopographySurface( pts ); // 2013


            ElementId idType = null;
            ElementId idLevel = null;
            var toposolid = Toposolid.Create(doc, pts, idType, idLevel);

            t.Commit();

            return Result.Succeeded;
        }
    }
}

// C:\a\doc\revit\blog\zip\LandXMLfiles\GSG_features_surfaces_with_volumes.xml
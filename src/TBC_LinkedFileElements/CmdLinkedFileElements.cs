#region Header

//
// CmdLinkedFileElements.cs - list elements in linked files
//
// Copyright (C) 2009-2021 by Jeremy Tammik,
// Autodesk Inc. All rights reserved.
//
// Keywords: The Building Coder Revit API C# .NET add-in.
//

#endregion // Header

#region Namespaces

using System;
using System.Collections.Generic;
using System.Linq;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;

#endregion // Namespaces

namespace BuildingCoder
{
    public class ElementData
    {
        private readonly double _x;
        private readonly double _y;
        private readonly double _z;

        public ElementData(
            string path,
            string elementName,
            long id,
            double x,
            double y,
            double z,
            string uniqueId)
        {
            var i = path.LastIndexOf("\\");
            Document = path.Substring(i + 1);
            Element = elementName;
            Id = id;
            _x = x;
            _y = y;
            _z = z;
            UniqueId = uniqueId;
            Folder = path.Substring(0, i);
        }

        public string Document { get; }

        public string Element { get; }

        public long Id { get; }

        public string X => Util.RealString(_x);

        public string Y => Util.RealString(_y);

        public string Z => Util.RealString(_z);

        public string UniqueId { get; }

        public string Folder { get; }
    }

    [Transaction(TransactionMode.ReadOnly)]
    internal class CmdLinkedFileElements : IExternalCommand
    {
        public Result Execute(
            ExternalCommandData commandData,
            ref string message,
            ElementSet highlightElements)
        {
            /*
      
            // retrieve all link elements:
      
            Document doc = app.ActiveUIDocument.Document;
            List<Element> links = GetElements(
              BuiltInCategory.OST_RvtLinks,
              typeof( Instance ), app, doc );
      
            // determine the link paths:
      
            DocumentSet docs = app.Documents;
            int n = docs.Size;
            Dictionary<string, string> paths
              = new Dictionary<string, string>( n );
      
            foreach( Document d in docs )
            {
              string path = d.PathName;
              int i = path.LastIndexOf( "\\" ) + 1;
              string name = path.Substring( i );
              paths.Add( name, path );
            }
            */

            // Retrieve lighting fixture element
            // data from linked documents:

            var data = new List<ElementData>();
            var app = commandData.Application;
            var docs = app.Application.Documents;

            foreach (Document doc in docs)
            {
                var a
                    = Util.GetElementsOfType(doc,
                        typeof(FamilyInstance),
                        BuiltInCategory.OST_LightingFixtures);

                foreach (FamilyInstance e in a)
                {
                    var name = e.Name;
                    if (e.Location is LocationPoint lp)
                    {
                        var p = lp.Point;
                        data.Add(new ElementData(doc.PathName, e.Name,
                            e.Id.Value, p.X, p.Y, p.Z, e.UniqueId));
                    }
                }
            }

            // Display data:

            using var dlg = new CmdLinkedFileElementsForm(data);
            dlg.ShowDialog();

            return Result.Succeeded;
        }
    }
}
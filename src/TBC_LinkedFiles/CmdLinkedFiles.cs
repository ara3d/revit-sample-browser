#region Header

//
// CmdLinkedFiles.cs - retrieve linked files
// in current project
//
// Copyright (C) 2008-2021 by Jeremy Tammik,
// Autodesk Inc. All rights reserved.
//
// Keywords: The Building Coder Revit API C# .NET add-in.
//

#endregion // Header

#region Namespaces

using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

#endregion // Namespaces

namespace BuildingCoder
{
    [Transaction(TransactionMode.ReadOnly)]
    internal class CmdLinkedFiles : IExternalCommand
    {
        public Result Execute(
            ExternalCommandData commandData,
            ref string message,
            ElementSet elements)
        {
            var uiapp = commandData.Application;
            var app = uiapp.Application;
            var doc = uiapp.ActiveUIDocument.Document;

            var dict
                = Util.GetFilePaths(app, true);

            var links
                = Util.GetElementsOfType(doc,
                    typeof(Instance),
                    BuiltInCategory.OST_RvtLinks).ToElements();

            var n = links.Count;
            Debug.Print(
                "There {0} {1} linked Revit model{2}.",
                1 == n ? "is" : "are", n,
                Util.PluralSuffix(n));

            string name;
            var sep = new[] { ':' };
            string[] a;

            foreach (var link in links)
            {
                name = link.Name;
                a = name.Split(sep);
                name = a[0].Trim();

                Debug.Print(
                    "Link '{0}' full path is '{1}'.",
                    name, dict[name]);

                #region Explore Location

                var loc = link.Location; // unknown content in here
                if (loc is LocationPoint lp)
                {
                    var p = lp.Point;
                }

                var e = link.get_Geometry(new Options());
                if (null != e) // no geometry defined
                    //n = objects.Size; // 2012
                    n = e.Count(); // 2013

                #endregion // Explore Location

                #region Explore Pinning

                if (link is ImportInstance instance) // nope, this never happens ...
                {
                    var s = instance.Pinned ? "" : "not ";
                    Debug.Print("{1}pinned", s);
                    instance.Pinned = !instance.Pinned;
                }

                #endregion // Explore Pinning
            }

            return Result.Succeeded;
        }
    }
}
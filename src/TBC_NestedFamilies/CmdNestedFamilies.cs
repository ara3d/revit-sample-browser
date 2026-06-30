#region Header

//
// CmdNestedFamilies.cs - list nested family files and instances in a family document
//
// Copyright (C) 2010-2021 by Jeremy Tammik, Autodesk Inc. All rights reserved.
//
// Keywords: The Building Coder Revit API C# .NET add-in.
//

#endregion // Header

#region Namespaces

using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System.Collections.Generic;
using System.Diagnostics;

#endregion // Namespaces

namespace BuildingCoder
{
    [Transaction(TransactionMode.ReadOnly)]
    internal class CmdNestedFamilies : IExternalCommand
    {
        public Result Execute(
            ExternalCommandData commandData,
            ref string message,
            ElementSet elements)
        {
            var app = commandData.Application;
            var doc = app.ActiveUIDocument.Document;

            var familyFilenameFilter = string.Empty;
            var typeNameFilter = string.Empty;
            var caseSensitive = false;

            var nestedFamilies
                = Util.GetFilteredNestedFamilyDefinitions(
                    familyFilenameFilter, doc, caseSensitive);

            foreach (var f in nestedFamilies) Debug.WriteLine(f.Name);

            var instances
                = Util.GetFilteredNestedFamilyInstances(
                    familyFilenameFilter, typeNameFilter, doc, caseSensitive);

            foreach (var fi in instances) Debug.WriteLine(Util.ElementDescription(fi));

            return Result.Failed;
        }
    }
}

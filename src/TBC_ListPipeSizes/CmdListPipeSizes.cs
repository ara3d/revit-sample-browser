#region Header

//
// CmdListPipeSizes.cs - list pipe sizes in a project
//
// Copyright (C) 2015-2021 by Jeremy Tammik,
// Autodesk Inc. All rights reserved.
//
// Keywords: The Building Coder Revit API C# .NET add-in.
//

#endregion // Header

#region Namespaces

using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

#endregion // Namespaces

namespace BuildingCoder
{
    [Transaction(TransactionMode.ReadOnly)]
    internal class CmdListPipeSizes : IExternalCommand
    {
        private const string _filename = "C:/pipesizes.txt";

        public Result Execute(
            ExternalCommandData commandData,
            ref string message,
            ElementSet elements)
        {
            var app = commandData.Application;
            var uidoc = app.ActiveUIDocument;
            var doc = uidoc.Document;
            Util.GetPipeSegmentSizes(doc, _filename);
            return Result.Succeeded;
        }
    }
}
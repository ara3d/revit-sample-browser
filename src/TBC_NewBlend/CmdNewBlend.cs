#region Header

//
// CmdNewBlend.cs - create a new blend element using the NewBlend method
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

#endregion // Namespaces

namespace BuildingCoder
{
    [Transaction(TransactionMode.Manual)]
    internal class CmdNewBlend : IExternalCommand
    {
        public Result Execute(
            ExternalCommandData commandData,
            ref string message,
            ElementSet elements)
        {
            var app = commandData.Application;
            var doc = app.ActiveUIDocument.Document;

            if (doc.IsFamilyDocument)
            {
                using var t = new Transaction(doc);
                t.Start("New Blend");

                Util.CreateBlend(doc);

                t.Commit();

                return Result.Succeeded;
            }

            message = "Please run this command "
                      + "in a family document.";

            return Result.Failed;
        }
    }
}
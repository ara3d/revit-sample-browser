#region Header

//
// CmdNewSweptBlend.cs - create a new swept blend element
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
    internal class CmdNewSweptBlend : IExternalCommand
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
                using var tx = new Transaction(doc);
                tx.Start("Create New Swept Blend");

                Util.CreateNewSweptBlend(doc);
                Util.CreateNewSweptBlendArc(doc);
                Util.CreateSweepWithMultipleLoops(doc);
                tx.Commit();

                return Result.Succeeded;
            }

            message
                = "Please run this command in a family document.";

            return Result.Failed;
        }

        #region Create Sweep from FamilySymbolProfile

        // https://forums.autodesk.com/t5/revit-api-forum/can-t-create-sweep-from-familysymbolprofile/m-p/9591593

        #endregion // Create Sweep from FamilySymbolProfile
    }
}
#region Header

//
// CmdNewLightingFixture.cs - insert new lighting fixture family instance
//
// Copyright (C) 2009-2021 by Jeremy Tammik,
// Autodesk Inc. All rights reserved.
//
// Keywords: The Building Coder Revit API C# .NET add-in.
//

#endregion // Header

#region Namespaces

using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Structure;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;

#endregion // Namespaces

namespace BuildingCoder
{
    [Transaction(TransactionMode.Manual)]
    internal class CmdNewLightingFixture : IExternalCommand
    {
        public Result Execute(
            ExternalCommandData commandData,
            ref string message,
            ElementSet elements)
        {
            var uiapp = commandData.Application;
            var uidoc = uiapp.ActiveUIDocument;
            var app = uiapp.Application;
            var doc = uidoc.Document;

            // Get a lighting fixture family symbol:

            var symbols
                = Util.GetElementsOfType(doc,
                    typeof(FamilySymbol),
                    BuiltInCategory.OST_LightingFixtures);

            if (symbols.FirstElement() is not FamilySymbol sym)
            {
                message = "No lighting fixture symbol found.";
                return Result.Failed;
            }

            // Pick the ceiling:

#if _2010
      uidoc.Selection.StatusbarTip
        = "Please select ceiling to host lighting fixture";

      uidoc.Selection.PickOne();

      Element ceiling = null;

      foreach( Element e in uidoc.Selection.Elements )
      {
        ceiling = e as Element;
        break;
      }
#endif // _2010

            var r = uidoc.Selection.PickObject(
                ObjectType.Element,
                "Please select ceiling to host lighting fixture");

            if (null == r)
            {
                message = "Nothing selected.";
                return Result.Failed;
            }

            // 'Autodesk.Revit.DB.Reference.Element' is
            // obsolete: Property will be removed. Use
            // Document.GetElement(Reference) instead.
            //Element ceiling = r.Element; // 2011

            Element ceiling = doc.GetElement(r) as Wall; // 2012

            // Get the level 1:

            if (Util.GetFirstElementOfTypeNamed(
                doc, typeof(Level), "Level 1") is not Level level)
            {
                message = "Level 1 not found.";
                return Result.Failed;
            }

            // Create the family instance:

            var p = app.Create.NewXYZ(-43, 28, 0);

            using Transaction t = new(doc);
            t.Start("Place New Lighting Fixture Instance");

            var instLight
                = doc.Create.NewFamilyInstance(
                    p, sym, ceiling, level,
                    StructuralType.NonStructural);

            t.Commit();

            return Result.Succeeded;
        }

        #region PlaceFamilyInstanceOnFace

        // See Util.PlaceFamilyInstanceOnFace

        #endregion // PlaceFamilyInstanceOnFace
    }
}
#region Header

//
// CmdCategories.cs - list document and built-in categories
//
// Copyright (C) 2010-2021 by Jeremy Tammik, Autodesk Inc. All rights reserved.
//
// Keywords: The Building Coder Revit API C# .NET add-in.
//

#endregion // Header

#region Namespaces

using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

#endregion // Namespaces

namespace BuildingCoder
{
    [Transaction(TransactionMode.ReadOnly)]
    internal class CmdCategories : IExternalCommand
    {
        public Result Execute(
            ExternalCommandData commandData,
            ref string message,
            ElementSet elements)
        {
            var uiapp = commandData.Application;
            var app = uiapp.Application;
            var doc = uiapp.ActiveUIDocument.Document;
            var categories = doc.Settings.Categories;

            var nCategories = categories.Size;

            Debug.Print(
                "{0} categories and their parents obtained "
                + "from the Categories collection:",
                nCategories);

            foreach (Category c in categories)
            {
                var p = c.Parent;

                Debug.Print("  {0} ({1}), parent {2}",
                    c.Name, c.Id.Value,
                    null == p ? "<none>" : p.Name);
            }

            var bics = Enum.GetValues(
                typeof(BuiltInCategory));

            var nBics = bics.Length;

            Debug.Print("{0} built-in categories and the "
                        + "corresponding document ones:", nBics);

            Category cat;
            string s;

            var bics_null
                = new List<BuiltInCategory>();

            var bics_exception
                = new List<BuiltInCategory>();

            foreach (BuiltInCategory bic in bics)
            {
                try
                {
                    cat = categories.get_Item(bic);

                    if (null == cat)
                    {
                        bics_null.Add(bic);
                        s = "<null>";
                    }
                    else
                    {
                        s = $"{cat.Name} ({cat.Id.Value})";
                    }
                }
                catch (Exception ex)
                {
                    bics_exception.Add(bic);

                    s = $"{ex.GetType().Name} {ex.Message}";
                }

                Debug.Print("  {0} --> {1}",
                    bic.ToString(), s);
            }

            var nBicsNull = bics_null.Count;
            var nBicsException = bics_exception.Count;

#if ACCESS_HIDDEN_CATEGORIES_THROUGH_FILTERED_ELEMENT_COLLECTOR
      // Trying to use OfClass( typeof( Category ) )
      // throws an ArgumentException exception saying
      // "Input type Category is not a recognized 
      // Revit API type".

      IEnumerable<Category> cats
        = new FilteredElementCollector( doc )
          .WhereElementIsNotElementType()
          .WhereElementIsViewIndependent()
          .Cast<Category>();

      // Unable to cast object of type 
      // 'Autodesk.Revit.DB.Element' to type 
      // 'Autodesk.Revit.DB.Category':

      int nCategoriesFiltered = cats.Count<Category>();

      Debug.Print(
        "{0} categories obtained from a filtered "
        + "element collector:",
        nCategoriesFiltered );

      foreach( Category c in cats )
      {
        Debug.Print( "  {0}", c.Name );
      }
#endif // ACCESS_HIDDEN_CATEGORIES_THROUGH_FILTERED_ELEMENT_COLLECTOR

            var dlg = new TaskDialog(
                "Hidden Built-in Categories");

            s =
                $"{nCategories} categories obtained from the Categories collection;\r\n{nBics} built-in categories;\r\n{nBicsNull} built-in categories retrieve null result;\r\n{nBicsException} built-in categories throw an exception:\r\n";

            Debug.Print(s);

            dlg.MainInstruction = s;

            s = bics_exception
                .Aggregate(
                    string.Empty,
                    (a, bic) => $"{a}\n{bic}");

            Debug.Print(s);

            dlg.MainContent = s;

            dlg.Show();

            return Result.Succeeded;
        }
    }
}
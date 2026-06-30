#region Copyright
// (C) Copyright 2011-2014 by Autodesk, Inc.
//
// Permission to use, copy, modify, and distribute this software
// in object code form for any purpose and without fee is hereby
// granted, provided that the above copyright notice appears in
// all copies and that both that copyright notice and the limited
// warranty and restricted rights notice below appear in all
// supporting documentation.
//
// AUTODESK PROVIDES THIS PROGRAM "AS IS" AND WITH ALL FAULTS.
// AUTODESK SPECIFICALLY DISCLAIMS ANY IMPLIED WARRANTY OF
// MERCHANTABILITY OR FITNESS FOR A PARTICULAR USE.  AUTODESK,
// INC. DOES NOT WARRANT THAT THE OPERATION OF THE PROGRAM WILL
// BE UNINTERRUPTED OR ERROR FREE.
//
// Use, duplication, or disclosure by the U.S. Government is
// subject to restrictions set forth in FAR 52.227-19 (Commercial
// Computer Software - Restricted Rights) and DFAR 252.227-7013(c)
// (1)(ii)(Rights in Technical Data and Computer Software), as
// applicable.
#endregion // Copyright

#region Namespaces
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.Windows.Forms;
#endregion

namespace Ara3D.RevitSampleBrowser.StringSearch.CS
{
    [Transaction(TransactionMode.ReadOnly)]
    [Regeneration(RegenerationOption.Manual)]
    [Journaling(JournalingMode.NoCommandData)]
    public class Command : IExternalCommand
    {
        public const string ProductName = "Revit String Search";

        public static void InfoMsg(string msg)
        {
            TaskDialog.Show(ProductName, msg);
        }

        public Result Execute(
            ExternalCommandData commandData,
            ref string message,
            ElementSet elements)
        {
            var uiapp = commandData.Application;
            var uidoc = uiapp.ActiveUIDocument;
            var doc = uidoc.Document;

            StringSearchHost.EnsureInitialized(uiapp);

            var selids = uidoc.Selection.GetElementIds();

            try
            {
                using JtLogFile log = new("SearchString");
                SortableBindingList<SearchHit> data = null;

                while (data == null || data.Count == 0)
                {
                    SearchForm form = new(log.Path);

                    var r = form.ShowDialog();

                    if (r == DialogResult.Cancel)
                    {
                        message = string.Empty;
                        return Result.Cancelled;
                    }

                    if (form.CurrentSelection && selids.Count == 0)
                    {
                        InfoMsg("Sorry; you cannot search the current element selection, because it is empty.");
                        continue;
                    }

                    #region Set up filtered element collector
                    var a
                        = form.CurrentView
                            ? new FilteredElementCollector(doc, doc.ActiveView.Id)
                        : form.CurrentSelection
                            ? new FilteredElementCollector(doc, uidoc.Selection.GetElementIds())
                        : new FilteredElementCollector(doc);

                    if (form.ElementType && form.NonElementType)
                    {
                        a.WhereElementIsElementType();

                        var b = form.CurrentView
                            ? new FilteredElementCollector(doc, doc.ActiveView.Id)
                            : new FilteredElementCollector(doc);

                        b.WhereElementIsNotElementType();

                        a.UnionWith(b);
                    }
                    else if (form.ElementType)
                    {
                        a.WhereElementIsElementType();
                    }
                    else if (form.NonElementType)
                    {
                        a.WhereElementIsNotElementType();
                    }
                    else
                    {
                        message = "Please select at least one or both of Element type and non-Element type.";
                        return Result.Failed;
                    }

                    if (!form.AllCategories)
                    {
                        var bic = (BuiltInCategory)Enum.Parse(
                            typeof(BuiltInCategory),
                            form.CategoryName);

                        a.OfCategory(bic);
                    }
                    #endregion // Set up filtered element collector

                    StringSearcher ss = new(a, form.SearchOptions);

                    try
                    {
                        data = ss.Run(log, out message);

                        if (data.Count == 0)
                        {
                            InfoMsg(message.Length > 0 ? message : "No occurrences found.");
                        }
                    }
                    catch (ArgumentException ex)
                    {
                        if (ex.StackTrace != null &&
                            ex.StackTrace.Contains("RegularExpressions.RegexParser.ScanRegex"))
                        {
                            InfoMsg("Invalid regular expression. Error message:\r\n"
                                + ex.Message
                                + "\r\nIf you don't know what a regular expression is, don't use it"
                                + "\r\n(cheat sheet: http://regexlib.com/cheatsheet.aspx).");
                        }
                        else
                        {
                            throw;
                        }
                    }
                }

                StringSearchHost.ShowNavigator(data);

                return Result.Succeeded;
            }
            catch (Exception ex)
            {
                message = ex.Message;
                return Result.Failed;
            }
        }
    }
}

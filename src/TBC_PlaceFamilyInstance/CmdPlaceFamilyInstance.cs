#region Header

//
// CmdPlaceFamilyInstance.cs - call PromptForFamilyInstancePlacement
// to place family instances and use the DocumentChanged event to
// capture the newly added element ids
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
using Autodesk.Revit.DB.Events;
using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows.Forms;
using OperationCanceledException = Autodesk.Revit.Exceptions.OperationCanceledException;

#endregion // Namespaces

namespace BuildingCoder
{
    [Transaction(TransactionMode.Manual)]
    internal class CmdPlaceFamilyInstance : IExternalCommand
    {
        private static readonly bool Place_one_single_instance_then_abort
            = true;

        private readonly List<ElementId> _added_element_ids
            = [];
        private IntPtr _revit_window;

        public Result Execute(
            ExternalCommandData commandData,
            ref string message,
            ElementSet elements)
        {
            var uiapp = commandData.Application;
            var uidoc = uiapp.ActiveUIDocument;
            var app = uiapp.Application;
            var doc = uidoc.Document;

            _revit_window = uiapp.MainWindowHandle;

            FilteredElementCollector collector
                = new(doc);

            collector.OfCategory(BuiltInCategory.OST_Doors);
            collector.OfClass(typeof(FamilySymbol));

            var symbol = collector.FirstElement()
                as FamilySymbol;

            _added_element_ids.Clear();

            app.DocumentChanged
                += OnDocumentChanged;

            try
            {
                uidoc.PromptForFamilyInstancePlacement(symbol);
            }
            catch (OperationCanceledException ex)
            {
                Debug.Print(ex.Message);
            }

            app.DocumentChanged
                -= OnDocumentChanged;

            var n = _added_element_ids.Count;

            Autodesk.Revit.UI.TaskDialog.Show(
                "Place Family Instance",
                $"{n} element{(1 == n ? "" : "s")} added.");

            return Result.Succeeded;
        }

        private void OnDocumentChanged(
            object sender,
            DocumentChangedEventArgs e)
        {
            var idsAdded
                = e.GetAddedElementIds();

            var n = idsAdded.Count;

            Debug.Print("{0} id{1} added.",
                n, Util.PluralSuffix(n));

            // Append each batch; assignment would overwrite prior ids.

            _added_element_ids.AddRange(idsAdded);

            if (Place_one_single_instance_then_abort
                && 0 < n)
            {

                Press.PostMessage(_revit_window,
                    (uint)Press.KEYBOARD_MSG.WM_KEYDOWN,
                    (uint)Keys.Escape, 0);

                Press.PostMessage(_revit_window,
                    (uint)Press.KEYBOARD_MSG.WM_KEYDOWN,
                    (uint)Keys.Escape, 0);
            }
        }
    }
}
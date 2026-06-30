// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Structure;
using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace Ara3D.RevitSampleBrowser.TagBeam.CS
{
    [Transaction(TransactionMode.Manual)]
    [Journaling(JournalingMode.NoCommandData)]
    public class Command : IExternalCommand
    {
        public Result Execute(
            ExternalCommandData commandData,
            ref string message,
            ElementSet elements)
        {
            try
            {
                //prepare data
                TagBeamData dataBuffer = new(commandData);

                // show UI
                using TagBeamForm displayForm = new(dataBuffer);
                var result = displayForm.ShowDialog();
                return DialogResult.OK != result ? Result.Cancelled : Result.Succeeded;
            }
            catch (Exception e)
            {
                message = e.Message;
                return Result.Failed;
            }
        }
    }

    [Transaction(TransactionMode.Manual)]
    public class TagRebar : IExternalCommand
    {
        public Result Execute(
            ExternalCommandData commandData,
            ref string message,
            ElementSet elements)
        {
            try
            {
                // Get the active document and view
                var revitDoc = commandData.Application.ActiveUIDocument;
                var view = revitDoc.Document.ActiveView;
                foreach (var elemId in revitDoc.Selection.GetElementIds())
                {
                    var elem = revitDoc.Document.GetElement(elemId);
                    if (elem.GetType() == typeof(Rebar))
                    {
                        // cast to Rebar and get its first curve
                        var rebar = (Rebar)elem;
                        var curve = rebar.GetCenterlineCurves(false, false, false,
                            MultiplanarOption.IncludeAllMultiplanarCurves, 0)[0];
                        var subelements = rebar.GetSubelements();

                        // create a rebar tag at the first end point of the first curve
                        using Transaction t = new(revitDoc.Document);
                        t.Start("Create new tag");
                        IndependentTag.Create(revitDoc.Document, view.Id, subelements[0].GetReference(), true,
                            TagMode.TM_ADDBY_CATEGORY,
                            TagOrientation.Horizontal, curve.GetEndPoint(0));
                        t.Commit();

                        return Result.Succeeded;
                    }
                }

                message = "No rebar selected!";
                return Result.Failed;
            }
            catch (Exception e)
            {
                message = e.Message;
                return Result.Failed;
            }
        }
    }

    [Transaction(TransactionMode.Manual)]
    public class CreateText : IExternalCommand
    {
        public Result Execute(
            ExternalCommandData commandData,
            ref string message,
            ElementSet elements)
        {
            try
            {
                // get the active document and view
                var revitDoc = commandData.Application.ActiveUIDocument;
                var view = revitDoc.ActiveView;
                var dbDoc = revitDoc.Document;
                var currentTextTypeId = dbDoc.GetDefaultElementTypeId(ElementTypeGroup.TextNoteType);

                foreach (var elemId in revitDoc.Selection.GetElementIds())
                {
                    var elem = dbDoc.GetElement(elemId);
                    if (elem.GetType() == typeof(Rebar))
                    {
                        // cast to Rebar and get its first curve
                        var rebar = (Rebar)elem;
                        var curve = rebar.GetCenterlineCurves(false, false, false,
                            MultiplanarOption.IncludeAllMultiplanarCurves, 0)[0];

                        // calculate necessary arguments
                        XYZ origin = new(
                            curve.GetEndPoint(0).X + curve.Length,
                            curve.GetEndPoint(0).Y,
                            curve.GetEndPoint(0).Z);
                        var strText = $"This is {rebar.Category.Name} : {rebar.Name}";

                        // create the text
                        using Transaction t = new(dbDoc);
                        t.Start("New text note");
                        TextNote.Create(dbDoc, view.Id, origin, strText, currentTextTypeId);
                        t.Commit();

                        return Result.Succeeded;
                    }
                }

                message = "No rebar selected!";
                return Result.Failed;
            }
            catch (Exception e)
            {
                message = e.Message;
                return Result.Failed;
            }
        }
    }
}

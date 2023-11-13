//
// (C) Copyright 2003-2019 by Autodesk, Inc.
//
// Permission to use, copy, modify, and distribute this software in
// object code form for any purpose and without fee is hereby granted,
// provided that the above copyright notice appears in all copies and
// that both that copyright notice and the limited warranty and
// restricted rights notice below appear in all supporting
// documentation.
//
// AUTODESK PROVIDES THIS PROGRAM "AS IS" AND WITH ALL FAULTS.
// AUTODESK SPECIFICALLY DISCLAIMS ANY IMPLIED WARRANTY OF
// MERCHANTABILITY OR FITNESS FOR A PARTICULAR USE. AUTODESK, INC.
// DOES NOT WARRANT THAT THE OPERATION OF THE PROGRAM WILL BE
// UNINTERRUPTED OR ERROR FREE.
//
// Use, duplication, or disclosure by the U.S. Government is subject to
// restrictions set forth in FAR 52.227-19 (Commercial Computer
// Software - Restricted Rights) and DFAR 252.227-7013(c)(1)(ii)
// (Rights in Technical Data and Computer Software), as applicable.
//

using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

using Autodesk.Revit;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.DB.Structure;

namespace Revit.SDK.Samples.TagBeam.CS
{
    /// <summary>
    /// Implements the Revit add-in interface IExternalCommand
    /// </summary>
    [Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]
    [Autodesk.Revit.Attributes.Journaling(Autodesk.Revit.Attributes.JournalingMode.NoCommandData)]
    public class Command : IExternalCommand
    {
        #region IExternalCommand Members Implementation
        /// <summary>
        /// Implement this method as an external command for Revit.
        /// </summary>
        /// <param name="commandData">An object that is passed to the external application 
        /// which contains data related to the command, 
        /// such as the application object and active view.</param>
        /// <param name="message">A message that can be set by the external application 
        /// which will be displayed if a failure or cancellation is returned by 
        /// the external command.</param>
        /// <param name="elements">A set of elements to which the external application 
        /// can add elements that are to be highlighted in case of failure or cancellation.</param>
        /// <returns>Return the status of the external command. 
        /// A result of Succeeded means that the API external method functioned as expected. 
        /// Cancelled can be used to signify that the user cancelled the external operation 
        /// at some point. Failure should be returned if the application is unable to proceed with 
        /// the operation.</returns>
        public Autodesk.Revit.UI.Result Execute(
            ExternalCommandData commandData,
            ref string message,
            ElementSet elements)
        {
            try
            {
                //prepare data
                var dataBuffer = new TagBeamData(commandData);

                // show UI
                using (var displayForm = new TagBeamForm(dataBuffer))
                {
                    var result = displayForm.ShowDialog();
                    if (DialogResult.OK != result)
                    {
                        return Autodesk.Revit.UI.Result.Cancelled;
                    }
                }

                return Autodesk.Revit.UI.Result.Succeeded;
            }
            catch (Exception e)
            {
                message = e.Message;
                return Autodesk.Revit.UI.Result.Failed;
            }
        }
        #endregion IExternalCommand Members Implementation
    }

    [Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]
    public class TagRebar : IExternalCommand
    {
        public Autodesk.Revit.UI.Result Execute(
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
                    if (elem.GetType() == typeof(Autodesk.Revit.DB.Structure.Rebar))
                    {
                        // cast to Rebar and get its first curve
                        var rebar = (Autodesk.Revit.DB.Structure.Rebar)elem;
                        var curve = rebar.GetCenterlineCurves(false, false, false,MultiplanarOption.IncludeAllMultiplanarCurves,0)[0];
                        var subelements = rebar.GetSubelements();  

                        // create a rebar tag at the first end point of the first curve
                        using( var t = new Transaction(revitDoc.Document))
                        {
                           t.Start("Create new tag");
                           var tag = IndependentTag.Create(revitDoc.Document, view.Id, subelements[0].GetReference(), true,
                               Autodesk.Revit.DB.TagMode.TM_ADDBY_CATEGORY,
                               Autodesk.Revit.DB.TagOrientation.Horizontal, curve.GetEndPoint(0));
                           t.Commit();
                        }
                        return Autodesk.Revit.UI.Result.Succeeded;
                    }
                }
                message = "No rebar selected!";
                return Autodesk.Revit.UI.Result.Failed;
            }
            catch (Exception e)
            {
                message = e.Message;
                return Autodesk.Revit.UI.Result.Failed;
            }
        }
    }

    [Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]
    public class CreateText : IExternalCommand
    {
        public Autodesk.Revit.UI.Result Execute(
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
                    if (elem.GetType() == typeof(Autodesk.Revit.DB.Structure.Rebar))
                    {
                        // cast to Rebar and get its first curve
                        var rebar = (Autodesk.Revit.DB.Structure.Rebar)elem;
                        var curve = rebar.GetCenterlineCurves(false, false, false, MultiplanarOption.IncludeAllMultiplanarCurves, 0)[0];

                        // calculate necessary arguments
                        var origin = new XYZ(
                            curve.GetEndPoint(0).X + curve.Length,
                            curve.GetEndPoint(0).Y,
                            curve.GetEndPoint(0).Z);
                        var strText = "This is " + rebar.Category.Name + " : " + rebar.Name;

                        // create the text
                        using( var t = new Transaction(dbDoc))
                        {
                           t.Start("New text note");
                           TextNote.Create(dbDoc, view.Id, origin, strText, currentTextTypeId);
                           t.Commit();
                        }
                        return Autodesk.Revit.UI.Result.Succeeded;
                    }
                }
                message = "No rebar selected!";
                return Autodesk.Revit.UI.Result.Failed;
            }
            catch (Exception e)
            {
                message = e.Message;
                return Autodesk.Revit.UI.Result.Failed;
            }
        }
    }
}

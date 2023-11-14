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
using System.Windows.Forms;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace Revit.SDK.Samples.DoorSwing.CS
{
    /// <summary>
    ///     This command will add needed shared parameters and initialize them.
    ///     It will initialize door opening parameter based on family's actual geometry and
    ///     country's standard. It will initialize each door instance's opening, ToRoom, FromRoom and
    ///     internal door flag values according to door's current geometry.
    /// </summary>
    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    [Journaling(JournalingMode.NoCommandData)]
    public class InitializeCommand : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData,
            ref string message,
            ElementSet elements)
        {
            var returnCode = Result.Cancelled;

            var tran = new Transaction(commandData.Application.ActiveUIDocument.Document, "Initialize Command");
            tran.Start();

            try
            {
                // one instance of DoorSwingData class.
                var databuffer = new DoorSwingData(commandData.Application);

                using (var initForm = new InitializeForm(databuffer))
                {
                    // Show UI
                    var dialogResult = initForm.ShowDialog();

                    if (DialogResult.OK == dialogResult)
                    {
                        databuffer.DeleteTempDoorInstances();

                        // update door type's opening feature based on family's actual geometry and 
                        // country's standard.
                        databuffer.UpdateDoorFamiliesOpeningFeature();

                        // update each door instance's Opening feature and internal door flag
                        returnCode = DoorSwingData.UpdateDoorsInfo(commandData.Application.ActiveUIDocument.Document,
                            false, true, ref message);
                    }
                }
            }
            catch (Exception ex)
            {
                // if there is anything wrong, give error information and return failed.
                message = ex.Message;
                returnCode = Result.Failed;
            }

            if (Result.Succeeded == returnCode)
                tran.Commit();
            else
                tran.RollBack();
            return returnCode;
        }
    }

    /// <summary>
    ///     A ExternalCommand class inherited IExternalCommand interface.
    ///     This command will update each door instance's opening, ToRoom, FromRoom and
    ///     internal door flag values according to door's current geometry.
    /// </summary>
    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    public class UpdateParamsCommand : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            var returnCode = Result.Succeeded;
            var app = commandData.Application;
            var doc = app.ActiveUIDocument;
            var tran = new Transaction(doc.Document, "Update Parameters Command");
            tran.Start();

            try
            {
                var elementSet = new ElementSet();
                foreach (var elementId in doc.Selection.GetElementIds())
                    elementSet.Insert(doc.Document.GetElement(elementId));
                if (elementSet.IsEmpty)
                    returnCode = DoorSwingData.UpdateDoorsInfo(doc.Document, false, true, ref message);
                else
                    returnCode = DoorSwingData.UpdateDoorsInfo(doc.Document, true, true, ref message);
            }
            catch (Exception ex)
            {
                // if there is anything wrong, give error information and return failed.
                message = ex.Message;
                returnCode = Result.Failed;
            }

            if (Result.Succeeded == returnCode)
                tran.Commit();
            else
                tran.RollBack();
            return returnCode;
        }
    }

    /// <summary>
    ///     A ExternalCommand class inherited IExternalCommand interface.
    ///     This command will update door instance's geometry according to door's
    ///     current To/From Room value.
    /// </summary>
    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    public class UpdateGeometryCommand : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            var returnCode = Result.Succeeded;
            var app = commandData.Application;
            var doc = app.ActiveUIDocument;
            var tran = new Transaction(doc.Document, "Update Geometry Command");
            tran.Start();

            try
            {
                var elementSet = new ElementSet();
                foreach (var elementId in doc.Selection.GetElementIds())
                    elementSet.Insert(doc.Document.GetElement(elementId));
                if (elementSet.IsEmpty)
                    DoorSwingData.UpdateDoorsGeometry(doc.Document, false);
                else
                    DoorSwingData.UpdateDoorsGeometry(doc.Document, true);

                returnCode = Result.Succeeded;
            }
            catch (Exception ex)
            {
                // if there is anything wrong, give error information and return failed.
                message = ex.Message;
                returnCode = Result.Failed;
            }

            if (Result.Succeeded == returnCode)
                tran.Commit();
            else
                tran.RollBack();

            return returnCode;
        }
    }
}
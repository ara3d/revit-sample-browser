// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using System;
using System.Windows.Forms;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace Ara3D.RevitSampleBrowser.DoorSwing.CS
{
    /// <summary>
    ///     This command will add needed shared parameters and initialize them.
    ///     It will initialize door opening parameter based on family's actual geometry and
    ///     country's standard. It will initialize each door instance's opening, ToRoom, FromRoom and
    ///     public door flag values according to door's current geometry.
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

                        // update each door instance's Opening feature and public door flag
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
    ///     public door flag values according to door's current geometry.
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
                {
                    elementSet.Insert(doc.Document.GetElement(elementId));
                }

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
                {
                    elementSet.Insert(doc.Document.GetElement(elementId));
                }

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

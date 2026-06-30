// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System;
using System.Windows.Forms;

namespace Ara3D.RevitSampleBrowser.DoorSwing.CS
{
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

            Transaction tran = new(commandData.Application.ActiveUIDocument.Document, "Initialize Command");
            tran.Start();

            try
            {
                // one instance of DoorSwingData class.
                DoorSwingData databuffer = new(commandData.Application);

                using InitializeForm initForm = new(databuffer);
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

    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    public class UpdateParamsCommand : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            var returnCode = Result.Succeeded;
            var app = commandData.Application;
            var doc = app.ActiveUIDocument;
            Transaction tran = new(doc.Document, "Update Parameters Command");
            tran.Start();

            try
            {
                ElementSet elementSet = new();
                foreach (var elementId in doc.Selection.GetElementIds())
                {
                    elementSet.Insert(doc.Document.GetElement(elementId));
                }

                returnCode = elementSet.IsEmpty
                    ? DoorSwingData.UpdateDoorsInfo(doc.Document, false, true, ref message)
                    : DoorSwingData.UpdateDoorsInfo(doc.Document, true, true, ref message);
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

    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    public class UpdateGeometryCommand : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            var returnCode = Result.Succeeded;
            var app = commandData.Application;
            var doc = app.ActiveUIDocument;
            Transaction tran = new(doc.Document, "Update Geometry Command");
            tran.Start();

            try
            {
                ElementSet elementSet = new();
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

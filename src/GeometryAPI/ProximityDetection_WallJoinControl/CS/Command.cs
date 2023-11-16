// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using System;
using System.Windows.Forms;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace Ara3D.RevitSampleBrowser.GeometryAPI.ProximityDetection_WallJoinControl.CS
{
    [Transaction(TransactionMode.Manual)]
    public class Command : IExternalCommand
    {
        public virtual Result Execute(ExternalCommandData commandData
            , ref string message, ElementSet elements)
        {
            try
            {
                var application = commandData.Application.Application;
                var document = commandData.Application.ActiveUIDocument.Document;

                // Create an object that is responsible for proximity detection
                var proximityDetection = ProximityDetection.GetInstance(application, document);

                // Create an object that is responsible for controlling the joint of walls
                var walljoinControl = WallJoinControl.GetInstance(application, document);

                // Create the UI for users select operation and view the results.
                using (var form =
                       new ProximityDetectionAndWallJoinControlForm(document, proximityDetection, walljoinControl))
                {
                    // show dialog to browser operations and results
                    if (DialogResult.OK != form.ShowDialog()) return Result.Cancelled;
                }

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

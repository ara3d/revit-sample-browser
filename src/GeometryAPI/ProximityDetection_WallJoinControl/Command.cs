// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System;
using System.Windows.Forms;

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

                var proximityDetection = ProximityDetection.GetInstance(application, document);
                var walljoinControl = WallJoinControl.GetInstance(application, document);

                using ProximityDetectionAndWallJoinControlForm form =
                       new(document, proximityDetection, walljoinControl);
                return DialogResult.OK != form.ShowDialog() ? Result.Cancelled : Result.Succeeded;
            }
            catch (Exception ex)
            {
                message = ex.Message;
                return Result.Failed;
            }
        }
    }
}

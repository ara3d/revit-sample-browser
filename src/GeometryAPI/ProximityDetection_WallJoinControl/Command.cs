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

                var proximityDetection = ProximityDetection.GetInstance(application, document);
                var walljoinControl = WallJoinControl.GetInstance(application, document);

                using (var form =
                       new ProximityDetectionAndWallJoinControlForm(document, proximityDetection, walljoinControl))
                {
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

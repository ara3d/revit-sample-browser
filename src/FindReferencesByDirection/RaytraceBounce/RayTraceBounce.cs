// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using System;
using System.Collections.Generic;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

using Ara3D.RevitSampleBrowser.Common.Documents;
using Ara3D.RevitSampleBrowser.Common.Geometry;
using Ara3D.RevitSampleBrowser.Common.Infrastructure;
namespace Ara3D.RevitSampleBrowser.FindReferencesByDirection.RaytraceBounce.CS
{
    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    [Journaling(JournalingMode.NoCommandData)]
    public class Command : IExternalCommand
    {
        private UIApplication m_app;
        private View3D m_view;

        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            try
            {
                // Requires a document line style named "bounce" before running.
                m_app = commandData.Application;
                m_view = ElementQuery.Get3DView(m_app.ActiveUIDocument.Document);
                if (m_view == null)
                {
                    TaskDialog.Show("Revit", "A default 3D view (named {3D}) must exist before running this command");
                    return Result.Cancelled;
                }

                var form = new RayTraceBounceForm(commandData, m_view);
                form.ShowDialog();
                return Result.Succeeded;
            }
            catch (Exception e)
            {
                message = e.ToString();
                return Result.Failed;
            }
        }
    }
}

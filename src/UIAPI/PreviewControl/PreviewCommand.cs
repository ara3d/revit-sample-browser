// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System;

namespace Ara3D.RevitSampleBrowser.UIAPI.CS.PreviewControl
{
    [Transaction(TransactionMode.Manual)]
    public class PreviewCommand : IExternalCommand
    {
        private Document m_dbdocument;

        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            m_dbdocument = commandData.Application.ActiveUIDocument.Document;

            TransactionGroup outerGroup = new(m_dbdocument, "preview control");
            outerGroup.Start();

            try
            {
                PreviewModel form = new(commandData.Application.Application, ElementId.InvalidElementId);
                form.ShowDialog();
            }
            catch (Exception e)
            {
                throw e;
            }
            finally
            {
                outerGroup.RollBack();
            }

            return Result.Succeeded;
        }
    }
}

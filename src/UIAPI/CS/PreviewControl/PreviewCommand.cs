// Copyright 2023. See https://github.com/ara3d/revit-samples/LICENSE.txt

using System;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace Revit.SDK.Samples.UIAPI.CS
{
    [Transaction(TransactionMode.Manual)]
    public class PreviewCommand : IExternalCommand
    {
        private Document _dbdocument;

        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            _dbdocument = commandData.Application.ActiveUIDocument.Document;

            var outerGroup = new TransactionGroup(_dbdocument, "preview control");
            outerGroup.Start();

            try
            {
                var form = new PreviewModel(commandData.Application.Application, ElementId.InvalidElementId);
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

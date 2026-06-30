// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using Ara3D.RevitSampleBrowser.Common.Views;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System;
namespace Ara3D.RevitSampleBrowser.ViewTemplateCreation.CS
{
    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    [Journaling(JournalingMode.NoCommandData)]
    public class Command : IExternalCommand
    {
        public virtual Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            try
            {
                var document = commandData.Application.ActiveUIDocument.Document;
                ViewTemplateCreationForm form = new(document);
                form.ShowDialog();

                return Result.Succeeded;
            }
            catch (Exception ex)
            {
                ViewTemplateHelper.ShowWarningMessageBox(ex.ToString());
                return Result.Failed;
            }
        }
    }
}

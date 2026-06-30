// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System.Windows.Forms;

namespace Ara3D.RevitSampleBrowser.GeometryAPI.EnergyAnalysisModel.CS
{
    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    [Journaling(JournalingMode.NoCommandData)]
    public class Command : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData,
            ref string message, ElementSet elements)
        {
            Transaction trans = new(commandData.Application.ActiveUIDocument.Document,
                "Ara3D.RevitSampleBrowser.EnergyAnalysisModel");
            trans.Start();
            EnergyAnalysisModel analysisModel = new(commandData.Application.ActiveUIDocument.Document);

            using (OptionsAndAnalysisForm form = new(analysisModel))
            {
                analysisModel.Initialize();
                if (DialogResult.OK != form.ShowDialog())
                {
                    trans.RollBack();
                    return Result.Cancelled;
                }
            }

            trans.Commit();
            return Result.Succeeded;
        }
    }
}

// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using System.Windows.Forms;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

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
            var trans = new Transaction(commandData.Application.ActiveUIDocument.Document,
                "Ara3D.RevitSampleBrowser.EnergyAnalysisModel");
            trans.Start();
            var analysisModel = new EnergyAnalysisModel(commandData.Application.ActiveUIDocument.Document);

            using (var form = new OptionsAndAnalysisForm(analysisModel))
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

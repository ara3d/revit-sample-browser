// Copyright 2023. See https://github.com/ara3d/revit-samples/LICENSE.txt

using System.Windows.Forms;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace RevitMultiSample.EnergyAnalysisModel.CS
{
    /// <summary>
    ///     A class inherits IExternalCommand interface.
    ///     this class controls the class which subscribes handle events and the events' information UI.
    ///     like a bridge between them.
    /// </summary>
    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    [Journaling(JournalingMode.NoCommandData)]
    public class Command : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData,
            ref string message, ElementSet elements)
        {
            var trans = new Transaction(commandData.Application.ActiveUIDocument.Document,
                "RevitMultiSample.EnergyAnalysisModel");
            trans.Start();
            // Create an object that is responsible for collecting users inputs and getting analysis data of current model.
            var analysisModel = new EnergyAnalysisModel(commandData.Application.ActiveUIDocument.Document);

            // Create the UI for users inputs options and view analysis models.
            using (var form = new OptionsAndAnalysisForm(analysisModel))
            {
                // make analysis data ready
                analysisModel.Initialize();
                // show dialog to browser analysis model
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

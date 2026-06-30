// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using Ara3D.RevitSampleBrowser.NewRoof.CS.RoofForms;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System;
using System.Windows.Forms;
using View = Autodesk.Revit.DB.View;

namespace Ara3D.RevitSampleBrowser.NewRoof.CS
{
    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    [Journaling(JournalingMode.NoCommandData)]
    public class Command : IExternalCommand
    {
        // public buffer

        public static View ActiveView { get; private set; }

        public Result Execute(ExternalCommandData commandData,
            ref string message,
            ElementSet elements)
        {
            try
            {
                ActiveView = commandData.Application.ActiveUIDocument.Document.ActiveView;

                RoofsManager.RoofsManager roofsManager = new(commandData);
                LevelConverter.SetStandardValues(roofsManager.Levels);

                // Create a form to create and edit a roof.
                var result = DialogResult.None;
                while (result is DialogResult.None or DialogResult.Retry)
                {
                    if (result == DialogResult.Retry) roofsManager.WindowSelect();

                    using RoofForm mainForm = new(roofsManager);
                    result = mainForm.ShowDialog();
                }

                return result == DialogResult.OK ? Result.Succeeded : Result.Cancelled;
            }
            catch (Exception ex)
            {
                // If there are something wrong, give error information and return failed
                message = ex.Message;
                return Result.Failed;
            }
        }
    }
}

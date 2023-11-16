// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using System;
using System.Windows.Forms;
using Ara3D.RevitSampleBrowser.NewRoof.CS.RoofForms;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using View = Autodesk.Revit.DB.View;

namespace Ara3D.RevitSampleBrowser.NewRoof.CS
{
    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    [Journaling(JournalingMode.NoCommandData)]
    public class Command : IExternalCommand
    {
        // internal buffer

        /// <summary>
        ///     singleton in the external application
        /// </summary>
        public static View ActiveView { get; private set; }

        public Result Execute(ExternalCommandData commandData,
            ref string message,
            ElementSet elements)
        {
            try
            {
                ActiveView = commandData.Application.ActiveUIDocument.Document.ActiveView;

                //// Create a new instance of class DataManager
                var roofsManager = new RoofsManager.RoofsManager(commandData);
                LevelConverter.SetStandardValues(roofsManager.Levels);

                // Create a form to create and edit a roof.
                var result = DialogResult.None;
                while (result == DialogResult.None || result == DialogResult.Retry)
                {
                    if (result == DialogResult.Retry) roofsManager.WindowSelect();

                    using (var mainForm = new RoofForm(roofsManager))
                    {
                        result = mainForm.ShowDialog();
                    }
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

// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System;
using System.Windows.Forms;

namespace Ara3D.RevitSampleBrowser.VisibilityControl.CS
{
    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    [Journaling(JournalingMode.NoCommandData)]
    public class Command : IExternalCommand
    {
        public virtual Result Execute(ExternalCommandData commandData
            , ref string message, ElementSet elements)
        {
            Transaction trans = new(commandData.Application.ActiveUIDocument.Document,
                "Ara3D.RevitSampleBrowser.VisibilityControl");
            trans.Start();
            try
            {
                if (commandData == null)
                {
                    trans.RollBack();
                    throw new ArgumentNullException(nameof(commandData));
                }

                // create an instance of VisibilityCtrl
                VisibilityCtrl visiController = new(commandData.Application.ActiveUIDocument);

                // create a user interface form
                using (VisibilityCtrlForm dlg = new(visiController))
                {
                    // show dialog
                    var result = dlg.ShowDialog();

                    switch (result)
                    {
                        case DialogResult.OK:
                            trans.Commit();
                            return Result.Succeeded;
                        case DialogResult.Yes:
                            // isolate the selected element(s)
                            visiController.Isolate();
                            trans.Commit();
                            return Result.Succeeded;
                    }
                }

                trans.RollBack();
                return Result.Cancelled;
            }
            catch (Exception ex)
            {
                message = ex.Message;
                trans.RollBack();
                return Result.Failed;
            }
        }
    }
}

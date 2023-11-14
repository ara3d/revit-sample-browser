// Copyright 2023. See https://github.com/ara3d/revit-samples/LICENSE.txt

using System;
using System.Windows.Forms;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace Revit.SDK.Samples.VisibilityControl.CS
{
    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    [Journaling(JournalingMode.NoCommandData)]
    public class Command : IExternalCommand
    {
        public virtual Result Execute(ExternalCommandData commandData
            , ref string message, ElementSet elements)
        {
            var trans = new Transaction(commandData.Application.ActiveUIDocument.Document,
                "Revit.SDK.Samples.VisibilityControl");
            trans.Start();
            try
            {
                if (null == commandData)
                {
                    trans.RollBack();
                    throw new ArgumentNullException(nameof(commandData));
                }

                // create an instance of VisibilityCtrl
                var visiController = new VisibilityCtrl(commandData.Application.ActiveUIDocument);

                // create a user interface form
                using (var dlg = new VisibilityCtrlForm(visiController))
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

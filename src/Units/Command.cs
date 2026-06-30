// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System;
using System.Windows.Forms;

namespace Ara3D.RevitSampleBrowser.Units.CS
{
    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    [Journaling(JournalingMode.NoCommandData)]
    public class Command : IExternalCommand
    {
        public virtual Result Execute(ExternalCommandData commandData
            , ref string message, ElementSet elements)
        {
            try
            {
                var document = commandData.Application.ActiveUIDocument.Document;
                var units = document.GetUnits();

                // show UI
                using UnitsForm displayForm = new(units);
                var result = displayForm.ShowDialog();
                if (DialogResult.OK == result)
                    using (Transaction tran = new(document, "SetUnits"))
                    {
                        tran.Start();
                        document.SetUnits(units);
                        tran.Commit();
                    }
                else
                    return Result.Cancelled;

                return Result.Succeeded;
            }
            catch (Exception ex)
            {
                message = ex.Message;
                return Result.Failed;
            }
        }
    }
}

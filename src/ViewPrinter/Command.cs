// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using System;
using System.Windows.Forms;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace Ara3D.RevitSampleBrowser.ViewPrinter.CS
{
    /// <summary>
    ///     To add an external command to Autodesk Revit
    ///     the developer should implement an object that
    ///     supports the IExternalCommand interface.
    /// </summary>
    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    [Journaling(JournalingMode.NoCommandData)]
    public class Command : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData,
            ref string message, ElementSet elements)
        {
            Transaction newTran = null;
            try
            {
                newTran = new Transaction(commandData.Application.ActiveUIDocument.Document, "ViewPrinter");
                newTran.Start();

                var pMgr = new PrintMgr(commandData);

                if (null == pMgr.InstalledPrinterNames)
                {
                    PrintMgr.MyMessageBox("No installed printer, the external command can't work.");
                    return Result.Cancelled;
                }

                using (var pmDlg = new PrintMgrForm(pMgr))
                {
                    if (pmDlg.ShowDialog() != DialogResult.Cancel)
                    {
                        newTran.Commit();
                        return Result.Succeeded;
                    }

                    newTran.RollBack();
                }
            }
            catch (Exception ex)
            {
                newTran?.RollBack();
                message = ex.ToString();
                return Result.Failed;
            }

            return Result.Cancelled;
        }
    }
}

//
// (C) Copyright 2003-2019 by Autodesk, Inc.
//
// Permission to use, copy, modify, and distribute this software in
// object code form for any purpose and without fee is hereby granted,
// provided that the above copyright notice appears in all copies and
// that both that copyright notice and the limited warranty and
// restricted rights notice below appear in all supporting
// documentation.
//
// AUTODESK PROVIDES THIS PROGRAM "AS IS" AND WITH ALL FAULTS.
// AUTODESK SPECIFICALLY DISCLAIMS ANY IMPLIED WARRANTY OF
// MERCHANTABILITY OR FITNESS FOR A PARTICULAR USE. AUTODESK, INC.
// DOES NOT WARRANT THAT THE OPERATION OF THE PROGRAM WILL BE
// UNINTERRUPTED OR ERROR FREE.
//
// Use, duplication, or disclosure by the U.S. Government is subject to
// restrictions set forth in FAR 52.227-19 (Commercial Computer
// Software - Restricted Rights) and DFAR 252.227-7013(c)(1)(ii)
// (Rights in Technical Data and Computer Software), as applicable.
//

using System;
using System.Windows.Forms;
using Autodesk.Revit.UI;

namespace Revit.SDK.Samples.ViewPrinter.CS
{
    /// <summary>
    /// To add an external command to Autodesk Revit 
    /// the developer should implement an object that 
    /// supports the IExternalCommand interface.
    /// </summary>
    [Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]
    [Autodesk.Revit.Attributes.Regeneration(Autodesk.Revit.Attributes.RegenerationOption.Manual)]
    [Autodesk.Revit.Attributes.Journaling(Autodesk.Revit.Attributes.JournalingMode.NoCommandData)]
    public class Command : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData,
        ref string message, Autodesk.Revit.DB.ElementSet elements)
        {
            Autodesk.Revit.DB.Transaction newTran = null;
            try
            {
                newTran = new Autodesk.Revit.DB.Transaction(commandData.Application.ActiveUIDocument.Document, "ViewPrinter");
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
                if (null != newTran)
                    newTran.RollBack();
                message = ex.ToString();
                return Result.Failed;
            }

            return Result.Cancelled;
        }

    }
}

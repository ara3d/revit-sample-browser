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
                    throw new ArgumentNullException("commandData");
                }

                // create an instance of VisibilityCtrl
                var visiController = new VisibilityCtrl(commandData.Application.ActiveUIDocument);

                // create a user interface form
                using (var dlg = new VisibilityCtrlForm(visiController))
                {
                    // show dialog
                    var result = dlg.ShowDialog();

                    if (result == DialogResult.OK)
                    {
                        trans.Commit();
                        return Result.Succeeded;
                    }

                    if (result == DialogResult.Yes)
                    {
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
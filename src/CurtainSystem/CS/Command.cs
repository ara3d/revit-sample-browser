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
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Revit.SDK.Samples.CurtainSystem.CS.CurtainSystem;
using Revit.SDK.Samples.CurtainSystem.CS.Data;
using Revit.SDK.Samples.CurtainSystem.CS.Properties;
using Revit.SDK.Samples.CurtainSystem.CS.UI;

namespace Revit.SDK.Samples.CurtainSystem.CS
{
    /// <summary>
    ///     the entry point of the sample (to launch the sample dialog and allows further operations)
    /// </summary>
    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    [Journaling(JournalingMode.NoCommandData)]
    internal class Command : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            // data verification
            if (null == commandData.Application.ActiveUIDocument) return Result.Failed;

            var mydocument = new MyDocument(commandData);


            // check whether the mass is kind of parallelepiped
            var checker = new MassChecker(mydocument);
            var validMass = checker.CheckSelectedMass();

            if (!validMass)
            {
                message = Resources.MSG_InvalidSelection;
                return Result.Cancelled;
            }

            CurtainForm curtainForm = null;
            var transactionGroup = new TransactionGroup(commandData.Application.ActiveUIDocument.Document);
            try
            {
                transactionGroup.Start("CurtainSystemOperation");
                curtainForm = new CurtainForm(mydocument);

                if (null != curtainForm && false == curtainForm.IsDisposed) curtainForm.ShowDialog();

                transactionGroup.Commit();
            }
            catch (Exception ex)
            {
                transactionGroup.RollBack();
                message = ex.Message;
                return Result.Failed;
            }
            finally
            {
                if (null != curtainForm && false == curtainForm.IsDisposed) curtainForm.Dispose();
            }

            return Result.Succeeded;
        }
    }
}
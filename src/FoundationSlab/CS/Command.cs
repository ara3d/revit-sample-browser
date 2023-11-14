//
// (C) Copyright 2003-2019 by Autodesk, Inc.
//
// Permission to use, copy, modify, and distribute this software in
// object code form for any purpose and without fee is hereby granted
// provided that the above copyright notice appears in all copies and
// that both that copyright notice and the limited warranty and
// restricted rights notice below appear in all supporting
// documentation.
//
// AUTODESK PROVIDES THIS PROGRAM 'AS IS' AND WITH ALL ITS FAULTS.
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
using Autodesk.Revit.UI;

namespace Revit.SDK.Samples.FoundationSlab.CS
{
    [Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]
   [Autodesk.Revit.Attributes.Regeneration(Autodesk.Revit.Attributes.RegenerationOption.Manual)]
    [Autodesk.Revit.Attributes.Journaling(Autodesk.Revit.Attributes.JournalingMode.NoCommandData)]
    public class Command : IExternalCommand
    {
        #region IExternalCommand Members Implementation
        public Result Execute(ExternalCommandData commandData, ref string message, Autodesk.Revit.DB.ElementSet elements)
        {
            try
            {
                // Check commandData parameter.
                if (null == commandData)
                {
                    return Result.Failed;
                }

                SlabData revitDatas = null;
                try
                {
                    // The Datas for UI.
                    revitDatas = new SlabData(commandData.Application);
                }
                catch (NullReferenceException e)
                {
                    message = e.Message;
                    return Result.Cancelled;  // Data error.
                }
                // Display form.
                using (var displayForm = new FoundationSlabForm(revitDatas))
                {
                    if (displayForm.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                    {
                        return Result.Succeeded; // Create foundation slabs successfully.
                    }
                }
                return Result.Cancelled;  // Cancel creation.
            }
            catch (Exception e)
            {
                message = e.Message;
                return Result.Failed; // Unknow error.
            }
        }
        #endregion IExternalCommand Members Implementation
    }
}

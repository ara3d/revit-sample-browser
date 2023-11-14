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
using System.Collections.Generic;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace Revit.SDK.Samples.GenerateFloor.CS
{
    [Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]
    [Autodesk.Revit.Attributes.Regeneration(Autodesk.Revit.Attributes.RegenerationOption.Manual)]
    [Autodesk.Revit.Attributes.Journaling(Autodesk.Revit.Attributes.JournalingMode.NoCommandData)]
    public class Command : IExternalCommand
    {
                public Result Execute(ExternalCommandData commandData,
            ref string message, ElementSet elements)
        {
           var tran = new Transaction(commandData.Application.ActiveUIDocument.Document, "Generate Floor");
           tran.Start();

            try
            {
                if (null == commandData)
                {
                    throw new ArgumentNullException("commandData");
                }

                var data = new Data();
                data.ObtainData(commandData);

                var dlg = new GenerateFloorForm(data);

                if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    CreateFloor(data, commandData.Application.ActiveUIDocument.Document);

                    tran.Commit();
                    return Result.Succeeded;
                }
                else
                {
                   tran.RollBack();
                    return Result.Cancelled;
                }                
            }
            catch (Exception e)
            {
                message = e.Message;
                tran.RollBack();
                return Result.Failed;
            }
        }

        
        /// <summary>
        /// create a floor by the data obtain from revit.
        /// </summary>
        /// <param name="data">Data including the profile, level etc, which is need for create a floor.</param>
        /// <param name="doc">Retrieves an object that represents the currently active project.</param>
        static public void CreateFloor(Data data, Document doc)
        {
            var loop = new CurveLoop();
            foreach (Curve curve in data.Profile)
            {
                loop.Append(curve);
            }

            var floorLoops = new List<CurveLoop> { loop };

            Floor.Create(doc, floorLoops, data.FloorType.Id, data.Level.Id, data.Structural, null, 0.0);
        }
    }
}

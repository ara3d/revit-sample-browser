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

using System.Windows.Forms;

using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace Revit.SDK.Samples.EnergyAnalysisModel.CS
{
    /// <summary>
    /// A class inherits IExternalCommand interface.
    /// this class controls the class which subscribes handle events and the events' information UI.
    /// like a bridge between them.
    /// </summary>
    [Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]
    [Autodesk.Revit.Attributes.Regeneration(Autodesk.Revit.Attributes.RegenerationOption.Manual)]
    [Autodesk.Revit.Attributes.Journaling(Autodesk.Revit.Attributes.JournalingMode.NoCommandData)]
    public class Command : IExternalCommand
    {
                public Result Execute(ExternalCommandData commandData,
            ref string message, ElementSet elements)
        {
            var trans = new Transaction(commandData.Application.ActiveUIDocument.Document, "Revit.SDK.Samples.EnergyAnalysisModel");
            trans.Start();
            // Create an object that is responsible for collecting users inputs and getting analysis data of current model.
            var analysisModel = new EnergyAnalysisModel(commandData.Application.ActiveUIDocument.Document);

            // Create the UI for users inputs options and view analysis models.
            using (var form = new OptionsAndAnalysisForm(analysisModel))
            {
                // make analysis data ready
                analysisModel.Initialize();
                // show dialog to browser analysis model
                if (DialogResult.OK != form.ShowDialog())
                {
                    trans.RollBack();
                    return Result.Cancelled;
                }
            }
            trans.Commit();
            return Result.Succeeded;
        }

            }
}
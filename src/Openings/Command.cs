// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using System;
using System.Collections.Generic;
using System.Linq;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace Ara3D.RevitSampleBrowser.Openings.CS
{
    /// <summary>
    ///     The entrance of this example, implement the Execute method of IExternalCommand
    /// </summary>
    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    [Journaling(JournalingMode.NoCommandData)]
    public class Command : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData,
            ref string message, ElementSet elements)
        {
            var transaction = new Transaction(commandData.Application.ActiveUIDocument.Document, "External Tool");
            try
            {
                transaction.Start();
                var openingInfos = new FilteredElementCollector(commandData.Application.ActiveUIDocument.Document)
                    .OfClass(typeof(Opening))
                    .Cast<Opening>()
                    .Select(opening => new OpeningInfo(opening, commandData.Application))
                    .ToList();

                if (openingInfos.Count == 0)
                {
                    message = "don't have opening in the project";
                    return Result.Cancelled;
                }

                //show dialogue
                using (var openingForm = new OpeningForm(openingInfos))
                {
                    openingForm.ShowDialog();
                }
            }
            catch (Exception e)
            {
                message = e.ToString();
                return Result.Failed;
            }
            finally
            {
                transaction.Commit();
            }

            return Result.Succeeded;
        }
    }
}

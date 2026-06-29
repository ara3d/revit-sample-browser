// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using System;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace Ara3D.RevitSampleBrowser.ContextualAnalyticalModel.CS
{
    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    public class FlipAnalyticalMember : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            //Expected results: The Analytical Member is flipped, you can observe the IDs of the end nodes
            try
            {
                var document = commandData.Application.ActiveUIDocument.Document;

                using var transaction = new Transaction(document, "Flip Analytical Member");
                transaction.Start();
                CreateAnalyticalMember.CreateMember(document).FlipCurve();
                transaction.Commit();

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

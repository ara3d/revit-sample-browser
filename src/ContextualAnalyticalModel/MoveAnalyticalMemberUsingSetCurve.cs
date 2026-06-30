// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using System;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace Ara3D.RevitSampleBrowser.ContextualAnalyticalModel.CS
{
    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    public class MoveAnalyticalMemberUsingSetCurve : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            try
            {
                var document = commandData.Application.ActiveUIDocument.Document;

                var analyticalMember = CreateAnalyticalMember.CreateMember(document);

                CreateAnalyticalMember.CreateConvergentMember(document);

                // Start transaction
                using (var transaction = new Transaction(document, "Offset member"))
                {
                    transaction.Start();

                    var originalCurve = analyticalMember.GetCurve();
                    var originalCurveStart = originalCurve.GetEndPoint(0);
                    var originalCurveEnd = originalCurve.GetEndPoint(1);

                    // Create new start and end with offset value
                    double offset = 15;
                    var newLineStart = new XYZ(originalCurveStart.X + offset, 0, 0);
                    var newLineEnd = new XYZ(originalCurveEnd.X + offset, 0, 0);

                    var line = Line.CreateBound(newLineStart, newLineEnd);

                    analyticalMember.SetCurve(line);

                    transaction.Commit();
                }

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

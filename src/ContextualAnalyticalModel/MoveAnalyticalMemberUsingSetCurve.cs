// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Structure;
using Autodesk.Revit.UI;
using System;

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
                using Transaction transaction = new(document, "Offset member");
                transaction.Start();

                var originalCurve = analyticalMember.GetCurve();
                var originalCurveStart = originalCurve.GetEndPoint(0);
                var originalCurveEnd = originalCurve.GetEndPoint(1);

                // Create new start and end with offset value
                double offset = 15;
                XYZ newLineStart = new(originalCurveStart.X + offset, 0, 0);
                XYZ newLineEnd = new(originalCurveEnd.X + offset, 0, 0);

                var line = Line.CreateBound(newLineStart, newLineEnd);

                analyticalMember.SetCurve(line);

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

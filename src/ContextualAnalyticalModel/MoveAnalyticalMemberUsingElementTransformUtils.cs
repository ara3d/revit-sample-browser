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
    public class MoveAnalyticalMemberUsingElementTransformUtils : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            try
            {
                var document = commandData.Application.ActiveUIDocument.Document;

                var analyticalMember = CreateAnalyticalMember.CreateMember(document);

                CreateAnalyticalMember.CreateConvergentMember(document);

                // Move the first Analytical Member using ElementTransformUtils
                using Transaction transaction = new(document, "Move member with ElementTransformUtils");
                transaction.Start();

                ElementTransformUtils.MoveElement(document, analyticalMember.Id, new XYZ(15, 0, 0));

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

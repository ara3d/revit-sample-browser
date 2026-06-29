// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using System;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Structure;
using Autodesk.Revit.UI;

namespace Ara3D.RevitSampleBrowser.ContextualAnalyticalModel.CS
{
    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    public class CreateAnalyticalMember : IExternalCommand
    {
        public virtual Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            try
            {
                var document = commandData.Application.ActiveUIDocument.Document;
                CreateMember(document);
                return Result.Succeeded;
            }
            catch (Exception ex)
            {
                message = ex.Message;
                return Result.Failed;
            }
        }

        /// <summary>
        ///     Creates an Analytical Member
        /// </summary>
        /// <param name="document"></param>
        /// <returns></returns>
        public static AnalyticalMember CreateMember(Document document)
        {
            using var transaction = new Transaction(document, "Create Analytical Member");
            transaction.Start();
            var analyticalMember = CreateAnalyticalMemberFromEndpoints(document, new XYZ(-5, 0, 0), new XYZ(0, 0, 0));
            transaction.Commit();
            return analyticalMember;
        }

        public static AnalyticalMember CreateConvergentMember(Document document)
        {
            using var transaction = new Transaction(document, "Create Convergent Analytical Member");
            transaction.Start();
            var analyticalMember = CreateAnalyticalMemberFromEndpoints(document, new XYZ(0, 0, 0), new XYZ(-5, 5, 0));
            transaction.Commit();
            return analyticalMember;
        }

        private static AnalyticalMember CreateAnalyticalMemberFromEndpoints(Document doc, XYZ start, XYZ end)
        {
            var analyticalMember = AnalyticalMember.Create(doc, Line.CreateBound(start, end));
            analyticalMember.StructuralRole = AnalyticalStructuralRole.StructuralRoleBeam;
            analyticalMember.AnalyzeAs = AnalyzeAs.Lateral;
            return analyticalMember;
        }
    }
}

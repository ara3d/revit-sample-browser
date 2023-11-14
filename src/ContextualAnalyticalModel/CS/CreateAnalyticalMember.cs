// Copyright 2023. See https://github.com/ara3d/revit-samples/LICENSE.txt

using System;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Structure;
using Autodesk.Revit.UI;

namespace ContextualAnalyticalModel
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
            AnalyticalMember analyticalMember = null;
            //start Transaction
            using (var transaction = new Transaction(document, "Create Analytical Member"))
            {
                transaction.Start();

                analyticalMember = CreateAnalyticalMemberFromEndpoints(document, new XYZ(-5, 0, 0), new XYZ(0, 0, 0));

                transaction.Commit();
            }

            return analyticalMember;
        }

        /// <summary>
        ///     Creates another Analytical Member convergent with the one above
        /// </summary>
        /// <param name="document"></param>
        /// <returns></returns>
        public static AnalyticalMember CreateConvergentMember(Document document)
        {
            AnalyticalMember analyticalMember = null;
            //start Transaction
            using (var transaction = new Transaction(document, "Create Convergent Analytical Member"))
            {
                transaction.Start();

                analyticalMember = CreateAnalyticalMemberFromEndpoints(document, new XYZ(0, 0, 0), new XYZ(-5, 5, 0));

                transaction.Commit();
            }

            return analyticalMember;
        }

        /// <summary>
        ///     Creates a new Analytical Member in the Document using the endpoints start and end.
        ///     Optionally the Structural Role can be specified
        /// </summary>
        /// <param name="doc"></param>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <returns></returns>
        private static AnalyticalMember CreateAnalyticalMemberFromEndpoints(Document doc, XYZ start, XYZ end)
        {
            //create curve which will be assigned to the analytical member
            var line = Line.CreateBound(start, end);

            //create the AnalyticalMember
            var analyticalMember = AnalyticalMember.Create(doc, line);

            analyticalMember.StructuralRole = AnalyticalStructuralRole.StructuralRoleBeam;
            analyticalMember.AnalyzeAs = AnalyzeAs.Lateral;

            return analyticalMember;
        }
    }
}

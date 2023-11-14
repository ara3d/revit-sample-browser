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
    public class ReleaseConditionsAnalyticalMember : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            try
            {
                // Get the Document
                var document = commandData.Application.ActiveUIDocument.Document;

                // Create an Analytical Member
                var analyticalMember = CreateAnalyticalMember.CreateMember(document);

                // Start transaction
                using (var transaction = new Transaction(document, "Release Conditions"))
                {
                    transaction.Start();

                    // Get release conditions of analytical member
                    var releaseConditions = analyticalMember.GetReleaseConditions();
                    foreach (var rc in releaseConditions)
                        Console.WriteLine("Position: " + rc.Start +
                                          "Fx: " + rc.Fx +
                                          "Fy: " + rc.Fy +
                                          "Fz: " + rc.Fz +
                                          "Mx: " + rc.Mx +
                                          "My: " + rc.My +
                                          "Mz: " + rc.Mz);

                    // Get release type at start
                    analyticalMember.GetReleaseType(true);

                    // Change release type
                    analyticalMember.SetReleaseType(true, ReleaseType.UserDefined);

                    try
                    {
                        analyticalMember.SetReleaseConditions(new ReleaseConditions(true, false, true, false, true,
                            false, true));
                    }
                    catch (InvalidOperationException ex)
                    {
                        message = ex.Message;
                        return Result.Failed;
                    }

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

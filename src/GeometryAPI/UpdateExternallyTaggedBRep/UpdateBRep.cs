// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using Ara3D.RevitSampleBrowser.Common.Infrastructure;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System;
namespace Ara3D.RevitSampleBrowser.GeometryAPI.UpdateExternallyTaggedBRep.CS
{
    /// <summary>Updates the ExternallyTaggedBRep in the DirectShape created by <see cref="CreateBRep"/>.</summary>
    [Transaction(TransactionMode.Manual)]
    public class UpdateBRep : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            var dbDocument = commandData.Application.ActiveUIDocument.Document;

            try
            {
                // Requires a valid DirectShape from CreateBRep in the same document.
                if (null == CreateBRep.CreatedDirectShape ||
                    !CreateBRep.CreatedDirectShape.IsValidObject ||
                    !CreateBRep.CreatedDirectShape.Document.Equals(dbDocument))
                    if (Result.Succeeded != SampleBrowserUtils.ExecuteCreateBRepCommand(dbDocument))
                        return Result.Failed;

                using Transaction transaction = new(dbDocument, "UpdateExternallyTaggedBRep");
                transaction.Start();

                var resizedTaggedBRep = SampleBrowserUtils.CreateExternallyTaggedPodium(120.0, 20.0, 60.0);
                if (null == resizedTaggedBRep)
                    return Result.Failed;

                CreateBRep.CreatedDirectShape.RemoveExternallyTaggedGeometry(Podium.ExternalId);

                if (CreateBRep.CreatedDirectShape.HasExternalGeometry(Podium.ExternalId))
                    return Result.Failed;

                CreateBRep.CreatedDirectShape.AddExternallyTaggedGeometry(resizedTaggedBRep);

                transaction.Commit();
            }
            catch (Exception ex)
            {
                message = ex.Message;
                return Result.Failed;
            }

            return Result.Succeeded;
        }
    }
}

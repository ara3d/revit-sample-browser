// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using Ara3D.RevitSampleBrowser.Common.Infrastructure;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System;
namespace Ara3D.RevitSampleBrowser.GeometryAPI.UpdateExternallyTaggedBRep.CS
{
    /// <summary>
    /// Creates an ExternallyTaggedBRep (Podium) in a DirectShape and demonstrates retrieval by ExternalGeometryId.
    /// </summary>
    [Transaction(TransactionMode.Manual)]
    public class CreateBRep : IExternalCommand
    {
        /// <summary>DirectShape created by this command; used by <see cref="UpdateBRep"/>.</summary>
        public static DirectShape CreatedDirectShape { get; set; }

        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            var dbDocument = commandData.Application.ActiveUIDocument.Document;

            try
            {
                if (Result.Succeeded != SampleBrowserUtils.ExecuteCreateBRepCommand(dbDocument))
                    return Result.Failed;
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

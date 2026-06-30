// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using Ara3D.RevitSampleBrowser.Common.Infrastructure;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Architecture;
using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
namespace Ara3D.RevitSampleBrowser.CreateTrianglesTopography.CS
{
    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    public class Command : IExternalCommand
    {
        public virtual Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            try
            {
                var document = commandData.Application.ActiveUIDocument.Document;

                var trianglesData = SampleBrowserUtils.LoadTrianglesData();

                using Transaction tran = new(document, "CreateTrianglesTopography");
                tran.Start();
                List<PolymeshFacet> triangleFacets = new();
                foreach (var facet in trianglesData.Facets)
                {
                    triangleFacets.Add(new PolymeshFacet(facet[0], facet[1], facet[2]));
                }

                var topoSurface = TopographySurface.Create(document, trianglesData.Points, triangleFacets);
                var name = topoSurface.get_Parameter(BuiltInParameter.ROOM_NAME);
                name?.Set("CreateTrianglesTopography");
                tran.Commit();

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

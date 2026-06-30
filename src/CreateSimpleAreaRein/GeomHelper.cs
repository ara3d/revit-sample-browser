// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using Ara3D.RevitSampleBrowser.Common.Infrastructure;
using Ara3D.RevitSampleBrowser.Common.Parameters;
using Ara3D.RevitSampleBrowser.Common.Structural;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Structure;
using System.Collections.Generic;
using System.Linq;
namespace Ara3D.RevitSampleBrowser.CreateSimpleAreaRein.CS
{
    public class GeomHelper
    {
        private readonly Document m_currentDoc; //active document

        public GeomHelper()
        {
            m_currentDoc = Command.CommandData.Application.ActiveUIDocument.Document;
        }

        public bool GetWallGeom(Wall wall, ref Reference refer, ref IList<Curve> curves)
        {
            var faces = SampleBrowserUtils.GetFaces(wall);
            //unless API has bug, locCurve can't be null
            if (wall.Location is not LocationCurve locCurve) return false;
            //check the location is line
            var locLine = locCurve.Curve as Line;
            if (null == locLine) return false;

            foreach (Face face in faces)
            {
                if (ParameterAccess.IsParallel(face, locLine))
                {
                    refer = face.Reference;
                    break;
                }
            }

            //can't find proper reference
            if (null == refer) return false;
            //check the analytical model profile is rectangular
            var document = wall.Document;
            var assocManager =
                AnalyticalToPhysicalAssociationManager.GetAnalyticalToPhysicalAssociationManager(document);
            AnalyticalPanel model = null;
            if (assocManager != null)
            {
                var associatedElementId = assocManager.GetAssociatedElementId(wall.Id);
                if (associatedElementId != ElementId.InvalidElementId)
                {
                    var associatedElement = document.GetElement(associatedElementId);
                    if (associatedElement is not null and AnalyticalPanel panel)
                        model = panel;
                }
            }

            if (null == model) return false;

            curves = model.GetOuterContour().ToList();

            return AreaReinforcementHelper.IsRectangular(curves);
        }

        public bool GetFloorGeom(Floor floor, ref Reference refer, ref IList<Curve> curves)
        {
            //get horizontal face reference
            var faces = SampleBrowserUtils.GetFaces(floor);
            foreach (Face face in faces)
            {
                if (SampleBrowserUtils.IsHorizontalFace(face))
                {
                    refer = face.Reference;
                    break;
                }
            }

            //no proper reference
            if (null == refer) return false;

            //check the analytical model profile is rectangular
            //check the analytical model profile is rectangular
            var document = floor.Document;
            var assocManager =
                AnalyticalToPhysicalAssociationManager.GetAnalyticalToPhysicalAssociationManager(document);
            AnalyticalPanel model = null;
            if (assocManager != null)
            {
                var associatedElementId = assocManager.GetAssociatedElementId(floor.Id);
                if (associatedElementId != ElementId.InvalidElementId)
                {
                    var associatedElement = document.GetElement(associatedElementId);
                    if (associatedElement is not null and AnalyticalPanel panel)
                        model = panel;
                }
            }

            if (null == model) return false;
            curves = model.GetOuterContour().ToList();

            return AreaReinforcementHelper.IsRectangular(curves);
        }
    }
}

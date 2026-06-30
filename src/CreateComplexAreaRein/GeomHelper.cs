// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using System.Collections.Generic;
using System.Linq;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Structure;

using Ara3D.RevitSampleBrowser.Common.Geometry;
using Ara3D.RevitSampleBrowser.Common.Infrastructure;
using Ara3D.RevitSampleBrowser.Common.Parameters;
using Ara3D.RevitSampleBrowser.Common.Structural;
namespace Ara3D.RevitSampleBrowser.CreateComplexAreaRein.CS
{
    public class GeomHelper
    {
        private Document m_currentDoc; //active document

        public GeomHelper()
        {
            m_currentDoc = Command.CommandData.Application.ActiveUIDocument.Document;
        }

        public bool GetFloorGeom(Floor floor, ref Reference refer, ref IList<Curve> curves)
        {
            //get horizontal face's reference
            var faces = FaceAndSolidGeometry.GetFaces(floor);
            foreach (Face face in faces)
            {
                if (FaceAndSolidGeometry.IsHorizontalFace(face))
                {
                    refer = face.Reference;
                    break;
                }
            }

            if (null == refer) return false;
            //get analytical model profile
            AnalyticalPanel model = null;
            var document = floor.Document;
            var assocManager =
                AnalyticalToPhysicalAssociationManager.GetAnalyticalToPhysicalAssociationManager(document);
            if (assocManager != null)
            {
                var associatedElementId = assocManager.GetAssociatedElementId(floor.Id);
                if (associatedElementId != ElementId.InvalidElementId)
                {
                    var associatedElement = document.GetElement(associatedElementId);
                    if (associatedElement != null && associatedElement is AnalyticalPanel panel)
                        model = panel;
                }
            }

            if (null == model) return false;

            curves = model.GetOuterContour().ToList();
            if (!AreaReinforcementHelper.IsRectangular(curves)) return false;
            curves = AddInlaidCurves(curves, 0.5);

            return true;
        }

        private IList<Curve> AddInlaidCurves(IList<Curve> curves, double scale)
        {
            //because curves is readonly, can't use method Curve.Append(Curve)
            var lines = new List<Line>();
            for (var i = 0; i < 4; i++)
            {
                var temp = curves[i] as Line;
                lines.Add(temp);
            }

            //length and width of the rectangle
            var length = XyzMath.GetLength(lines[0]);
            var width = XyzMath.GetLength(lines[1]);
            for (var i = 0; i < 2; i++)
            {
                //height line
                var tempLine1 = lines[i * 2];
                var scaledLine1 = XyzMath.GetScaledLine(tempLine1, scale);
                var distance1 = scale / 2 * width;
                var movedLine1 = ParameterAccess.GetXyParallelLine(scaledLine1, distance1);
                lines.Add(movedLine1);

                //width line
                var tempLine2 = lines[i * 2 + 1];
                var scaledLine2 = XyzMath.GetScaledLine(tempLine2, scale);
                var distance2 = scale / 2 * length;
                var movedLine2 = ParameterAccess.GetXyParallelLine(scaledLine2, distance2);
                lines.Add(movedLine2);
            }

            //add all 8 lines into return array
            IList<Curve> allLines = new List<Curve>();
            for (var i = 0; i < 8; i++) allLines.Add(lines[i]);
            return allLines;
        }
    }
}

// Copyright 2023. See https://github.com/ara3d/revit-samples/LICENSE.txt

using System.Collections.Generic;
using System.Linq;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Structure;

namespace Revit.SDK.Samples.CreateComplexAreaRein.CS
{
    /// <summary>
    ///     provide utility method to get geometry data for
    ///     creating AreaReinforcement on wall or floor
    /// </summary>
    internal class GeomHelper
    {
        private Document m_currentDoc; //active document

        /// <summary>
        ///     constructor, initialize m_currentDoc
        /// </summary>
        public GeomHelper()
        {
            m_currentDoc = Command.CommandData.Application.ActiveUIDocument.Document;
        }

        /// <summary>
        ///     get necessary data when create AreaReinforcement on a horizontal floor
        /// </summary>
        /// <param name="floor">floor on which to create AreaReinforcemen</param>
        /// <param name="refer">reference of the horizontal face on the floor</param>
        /// <param name="curves">curves compose the horizontal face of the floor</param>
        /// <returns>is successful</returns>
        public bool GetFloorGeom(Floor floor, ref Reference refer, ref IList<Curve> curves)
        {
            //get horizontal face's reference
            var faces = GeomUtil.GetFaces(floor);
            foreach (Face face in faces)
                if (GeomUtil.IsHorizontalFace(face))
                {
                    refer = face.Reference;
                    break;
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
            if (!GeomUtil.IsRectangular(curves)) return false;
            curves = AddInlaidCurves(curves, 0.5);

            return true;
        }

        /// <summary>
        ///     create CurveArray which contain 8 curves, 4 is exterior lines and 4 is interior lines
        /// </summary>
        /// <param name="curves"></param>
        /// <param name="scale"></param>
        /// <returns></returns>
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
            var length = GeomUtil.GetLength(lines[0]);
            var width = GeomUtil.GetLength(lines[1]);
            for (var i = 0; i < 2; i++)
            {
                //height line
                var tempLine1 = lines[i * 2];
                var scaledLine1 = GeomUtil.GetScaledLine(tempLine1, scale);
                var distance1 = scale / 2 * width;
                var movedLine1 = GeomUtil.GetXYParallelLine(scaledLine1, distance1);
                lines.Add(movedLine1);

                //width line
                var tempLine2 = lines[i * 2 + 1];
                var scaledLine2 = GeomUtil.GetScaledLine(tempLine2, scale);
                var distance2 = scale / 2 * length;
                var movedLine2 = GeomUtil.GetXYParallelLine(scaledLine2, distance2);
                lines.Add(movedLine2);
            }

            //add all 8 lines into return array
            IList<Curve> allLines = new List<Curve>();
            for (var i = 0; i < 8; i++) allLines.Add(lines[i]);
            return allLines;
        }
    }
}

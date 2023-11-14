// Copyright 2023. See https://github.com/ara3d/revit-samples/LICENSE.txt

using System.Collections.Generic;
using System.Linq;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Structure;

namespace Revit.SDK.Samples.CreateSimpleAreaRein.CS
{
    using GeoElement = GeometryElement;


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
        ///     get necessary data when create AreaReinforcement on a straight wall
        /// </summary>
        /// <param name="wall">wall on which to create AreaReinforcemen</param>
        /// <param name="refer">reference of the vertical straight face on the wall</param>
        /// <param name="curves">curves compose the vertical face of the wall</param>
        /// <returns>is successful</returns>
        public bool GetWallGeom(Wall wall, ref Reference refer, ref IList<Curve> curves)
        {
            var faces = GeomUtil.GetFaces(wall);
            var locCurve = wall.Location as LocationCurve;
            //unless API has bug, locCurve can't be null
            if (null == locCurve) return false;
            //check the location is line
            var locLine = locCurve.Curve as Line;
            if (null == locLine) return false;

            //get the face reference
            foreach (Face face in faces)
                if (GeomUtil.IsParallel(face, locLine))
                {
                    refer = face.Reference;
                    break;
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
                    if (associatedElement != null && associatedElement is AnalyticalPanel)
                        model = associatedElement as AnalyticalPanel;
                }
            }

            if (null == model) return false;

            curves = model.GetOuterContour().ToList();

            if (!GeomUtil.IsRectangular(curves)) return false;

            return true;
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
            //get horizontal face reference
            var faces = GeomUtil.GetFaces(floor);
            foreach (Face face in faces)
                if (GeomUtil.IsHorizontalFace(face))
                {
                    refer = face.Reference;
                    break;
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
                    if (associatedElement != null && associatedElement is AnalyticalPanel)
                        model = associatedElement as AnalyticalPanel;
                }
            }

            if (null == model) return false;
            curves = model.GetOuterContour().ToList();

            if (!GeomUtil.IsRectangular(curves)) return false;

            return true;
        }
    }
}

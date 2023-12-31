// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using System.Collections.Generic;
using System.Linq;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Structure;
using Autodesk.Revit.UI;

namespace Ara3D.RevitSampleBrowser.NewPathReinforcement.CS
{
    /// <summary>
    ///     ProfileFloor class contains the information about profile of floor,
    ///     and contains method used to create PathReinforcement on floor
    /// </summary>
    public class ProfileFloor : Profile
    {
        private readonly Floor m_data;

        /// <summary>
        ///     constructor
        /// </summary>
        /// <param name="floor">floor to create reinforcement on</param>
        /// <param name="commandData">object which contains reference to Revit Application</param>
        public ProfileFloor(Floor floor, ExternalCommandData commandData)
            : base(commandData)
        {
            m_data = floor;
            var faces = GetFaces(m_data);
            Points = GetNeedPoints(faces);
            To2DMatrix = GetTo2DMatrix();
        }

        /// <summary>
        ///     Get points of the first face
        /// </summary>
        /// <param name="faces">edges in all faces</param>
        /// <returns>points of first face</returns>
        public override List<List<XYZ>> GetNeedPoints(List<List<Edge>> faces)
        {
            var needPoints = new List<List<XYZ>>();
            foreach (var edge in faces[0])
            {
                var edgexyzs = edge.Tessellate() as List<XYZ>;
                needPoints.Add(edgexyzs);
            }

            return needPoints;
        }

        /// <summary>
        ///     Get a matrix which can transform points to 2D
        /// </summary>
        /// <returns>matrix which can transform points to 2D</returns>
        public override Matrix4 GetTo2DMatrix()
        {
            // select the view which is named "Level 2"
            // Once use the View type to filterrrr the element ,please skip the view templates 
            // because they're behind-the-scene and invisible in project browser; also invalid for API.
            var views = from elem in
                    new FilteredElementCollector(CommandData.Application.ActiveUIDocument.Document)
                        .OfClass(typeof(ViewPlan)).ToElements()
                let view = elem as View
                where view != null && !view.IsTemplate && view.Name == "Level 2"
                select view;
            var viewLevel2 = views.First();

            var xAxis = new Vector4(viewLevel2.RightDirection);
            //Because Y axis in windows UI is downward, so we should Multiply(-1) here
            var yAxis = new Vector4(viewLevel2.UpDirection.Multiply(-1));
            var zAxis = new Vector4(viewLevel2.ViewDirection);

            var result = new Matrix4(xAxis, yAxis, zAxis);
            return result;
        }

        /// <summary>
        ///     Create PathReinforcement on floor
        /// </summary>
        /// <param name="points">points used to create PathReinforcement</param>
        /// <param name="flip">used to specify whether new PathReinforcement is Filp</param>
        /// <returns>new created PathReinforcement</returns>
        public override Autodesk.Revit.DB.Structure.PathReinforcement CreatePathReinforcement(List<Vector4> points,
            bool flip)
        {
            IList<Curve> curves = new List<Curve>();
            for (var i = 0; i < points.Count - 1; i++)
            {
                var p1 = new XYZ(points[i].X, points[i].Y, points[i].Z);
                var p2 = new XYZ(points[i + 1].X, points[i + 1].Y, points[i + 1].Z);
                var curve = Line.CreateBound(p1, p2);
                curves.Add(curve);
            }

            var pathReinforcementTypeId = PathReinforcementType.CreateDefaultPathReinforcementType(Document);
            var rebarBarTypeId = RebarBarType.CreateDefaultRebarBarType(Document);
            var rebarHookTypeId = RebarHookType.CreateDefaultRebarHookType(Document);
            return Autodesk.Revit.DB.Structure.PathReinforcement.Create(Document, m_data, curves, flip,
                pathReinforcementTypeId, rebarBarTypeId, rebarHookTypeId, rebarHookTypeId);
        }
    }
}

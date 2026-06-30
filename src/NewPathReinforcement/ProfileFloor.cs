// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Structure;
using Autodesk.Revit.UI;
using System.Collections.Generic;
using System.Linq;
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

        public override List<List<XYZ>> GetNeedPoints(List<List<Edge>> faces)
        {
            List<List<XYZ>> needPoints = new();
            foreach (var edge in faces[0])
            {
                var edgexyzs = edge.Tessellate() as List<XYZ>;
                needPoints.Add(edgexyzs);
            }

            return needPoints;
        }

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

            Vector4 xAxis = new(viewLevel2.RightDirection);
            //Because Y axis in windows UI is downward, so we should Multiply(-1) here
            Vector4 yAxis = new(viewLevel2.UpDirection.Multiply(-1));
            Vector4 zAxis = new(viewLevel2.ViewDirection);

            Matrix4 result = new(xAxis, yAxis, zAxis);
            return result;
        }

        public override Autodesk.Revit.DB.Structure.PathReinforcement CreatePathReinforcement(List<Vector4> points,
            bool flip)
        {
            IList<Curve> curves = [];
            for (var i = 0; i < points.Count - 1; i++)
            {
                XYZ p1 = new(points[i].X, points[i].Y, points[i].Z);
                XYZ p2 = new(points[i + 1].X, points[i + 1].Y, points[i + 1].Z);
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

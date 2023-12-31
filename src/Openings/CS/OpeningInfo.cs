// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using System.Collections.Generic;
using System.Collections.ObjectModel;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace Ara3D.RevitSampleBrowser.Openings.CS
{
    /// <summary>
    ///     This class contain the data about the Opening (get from Revit)
    ///     Such as BoundingBox, Profile Curve...
    /// </summary>
    public class OpeningInfo
    {
        private readonly List<Line3D> m_lines = new List<Line3D>(); //contains lines in curve
        private UIApplication m_revit; //Application of Revit

        /// <summary>
        ///     The default constructor,
        ///     get the information we want from Opening
        ///     get OpeningProperty, BoundingBox and Profile
        /// </summary>
        /// <param name="opening">an opening in revit</param>
        /// <param name="app">application object</param>
        public OpeningInfo(Opening opening, UIApplication app)
        {
            Opening = opening;
            m_revit = app;

            //get OpeningProperty which can use in PropertyGrid control
            var openingProperty = new OpeningProperty(Opening);
            Property = openingProperty;

            //get BoundingBox of Opening
            var boxXyz = Opening.get_BoundingBox(m_revit.ActiveUIDocument.Document.ActiveView);
            var boundingBox = new BoundingBox(boxXyz);
            BoundingBox = boundingBox;

            //get profile
            GetProfile();
        }

        //OpeningProperty class which can use in PropertyGrid control

        //property
        /// <summary>
        ///     Property to get and set Application of Revit
        /// </summary>
        public UIApplication Revit
        {
            get => m_revit;
            set
            {
                if (value != m_revit)
                    m_revit = value;
            }
        }

        /// <summary>
        ///     Property to get Opening store in OpeningInfo
        /// </summary>
        public Opening Opening { get; }

        /// <summary>
        ///     Property to get Name and Id
        ///     eg: "Opening Cut (114389)"
        /// </summary>
        public string NameAndId => $"{Opening.Name} ({Opening.Id})";

        /// <summary>
        ///     Property to get bool the define whether opening is Shaft Opening
        /// </summary>
        public bool IsShaft
            => "Shaft Openings" == Opening.Category?.Name;

        /// <summary>
        ///     Property to get OpeningProperty class
        ///     which can use in PropertyGrid control
        /// </summary>
        public OpeningProperty Property { get; }

        /// <summary>
        ///     Property to get Profile information of opening
        /// </summary>
        public WireFrame Sketch { get; private set; }

        /// <summary>
        ///     Property to get BoundingBox of Opening
        /// </summary>
        public BoundingBox BoundingBox { get; }

        /// <summary>
        ///     get Profile of Opening
        /// </summary>
        private void GetProfile()
        {
            var curveArray = Opening.BoundaryCurves;
            if (null != curveArray)
            {
                m_lines.Clear();
                foreach (Curve curve in curveArray)
                {
                    var points = curve.Tessellate() as List<XYZ>;
                    AddLine(points);
                }

                var wireFrameSketch = new WireFrame(new ReadOnlyCollection<Line3D>(m_lines));
                Sketch = wireFrameSketch;
            }
            else if (Opening.IsRectBoundary)
            {
                //if opening profile is RectBoundary, 
                //just can get profile info from BoundaryRect Property
                m_lines.Clear();
                var boundRect = Opening.BoundaryRect as List<XYZ>;
                var rectPoints = GetPoints(boundRect);
                AddLine(rectPoints);
                var wireFrameSketch = new WireFrame(new ReadOnlyCollection<Line3D>(m_lines));
                Sketch = wireFrameSketch;
            }
            else
            {
                Sketch = null;
            }
        }

        /// <summary>
        ///     get four corner points of a rectangular in same plane
        /// </summary>
        /// <param name="boundRect">
        ///     an array contain two Autodesk.Revit.DB.XYZ struct store the max and min
        ///     coordinate of rectangular
        /// </param>
        private List<XYZ> GetPoints(List<XYZ> boundRect)
        {
            var points = new List<XYZ>();
            var p1 = boundRect[0];
            points.Add(p1);

            var p2 = new XYZ(
                boundRect[0].X,
                boundRect[0].Y,
                boundRect[1].Z);
            points.Add(p2);

            var p3 = boundRect[1];
            points.Add(p3);

            var p4 = new XYZ(
                boundRect[1].X,
                boundRect[1].Y,
                boundRect[0].Z);
            points.Add(p4);

            //make rectangle close
            var p5 = boundRect[0];
            points.Add(p5);

            return points;
        }

        /// <summary>
        ///     get line from List<XYZ>(points) and add line to m_lines list
        /// </summary>
        /// <param name="points">a List<XYZ> contain points of the Curve</param>
        private void AddLine(List<XYZ> points)
        {
            if (null == points || 0 == points.Count) return;

            var previousPoint = points[0];

            for (var i = 1; i < points.Count; i++)
            {
                var point = points[i];

                var line = new Line3D();
                var pointStart = new Vector();
                var pointEnd = new Vector();
                for (var j = 0; j < 3; j++)
                {
                    pointStart[j] = previousPoint[j];
                    pointEnd[j] = point[j];
                }

                line.StartPoint = pointStart;
                line.EndPoint = pointEnd;

                m_lines.Add(line);

                previousPoint = point;
            }
        }
    }
}

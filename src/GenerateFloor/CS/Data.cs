// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using Autodesk.Revit.Creation;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Document = Autodesk.Revit.DB.Document;

namespace Ara3D.RevitSampleBrowser.GenerateFloor.CS
{
    /// <summary>
    ///     obtain all data for this sample.
    ///     all possible types for floor
    ///     the level that walls based
    /// </summary>
    public class Data
    {
        private const double Precision = 0.00000001;
        private Application m_creApp;
        private Document m_document;
        private Hashtable m_floorTypes;

        /// <summary>
        ///     A floor type to be used by the new floor instead of the default type.
        /// </summary>
        public FloorType FloorType { get; set; }

        /// <summary>
        ///     The level on which the floor is to be placed.
        /// </summary>
        public Level Level { get; set; }

        /// <summary>
        ///     A array of planar lines and arcs that represent the horizontal profile of the floor.
        /// </summary>
        public CurveArray Profile { get; set; }

        /// <summary>
        ///     If set, specifies that the floor is structural in nature.
        /// </summary>
        public bool Structural { get; set; }

        /// <summary>
        ///     Points to be draw.
        /// </summary>
        public PointF[] Points { get; set; }

        /// <summary>
        ///     the graphics' max length
        /// </summary>
        public double MaxLength { get; set; }

        /// <summary>
        ///     List of all floor types name could be used by the floor.
        /// </summary>
        public List<string> FloorTypesName { get; set; }

        /// <summary>
        ///     Obtain all data which is necessary for generate floor.
        /// </summary>
        /// <param name="commandData">
        ///     An object that is passed to the external application
        ///     which contains data related to the command,
        ///     such as the application object and active view.
        /// </param>
        public void ObtainData(ExternalCommandData commandData)
        {
            if (null == commandData) throw new ArgumentNullException(nameof(commandData));

            var doc = commandData.Application.ActiveUIDocument;
            m_document = doc.Document;
            var es = new ElementSet();
            foreach (var elementId in doc.Selection.GetElementIds()) es.Insert(doc.Document.GetElement(elementId));
            var walls = WallFilter(es);
            m_creApp = commandData.Application.Application.Create;
            Profile = m_creApp.NewCurveArray();

            var iter = new FilteredElementCollector(doc.Document).OfClass(typeof(FloorType)).GetElementIterator();
            ObtainFloorTypes(iter);
            ObtainProfile(walls);
            ObtainLevel(walls);
            Generate2D();
            Structural = true;
        }

        /// <summary>
        ///     Set the floor type to generate by its name.
        /// </summary>
        /// <param name="typeName">the floor type's name</param>
        public void ChooseFloorType(string typeName)
        {
            FloorType = m_floorTypes[typeName] as FloorType;
        }

        /// <summary>
        ///     Obtain all types are available for floor.
        /// </summary>
        /// <param name="elements">all elements within the Document.</param>
        private void ObtainFloorTypes(FilteredElementIterator elements)
        {
            m_floorTypes = new Hashtable();
            FloorTypesName = new List<string>();

            elements.Reset();
            while (elements.MoveNext())
            {
                if (!(elements.Current is FloorType ft) || null == ft.Category || !ft.Category.Name.Equals("Floors")) continue;

                m_floorTypes.Add(ft.Name, ft);
                FloorTypesName.Add(ft.Name);
                FloorType = ft;
            }
        }

        /// <summary>
        ///     Obtain the wall's level
        /// </summary>
        /// <param name="walls">the selection of walls that make a closed outline </param>
        private void ObtainLevel(ElementSet walls)
        {
            Level temp = null;

            foreach (Wall w in walls)
            {
                if (null == temp)
                {
                    temp = m_document.GetElement(w.LevelId) as Level;
                    Level = temp;
                }

                if (Level.Elevation != (m_document.GetElement(w.LevelId) as Level).Elevation)
                    throw new InvalidOperationException("All walls should base the same level.");
            }
        }

        /// <summary>
        ///     Obtain a profile to generate floor.
        /// </summary>
        /// <param name="walls">the selection of walls that make a closed outline</param>
        private void ObtainProfile(ElementSet walls)
        {
            var temp = new CurveArray();
            foreach (Wall w in walls)
            {
                var curve = w.Location as LocationCurve;
                temp.Append(curve.Curve);
            }

            SortCurves(temp);
        }

        /// <summary>
        ///     Generate 2D data for preview pane.
        /// </summary>
        private void Generate2D()
        {
            var tempArray = new ArrayList();
            double xMin = 0;
            double xMax = 0;
            double yMin = 0;
            double yMax = 0;

            foreach (Curve c in Profile)
            {
                var xyzArray = c.Tessellate() as List<XYZ>;
                foreach (var xyz in xyzArray)
                {
                    var temp = new XYZ(xyz.X, -xyz.Y, xyz.Z);
                    FindMinMax(temp, ref xMin, ref xMax, ref yMin, ref yMax);
                    tempArray.Add(temp);
                }
            }

            MaxLength = xMax - xMin > yMax - yMin ? xMax - xMin : yMax - yMin;

            Points = new PointF[tempArray.Count / 2 + 1];
            for (var i = 0; i < tempArray.Count; i = i + 2)
            {
                var point = (XYZ)tempArray[i];
                Points.SetValue(new PointF((float)(point.X - xMin), (float)(point.Y - yMin)), i / 2);
            }

            var end = (PointF)Points.GetValue(0);
            Points.SetValue(end, tempArray.Count / 2);
        }

        /// <summary>
        ///     Estimate the current point is left_bottom or right_up.
        /// </summary>
        /// <param name="point">current point</param>
        /// <param name="xMin">left</param>
        /// <param name="xMax">right</param>
        /// <param name="yMin">bottom</param>
        /// <param name="yMax">up</param>
        private static void FindMinMax(XYZ point, ref double xMin, ref double xMax, ref double yMin, ref double yMax)
        {
            if (point.X < xMin) xMin = point.X;
            if (point.X > xMax) xMax = point.X;
            if (point.Y < yMin) yMin = point.Y;
            if (point.Y > yMax) yMax = point.Y;
        }

        /// <summary>
        ///     Filter none-wall elements.
        /// </summary>
        /// <param name="miscellanea">The currently selected Elements in Autodesk Revit</param>
        /// <returns></returns>
        private static ElementSet WallFilter(ElementSet miscellanea)
        {
            var walls = new ElementSet();
            foreach (Element e in miscellanea)
            {
                if (e is Wall w) walls.Insert(w);
            }

            if (0 == walls.Size) throw new InvalidOperationException("Please select wall first.");

            return walls;
        }

        /// <summary>
        ///     Chaining the profile.
        /// </summary>
        /// <param name="lines">none-chained profile</param>
        private void SortCurves(CurveArray lines)
        {
            var temp = lines.get_Item(0).GetEndPoint(1);
            var temCurve = lines.get_Item(0);

            Profile.Append(temCurve);

            while (Profile.Size != lines.Size)
            {
                temCurve = GetNext(lines, temp, temCurve);

                if (Math.Abs(temp.X - temCurve.GetEndPoint(0).X) < Precision
                    && Math.Abs(temp.Y - temCurve.GetEndPoint(0).Y) < Precision)
                    temp = temCurve.GetEndPoint(1);
                else
                    temp = temCurve.GetEndPoint(0);

                Profile.Append(temCurve);
            }

            if (Math.Abs(temp.X - lines.get_Item(0).GetEndPoint(0).X) > Precision
                || Math.Abs(temp.Y - lines.get_Item(0).GetEndPoint(0).Y) > Precision
                || Math.Abs(temp.Z - lines.get_Item(0).GetEndPoint(0).Z) > Precision)
                throw new InvalidOperationException("The selected walls should be closed.");
        }

        /// <summary>
        ///     Get the connected curve for current curve
        /// </summary>
        /// <param name="profile">a closed outline made by the selection of walls</param>
        /// <param name="connected">current curve's end point</param>
        /// <param name="line">current curve</param>
        /// <returns>a appropriate curve for generate floor</returns>
        private Curve GetNext(CurveArray profile, XYZ connected, Curve line)
        {
            foreach (Curve c in profile)
            {
                if (c.Equals(line)) continue;
                if (Math.Abs(c.GetEndPoint(0).X - line.GetEndPoint(1).X) < Precision &&
                    Math.Abs(c.GetEndPoint(0).Y - line.GetEndPoint(1).Y) < Precision &&
                    Math.Abs(c.GetEndPoint(0).Z - line.GetEndPoint(1).Z) < Precision
                    && Math.Abs(c.GetEndPoint(1).X - line.GetEndPoint(0).X) < Precision &&
                    Math.Abs(c.GetEndPoint(1).Y - line.GetEndPoint(0).Y) < Precision &&
                    Math.Abs(c.GetEndPoint(1).Z - line.GetEndPoint(0).Z) < Precision
                    && 2 != profile.Size)
                    continue;

                if (Math.Abs(c.GetEndPoint(0).X - connected.X) < Precision &&
                    Math.Abs(c.GetEndPoint(0).Y - connected.Y) < Precision &&
                    Math.Abs(c.GetEndPoint(0).Z - connected.Z) < Precision) return c;

                if (Math.Abs(c.GetEndPoint(1).X - connected.X) < Precision &&
                    Math.Abs(c.GetEndPoint(1).Y - connected.Y) < Precision &&
                    Math.Abs(c.GetEndPoint(1).Z - connected.Z) < Precision)
                {
                    switch (c.GetType().Name)
                    {
                        case "Line":
                        {
                            var start = c.GetEndPoint(1);
                            var end = c.GetEndPoint(0);
                            return Line.CreateBound(start, end);
                        }
                        case "Arc":
                        {
                            var size = c.Tessellate().Count;
                            var start = c.Tessellate()[0];
                            var middle = c.Tessellate()[size / 2];
                            var end = c.Tessellate()[size];

                            return Arc.Create(start, end, middle);
                        }
                    }
                }
            }

            throw new InvalidOperationException("The selected walls should be closed.");
        }
    }
}

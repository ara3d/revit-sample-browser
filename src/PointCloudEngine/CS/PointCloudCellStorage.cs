// Copyright 2023. See https://github.com/ara3d/revit-samples/LICENSE.txt

using System;
using System.Xml.Linq;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.PointClouds;
using Autodesk.Revit.UI;

namespace RevitMultiSample.CS.PointCloudEngine
{
    /// <summary>
    ///     This class is used to calculate and store points for a given cell.
    /// </summary>
    public class PointCloudCellStorage
    {
        private const int SMaxNumberOfPoints = 1000000;
        private const float SDelta = 0.1f;

        private readonly int m_color;
        private readonly Random m_random = new Random();
        private readonly bool m_randomize;

        /// <summary>
        ///     Creates a new instance of a rectangular cell.
        /// </summary>
        /// <param name="lowerLeft">The lower left point of the cell.</param>
        /// <param name="upperRight">The upper right point of the cell.</param>
        /// <param name="color">The color used for points in the cell.</param>
        /// <param name="randomize">
        ///     True to apply randomization to the number and location of points, false for a regular
        ///     arrangement of points.
        /// </param>
        public PointCloudCellStorage(XYZ lowerLeft, XYZ upperRight, int color, bool randomize)
        {
            LowerLeft = lowerLeft;
            UpperRight = upperRight;
            m_color = color;
            m_randomize = randomize;

            PointsBuffer = new CloudPoint[SMaxNumberOfPoints];
            NumberOfPoints = 0;
        }

        /// <summary>
        ///     Constructs a new instance of a rectangular cell from an XML element.
        /// </summary>
        /// <param name="rootElement">The XML element representing the cell.</param>
        public PointCloudCellStorage(XElement rootElement)
        {
            LowerLeft = XmlUtils.GetXyz(rootElement.Element("LowerLeft"));
            UpperRight = XmlUtils.GetXyz(rootElement.Element("UpperRight"));
            m_color = XmlUtils.GetColor(rootElement.Element("Color"));
            m_randomize = XmlUtils.GetBoolean(rootElement.Element("Randomize"));

            PointsBuffer = new CloudPoint[SMaxNumberOfPoints];
            NumberOfPoints = 0;
        }

        /// <summary>
        ///     The number of points in the cell.
        /// </summary>
        public int NumberOfPoints { get; private set; }

        /// <summary>
        ///     The lower left point of the cell.
        /// </summary>
        public XYZ LowerLeft { get; }

        /// <summary>
        ///     The upper right point of the cell.
        /// </summary>
        public XYZ UpperRight { get; }

        /// <summary>
        ///     The points in the cell.
        /// </summary>
        public CloudPoint[] PointsBuffer { get; }

        /// <summary>
        ///     Invokes the calculation for all points in the cell.
        /// </summary>
        public void GeneratePoints()
        {
            // X direction lines
            var xDistance = (float)(UpperRight.X - LowerLeft.X);
            AddLine(LowerLeft, XYZ.BasisX, PointDirections.PlusY | PointDirections.PlusZ, xDistance);
            AddLine(new XYZ(LowerLeft.X, LowerLeft.Y, UpperRight.Z), XYZ.BasisX,
                PointDirections.PlusY | PointDirections.MinusZ, xDistance);
            AddLine(new XYZ(LowerLeft.X, UpperRight.Y, LowerLeft.Z), XYZ.BasisX,
                PointDirections.MinusY | PointDirections.PlusZ, xDistance);
            AddLine(new XYZ(LowerLeft.X, UpperRight.Y, UpperRight.Z), XYZ.BasisX,
                PointDirections.MinusY | PointDirections.MinusZ, xDistance);

            // Y direction lines
            var yDistance = (float)(UpperRight.Y - LowerLeft.Y);
            AddLine(LowerLeft, XYZ.BasisY, PointDirections.PlusX | PointDirections.PlusZ, yDistance);
            AddLine(new XYZ(LowerLeft.X, LowerLeft.Y, UpperRight.Z), XYZ.BasisY,
                PointDirections.PlusX | PointDirections.MinusZ, yDistance);
            AddLine(new XYZ(UpperRight.X, LowerLeft.Y, LowerLeft.Z), XYZ.BasisY,
                PointDirections.MinusX | PointDirections.PlusZ, yDistance);
            AddLine(new XYZ(UpperRight.X, LowerLeft.Y, UpperRight.Z), XYZ.BasisY,
                PointDirections.MinusX | PointDirections.MinusZ, yDistance);

            // Z direction lines
            var zDistance = (float)(UpperRight.Z - LowerLeft.Z);
            AddLine(LowerLeft, XYZ.BasisZ, PointDirections.PlusX | PointDirections.PlusY, zDistance);
            AddLine(new XYZ(LowerLeft.X, UpperRight.Y, LowerLeft.Z), XYZ.BasisZ,
                PointDirections.PlusX | PointDirections.MinusY, zDistance);
            AddLine(new XYZ(UpperRight.X, LowerLeft.Y, LowerLeft.Z), XYZ.BasisZ,
                PointDirections.MinusX | PointDirections.PlusY, zDistance);
            AddLine(new XYZ(UpperRight.X, UpperRight.Y, LowerLeft.Z), XYZ.BasisZ,
                PointDirections.MinusX | PointDirections.MinusY, zDistance);
        }

        private int AddLine(XYZ startPoint, XYZ direction, PointDirections directions, float distance)
        {
            var deltaX = 0.0f;
            var totalRead = 0;

            while (deltaX < distance)
            {
                AddPoints(startPoint + direction * deltaX, directions);
                deltaX += SDelta;
            }

            return totalRead;
        }

        private void AddModifiedPoint(XYZ point, XYZ modification, double transverseDelta, int pointNumber)
        {
            var cloudPointXyz = point + modification * Math.Pow(transverseDelta * pointNumber, 4.0);
            var cp = new CloudPoint((float)cloudPointXyz.X, (float)cloudPointXyz.Y, (float)cloudPointXyz.Z, m_color);
            PointsBuffer[NumberOfPoints] = cp;
            NumberOfPoints++;
            if (NumberOfPoints == SMaxNumberOfPoints)
            {
                TaskDialog.Show("Point  cloud engine",
                    "A single cell is requiring more than the maximum hardcoded number of points for one cell: " +
                    SMaxNumberOfPoints);
                throw new Exception("Reached maximum number of points.");
            }
        }

        private void AddPoints(XYZ point, PointDirections directions)
        {
            // Two random items: number of points, and delta
            var numberOfPoints = 5;
            var transverseDelta = 0.1;
            if (m_randomize)
            {
                numberOfPoints = 5 + m_random.Next(10);
                transverseDelta = m_random.NextDouble() * 0.1;
            }

            for (var i = 1; i < numberOfPoints; i++)
            {
                if ((directions & PointDirections.PlusX) == PointDirections.PlusX)
                    AddModifiedPoint(point, XYZ.BasisX, transverseDelta, i);

                if ((directions & PointDirections.MinusX) == PointDirections.MinusX)
                    AddModifiedPoint(point, -XYZ.BasisX, transverseDelta, i);

                if ((directions & PointDirections.PlusY) == PointDirections.PlusY)
                    AddModifiedPoint(point, XYZ.BasisY, transverseDelta, i);

                if ((directions & PointDirections.MinusY) == PointDirections.MinusY)
                    AddModifiedPoint(point, -XYZ.BasisY, transverseDelta, i);

                if ((directions & PointDirections.PlusZ) == PointDirections.PlusZ)
                    AddModifiedPoint(point, XYZ.BasisZ, transverseDelta, i);

                if ((directions & PointDirections.MinusZ) == PointDirections.MinusZ)
                    AddModifiedPoint(point, -XYZ.BasisZ, transverseDelta, i);
            }
        }

        /// <summary>
        ///     Serializes the properties of the cell to an XML element.
        /// </summary>
        /// <param name="rootElement">The element to which the properties are added as subelements.</param>
        public void SerializeObjectData(XElement rootElement)
        {
            rootElement.Add(XmlUtils.GetXElement(LowerLeft, "LowerLeft"));
            rootElement.Add(XmlUtils.GetXElement(UpperRight, "UpperRight"));
            rootElement.Add(XmlUtils.GetColorXElement(m_color, "Color"));
            rootElement.Add(XmlUtils.GetXElement(m_randomize, "Randomize"));
        }

        [Flags]
        private enum PointDirections
        {
            PlusX = 1,
            MinusX = 2,
            PlusY = 4,
            MinusY = 8,
            PlusZ = 16,
            MinusZ = 32
        }
    }
}

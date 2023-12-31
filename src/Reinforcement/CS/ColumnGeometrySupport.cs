// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using System;
using System.Collections.Generic;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Structure;

namespace Ara3D.RevitSampleBrowser.Reinforcement.CS
{
    /// <summary>
    ///     The geometry support for reinforcement creation on conlumn.
    ///     It can prepare the geometry information for transverse and vertical rebar creation
    /// </summary>
    public class ColumnGeometrySupport : GeometrySupport
    {
        private readonly double m_columnHeight; //the height of the column

        // Private members
        private readonly double m_columnLength; //the length of the column
        private readonly double m_columnWidth; //the width of the column

        /// <summary>
        ///     constructor for the ColumnGeometrySupport
        /// </summary>
        /// <param name="element">the column which the rebars are placed on</param>
        /// <param name="geoOptions">the geometry option</param>
        public ColumnGeometrySupport(FamilyInstance element, Options geoOptions)
            : base(element, geoOptions)
        {
            // assert the host element is a column
            if (!element.StructuralType.Equals(StructuralType.Column))
                throw new Exception("ColumnGeometrySupport can only work for column instance.");

            // Get the length, width and height of the column.
            m_columnHeight = GetDrivingLineLength();
            m_columnLength = GetColumnLength();
            m_columnWidth = GetColumnWidth();
        }

        /// <summary>
        ///     Get the geometry information of the transverse rebar
        /// </summary>
        /// <param name="location">the location of transverse rebar</param>
        /// <param name="spacing">the spacing value of the rebar</param>
        /// <returns>the gotted geometry information</returns>
        public RebarGeometry GetTransverseRebar(TransverseRebarLocation location, double spacing)
        {
            // sort the points of the swept profile
            var comparer = new XyzHeightComparer();
            Points.Sort(comparer);

            // the offset from the column surface to the rebar
            var offset = ColumnRebarData.TransverseOffset;
            //the length of the transverse rebar
            double rebarLength = 0;

            // get the origin and normal parameter for rebar creation
            var normal = DrivingVector;
            double curveOffset = 0;

            //set rebar length and origin according to the location of rebar
            switch (location)
            {
                case TransverseRebarLocation.Start: // start transverse rebar
                    rebarLength = m_columnHeight / 4;
                    break;
                case TransverseRebarLocation.Center: // center transverse rebar
                    rebarLength = m_columnHeight / 2;
                    curveOffset = m_columnHeight / 4 + rebarLength % spacing / 2;
                    break;
                case TransverseRebarLocation.End: // end transverse rebar
                    rebarLength = m_columnHeight / 4;
                    curveOffset = m_columnHeight - rebarLength + rebarLength % spacing;
                    break;
                default:
                    throw new Exception("The program should never go here.");
            }

            // the number of the transverse rebar
            var rebarNumber = (int)(rebarLength / spacing) + 1;
            // get the profile of the transverse rebar
            var movedPoints = OffsetPoints(offset);

            var translatedPoints = new List<XYZ>();
            foreach (var point in movedPoints)
            {
                translatedPoints.Add(GeomUtil.OffsetPoint(point, DrivingVector, curveOffset));
            }

            IList<Curve> curves = new List<Curve>(); //the profile of the transverse rebar
            var first = translatedPoints[0];
            var second = translatedPoints[1];
            var third = translatedPoints[2];
            var fourth = translatedPoints[3];
            curves.Add(Line.CreateBound(first, second));
            curves.Add(Line.CreateBound(second, fourth));
            curves.Add(Line.CreateBound(fourth, third));
            curves.Add(Line.CreateBound(third, first));

            // return the rebar geometry information
            return new RebarGeometry(normal, curves, rebarNumber, spacing);
        }

        /// <summary>
        ///     Get the geometry information of vertical rebar
        /// </summary>
        /// <param name="location">the location of vertical rebar</param>
        /// <param name="rebarNumber">the spacing value of the rebar</param>
        /// <returns>the gotted geometry information</returns>
        public RebarGeometry GetVerticalRebar(VerticalRebarLocation location, int rebarNumber)
        {
            // sort the points of the swept profile
            var comparer = new XyzHeightComparer();
            Points.Sort(comparer);

            // Get the offset and rebar length of rebar
            var offset = ColumnRebarData.VerticalOffset;
            var rebarLength = m_columnHeight + 3; //the length of rebar

            // Get the start point of the vertical rebar curve
            var startPoint = DrivingLine.GetEndPoint(0);

            var movedPoints = OffsetPoints(offset);
            movedPoints.Sort(comparer);

            var normal = new XYZ(); // the normal parameter
            double rebarOffset = 0; // rebar offset, equal to rebarNumber* spacing 
            // get the normal, start point and rebar offset of vertical rebar
            switch (location)
            {
                case VerticalRebarLocation.East: //vertical rebar in east 
                    normal = new XYZ(0, 1, 0);
                    rebarOffset = m_columnWidth - 2 * offset;
                    startPoint = movedPoints[1];
                    break;
                case VerticalRebarLocation.North: //vertical rebar in north
                    normal = new XYZ(-1, 0, 0);
                    rebarOffset = m_columnLength - 2 * offset;
                    startPoint = movedPoints[3];
                    break;
                case VerticalRebarLocation.West: //vertical rebar in west
                    normal = new XYZ(0, -1, 0);
                    rebarOffset = m_columnWidth - 2 * offset;
                    startPoint = movedPoints[2];
                    break;
                case VerticalRebarLocation.South: //vertical rebar in south
                    normal = new XYZ(1, 0, 0);
                    rebarOffset = m_columnLength - 2 * offset;
                    startPoint = movedPoints[0];
                    break;
            }

            var spacing = rebarOffset / rebarNumber; //spacing value of the rebar
            var endPoint = GeomUtil.OffsetPoint(startPoint, DrivingVector, rebarLength);

            IList<Curve> curves = new List<Curve>
            {
                Line.CreateBound(startPoint, endPoint)
            }; //profile of the rebar

            // return the rebar geometry information
            return new RebarGeometry(normal, curves, rebarNumber, spacing);
        }

        /// <summary>
        ///     Get the length of the column
        /// </summary>
        /// <returns>the length data</returns>
        private double GetColumnLength()
        {
            var comparer = new XyzHeightComparer();
            Points.Sort(comparer);

            var refPoint = Points[0];
            var directions = GetRelatedVectors(refPoint);
            directions.Sort(comparer);

            return GeomUtil.GetLength(directions[0]);
        }

        /// <summary>
        ///     Get the width of the column
        /// </summary>
        /// <returns>the width data</returns>
        private double GetColumnWidth()
        {
            var comparer = new XyzHeightComparer();
            Points.Sort(comparer);

            var refPoint = Points[0];
            var directions = GetRelatedVectors(refPoint);
            directions.Sort(comparer);

            return GeomUtil.GetLength(directions[1]);
        }
    }
}

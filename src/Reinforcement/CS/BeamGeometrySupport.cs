// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using System;
using System.Collections.Generic;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Structure;

namespace Ara3D.RevitSampleBrowser.Reinforcement.CS
{
    /// <summary>
    ///     The geometry support for reinforcement creation on beam.
    ///     It can prepare the geometry information for top rebar, bottom and transverse rebar creation
    /// </summary>
    public class BeamGeometrySupport : GeometrySupport
    {
        private double m_beamHeight; //the height of the beam

        // Private members
        private readonly double m_beamLength; //the length of the beam
        private readonly double m_beamWidth; //the width of the beam

        /// <summary>
        ///     constructor
        /// </summary>
        /// <param name="element">the beam which the rebars are placed on</param>
        /// <param name="geoOptions">the geometry option</param>
        public BeamGeometrySupport(FamilyInstance element, Options geoOptions)
            : base(element, geoOptions)
        {
            // assert the host element is a beam
            if (!element.StructuralType.Equals(StructuralType.Beam))
                throw new Exception("BeamGeometrySupport can only work for beam instance.");

            // Get the length, width and height of the beam.
            m_beamLength = GetDrivingLineLength();
            m_beamWidth = GetBeamWidth();
            m_beamHeight = GetBeamHeight();
        }

        /// <summary>
        ///     Get the geometry information for top rebar
        /// </summary>
        /// <param name="location">indicate where top rebar is placed</param>
        /// <returns>the gotten geometry information</returns>
        public RebarGeometry GetTopRebar(TopRebarLocation location)
        {
            // sort the points of the swept profile
            var comparer = new XyzHeightComparer();
            Points.Sort(comparer);

            // Get the normal parameter for rebar creation
            var directions = GetRelatedVectors(Points[3]);
            directions.Sort(comparer);
            var normal = directions[1];

            double offset = 0; //the offset from the beam surface to the rebar
            double startPointOffset = 0; // the offset of start point from swept profile
            var rebarLength = m_beamLength / 3; //the length of the rebar
            var rebarNumber = BeamRebarData.TopRebarNumber; //the number of the rebar

            // set offset and startPointOffset according to the location of rebar
            switch (location)
            {
                case TopRebarLocation.Start: // top start rebar
                    offset = BeamRebarData.TopEndOffset;
                    break;
                case TopRebarLocation.Center: // top center rebar
                    offset = BeamRebarData.TopCenterOffset;
                    startPointOffset = m_beamLength / 3 - 0.5;
                    rebarLength = m_beamLength / 3 + 1;
                    break;
                case TopRebarLocation.End: // top end rebar
                    offset = BeamRebarData.TopEndOffset;
                    startPointOffset = m_beamLength * 2 / 3;
                    break;
                default:
                    throw new Exception("The program should never go here.");
            }

            // Get the curve which define the shape of the top rebar curve
            var movedPoints = OffsetPoints(offset);
            var startPoint = movedPoints[movedPoints.Count - 1];

            // offset the start point according startPointOffset
            startPoint = GeomUtil.OffsetPoint(startPoint, DrivingVector, startPointOffset);
            // get the coordinate of endpoint 
            var endPoint = GeomUtil.OffsetPoint(startPoint, DrivingVector, rebarLength);
            IList<Curve> curves = new List<Curve>
            {
                Line.CreateBound(startPoint, endPoint)
            }; //the profile of the top rebar

            // the spacing of the rebar
            double spacing = spacing = (m_beamWidth - 2 * offset) / (rebarNumber - 1);

            // return the rebar geometry information
            return new RebarGeometry(normal, curves, rebarNumber, spacing);
        }

        /// <summary>
        ///     Get the geometry information of bottom rebar
        /// </summary>
        /// <returns>the gotten geometry information</returns>
        public RebarGeometry GetBottomRebar()
        {
            // sort the points of the swept profile
            var comparer = new XyzHeightComparer();
            Points.Sort(comparer);

            // Get the normal parameter for bottom rebar creation
            var directions = GetRelatedVectors(Points[0]);
            directions.Sort(comparer);
            var normal = directions[0];

            var offset = BeamRebarData.BottomOffset; //offset value of the rebar
            var rebarNumber = BeamRebarData.BottomRebarNumber; //the number of the rebar
            // the spacing of the rebar
            var spacing = (m_beamWidth - 2 * offset) / (rebarNumber - 1);

            // Get the curve which define the shape of the bottom rebar curve
            var movedPoints = OffsetPoints(offset);
            var startPoint = movedPoints[0]; //get the coordinate of startpoint 
            //get the coordinate of endpoint  
            var endPoint = GeomUtil.OffsetPoint(startPoint, DrivingVector, m_beamLength);

            IList<Curve> curves = new List<Curve>
            {
                Line.CreateBound(startPoint, endPoint)
            }; //the profile of the bottom rebar

            // return the rebar geometry information
            return new RebarGeometry(normal, curves, rebarNumber, spacing);
        }

        /// <summary>
        ///     Get the geometry information of transverse rebar
        /// </summary>
        /// <param name="location">indicate which part of transverse rebar</param>
        /// <param name="spacing">the spacing of the rebar</param>
        /// <returns>the gotten geometry information</returns>
        public RebarGeometry GetTransverseRebar(TransverseRebarLocation location, double spacing)
        {
            // sort the points of the swept profile
            var comparer = new XyzHeightComparer();
            Points.Sort(comparer);

            // the offset from the beam surface to the rebar
            var offset = BeamRebarData.TransverseOffset;
            // the offset from the beam end to the transverse end
            var endOffset = BeamRebarData.TransverseEndOffset;
            // the offset between two transverses
            var betweenOffset = BeamRebarData.TransverseSpaceBetween;
            // the length of the transverse rebar
            var rebarLength = (m_beamLength - 2 * endOffset - 2 * betweenOffset) / 3;
            // the number of the transverse rebar
            var rebarNumber = (int)(rebarLength / spacing) + 1;

            // get the origin and normal parameter for rebar creation
            var normal = DrivingVector;
            double curveOffset = 0;

            //judge the coordinate of transverse rebar according to the location
            switch (location)
            {
                case TransverseRebarLocation.Start: // start transverse rebar
                    curveOffset = endOffset;
                    break;
                case TransverseRebarLocation.Center: // center transverse rebar
                    curveOffset = endOffset + rebarLength + betweenOffset;
                    curveOffset += rebarLength % spacing / 2;
                    break;
                case TransverseRebarLocation.End: // end transverse rebar
                    curveOffset = m_beamLength - endOffset - rebarLength + rebarLength % spacing;
                    break;
                default:
                    throw new Exception("The program should never go here.");
            }

            // get the profile of the transverse rebar
            var movedPoints = OffsetPoints(offset);

            // Translate curves points
            var translatedPoints = new List<XYZ>();
            foreach (var point in movedPoints)
            {
                translatedPoints.Add(GeomUtil.OffsetPoint(point, DrivingVector, curveOffset));
            }

            IList<Curve> curves = new List<Curve>();
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
        ///     Get the down direction, which stand for the top hook direction
        /// </summary>
        /// <returns>the down direction</returns>
        public XYZ GetDownDirection()
        {
            var comparer = new XyzHeightComparer();
            Points.Sort(comparer);

            var refPoint = Points[3];
            var directions = GetRelatedVectors(refPoint);
            directions.Sort(comparer);

            return directions[0];
        }

        /// <summary>
        ///     Get the width of the beam
        /// </summary>
        /// <returns>the width data</returns>
        private double GetBeamWidth()
        {
            var comparer = new XyzHeightComparer();
            Points.Sort(comparer);

            var refPoint = Points[0];
            var directions = GetRelatedVectors(refPoint);
            directions.Sort(comparer);

            return GeomUtil.GetLength(directions[0]);
        }

        /// <summary>
        ///     Get the height of the beam
        /// </summary>
        /// <returns>the height data</returns>
        private double GetBeamHeight()
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

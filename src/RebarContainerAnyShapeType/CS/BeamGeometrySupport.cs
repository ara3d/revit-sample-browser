//
// (C) Copyright 2015 by Autodesk, Inc.
//
// Permission to use, copy, modify, and distribute this software in
// object code form for any purpose and without fee is hereby granted,
// provided that the above copyright notice appears in all copies and
// that both that copyright notice and the limited warranty and
// restricted rights notice below appear in all supporting
// documentation.
//
// AUTODESK PROVIDES THIS PROGRAM "AS IS" AND WITH ALL FAULTS.
// AUTODESK SPECIFICALLY DISCLAIMS ANY IMPLIED WARRANTY OF
// MERCHANTABILITY OR FITNESS FOR A PARTICULAR USE. AUTODESK, INC.
// DOES NOT WARRANT THAT THE OPERATION OF THE PROGRAM WILL BE
// UNINTERRUPTED OR ERROR FREE.
//
// Use, duplication, or disclosure by the U.S. Government is subject to
// restrictions set forth in FAR 52.227-19 (Commercial Computer
// Software - Restricted Rights) and DFAR 252.227-7013(c)(1)(ii)
// (Rights in Technical Data and Computer Software), as applicable.
//


using System;
using System.Collections.Generic;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Structure;

namespace Revit.SDK.Samples.RebarContainerAnyShapeType.CS
{
    /// <summary>
    ///     The geometry support for reinforcement creation on beam.
    ///     It can prepare the geometry information for top reinforcement, bottom and transverse reinforcement creation
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
            var comparer = new XYZHeightComparer();
            m_points.Sort(comparer);

            // Get the normal parameter for reinforcement creation
            var directions = GetRelatedVectors(m_points[3]);
            directions.Sort(comparer);
            var normal = directions[1];

            double offset = 0; //the offset from the beam surface to the reinforcement
            double startPointOffset = 0; // the offset of start point from swept profile
            var rebarLength = m_beamLength / 3; //the length of the reinforcement
            var rebarNumber = BeamRebarData.TopRebarNumber; //the number of the reinforcement

            // set offset and startPointOffset according to the location of reinforcement
            switch (location)
            {
                case TopRebarLocation.Start: // top start reinforcement
                    offset = BeamRebarData.TopEndOffset;
                    break;
                case TopRebarLocation.Center: // top center reinforcement
                    offset = BeamRebarData.TopCenterOffset;
                    startPointOffset = m_beamLength / 3 - 0.5;
                    rebarLength = m_beamLength / 3 + 1;
                    break;
                case TopRebarLocation.End: // top end reinforcement
                    offset = BeamRebarData.TopEndOffset;
                    startPointOffset = m_beamLength * 2 / 3;
                    break;
                default:
                    throw new Exception("The program should never go here.");
            }

            // Get the curve which define the shape of the top reinforcement curve
            var movedPoints = OffsetPoints(offset);
            var startPoint = movedPoints[movedPoints.Count - 1];

            // offset the start point according startPointOffset
            startPoint = GeomUtil.OffsetPoint(startPoint, m_drivingVector, startPointOffset);
            // get the coordinate of endpoint 
            var endPoint = GeomUtil.OffsetPoint(startPoint, m_drivingVector, rebarLength);
            IList<Curve> curves = new List<Curve>(); //the profile of the top reinforcement
            curves.Add(Line.CreateBound(startPoint, endPoint));

            // the spacing of the reinforcement
            double spacing = spacing = (m_beamWidth - 2 * offset) / (rebarNumber - 1);

            // return the reinforcement geometry information
            return new RebarGeometry(normal, curves, rebarNumber, spacing);
        }

        /// <summary>
        ///     Get the geometry information of bottom reinforcement
        /// </summary>
        /// <returns>the gotten geometry information</returns>
        public RebarGeometry GetBottomRebar()
        {
            // sort the points of the swept profile
            var comparer = new XYZHeightComparer();
            m_points.Sort(comparer);

            // Get the normal parameter for bottom reinforcement creation
            var directions = GetRelatedVectors(m_points[0]);
            directions.Sort(comparer);
            var normal = directions[0];

            var offset = BeamRebarData.BottomOffset; //offset value of the reinforcement
            var rebarNumber = BeamRebarData.BottomRebarNumber; //the number of the reinforcement
            // the spacing of the reinforcement
            var spacing = (m_beamWidth - 2 * offset) / (rebarNumber - 1);

            // Get the curve which define the shape of the bottom reinforcement curve
            var movedPoints = OffsetPoints(offset);
            var startPoint = movedPoints[0]; //get the coordinate of startpoint 
            //get the coordinate of endpoint  
            var endPoint = GeomUtil.OffsetPoint(startPoint, m_drivingVector, m_beamLength);

            IList<Curve> curves = new List<Curve>(); //the profile of the bottom reinforcement
            curves.Add(Line.CreateBound(startPoint, endPoint));

            // return the reinforcement geometry information
            return new RebarGeometry(normal, curves, rebarNumber, spacing);
        }

        /// <summary>
        ///     Get the geometry information of transverse reinforcement
        /// </summary>
        /// <param name="location">indicate which part of transverse rebar</param>
        /// <param name="spacing">the spacing of the rebar</param>
        /// <returns>the gotten geometry information</returns>
        public RebarGeometry GetTransverseRebar(TransverseRebarLocation location, double spacing)
        {
            // sort the points of the swept profile
            var comparer = new XYZHeightComparer();
            m_points.Sort(comparer);

            // the offset from the beam surface to the reinforcement
            var offset = BeamRebarData.TransverseOffset;
            // the offset from the beam end to the transverse end
            var endOffset = BeamRebarData.TransverseEndOffset;
            // the offset between two transverses
            var betweenOffset = BeamRebarData.TransverseSpaceBetween;
            // the length of the transverse reinforcement
            var rebarLength = (m_beamLength - 2 * endOffset - 2 * betweenOffset) / 3;
            // the number of the transverse reinforcement
            var rebarNumber = (int)(rebarLength / spacing) + 1;

            // get the origin and normal parameter for reinforcement creation
            var normal = m_drivingVector;
            double curveOffset = 0;

            //judge the coordinate of transverse reinforcement according to the location
            switch (location)
            {
                case TransverseRebarLocation.Start: // start transverse reinforcement
                    curveOffset = endOffset;
                    break;
                case TransverseRebarLocation.Center: // center transverse reinforcement
                    curveOffset = endOffset + rebarLength + betweenOffset;
                    curveOffset = curveOffset + rebarLength % spacing / 2;
                    break;
                case TransverseRebarLocation.End: // end transverse reinforcement
                    curveOffset = m_beamLength - endOffset - rebarLength + rebarLength % spacing;
                    break;
                default:
                    throw new Exception("The program should never go here.");
            }

            // get the profile of the transverse reinforcement
            var movedPoints = OffsetPoints(offset);

            // Translate curves points
            var translatedPoints = new List<XYZ>();
            foreach (var point in movedPoints)
                translatedPoints.Add(GeomUtil.OffsetPoint(point, m_drivingVector, curveOffset));

            IList<Curve> curves = new List<Curve>();
            var first = translatedPoints[0];
            var second = translatedPoints[1];
            var third = translatedPoints[2];
            var fourth = translatedPoints[3];

            curves.Add(Line.CreateBound(first, second));
            curves.Add(Line.CreateBound(second, fourth));
            curves.Add(Line.CreateBound(fourth, third));
            curves.Add(Line.CreateBound(third, first));

            // return the reinforcement geometry information
            return new RebarGeometry(normal, curves, rebarNumber, spacing);
        }


        /// <summary>
        ///     Get the down direction, which stand for the top hook direction
        /// </summary>
        /// <returns>the down direction</returns>
        public XYZ GetDownDirection()
        {
            var comparer = new XYZHeightComparer();
            m_points.Sort(comparer);

            var refPoint = m_points[3];
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
            var comparer = new XYZHeightComparer();
            m_points.Sort(comparer);

            var refPoint = m_points[0];
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
            var comparer = new XYZHeightComparer();
            m_points.Sort(comparer);

            var refPoint = m_points[0];
            var directions = GetRelatedVectors(refPoint);
            directions.Sort(comparer);

            return GeomUtil.GetLength(directions[1]);
        }
    }
}
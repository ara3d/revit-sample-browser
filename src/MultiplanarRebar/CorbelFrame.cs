// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using System;
using System.Collections.Generic;
using System.Linq;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Structure;

namespace Ara3D.RevitSampleBrowser.MultiplanarRebar.CS
{
    /// <summary>
    ///     This class represents the trapezoid wire frame profile of corbel.
    ///     Its two main functionalities are to create a multi-planar rebar shape and
    ///     to calculate the location for rebar creation when reinforcing corbel.
    /// </summary>
    public class Trapezoid
    {
        /// <summary>
        ///     Constructor to initialize the fields.
        /// </summary>
        /// <param name="top">Top Line</param>
        /// <param name="vertical">Left Vertical Line</param>
        /// <param name="bottom">Bottom Line</param>
        /// <param name="slanted">Right slanted Line</param>
        public Trapezoid(Line top, Line vertical, Line bottom, Line slanted)
        {
            Top = top;
            Vertical = vertical;
            Bottom = bottom;
            Slanted = slanted;
        }
        //
        //             TOP       
        //         |---------\      
        // Vertical|          \Slanted
        //         |   Bottom  \
        //---------|------------\
        //              
        // Top -> Vertical -> Bottom -> Slanted form counter clockwise orientation.

        /// <summary>
        ///     Top bound line of this trapezoid.
        /// </summary>
        public Line Top { get; set; }

        /// <summary>
        ///     Left vertical bound line of this trapezoid.
        /// </summary>
        public Line Vertical { get; set; }

        /// <summary>
        ///     Bottom bound line of this trapezoid.
        /// </summary>
        public Line Bottom { get; set; }

        /// <summary>
        ///     Right slanted bound line of this trapezoid.
        /// </summary>
        public Line Slanted { get; set; }

        /// <summary>
        ///     Draw the trapezoid wire-frame with Revit Model curves.
        ///     It's for debug use, to help developer see the exact location.
        /// </summary>
        /// <param name="revitDoc">Revit DB Document</param>
        public void Draw(Document revitDoc)
        {
            var topDir = (Top.GetEndPoint(1) - Top.GetEndPoint(0)).Normalize();
            var verticalDir = (Vertical.GetEndPoint(0) - Vertical.GetEndPoint(1)).Normalize();
            var normal = topDir.CrossProduct(verticalDir);

            var sketchplane =
                SketchPlane.Create(revitDoc, Plane.CreateByNormalAndOrigin(normal, Vertical.GetEndPoint(0)));

            var curves = new CurveArray();
            curves.Append(Top.Clone());
            curves.Append(Vertical.Clone());
            curves.Append(Bottom.Clone());
            curves.Append(Slanted.Clone());
            revitDoc.Create.NewModelCurveArray(curves, sketchplane);
        }

        /// <summary>
        ///     Offset the top line with given value, if the value is positive,
        ///     the offset direction is outside, otherwise inside.
        /// </summary>
        /// <param name="offset">Offset value</param>
        public void OffsetTop(double offset)
        {
            var verticalDir = (Vertical.GetEndPoint(0) - Vertical.GetEndPoint(1)).Normalize();
            var verticalDelta = verticalDir * offset;
            var verticalFinal = Vertical.GetEndPoint(0) + verticalDelta;

            var verticalLengthNew = Vertical.Length + offset;
            var slantedLengthNew = verticalLengthNew * Slanted.Length / Vertical.Length;

            var slantedDir = (Slanted.GetEndPoint(1) - Slanted.GetEndPoint(0)).Normalize();
            var slantedFinal = Slanted.GetEndPoint(0) + slantedDir * slantedLengthNew;

            Vertical = Line.CreateBound(verticalFinal, Vertical.GetEndPoint(1));
            Top = Line.CreateBound(slantedFinal, verticalFinal);
            Slanted = Line.CreateBound(Slanted.GetEndPoint(0), slantedFinal);
        }

        /// <summary>
        ///     Offset the Left Vertical line with given value, if the value is positive,
        ///     the offset direction is outside, otherwise inside.
        /// </summary>
        /// <param name="offset">Offset value</param>
        public void OffsetLeft(double offset)
        {
            var topDir = (Top.GetEndPoint(1) - Top.GetEndPoint(0)).Normalize();
            var topDelta = topDir * offset;

            var topFinal = Top.GetEndPoint(1) + topDelta;
            var bottomFinal = Bottom.GetEndPoint(0) + topDelta;

            Vertical = Line.CreateBound(topFinal, bottomFinal);
            Bottom = Line.CreateBound(bottomFinal, Bottom.GetEndPoint(1));
            Top = Line.CreateBound(Top.GetEndPoint(0), topFinal);
        }

        /// <summary>
        ///     Offset the bottom line with given value, if the value is positive,
        ///     the offset direction is outside, otherwise inside.
        /// </summary>
        /// <param name="offset">Offset value</param>
        public void OffsetBottom(double offset)
        {
            var verticalDir = (Vertical.GetEndPoint(1) - Vertical.GetEndPoint(0)).Normalize();
            var verticalDelta = verticalDir * offset;
            var verticalFinal = Vertical.GetEndPoint(1) + verticalDelta;

            var verticalLengthNew = Vertical.Length + offset;

            var slantedLengthNew = verticalLengthNew * Slanted.Length / Vertical.Length;

            var slantedDir = (Slanted.GetEndPoint(0) - Slanted.GetEndPoint(1)).Normalize();
            var slantedFinal = Slanted.GetEndPoint(1) + slantedDir * slantedLengthNew;

            Vertical = Line.CreateBound(Vertical.GetEndPoint(0), verticalFinal);
            Bottom = Line.CreateBound(verticalFinal, slantedFinal);
            Slanted = Line.CreateBound(slantedFinal, Slanted.GetEndPoint(1));
        }

        /// <summary>
        ///     Offset the right slanted line with given value, if the value is positive,
        ///     the offset direction is outside, otherwise inside.
        /// </summary>
        /// <param name="offset">Offset value</param>
        public void OffsetRight(double offset)
        {
            var bottomDir = (Bottom.GetEndPoint(1) - Bottom.GetEndPoint(0)).Normalize();
            var bottomDelta = bottomDir * (offset * Slanted.Length / Vertical.Length);

            var topFinal = Top.GetEndPoint(0) + bottomDelta;
            var bottomFinal = Bottom.GetEndPoint(1) + bottomDelta;

            Top = Line.CreateBound(topFinal, Top.GetEndPoint(1));
            Bottom = Line.CreateBound(Bottom.GetEndPoint(0), bottomFinal);
            Slanted = Line.CreateBound(bottomFinal, topFinal);
        }

        /// <summary>
        ///     Deep clone, to avoid mess up the original data during offsetting the boundary.
        /// </summary>
        /// <returns>Cloned object</returns>
        public Trapezoid Clone()
        {
            return new Trapezoid(
                Top.Clone() as Line,
                Vertical.Clone() as Line,
                Bottom.Clone() as Line,
                Slanted.Clone() as Line);
        }

        /// <summary>
        ///     Create the multi-planar Rebar Shape according to the trapezoid wire-frame.
        /// </summary>
        /// <param name="revitDoc">Revit DB Document</param>
        /// ///
        /// <param name="bendDiameter">OutOfPlaneBendDiameter for multi-planar shape</param>
        /// <returns>Created multi-planar Rebar Shape</returns>
        public RebarShape ConstructMultiplanarRebarShape(Document revitDoc, double bendDiameter)
        {
            // Construct a segment definition with 2 lines.
            var shapedef = new RebarShapeDefinitionBySegments(revitDoc, 2);

            // Define parameters for the dimension.
            var b = SharedParameterUtil.GetOrCreateDef("B", revitDoc);
            var h = SharedParameterUtil.GetOrCreateDef("H", revitDoc);
            var k = SharedParameterUtil.GetOrCreateDef("K", revitDoc);
            var mm = SharedParameterUtil.GetOrCreateDef("MM", revitDoc);

            // Set parameters default values according to the size Trapezoid shape. 
            shapedef.AddParameter(b, Top.Length);
            shapedef.AddParameter(h, Bottom.Length - Top.Length);
            shapedef.AddParameter(k, Vertical.Length);
            shapedef.AddParameter(mm, 15);

            // Rebar shape geometry curves consist of Line S0 and Line S1.
            // 
            //
            //         |Y       V1
            //         |--S0(B)--\      
            //         |          \S1(H, K)
            //         |           \
            //---------|O-----------\----X
            //         |       

            // Define Segment 0 (S0)
            //
            // S0's direction is fixed in positive X Axis.
            shapedef.SetSegmentFixedDirection(0, 1, 0);
            // S0's length is determined by parameter B
            shapedef.AddConstraintParallelToSegment(0, b, false, false);

            // Define Segment 1 (S1)
            //
            // Fix S1's direction.
            shapedef.SetSegmentFixedDirection(1, Bottom.Length - Top.Length, -Vertical.Length);
            // S1's length in positive X Axis is parameter H.            
            shapedef.AddConstraintToSegment(1, h, 1, 0, 1, false, false);
            // S1's length in negative Y Axis is parameter K.
            shapedef.AddConstraintToSegment(1, k, 0, -1, 1, false, false);

            // Define Vertex 1 (V1)
            //
            // S1 at V1 is turn to right and the angle is acute.
            shapedef.AddBendDefaultRadius(1, RebarShapeVertexTurn.Right, RebarShapeBendAngle.Acute);

            // Check to see if it's full constrained.  
            if (!shapedef.Complete) throw new Exception("Shape was not completed.");

            // Try to solve it to make sure the shape can be resolved with default parameter value.
            if (!shapedef.CheckDefaultParameterValues(0, 0)) throw new Exception("Can't resolve rebar shape.");

            // Define multi-planar definition
            var multiPlanarDef = new RebarShapeMultiplanarDefinition(bendDiameter)
            {
                DepthParamId = mm
            };

            // Realize the Rebar shape with creation static method.
            // The RebarStype is stirrupTie, and it will attach to the top cover.
            var newshape = RebarShape.Create(revitDoc, shapedef, multiPlanarDef,
                RebarStyle.StirrupTie, StirrupTieAttachmentType.InteriorFace,
                0, RebarHookOrientation.Left, 0, RebarHookOrientation.Left, 0);

            // Give a readable name
            newshape.Name = $"API Corbel Multi-Shape {newshape.Id}";

            // Make sure we can see the created shape from the browser.
            var curvesForBrowser = newshape.GetCurvesForBrowser();
            if (curvesForBrowser.Count == 0) throw new Exception("The Rebar shape is invisible in browser.");

            return newshape;
        }

        /// <summary>
        ///     Calculate the boundary coordinate of the wire-frame.
        /// </summary>
        /// <param name="origin">Origin coordinate</param>
        /// <param name="vX">X Vector</param>
        /// <param name="vY">Y Vector</param>
        public void Boundary(out XYZ origin, out XYZ vX, out XYZ vY)
        {
            origin = Vertical.GetEndPoint(1);
            vX = Bottom.GetEndPoint(1) - Bottom.GetEndPoint(0);
            vY = Vertical.GetEndPoint(0) - Vertical.GetEndPoint(1);
        }
    }

    /// <summary>
    ///     It represents the frame of Corbel, which is consist of a trapezoid profile and a extrusion line.
    ///     Corbel can be constructed by sweeping a trapezoid profile along the extrusion line.
    /// </summary>
    public class CorbelFrame
    {
        /// <summary>
        ///     Corbel family instance.
        /// </summary>
        private readonly FamilyInstance m_corbel;

        /// <summary>
        ///     Cover distance of corbel family instance.
        /// </summary>
        private readonly double m_corbelCoverDistance;

        /// <summary>
        ///     Extrusion line of corbel family instance.
        /// </summary>
        private readonly Line m_extrusionLine;

        /// <summary>
        ///     Cover distance of corbel host.
        /// </summary>
        private readonly double m_hostCoverDistance;

        /// <summary>
        ///     Depth of corbel host.
        /// </summary>
        private readonly double m_hostDepth;

        /// <summary>
        ///     Trapezoid profile of corbel family instance.
        /// </summary>
        private readonly Trapezoid m_profile;

        /// <summary>
        ///     Constructor to initialize the fields.
        /// </summary>
        /// <param name="corbel">Corbel family instance</param>
        /// <param name="profile">Trapezoid profile</param>
        /// <param name="path">Extrusion Line</param>
        /// <param name="hostDepth">Corbel Host Depth</param>
        /// <param name="hostTopCorverDistance">Corbel Host cover distance</param>
        public CorbelFrame(FamilyInstance corbel, Trapezoid profile,
            Line path, double hostDepth, double hostTopCorverDistance)
        {
            m_profile = profile;
            m_extrusionLine = path;
            m_corbel = corbel;
            m_hostDepth = hostDepth;
            m_hostCoverDistance = hostTopCorverDistance;

            // Get the cover distance of corbel from CommonCoverType.
            var rebarHost = RebarHostData.GetRebarHostData(m_corbel);
            m_corbelCoverDistance = rebarHost.GetCommonCoverType().CoverDistance;
        }

        /// <summary>
        ///     Parse the geometry of given Corbel and create a CorbelFrame if the corbel is slopped,
        ///     otherwise exception thrown.
        /// </summary>
        /// <param name="corbel">Corbel to parse</param>
        /// <returns>A created CorbelFrame</returns>
        public static CorbelFrame Parse(FamilyInstance corbel)
        {
            // This just delegates a call to GeometryUtil class.
            return GeometryUtil.ParseCorbelGeometry(corbel);
        }

        /// <summary>
        ///     Add bars to reinforce the Corbel FamilyInstance with given options.
        ///     The bars including:
        ///     a multi-planar bar,
        ///     top straight bars,
        ///     stirrup bars,
        ///     and host straight bars.
        /// </summary>
        /// <param name="rebarOptions">Options for Rebar Creation</param>
        public void Reinforce(CorbelReinforcementOptions rebarOptions)
        {
            PlaceStraightBars(rebarOptions);

            PlaceMultiplanarRebar(rebarOptions);

            PlaceStirrupBars(rebarOptions);

            PlaceCorbelHostBars(rebarOptions);
        }

        /// <summary>
        ///     Add straight bars into corbel with given options.
        /// </summary>
        /// <param name="options">Options for Rebar Creation</param>
        private void PlaceStraightBars(CorbelReinforcementOptions options)
        {
            var profileCopy = m_profile.Clone();
            profileCopy.OffsetTop(-m_corbelCoverDistance);
            profileCopy.OffsetLeft(-m_corbelCoverDistance
                                   - options.MultiplanarBarType.BarModelDiameter
                                   - options.TopBarType.BarModelDiameter * 0.5);
            profileCopy.OffsetBottom(m_hostDepth - m_hostCoverDistance
                                                 - options.StirrupBarType.BarModelDiameter
                                                 - options.HostStraightBarType.BarModelDiameter);
            profileCopy.OffsetRight(-m_corbelCoverDistance);

            //m_profile.Draw(options.RevitDoc);
            //profileCopy.Draw(options.RevitDoc);

            var extruDir = (m_extrusionLine.GetEndPoint(1) - m_extrusionLine.GetEndPoint(0)).Normalize();
            var offset = m_corbelCoverDistance +
                         options.StirrupBarType.BarModelDiameter +
                         options.MultiplanarBarType.BarModelDiameter +
                         0.5 * options.TopBarType.BarModelDiameter;

            var vetical = profileCopy.Vertical;
            var delta = extruDir * offset;
            Curve barLine = Line.CreateBound(vetical.GetEndPoint(1) + delta, vetical.GetEndPoint(0) + delta);
            IList<Curve> barCurves = new List<Curve>
            {
                barLine
            };

            var bars = Rebar.CreateFromCurves(options.RevitDoc, RebarStyle.Standard,
                options.TopBarType, null, null, m_corbel, extruDir, barCurves,
                RebarHookOrientation.Left, RebarHookOrientation.Left, true, true);

            bars.GetShapeDrivenAccessor().SetLayoutAsFixedNumber(options.TopBarCount + 2,
                m_extrusionLine.Length - 2 * offset, true, false, false);
        }

        /// <summary>
        ///     Add a multi-planar bar into corbel with given options.
        /// </summary>
        /// <param name="options">Options for Rebar Creation</param>
        private void PlaceMultiplanarRebar(CorbelReinforcementOptions options)
        {
            var profileCopy = m_profile.Clone();
            profileCopy.OffsetTop(-m_corbelCoverDistance
                                  - options.StirrupBarType.BarModelDiameter -
                                  0.5 * options.MultiplanarBarType.BarModelDiameter);
            profileCopy.OffsetLeft(-m_corbelCoverDistance - 0.5 * options.MultiplanarBarType.BarModelDiameter);
            profileCopy.OffsetBottom(m_hostDepth - m_hostCoverDistance
                                                 - options.HostStraightBarType.BarModelDiameter * 4
                                                 - options.StirrupBarType.BarModelDiameter);
            profileCopy.OffsetRight(-m_corbelCoverDistance - options.StirrupBarType.BarModelDiameter);

            //m_profile.Draw(options.RevitDoc);
            //profileCopy.Draw(options.RevitDoc);

            XYZ origin, vx, vy;
            profileCopy.Boundary(out origin, out vx, out vy);

            var vecX = vx.Normalize();
            var vecY = vy.Normalize();
            var barshape = profileCopy.ConstructMultiplanarRebarShape(options.RevitDoc,
                0.5 * options.MultiplanarBarType.StirrupTieBendDiameter);
            var newRebar = Rebar.CreateFromRebarShape(
                options.RevitDoc, barshape,
                options.MultiplanarBarType,
                m_corbel, origin, vecX, vecY);

            var extruDir = (m_extrusionLine.GetEndPoint(1) - m_extrusionLine.GetEndPoint(0)).Normalize();
            var offset = m_corbelCoverDistance +
                         options.StirrupBarType.BarModelDiameter +
                         0.5 * options.MultiplanarBarType.BarModelDiameter;

            newRebar.GetShapeDrivenAccessor().ScaleToBoxFor3D(origin + extruDir * (m_extrusionLine.Length - offset),
                vx, vy, m_extrusionLine.Length - 2 * offset);
        }

        /// <summary>
        ///     Add stirrup bars into corbel with given options.
        /// </summary>
        /// <param name="options">Options for Rebar Creation</param>
        private void PlaceStirrupBars(CorbelReinforcementOptions options)
        {
            var filter = new FilteredElementCollector(options.RevitDoc)
                .OfClass(typeof(RebarShape)).ToElements().Cast<RebarShape>()
                .Where(shape => shape.RebarStyle == RebarStyle.StirrupTie);

            RebarShape stirrupShape = null;
            foreach (var shape in filter)
            {
                if (shape.Name.Equals("T1"))
                {
                    stirrupShape = shape;
                    break;
                }
            }

            var profileCopy = m_profile.Clone();
            profileCopy.OffsetTop(-m_corbelCoverDistance - 0.5 * options.StirrupBarType.BarModelDiameter);
            profileCopy.OffsetLeft(-m_corbelCoverDistance - 0.5 * options.StirrupBarType.BarModelDiameter);
            profileCopy.OffsetBottom(m_hostDepth - m_hostCoverDistance - 0.5 * options.StirrupBarType.BarModelDiameter);
            profileCopy.OffsetRight(-m_corbelCoverDistance - 0.5 * options.StirrupBarType.BarModelDiameter);

            var extruDir = (m_extrusionLine.GetEndPoint(1) - m_extrusionLine.GetEndPoint(0)).Normalize();
            var offset = m_corbelCoverDistance + 0.5 * options.StirrupBarType.BarModelDiameter;

            var origin = profileCopy.Vertical.GetEndPoint(0) + extruDir * offset;
            var xAxis = extruDir;
            var yAxis = (profileCopy.Vertical.GetEndPoint(1) - profileCopy.Vertical.GetEndPoint(0)).Normalize();

            var stirrupBars = Rebar.CreateFromRebarShape(options.RevitDoc, stirrupShape,
                options.StirrupBarType, m_corbel, origin, xAxis, yAxis);

            var xLength = m_extrusionLine.Length - 2 * offset;
            var yLength = profileCopy.Vertical.Length;

            stirrupBars.GetShapeDrivenAccessor().SetLayoutAsFixedNumber(options.StirrupBarCount + 1,
                profileCopy.Top.Length, false, false, true);
            stirrupBars.GetShapeDrivenAccessor().ScaleToBox(origin, xAxis * xLength, yAxis * yLength);

            var space = profileCopy.Top.Length / options.StirrupBarCount;
            var step = space * m_profile.Vertical.Length / (m_profile.Bottom.Length - m_profile.Top.Length);

            var dirTop = (m_profile.Top.GetEndPoint(0) - m_profile.Top.GetEndPoint(1)).Normalize();
            var dirVertical = yAxis;
            var deltaStep = dirTop * space + dirVertical * step;

            origin = profileCopy.Top.GetEndPoint(0) + extruDir * offset;
            var count = (int)((m_profile.Vertical.Length - m_corbelCoverDistance -
                               0.5 * options.StirrupBarType.BarModelDiameter) / step);
            for (var i = 1; i <= count; i++)
            {
                origin += deltaStep;
                var stirrupBars2 = Rebar.CreateFromRebarShape(options.RevitDoc, stirrupShape,
                    options.StirrupBarType, m_corbel, origin, xAxis, yAxis);

                stirrupBars2.GetShapeDrivenAccessor().ScaleToBox(origin, xAxis * xLength, yAxis * (yLength - i * step));
            }
        }

        /// <summary>
        ///     Add straight bars into corbel Host to anchor corbel stirrup bars.
        /// </summary>
        /// <param name="options">Options for Rebar Creation</param>
        private void PlaceCorbelHostBars(CorbelReinforcementOptions options)
        {
            var profileCopy = m_profile.Clone();
            profileCopy.OffsetBottom(m_hostDepth - m_hostCoverDistance
                                                 - options.HostStraightBarType.BarModelDiameter * 0.5
                                                 - options.StirrupBarType.BarModelDiameter);

            //profileCopy.Draw(options.RevitDoc);

            var extruDir = (m_extrusionLine.GetEndPoint(1) - m_extrusionLine.GetEndPoint(0)).Normalize();
            var offset = m_corbelCoverDistance + options.StirrupBarType.BarModelDiameter
                                               + options.HostStraightBarType.BarModelDiameter * 0.5;
            var delta = extruDir * offset;

            var pt1 = profileCopy.Bottom.GetEndPoint(0) + delta;
            var pt2 = profileCopy.Bottom.GetEndPoint(1) + delta;

            Curve barLine = Line.CreateBound(pt1, pt2);
            IList<Curve> barCurves = new List<Curve>
            {
                barLine
            };

            var bars = Rebar.CreateFromCurves(
                options.RevitDoc, RebarStyle.Standard,
                options.HostStraightBarType, null, null, m_corbel.Host, extruDir, barCurves,
                RebarHookOrientation.Left, RebarHookOrientation.Left, true, true);

            bars.GetShapeDrivenAccessor()
                .SetLayoutAsFixedNumber(2, m_extrusionLine.Length - 2 * offset, true, true, true);
        }
    }
}

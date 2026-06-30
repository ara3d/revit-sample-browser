// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media.Imaging;
using System.Xml.Linq;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Architecture;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using Color = System.Drawing.Color;
using OperationCanceledException = Autodesk.Revit.Exceptions.OperationCanceledException;
using Rectangle = System.Drawing.Rectangle;
using WinForms = System.Windows.Forms;


namespace BuildingCoder
{
    internal static partial class Util
    {
        #region Geometrical Calculation

        /// <summary>
        ///     Return arbitrary X and Y axes for the given
        ///     normal vector according to the AutoCAD
        ///     Arbitrary Axis Algorithm
        ///     https://www.autodesk.com/techpubs/autocad/acadr14/dxf/arbitrary_axis_algorithm_al_u05_c.htm
        /// </summary>
        public static void GetArbitraryAxes(
            XYZ normal,
            out XYZ ax,
            out XYZ ay)
        {
            var limit = 1.0 / 64;

            var pick_cardinal_axis
                = IsZero(normal.X, limit)
                  && IsZero(normal.Y, limit)
                    ? XYZ.BasisY
                    : XYZ.BasisZ;

            ax = pick_cardinal_axis.CrossProduct(normal).Normalize();
            ay = normal.CrossProduct(ax).Normalize();
        }

        /// <summary>
        ///     Return the midpoint between two points.
        /// </summary>
        public static XYZ Midpoint(XYZ p, XYZ q)
        {
            return 0.5 * (p + q);
        }

        /// <summary>
        ///     Return the midpoint of a Line.
        /// </summary>
        public static XYZ Midpoint(Line line)
        {
            return Midpoint(line.GetEndPoint(0),
                line.GetEndPoint(1));
        }

        /// <summary>
        ///     Return the normal of a Line in the XY plane.
        /// </summary>
        public static XYZ Normal(Line line)
        {
            var p = line.GetEndPoint(0);
            var q = line.GetEndPoint(1);
            var v = q - p;

            //Debug.Assert( IsZero( v.Z ),
            //  "expected horizontal line" );

            return v.CrossProduct(XYZ.BasisZ).Normalize();
        }

        /// <summary>
        ///     Return the bounding box of a curve loop.
        /// </summary>
        public static BoundingBoxXYZ GetBoundingBox(
            CurveLoop curveLoop)
        {
            var pts = new List<XYZ>();
            foreach (var c in curveLoop) pts.AddRange(c.Tessellate());

            var bb = new BoundingBoxXYZ();
            bb.Clear();
            bb.ExpandToContain(pts);
            return bb;
        }

        /// <summary>
        ///     Return the bottom four XYZ corners of the given
        ///     bounding box in the XY plane at the given
        ///     Z elevation in the order lower left, lower
        ///     right, upper right, upper left:
        /// </summary>
        public static XYZ[] GetBottomCorners(
            BoundingBoxXYZ b,
            double z)
        {
            return new[]
            {
                new(b.Min.X, b.Min.Y, z),
                new XYZ(b.Max.X, b.Min.Y, z),
                new XYZ(b.Max.X, b.Max.Y, z),
                new XYZ(b.Min.X, b.Max.Y, z)
            };
        }

        /// <summary>
        ///     Return the bottom four XYZ corners of the given
        ///     bounding box in the XY plane at the bb minimum
        ///     Z elevation in the order lower left, lower
        ///     right, upper right, upper left:
        /// </summary>
        public static XYZ[] GetBottomCorners(
            BoundingBoxXYZ b)
        {
            return GetBottomCorners(b, b.Min.Z);
        }

        #region Intersect

#if INTERSECT
    // from /a/src/cpp/wykobi/wykobi.inl
    // from https://github.com/ArashPartow/wykobi
    bool Intersect<T>( 
      T x1, T y1,
      T x2, T y2,
      T x3, T y3,
      T x4, T y4)
   {
      T ax = x2 - x1;
      T bx = x3 - x4;

    T lowerx;
    T upperx;
    T uppery;
    T lowery;

      if (ax<T(0.0))
      {
         lowerx = x2;
         upperx = x1;
      }
      else
      {
         upperx = x2;
         lowerx = x1;
      }

      if (bx > T(0.0))
      {
         if ((upperx<x4) || (x3<lowerx))
         return false;
      }
      else if ((upperx<x3) || (x4<lowerx))
         return false;

      const T ay = y2 - y1;
const T by = y3 - y4;

      if (ay<T(0.0))
      {
         lowery = y2;
         uppery = y1;
      }
      else
      {
         uppery = y2;
         lowery = y1;
      }

      if (by > T(0.0))
      {
         if ((uppery<y4) || (y3<lowery))
            return false;
      }
      else if ((uppery<y3) || (y4<lowery))
         return false;

      const T cx = x1 - x3;
const T cy = y1 - y3;
const T d = ( by * cx ) - ( bx * cy );
const T f = ( ay * bx ) - ( ax * by );

      if (f > T(0.0))
      {
         if ((d<T(0.0)) || (d > f))
            return false;
      }
      else if ((d > T(0.0)) || (d<f))
         return false;

      const T e = ( ax * cy ) - ( ay * cx );

      if (f > T(0.0))
      {
         if ((e<T(0.0)) || (e > f))
            return false;
      }
      else if ((e > T(0.0)) || (e<f))
         return false;

      return true;
   }

   bool Intersect<T>(T x1, T y1,
                         T x2, T y2,
                         T x3, T y3,
                         T x4, T y4,
                               out T ix, out T iy)
{
  const T ax = x2 - x1;
  const T bx = x3 - x4;

  T lowerx;
  T upperx;
  T uppery;
  T lowery;

  if( ax < T( 0.0 ) )
  {
    lowerx = x2;
    upperx = x1;
  }
  else
  {
    upperx = x2;
    lowerx = x1;
  }

  if( bx > T( 0.0 ) )
  {
    if( ( upperx < x4 ) || ( x3 < lowerx ) )
      return false;
  }
  else if( ( upperx < x3 ) || ( x4 < lowerx ) )
    return false;

  const T ay = y2 - y1;
  const T by = y3 - y4;

  if( ay < T( 0.0 ) )
  {
    lowery = y2;
    uppery = y1;
  }
  else
  {
    uppery = y2;
    lowery = y1;
  }

  if( by > T( 0.0 ) )
  {
    if( ( uppery < y4 ) || ( y3 < lowery ) )
      return false;
  }
  else if( ( uppery < y3 ) || ( y4 < lowery ) )
    return false;

  const T cx = x1 - x3;
  const T cy = y1 - y3;
  const T d = ( by * cx ) - ( bx * cy );
  const T f = ( ay * bx ) - ( ax * by );

  if( f > T( 0.0 ) )
  {
    if( ( d < T( 0.0 ) ) || ( d > f ) )
      return false;
  }
  else if( ( d > T( 0.0 ) ) || ( d < f ) )
    return false;

  const T e = ( ax * cy ) - ( ay * cx );

  if( f > T( 0.0 ) )
  {
    if( ( e < T( 0.0 ) ) || ( e > f ) )
      return false;
  }
  else if( ( e > T( 0.0 ) ) || ( e < f ) )
    return false;

  T ratio = ( ax * -by ) - ( ay * -bx );

  if( not_equal( ratio, T( 0.0 ) ) )
  {
    ratio = ( ( cy * -bx ) - ( cx * -by ) ) / ratio;
    ix = x1 + ( ratio * ax );
    iy = y1 + ( ratio * ay );
  }
  else
  {
    if( is_equal( ( ax * -cy ), ( -cx * ay ) ) )
    {
      ix = x3;
      iy = y3;
    }
    else
    {
      ix = x4;
      iy = y4;
    }
  }
  return true;
}
#endif // INTERSECT

        #endregion // Intersect

        /// <summary>
        ///     Return the 2D intersection point between two
        ///     unbounded lines defined in the XY plane by the
        ///     given start and end points and vectors.
        ///     Return null if the two lines are coincident,
        ///     in which case the intersection is an infinite
        ///     line, or non-coincident and parallel, in which
        ///     case it is empty.
        ///     https://en.wikipedia.org/wiki/Line%E2%80%93line_intersection
        /// </summary>
        public static XYZ LineLineIntersection(
            XYZ p1, XYZ v1, XYZ p2, XYZ v2)
        {
            var w = p2 - p1;
            XYZ p5 = null;

            var c = (v2.X * w.Y - v2.Y * w.X)
                    / (v2.X * v1.Y - v2.Y * v1.X);

            if (!double.IsInfinity(c))
            {
                var x = p1.X + c * v1.X;
                var y = p1.Y + c * v1.Y;

                p5 = new XYZ(x, y, 0);
            }

            return p5;
        }

        /// <summary>
        ///     Return the 2D intersection point between two
        ///     unbounded lines defined in the XY plane by the
        ///     start and end points of the two given curves.
        ///     Return null if the two lines are coincident,
        ///     in which case the intersection is an infinite
        ///     line, or non-coincident and parallel, in which
        ///     case it is empty.
        ///     https://en.wikipedia.org/wiki/Line%E2%80%93line_intersection
        /// </summary>
        public static XYZ LineLineIntersection(
            Curve c1,
            Curve c2)
        {
            var p1 = c1.GetEndPoint(0);
            var q1 = c1.GetEndPoint(1);
            var p2 = c2.GetEndPoint(0);
            var q2 = c2.GetEndPoint(1);
            var v1 = q1 - p1;
            var v2 = q2 - p2;
            return LineLineIntersection(p1, v1, p2, v2);
        }

        /// <summary>
        ///     Return the 3D intersection point between
        ///     a line and a plane.
        ///     https://forums.autodesk.com/t5/revit-api-forum/how-can-we-calculate-the-intersection-between-the-plane-and-the/m-p/9785834
        ///     https://stackoverflow.com/questions/5666222/3d-line-plane-intersection
        ///     Determine the point of intersection between
        ///     a plane defined by a point and a normal vector
        ///     and a line defined by a point and a direction vector.
        ///     planePoint - A point on the plane.
        ///     planeNormal - The normal vector of the plane.
        ///     linePoint - A point on the line.
        ///     lineDirection - The direction vector of the line.
        ///     lineParameter - The intersection distance along the line.
        ///     Return - The point of intersection between the
        ///     line and the plane, null if the line is parallel
        ///     to the plane.
        ///     If enforceResultOnLine is true, return null if
        ///     the intersection point is not within the line itself;
        ///     i.e., suppress results on the extension of the line.
        /// </summary>
        public static XYZ LinePlaneIntersection(
            Line line,
            Plane plane,
            bool enforceResultOnLine,
            out double lineParameter)
        {
            var planePoint = plane.Origin;
            var planeNormal = plane.Normal;
            var linePoint = line.GetEndPoint(0);

            var lineDirection = (line.GetEndPoint(1)
                                 - linePoint).Normalize();

            // Is the line parallel to the plane, i.e.,
            // perpendicular to the plane normal?

            if (IsZero(planeNormal.DotProduct(lineDirection)))
            {
                lineParameter = double.NaN;
                return null;
            }

            lineParameter = (planeNormal.DotProduct(planePoint)
                             - planeNormal.DotProduct(linePoint))
                            / planeNormal.DotProduct(lineDirection);

            // Test whether the line parameter is inside 
            // the line using the "isInside" method.

            if (enforceResultOnLine
                && !line.IsInside(lineParameter))
            {
                lineParameter = double.NaN;
                return null;
            }

            return linePoint + lineParameter * lineDirection;
        }

        /// <summary>
        ///     Create transformation matrix to transform points
        ///     from the global space (XYZ) to the local space of
        ///     a face (UV representation of a bounding box).
        ///     Revit itself only supports Face.Transform(UV) that
        ///     translates a UV coordinate into XYZ coordinate space.
        ///     I reversed that Method to translate XYZ coords to
        ///     UV coords. At first i thought i could solve the
        ///     reverse transformation by solving a linear equation
        ///     with 2 unknown variables. But this wasn't general.
        ///     I finally found out that the transformation
        ///     consists of a displacement vector and a rotation matrix.
        /// </summary>
        public static double[,]
            CalculateMatrixForGlobalToLocalCoordinateSystem(
                Face face)
        {
            // face.Evaluate uses a rotation matrix and
            // a displacement vector to translate points

            var originDisplacementVectorUV = face.Evaluate(UV.Zero);
            var unitVectorUWithDisplacement = face.Evaluate(UV.BasisU);
            var unitVectorVWithDisplacement = face.Evaluate(UV.BasisV);

            var unitVectorU = unitVectorUWithDisplacement
                              - originDisplacementVectorUV;

            var unitVectorV = unitVectorVWithDisplacement
                              - originDisplacementVectorUV;

            // The rotation matrix A is composed of
            // unitVectorU and unitVectorV transposed.
            // To get the rotation matrix that translates from 
            // global space to local space, take the inverse of A.

            var a11i = unitVectorU.X;
            var a12i = unitVectorU.Y;
            var a21i = unitVectorV.X;
            var a22i = unitVectorV.Y;

            return new double[2, 2]
            {
                {a11i, a12i},
                {a21i, a22i}
            };
        }

        /// <summary>
        ///     Create an arc in the XY plane from a given
        ///     start point, end point and radius.
        /// </summary>
        public static Arc CreateArc2dFromRadiusStartAndEndPoint(
            XYZ ps,
            XYZ pe,
            double radius,
            bool largeSagitta = false,
            bool clockwise = false)
        {
            // https://forums.autodesk.com/t5/revit-api-forum/create-a-curve-when-only-the-start-point-end-point-amp-radius-is/m-p/7830079

            var midPointChord = 0.5 * (ps + pe);
            var v = pe - ps;
            var d = 0.5 * v.GetLength(); // half chord length

            // Small and large circle sagitta:
            // http://www.mathopenref.com/sagitta.html
            // https://en.wikipedia.org/wiki/Sagitta_(geometry)

            var s = largeSagitta
                ? radius + Math.Sqrt(radius * radius - d * d) // sagitta large
                : radius - Math.Sqrt(radius * radius - d * d); // sagitta small

            var midPointOffset = Transform
                .CreateRotation(XYZ.BasisZ, 0.5 * Math.PI)
                .OfVector(v.Normalize().Multiply(s));

            var midPointArc = clockwise
                ? midPointChord + midPointOffset
                : midPointChord - midPointOffset;

            return Arc.Create(ps, pe, midPointArc);
        }

        /// <summary>
        ///     Create a new CurveLoop from a list of points.
        /// </summary>
        public static CurveLoop CreateCurveLoop(
            List<XYZ> pts)
        {
            var n = pts.Count;
            var curveLoop = new CurveLoop();
            for (var i = 1; i < n; ++i)
                curveLoop.Append(Line.CreateBound(
                    pts[i - 1], pts[i]));
            curveLoop.Append(Line.CreateBound(
                pts[n - 1], pts[0]));
            return curveLoop;
        }

        /// <summary>
        ///     Offset a list of points by a distance in a
        ///     given direction in or out of the curve loop.
        /// </summary>
        public static IEnumerable<XYZ> OffsetPoints(
            List<XYZ> pts,
            double offset,
            XYZ normal)
        {
            var curveLoop = CreateCurveLoop(pts);

            var curveLoop2 = CurveLoop.CreateViaOffset(
                curveLoop, offset, normal);

            return curveLoop2.Select(
                c => c.GetEndPoint(0));
        }

        #endregion
    }
}

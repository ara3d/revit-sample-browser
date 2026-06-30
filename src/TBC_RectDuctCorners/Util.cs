#region Namespaces

using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Mechanical;
using System;
using System.Collections.Generic;
using System.Diagnostics;

#endregion // Namespaces

namespace BuildingCoder
{
    internal static partial class Util
    {
        public static XYZ Test1(Connector connector)
        {
            return connector.CoordinateSystem.OfPoint(
                new XYZ(connector.Width / 2,
                    connector.Height / 2, 0));
        }

        public static XYZ Test2(Connector connector)
        {
            return connector.CoordinateSystem.OfPoint(
                new XYZ(connector.Height / 2,
                    connector.Width / 2, 0));
        }

        public static bool GetFirstRectangularConnector(
            Duct duct,
            out Connector c1)
        {
            c1 = null;

            var connectors
                = duct.ConnectorManager.Connectors;

            if (0 < connectors.Size)
                foreach (Connector c in connectors)
                    if (ConnectorProfileType.Rectangular
                        == c.Shape)
                    {
                        c1 = c;
                        break;
                    }
                    else
                    {
                        Trace.WriteLine($"Connector shape: {c.Shape}");
                    }

            return null != c1;
        }

        public static bool FaceContainsConnector(
            Face face,
            Connector c)
        {
            var p = c.Origin;

            var result = face.Project(p);

            return null != result
                   && Math.Abs(result.Distance) < 1e-9;
        }

        public static bool AnalyseDuct(Duct duct)
        {
            var rc = false;

            if (!GetFirstRectangularConnector(duct, out var c1))
            {
                Trace.TraceError("The duct is not rectangular!");
            }
            else
            {
                var opt = new Options
                {
                    DetailLevel = ViewDetailLevel.Fine
                };
                var geoElement = duct.get_Geometry(opt);

                foreach (var obj in geoElement)
                {
                    var solid = obj as Solid;
                    if (solid != null)
                    {
                        var foundFace = false;
                        foreach (Face face in solid.Faces)
                        {
                            foundFace = FaceContainsConnector(face, c1);
                            if (foundFace)
                            {
                                Trace.WriteLine("==> Four face corners:");

                                var a = face.EdgeLoops.get_Item(0);

                                foreach (Edge e in a)
                                {
                                    var p = e.Evaluate(0.0);

                                    Trace.WriteLine($"Point = {PointString(p)}");
                                }

                                rc = true;
                                break;
                            }
                        }

                        if (!foundFace) Trace.WriteLine("[Error] Face not found");
                    }
                }
            }

            return rc;
        }

        public static List<Connector> GetElbowConnectors(Element e)
        {
            List<Connector> cons = null;
            if (e is FamilyInstance fi)
            {
                var m = fi.MEPModel;
                if (null != m)
                {
                    var cm = m.ConnectorManager;
                    if (null != cm)
                    {
                        var cs = cm.Connectors;
                        if (2 == cs.Size)
                        {
                            cons = new List<Connector>(2);
                            var first = true;
                            foreach (Connector c in cs)
                                if (first)
                                    cons[0] = c;
                                else
                                    cons[1] = c;
                        }
                    }
                }
            }

            return cons;
        }

        // Returns null when connector geometry is not coplanar.
        public static XYZ GetElbowCentre(Element e)
        {
            XYZ pc = null;
            var cons = GetElbowConnectors(e);
            if (null != cons)
            {
                var ps = cons[0].CoordinateSystem.Origin;
                var vs = cons[0].CoordinateSystem.BasisZ;

                var pe = cons[1].CoordinateSystem.Origin;
                var ve = cons[1].CoordinateSystem.BasisZ;

                var vd = pe - ps;

                var vz = vs.CrossProduct(vd);

                if (!vz.IsZeroLength())
                {
                    var vxs = vs.CrossProduct(vz);
                    var vxe = ve.CrossProduct(vz);
                    pc = LineLineIntersection(
                        ps, vxs, pe, vxe);
                }
            }

            return pc;
        }
    }
}

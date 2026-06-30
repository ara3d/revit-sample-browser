// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Mechanical;
using Autodesk.Revit.UI;
using GXYZ = Autodesk.Revit.DB.XYZ;

namespace Ara3D.RevitSampleBrowser.AutoRoute.CS
{
    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    [Journaling(JournalingMode.NoCommandData)]
    public class Command : IExternalCommand
    {
        private const double Min1FittingLength = 1;

        private const double MinDuctLength = 0.5;

        private const double VerticalTrunkOffset = 15;

        private const double HorizontalOptionalTrunkOffset = 5;

        private const double Min2FittingsLength = Min1FittingLength * 2;

        private const double Min1Duct2FittingsLength = Min1FittingLength * 2 + MinDuctLength;

        private static Application _application;

        private static Document _document;

        private DuctType m_dtRectangle;

        private readonly ElementId m_ductTypeId = new ElementId(139191L);

        private Level m_lvl;

        private MechanicalSystem m_mechanicalSystem;

        private ElementId m_systemTypeId = ElementId.InvalidElementId;

        public Result Execute(ExternalCommandData commandData, ref string message,
            ElementSet elements)
        {
            // set out default result to Failed.
            var retRes = Result.Failed;

            _application = commandData.Application.Application;
            _document = commandData.Application.ActiveUIDocument.Document;
            Trace.Listeners.Clear();
            Trace.AutoFlush = true;

            foreach (var type in _document.GetElements<MEPSystemType>())
            {
                if (type.SystemClassification == MEPSystemClassification.SupplyAir)
                {
                    m_systemTypeId = type.Id;
                    break;
                }
            }

            var outputFileName = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location),
                "AutoRoute.log");
            if (File.Exists(outputFileName)) File.Delete(outputFileName);
            var listener = new TextWriterTraceListener(outputFileName);
            Trace.Listeners.Add(listener);

            var transaction = new Transaction(_document, "Sample_AutoRoute");
            try
            {
                transaction.Start();

                //set the level of the duct            
                m_lvl = Level.Create(_document, 0.0);

                //Lists to temporarily record the created elements
                var ducts = new List<Duct>();
                var points = new List<GXYZ>();
                var connectors = new List<Connector>();
                var baseConnectors = new List<Connector>();

                var ids = new List<ElementId>
                {
                    new ElementId(378728L),
                    new ElementId(378707L),
                    new ElementId(378716L)
                };

                var instances = new FamilyInstance[3];
                var boxes = new BoundingBoxXYZ[3];
                var conns = new Connector[3];
                ConnectorSetIterator csi = null;
                for (var i = 0; i < ids.Count; ++i)
                {
                    var element = _document.GetElement(ids[i]);
                    if (null == element)
                    {
                        message = $"Element {ids[i]} can't be found.";
                        return Result.Failed;
                    }

                    instances[i] = element as FamilyInstance;
                    csi = ConnectorInfo.GetConnectors(ids[i]).ForwardIterator();
                    csi.MoveNext();
                    conns[i] = csi.Current as Connector;
                    boxes[i] = instances[i].get_BoundingBox(_document.ActiveView);
                }

                csi = ConnectorInfo.GetConnectors(ids[0]).ForwardIterator();
                while (csi.MoveNext())
                {
                    var conn = csi.Current as Connector;
                    if (conn.Direction == FlowDirectionType.Out && conn.DuctSystemType == DuctSystemType.SupplyAir)
                        conns[0] = conn;
                }

                m_mechanicalSystem = CreateMechanicalSystem(
                    //[378728][SupplyAir][Out][RectProfile][OST_MechanicalEquipment]
                    new ConnectorInfo(new ElementId(378728L), conns[0].Origin.X, conns[0].Origin.Y, conns[0].Origin.Z),
                    new[]
                    {
                        //[378707][SupplyAir][In][RectProfile]
                        new ConnectorInfo(new ElementId(378707L), conns[1].Origin.X, conns[1].Origin.Y,
                            conns[1].Origin.Z),
                        //[378716][SupplyAir][In][RectProfile]
                        new ConnectorInfo(new ElementId(378716L), conns[2].Origin.X, conns[2].Origin.Y,
                            conns[2].Origin.Z)
                    },
                    DuctSystemType.SupplyAir
                );

                var minX = conns[0].Origin.X;
                var minY = conns[0].Origin.Y;
                var maxX = conns[0].Origin.X;
                var maxY = conns[0].Origin.Y;
                var maxZ = conns[0].Origin.Z;
                for (var i = 1; i < boxes.Length; ++i)
                {
                    if (conns[i].Origin.X < minX)
                        minX = conns[i].Origin.X;
                    if (conns[i].Origin.Y < minY)
                        minY = conns[i].Origin.Y;
                    if (conns[i].Origin.X > maxX)
                        maxX = conns[i].Origin.X;
                    if (conns[i].Origin.Y > maxY)
                        maxY = conns[i].Origin.Y;
                    if (conns[i].Origin.Z > maxZ)
                        maxZ = conns[i].Origin.Z;
                }

                var midX = (minX + maxX) / 2;
                var midY = (minY + maxY) / 2;
                var baseXValues = new double[3] { midX, (minX + midX) / 2, (maxX + midX) / 2 };
                var baseYValues = new double[3] { midY, (minY + midY) / 2, (maxY + midY) / 2 };

                m_dtRectangle = _document.GetElement(m_ductTypeId) as DuctType;

                var connectorDirection = conns[0].CoordinateSystem.BasisZ;

                if (0 == connectorDirection.DistanceTo(new GXYZ(-1, 0, 0)))
                {
                    points.Add(new GXYZ(conns[0].Origin.X - Min1FittingLength, conns[0].Origin.Y, conns[0].Origin.Z));
                    points.Add(new GXYZ(conns[0].Origin.X - Min2FittingsLength, conns[0].Origin.Y,
                        conns[0].Origin.Z + Min1FittingLength));
                    points.Add(new GXYZ(conns[0].Origin.X - Min2FittingsLength, conns[0].Origin.Y,
                        maxZ + VerticalTrunkOffset - Min1FittingLength));
                }
                else if (0 == connectorDirection.DistanceTo(new GXYZ(1, 0, 0)))
                {
                    points.Add(new GXYZ(conns[0].Origin.X + Min1FittingLength, conns[0].Origin.Y, conns[0].Origin.Z));
                    points.Add(new GXYZ(conns[0].Origin.X + Min2FittingsLength, conns[0].Origin.Y,
                        conns[0].Origin.Z + Min1FittingLength));
                    points.Add(new GXYZ(conns[0].Origin.X + Min2FittingsLength, conns[0].Origin.Y,
                        maxZ + VerticalTrunkOffset - Min1FittingLength));
                }
                else if (0 == connectorDirection.DistanceTo(new GXYZ(0, -1, 0)))
                {
                    points.Add(new GXYZ(conns[0].Origin.X, conns[0].Origin.Y - Min1FittingLength, conns[0].Origin.Z));
                    points.Add(new GXYZ(conns[0].Origin.X, conns[0].Origin.Y - Min2FittingsLength,
                        conns[0].Origin.Z + Min1FittingLength));
                    points.Add(new GXYZ(conns[0].Origin.X, conns[0].Origin.Y - Min2FittingsLength,
                        maxZ + VerticalTrunkOffset - Min1FittingLength));
                }
                else if (0 == connectorDirection.DistanceTo(new GXYZ(0, 1, 0)))
                {
                    points.Add(new GXYZ(conns[0].Origin.X, conns[0].Origin.Y + Min1FittingLength, conns[0].Origin.Z));
                    points.Add(new GXYZ(conns[0].Origin.X, conns[0].Origin.Y + Min2FittingsLength,
                        conns[0].Origin.Z + Min1FittingLength));
                    points.Add(new GXYZ(conns[0].Origin.X, conns[0].Origin.Y + Min2FittingsLength,
                        maxZ + VerticalTrunkOffset - Min1FittingLength));
                }

                ducts.Add(Duct.Create(_document, m_ductTypeId, m_lvl.Id, conns[0], points[0]));

                ducts.Add(Duct.Create(_document, m_systemTypeId, m_ductTypeId, m_lvl.Id, points[1], points[2]));
                connectors.Add(ConnectorInfo.GetConnector(ducts[0].Id, points[0]));
                connectors.Add(ConnectorInfo.GetConnector(ducts[1].Id, points[1]));
                connectors.Add(ConnectorInfo.GetConnector(ducts[1].Id, points[2]));
                connectors[0].ConnectTo(connectors[1]);
                _document.Create.NewElbowFitting(connectors[0], connectors[1]);
                baseConnectors.Add(connectors[2]);

                points.Clear();
                ducts.Clear();

                points.Add(new GXYZ(conns[1].Origin.X, conns[1].Origin.Y,
                    maxZ + VerticalTrunkOffset - Min1FittingLength));
                points.Add(new GXYZ(conns[2].Origin.X, conns[2].Origin.Y,
                    maxZ + VerticalTrunkOffset - Min1FittingLength));
                ducts.Add(Duct.Create(_document, m_ductTypeId, m_lvl.Id, conns[1], points[0]));
                ducts.Add(Duct.Create(_document, m_ductTypeId, m_lvl.Id, conns[2], points[1]));
                baseConnectors.Add(ConnectorInfo.GetConnector(ducts[0].Id, points[0]));
                baseConnectors.Add(ConnectorInfo.GetConnector(ducts[1].Id, points[1]));

                SortConnectorsByX(baseConnectors);
                foreach (var baseY in baseYValues)
                {
                    if (ConnectSystemOnXAxis(baseConnectors, baseY))
                    {
                        LogUtility.WriteMechanicalSystem(m_mechanicalSystem);
                        return Result.Succeeded;
                    }
                }

                SortConnectorsByY(baseConnectors);
                foreach (var baseX in baseXValues)
                {
                    if (ConnectSystemOnYAxis(baseConnectors, baseX))
                    {
                        LogUtility.WriteMechanicalSystem(m_mechanicalSystem);
                        return Result.Succeeded;
                    }
                }

                //If all the cases fail to route the system, try the trunks out of the bounding box
                SortConnectorsByX(baseConnectors);
                if (ConnectSystemOnXAxis(baseConnectors, maxY + HorizontalOptionalTrunkOffset))
                {
                    LogUtility.WriteMechanicalSystem(m_mechanicalSystem);
                    return Result.Succeeded;
                }

                SortConnectorsByY(baseConnectors);
                if (ConnectSystemOnYAxis(baseConnectors, maxX + HorizontalOptionalTrunkOffset))
                {
                    LogUtility.WriteMechanicalSystem(m_mechanicalSystem);
                    return Result.Succeeded;
                }

                //If there's no path for the connection, choose one path and let Revit report the error
                connectors.Clear();
                SortConnectorsByX(baseConnectors);
                connectors.AddRange(CreateDuct(
                    new GXYZ(baseConnectors[0].Origin.X + Min1FittingLength, baseYValues[0],
                        maxZ + VerticalTrunkOffset),
                    new GXYZ(baseConnectors[1].Origin.X - Min1FittingLength, baseYValues[0],
                        maxZ + VerticalTrunkOffset)));
                connectors.AddRange(CreateDuct(
                    new GXYZ(baseConnectors[1].Origin.X + Min1FittingLength, baseYValues[0],
                        maxZ + VerticalTrunkOffset),
                    new GXYZ(baseConnectors[2].Origin.X - Min1FittingLength, baseYValues[0],
                        maxZ + VerticalTrunkOffset)));
                ConnectWithElbowFittingOnXAxis(baseConnectors[0], connectors[0]);
                ConnectWithElbowFittingOnXAxis(baseConnectors[2], connectors[3]);
                ConnectWithTeeFittingOnXAxis(baseConnectors[1], connectors[1], connectors[2], false);
            }
            catch (Exception ex)
            {
                Trace.WriteLine(ex.ToString());
                message = ex.Message;
                retRes = Result.Failed;
            }
            finally
            {
                transaction.Commit();
                Trace.Flush();
                listener.Close();
                Trace.Close();
                Trace.Listeners.Remove(listener);
            }

            return retRes;
        }

        private bool ConnectSystemOnXAxis(List<Connector> baseConnectors, double baseY)
        {
            if (null == baseConnectors || 3 != baseConnectors.Count) return false;
            for (var i = 0; i < baseConnectors.Count; ++i)
            {
                if (baseConnectors[i].Origin.Y != baseY &&
                    Math.Abs(baseConnectors[i].Origin.Y - baseY) < Min1Duct2FittingsLength) return false;
                for (var j = i + 1; j < baseConnectors.Count; ++j)
                    if (baseConnectors[j].Origin.X != baseConnectors[i].Origin.X &&
                        baseConnectors[j].Origin.X - baseConnectors[i].Origin.X < Min2FittingsLength)
                        return false;
            }

            try
            {
                var baseZ = baseConnectors[0].Origin.Z + Min1FittingLength;
                var connectors = new List<Connector>();

                if (baseConnectors[0].Origin.X == baseConnectors[1].Origin.X)
                {
                    //All 3 connectors are with the same X value
                    if (baseConnectors[1].Origin.X == baseConnectors[2].Origin.X)
                    {
                        return false;
                    }

                    //The 1st and 2nd base connectors are on the same side of the trunk
                    if (Math.Sign(baseConnectors[0].Origin.Y - baseY) * Math.Sign(baseConnectors[1].Origin.Y - baseY) ==
                        1) return false;

                    connectors = CreateDuct(new GXYZ(baseConnectors[0].Origin.X + Min1FittingLength, baseY, baseZ),
                        new GXYZ(baseConnectors[2].Origin.X - Min1FittingLength, baseY, baseZ));

                    ConnectWithTeeFittingOnXAxis(baseConnectors[0], baseConnectors[1], connectors[0], true);

                    ConnectWithElbowFittingOnXAxis(baseConnectors[2], connectors[1]);
                }
                else
                {
                    connectors = CreateDuct(new GXYZ(baseConnectors[0].Origin.X + Min1FittingLength, baseY, baseZ),
                        new GXYZ(baseConnectors[1].Origin.X - Min1FittingLength, baseY, baseZ));

                    ConnectWithElbowFittingOnXAxis(baseConnectors[0], connectors[0]);

                    if (baseConnectors[1].Origin.X == baseConnectors[2].Origin.X)
                    {
                        //The 2nd and 3rd connectors are on the same side of the trunk
                        if (Math.Sign(baseConnectors[1].Origin.Y - baseY) *
                            Math.Sign(baseConnectors[2].Origin.Y - baseY) == 1) return false;
                        ConnectWithTeeFittingOnXAxis(baseConnectors[1], baseConnectors[2], connectors[1], true);
                    }
                    else
                    {
                        connectors.AddRange(CreateDuct(
                            new GXYZ(baseConnectors[1].Origin.X + Min1FittingLength, baseY, baseZ),
                            new GXYZ(baseConnectors[2].Origin.X - Min1FittingLength, baseY, baseZ)));
                        ConnectWithTeeFittingOnXAxis(baseConnectors[1], connectors[1], connectors[2], false);
                        ConnectWithElbowFittingOnXAxis(baseConnectors[2], connectors[3]);
                    }
                }

                return true;
            }
            catch
            {
                return false;
            }
        }

        private void ConnectWithElbowFittingOnXAxis(Connector baseConn, Connector conn)
        {
            var baseY = conn.Origin.Y;
            var baseZ = conn.Origin.Z;
            var connectors = new List<Connector>();

            //If the distance of the two connectors on the Y axis is greater than 2, create a duct on the Y axis and then connect it to the 2 connectors with elbow fittings
            if (Math.Abs(baseConn.Origin.Y - baseY) > Min1Duct2FittingsLength)
            {
                connectors.AddRange(CreateDuct(
                    new GXYZ(baseConn.Origin.X, baseConn.Origin.Y - Math.Sign(baseConn.Origin.Y - baseY), baseZ),
                    new GXYZ(baseConn.Origin.X, baseY + Math.Sign(baseConn.Origin.Y - baseY), baseZ)));
                connectors[0].ConnectTo(baseConn);
                _document.Create.NewElbowFitting(connectors[0], baseConn);
                connectors[1].ConnectTo(conn);
                _document.Create.NewElbowFitting(connectors[1], conn);
            }
            //If the distance of the two connectors on the Y axis is less than 2, connect them with an elbow fitting
            else
            {
                baseConn.ConnectTo(conn);
                _document.Create.NewElbowFitting(baseConn, conn);
            }
        }

        private void ConnectWithTeeFittingOnXAxis(Connector conn1, Connector conn2, Connector conn3, bool flag)
        {
            var baseY = conn3.Origin.Y;
            var baseZ = conn3.Origin.Z;

            new List<GXYZ>();
            new List<Duct>();
            var connectors = new List<Connector>();

            //Connect two base connectors to a connector on the trunk
            if (flag)
            {
                var baseConn1 = conn1;
                var baseConn2 = conn2;
                var conn = conn3;

                connectors.AddRange(CreateDuct(
                    new GXYZ(baseConn1.Origin.X, baseConn1.Origin.Y - Math.Sign(baseConn1.Origin.Y - baseY), baseZ),
                    new GXYZ(baseConn1.Origin.X, baseY + Math.Sign(baseConn1.Origin.Y - baseY), baseZ)));
                connectors.AddRange(CreateDuct(
                    new GXYZ(baseConn2.Origin.X, baseConn2.Origin.Y - Math.Sign(baseConn2.Origin.Y - baseY), baseZ),
                    new GXYZ(baseConn2.Origin.X, baseY + Math.Sign(baseConn2.Origin.Y - baseY), baseZ)));

                connectors[0].ConnectTo(baseConn1);
                connectors[2].ConnectTo(baseConn2);
                _document.Create.NewElbowFitting(connectors[0], baseConn1);
                _document.Create.NewElbowFitting(connectors[2], baseConn2);

                connectors[1].ConnectTo(connectors[3]);
                connectors[1].ConnectTo(conn);
                connectors[3].ConnectTo(conn);
                _document.Create.NewTeeFitting(connectors[1], connectors[3], conn);
            }
            //Connect a base connector to two connectors on the trunk
            else
            {
                var baseConn = conn1;

                if (Math.Abs(baseConn.Origin.Y - baseY) > Min1Duct2FittingsLength)
                {
                    connectors.AddRange(CreateDuct(
                        new GXYZ(baseConn.Origin.X, baseConn.Origin.Y - Math.Sign(baseConn.Origin.Y - baseY), baseZ),
                        new GXYZ(baseConn.Origin.X, baseY + Math.Sign(baseConn.Origin.Y - baseY), baseZ)));
                    baseConn.ConnectTo(connectors[0]);
                    _document.Create.NewElbowFitting(connectors[0], baseConn);

                    connectors[1].ConnectTo(conn2);
                    connectors[1].ConnectTo(conn3);
                    conn2.ConnectTo(conn3);
                    _document.Create.NewTeeFitting(conn2, conn3, connectors[1]);
                }
                else
                {
                    baseConn.ConnectTo(conn2);
                    baseConn.ConnectTo(conn3);
                    conn2.ConnectTo(conn3);
                    _document.Create.NewTeeFitting(conn2, conn3, baseConn);
                }
            }
        }

        private void SortConnectorsByX(List<Connector> connectors)
        {
            for (var i = 0; i < connectors.Count; ++i)
            {
                var min = connectors[i].Origin.X;
                var minIndex = i;
                for (var j = i; j < connectors.Count; ++j)
                    if (connectors[j].Origin.X < min)
                    {
                        min = connectors[j].Origin.X;
                        minIndex = j;
                    }

                var t = connectors[i];
                connectors[i] = connectors[minIndex];
                connectors[minIndex] = t;
            }
        }

        private bool ConnectSystemOnYAxis(List<Connector> baseConnectors, double baseX)
        {
            if (null == baseConnectors || 3 != baseConnectors.Count) return false;
            for (var i = 0; i < baseConnectors.Count; ++i)
            {
                if (baseConnectors[i].Origin.X != baseX &&
                    Math.Abs(baseConnectors[i].Origin.X - baseX) < Min1Duct2FittingsLength) return false;
                for (var j = i + 1; j < baseConnectors.Count; ++j)
                    if (baseConnectors[j].Origin.Y != baseConnectors[i].Origin.Y &&
                        baseConnectors[j].Origin.Y - baseConnectors[i].Origin.Y < Min2FittingsLength)
                        return false;
            }

            try
            {
                var baseZ = baseConnectors[0].Origin.Z + Min1FittingLength;
                var connectors = new List<Connector>();

                if (baseConnectors[0].Origin.Y == baseConnectors[1].Origin.Y)
                {
                    //All 3 connectors are with the same Y value
                    if (baseConnectors[1].Origin.Y == baseConnectors[2].Origin.Y)
                    {
                        return false;
                    }

                    //The 1st and 2nd base connectors are on the same side of the trunk
                    if (Math.Sign(baseConnectors[0].Origin.X - baseX) * Math.Sign(baseConnectors[1].Origin.X - baseX) ==
                        1) return false;

                    connectors = CreateDuct(new GXYZ(baseX, baseConnectors[0].Origin.Y + Min1FittingLength, baseZ),
                        new GXYZ(baseX, baseConnectors[2].Origin.Y - Min1FittingLength, baseZ));

                    ConnectWithTeeFittingOnYAxis(baseConnectors[0], baseConnectors[1], connectors[0], true);

                    ConnectWithElbowFittingOnYAxis(baseConnectors[2], connectors[1]);
                }
                else
                {
                    connectors = CreateDuct(new GXYZ(baseX, baseConnectors[0].Origin.Y + Min1FittingLength, baseZ),
                        new GXYZ(baseX, baseConnectors[1].Origin.Y - Min1FittingLength, baseZ));

                    ConnectWithElbowFittingOnYAxis(baseConnectors[0], connectors[0]);

                    if (baseConnectors[1].Origin.Y == baseConnectors[2].Origin.Y)
                    {
                        //The 2nd and 3rd connectors are on the same side of the trunk
                        if (Math.Sign(baseConnectors[1].Origin.X - baseX) *
                            Math.Sign(baseConnectors[2].Origin.X - baseX) == 1) return false;
                        ConnectWithTeeFittingOnYAxis(baseConnectors[1], baseConnectors[2], connectors[1], true);
                    }
                    else
                    {
                        connectors.AddRange(CreateDuct(
                            new GXYZ(baseX, baseConnectors[1].Origin.Y + Min1FittingLength, baseZ),
                            new GXYZ(baseX, baseConnectors[2].Origin.Y - Min1FittingLength, baseZ)));
                        ConnectWithTeeFittingOnYAxis(baseConnectors[1], connectors[1], connectors[2], false);
                        ConnectWithElbowFittingOnYAxis(baseConnectors[2], connectors[3]);
                    }
                }

                return true;
            }
            catch
            {
                return false;
            }
        }

        private void ConnectWithElbowFittingOnYAxis(Connector baseConn, Connector conn)
        {
            var baseX = conn.Origin.X;
            var baseZ = conn.Origin.Z;
            var connectors = new List<Connector>();

            //If the distance of the two connectors on the X axis is greater than 2, create a duct on the X axis and then connect it to the 2 connectors with elbow fittings
            if (Math.Abs(baseConn.Origin.X - baseX) > Min1Duct2FittingsLength)
            {
                connectors.AddRange(CreateDuct(
                    new GXYZ(baseConn.Origin.X - Math.Sign(baseConn.Origin.X - baseX), baseConn.Origin.Y, baseZ),
                    new GXYZ(baseX + Math.Sign(baseConn.Origin.X - baseX), baseConn.Origin.Y, baseZ)));
                connectors[0].ConnectTo(baseConn);
                _document.Create.NewElbowFitting(connectors[0], baseConn);
                connectors[1].ConnectTo(conn);
                _document.Create.NewElbowFitting(connectors[1], conn);
            }
            //If the distance of the two connectors on the X axis is less than 2, connect them with an elbow fitting
            else
            {
                baseConn.ConnectTo(conn);
                _document.Create.NewElbowFitting(baseConn, conn);
            }
        }

        private void ConnectWithTeeFittingOnYAxis(Connector conn1, Connector conn2, Connector conn3, bool flag)
        {
            var baseX = conn3.Origin.X;
            var baseZ = conn3.Origin.Z;

            new List<GXYZ>();
            new List<Duct>();
            var connectors = new List<Connector>();

            //Connect two base connectors to a connector on the trunk
            if (flag)
            {
                var baseConn1 = conn1;
                var baseConn2 = conn2;
                var conn = conn3;

                connectors.AddRange(CreateDuct(
                    new GXYZ(baseConn1.Origin.X - Math.Sign(baseConn1.Origin.X - baseX), baseConn1.Origin.Y, baseZ),
                    new GXYZ(baseX + Math.Sign(baseConn1.Origin.X - baseX), baseConn1.Origin.Y, baseZ)));
                connectors.AddRange(CreateDuct(
                    new GXYZ(baseConn2.Origin.X - Math.Sign(baseConn2.Origin.X - baseX), baseConn2.Origin.Y, baseZ),
                    new GXYZ(baseX + Math.Sign(baseConn2.Origin.X - baseX), baseConn2.Origin.Y, baseZ)));

                connectors[0].ConnectTo(baseConn1);
                connectors[2].ConnectTo(baseConn2);
                _document.Create.NewElbowFitting(connectors[0], baseConn1);
                _document.Create.NewElbowFitting(connectors[2], baseConn2);

                connectors[1].ConnectTo(connectors[3]);
                connectors[1].ConnectTo(conn);
                connectors[3].ConnectTo(conn);
                _document.Create.NewTeeFitting(connectors[1], connectors[3], conn);
            }
            //Connect a base connector to two connectors on the trunk
            else
            {
                var baseConn = conn1;

                if (Math.Abs(baseConn.Origin.X - baseX) > Min1Duct2FittingsLength)
                {
                    connectors.AddRange(CreateDuct(
                        new GXYZ(baseConn.Origin.X - Math.Sign(baseConn.Origin.X - baseX), baseConn.Origin.Y, baseZ),
                        new GXYZ(baseX + Math.Sign(baseConn.Origin.X - baseX), baseConn.Origin.Y, baseZ)));
                    baseConn.ConnectTo(connectors[0]);
                    _document.Create.NewElbowFitting(connectors[0], baseConn);

                    connectors[1].ConnectTo(conn2);
                    connectors[1].ConnectTo(conn3);
                    conn2.ConnectTo(conn3);
                    _document.Create.NewTeeFitting(conn2, conn3, connectors[1]);
                }
                else
                {
                    baseConn.ConnectTo(conn2);
                    baseConn.ConnectTo(conn3);
                    conn2.ConnectTo(conn3);
                    _document.Create.NewTeeFitting(conn2, conn3, baseConn);
                }
            }
        }

        private void SortConnectorsByY(List<Connector> connectors)
        {
            for (var i = 0; i < connectors.Count; ++i)
            {
                var min = connectors[i].Origin.Y;
                var minIndex = i;
                for (var j = i; j < connectors.Count; ++j)
                    if (connectors[j].Origin.Y < min)
                    {
                        min = connectors[j].Origin.Y;
                        minIndex = j;
                    }

                var t = connectors[i];
                connectors[i] = connectors[minIndex];
                connectors[minIndex] = t;
            }
        }

        private List<Connector> CreateDuct(GXYZ point1, GXYZ point2)
        {
            var connectors = new List<Connector>();

            var duct = Duct.Create(_document, m_systemTypeId, m_ductTypeId, m_lvl.Id, point1, point2);

            connectors.Add(ConnectorInfo.GetConnector(duct.Id, point1));
            connectors.Add(ConnectorInfo.GetConnector(duct.Id, point2));

            return connectors;
        }

        private MechanicalSystem CreateMechanicalSystem(ConnectorInfo baseConnector, ConnectorInfo[] connectors,
            DuctSystemType systemType)
        {
            ConnectorSet cset = null;
            if (connectors != null)
            {
                cset = new ConnectorSet();
                foreach (var ci in connectors)
                {
                    cset.Insert(ci.Connector);
                }
            }

            var mechanicalSystem =
                _document.Create.NewMechanicalSystem(baseConnector == null ? null : baseConnector.Connector, cset,
                    systemType);
            return mechanicalSystem;
        }

        public class ConnectorInfo
        {
            private Connector m_connector;

            private GXYZ m_origin;

            private ElementId m_ownerId;

            public ConnectorInfo(ElementId ownerId, GXYZ origin)
            {
                m_ownerId = ownerId;
                m_origin = origin;
                m_connector = GetConnector(m_ownerId, origin);
            }

            public ConnectorInfo(ElementId ownerId, double x, double y, double z)
                : this(ownerId, new GXYZ(x, y, z))
            {
            }

            public Connector Connector
            {
                get => m_connector;
                set => m_connector = value;
            }

            public ElementId OwnerId
            {
                get => m_ownerId;
                set => m_ownerId = value;
            }

            public GXYZ Origin
            {
                get => m_origin;
                set => m_origin = value;
            }

            public static Connector GetConnector(ElementId ownerId, GXYZ connectorOrigin)
            {
                var connectors = GetConnectors(ownerId);
                foreach (Connector conn in connectors)
                {
                    if (conn.ConnectorType == ConnectorType.Logical)
                        continue;
                    if (conn.Origin.IsAlmostEqualTo(connectorOrigin))
                        return conn;
                }

                return null;
            }

            public static ConnectorSet GetConnectors(ElementId ownerId)
            {
                var element = _document.GetElement(ownerId);
                return GetConnectors(element);
            }

            public static ConnectorSet GetConnectors(Element element)
            {
                switch (element)
                {
                    case null:
                        return null;
                    case FamilyInstance fi when fi.MEPModel != null:
                        return fi.MEPModel.ConnectorManager.Connectors;
                    case MEPSystem system:
                        return system.ConnectorManager.Connectors;
                    default:
                    {
                        var duct = element as MEPCurve;
                        return duct?.ConnectorManager.Connectors;
                    }
                }
            }

            public static Connector[] FindConnectors(ConnectorManager connMgr, GXYZ pnt1, GXYZ pnt2)
            {
                var result = new Connector[2];
                var conns = connMgr.Connectors;
                foreach (Connector conn in conns)
                {
                    if (conn.Origin.IsAlmostEqualTo(pnt1))
                        result[0] = conn;
                    else if (conn.Origin.IsAlmostEqualTo(pnt2)) result[1] = conn;
                }

                return result;
            }
        }
    }

    public class LogUtility
    {
        public const string InvalidString = "[!]";

        public static void WriteElement(Element element)
        {
            WriteElement(element, true);
        }

        public static void WriteElement(Element element, bool writeId)
        {
            if (element == null)
            {
                Trace.WriteLine("null");
                return;
            }

            var elementId = element.Id;
            var familyId = element.get_Parameter(BuiltInParameter.ELEM_FAMILY_PARAM).AsElementId();
            var familyName = InvalidString;
            var objectType = GetElement<Element>(element.Document, familyId);
            var elementName = InvalidString;
            try
            {
                elementName = element.Name;
            }
            catch
            {
            }

            var familyNameParameter = objectType?.get_Parameter(BuiltInParameter.ALL_MODEL_FAMILY_NAME);
            if (familyNameParameter != null)
                familyName = familyNameParameter.AsString();

            var category = element.Category.BuiltInCategory;

            Trace.WriteLine($"Type: {element.GetType().FullName}");
            Trace.WriteLine($"Name: {familyName}:{elementName}");
            if (writeId) Trace.WriteLine($"Id: {elementId}");
            Trace.WriteLine($"Category: {category}");
            Trace.WriteLine($"FamilyId: {familyId}");
        }

        public static void WriteMechanicalSystem(MechanicalSystem system)
        {
            var flow = InvalidString;
            try
            {
                flow = system.GetFlow().ToString();
            }
            catch (Exception)
            {
            }

            Trace.WriteLine($"Flow: {flow}");
            Trace.WriteLine($"IsWellConnected: {system.IsWellConnected}");
            Trace.WriteLine($"SystemType: {system.SystemType}");
            Trace.WriteLine("+DuctNetwork");
            Trace.Indent();
            foreach (Element element in system.DuctNetwork)
            {
                WriteElement(element, false);
                Trace.WriteLine("");
            }

            Trace.Unindent();
            WriteMepSystem(system);
        }

        public static T GetElement<T>(Document document, ElementId eid) where T : Element
        {
            return document.GetElement(eid) as T;
        }

        public static void WriteMepSystem(MEPSystem system)
        {
            WriteElement(system.BaseEquipment);
            Trace.Unindent();
            Trace.WriteLine("+BaseEquipmentConnector");
            Trace.Indent();
            WriteConnector(system.BaseEquipmentConnector);
            Trace.Unindent();
            Trace.WriteLine("+Elements");
            Trace.Indent();
            foreach (Element element in system.Elements)
            {
                WriteElement(element);
                Trace.WriteLine("");
            }

            Trace.Unindent();
            Trace.WriteLine("+ConnectorManager");
            Trace.Indent();
            WriteConnectorManager(system.ConnectorManager);
            Trace.Unindent();
        }

        public static void WriteConnector(Connector connector)
        {
            if (connector == null)
            {
                Trace.WriteLine("null");
                return;
            }

            object connType = InvalidString;
            object connDirection = InvalidString;
            object connShape = InvalidString;
            try
            {
                connShape = connector.Shape;
            }
            catch
            {
            }

            object connSize = InvalidString;
            try
            {
                connSize = GetShapeInfo(connector);
            }
            catch
            {
            }

            var connLocation = GetLocation(connector);
            object connAType = connector.ConnectorType;
            object connIsConnected = InvalidString;
            switch (connector.Domain)
            {
                case Domain.DomainElectrical:
                    connType = connector.ElectricalSystemType;
                    break;
                case Domain.DomainHvac:
                    connType = connector.DuctSystemType;
                    connDirection = connector.Direction;
                    connIsConnected = connector.IsConnected;
                    break;
                case Domain.DomainPiping:
                    connType = connector.PipeSystemType;
                    connDirection = connector.Direction;
                    connIsConnected = connector.IsConnected;
                    break;
                case Domain.DomainUndefined:
                default:
                    connType = Domain.DomainUndefined;
                    break;
            }

            Trace.WriteLine($"Type: {connAType}");
            Trace.WriteLine($"SystemType: {connType}");
            Trace.WriteLine($"Direction: {connDirection}");
            Trace.WriteLine($"Shape: {connShape}");
            Trace.WriteLine($"Size: {connSize}");
            Trace.WriteLine($"Location: {connLocation}");
            Trace.WriteLine($"IsConnected: {connIsConnected}");
        }

        public static void WriteConnectorManager(ConnectorManager connectorManager)
        {
            Trace.WriteLine("+Connectors");
            Trace.Indent();
            WriteConnectorSet2(connectorManager.Connectors);
            Trace.Unindent();
            Trace.WriteLine("+UnusedConnectors");
            Trace.Indent();
            WriteConnectorSet2(connectorManager.UnusedConnectors);
            Trace.Unindent();
        }

        public static string GetConnectorId(Connector connector)
        {
            if (connector == null) return "null";
            var ownerId = connector.Owner.Id;
            var systemId = InvalidString;
            try
            {
                systemId = connector.MEPSystem.Id.ToString();
            }
            catch
            {
            }

            object connType = InvalidString;
            object connDirection = InvalidString;
            object connShape = InvalidString;
            try
            {
                connShape = connector.Shape;
            }
            catch
            {
            }

            object connSize = InvalidString;
            try
            {
                connSize = GetShapeInfo(connector);
            }
            catch
            {
            }

            var connLocation = GetLocation(connector);
            object connAType = connector.ConnectorType;
            object connIsConnected = InvalidString;
            switch (connector.Domain)
            {
                case Domain.DomainElectrical:
                    connType = connector.ElectricalSystemType;
                    break;
                case Domain.DomainHvac:
                    connType = connector.DuctSystemType;
                    connDirection = connector.Direction;
                    connIsConnected = connector.IsConnected;
                    break;
                case Domain.DomainPiping:
                    connType = connector.PipeSystemType;
                    connDirection = connector.Direction;
                    connIsConnected = connector.IsConnected;
                    break;
                case Domain.DomainUndefined:
                default:
                    connType = Domain.DomainUndefined;
                    break;
            }

            return
                $"[{ownerId}]\t[{connType}]\t[{connDirection}]\t[{connShape}]\t[{connSize}]\t[{connLocation}]\t[{connAType}]\t[{connIsConnected}]\t[{systemId}]\t";
        }

        private static string GetShapeInfo(Connector conn)
        {
            switch (conn.Shape)
            {
                case ConnectorProfileType.Invalid:
                    break;
                case ConnectorProfileType.Oval:
                    break;
                case ConnectorProfileType.Rectangular:
                    return $"{conn.Width}\" x {conn.Height}\"";
                case ConnectorProfileType.Round:
                    return $"{conn.Radius}\"";
            }

            return InvalidString;
        }

        private static object GetLocation(Connector conn)
        {
            if (conn.ConnectorType == ConnectorType.Logical) return InvalidString;
            var origin = conn.Origin;
            return $"{origin.X},{origin.Y},{origin.Z}";
        }

        private static void WriteConnectorSet2(ConnectorSet connectorSet)
        {
            var connectors = new SortedDictionary<string, List<Connector>>();
            foreach (Connector conn in connectorSet)
            {
                var connId = GetConnectorId(conn);
                if (conn.ConnectorType == ConnectorType.Logical)
                    foreach (Connector logLinkConn in conn.AllRefs)
                    {
                        connId += GetConnectorId(logLinkConn);
                    }

                if (!connectors.ContainsKey(connId)) connectors.Add(connId, new List<Connector>());

                connectors[connId].Add(conn);
            }

            foreach (var key in connectors.Keys)
            {
                foreach (var conn in connectors[key])
                {
                    WriteConnector(conn);
                    Trace.WriteLine("+AllRefs");
                    Trace.Indent();
                    WriteConnectorSet(conn.AllRefs);
                    Trace.Unindent();
                    Trace.WriteLine("");
                }
            }
        }

        private static void WriteConnectorSet(ConnectorSet connectorSet)
        {
            var connectors = new SortedDictionary<string, List<Connector>>();
            foreach (Connector conn in connectorSet)
            {
                var connId = GetConnectorId(conn);
                if (conn.ConnectorType == ConnectorType.Logical)
                    foreach (Connector logLinkConn in conn.AllRefs)
                    {
                        connId += GetConnectorId(logLinkConn);
                    }

                if (!connectors.ContainsKey(connId)) connectors.Add(connId, new List<Connector>());

                connectors[connId].Add(conn);
            }

            foreach (var key in connectors.Keys)
            {
                foreach (var conn in connectors[key])
                {
                    WriteConnector(conn);
                    Trace.WriteLine("");
                }
            }
        }
    }
}

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
    /// <summary>
    ///     route a set of ducts and fittings between a base air supply equipment and 2 terminals.
    /// </summary>
    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    [Journaling(JournalingMode.NoCommandData)]
    public class Command : IExternalCommand
    {
        /// <summary>
        ///     Minimum length of a fitting
        /// </summary>
        private const double Min1FittingLength = 1;

        /// <summary>
        ///     Minimum length of a duct
        /// </summary>
        private const double MinDuctLength = 0.5;

        /// <summary>
        ///     The vertical offset of the highest connector to the trunk ducts
        /// </summary>
        private const double VerticalTrunkOffset = 15;

        /// <summary>
        ///     Optional distance from trunk to the boundary of system bounding box
        ///     used when failed to lay out ducts in the region of bounding box
        /// </summary>
        private const double HorizontalOptionalTrunkOffset = 5;

        /// <summary>
        ///     Minimum length of 2 fittings
        /// </summary>
        private const double Min2FittingsLength = Min1FittingLength * 2;

        /// <summary>
        ///     The minimum length of 1 duct with 2 fittings
        /// </summary>
        private const double Min1Duct2FittingsLength = Min1FittingLength * 2 + MinDuctLength;

        /// <summary>
        ///     The revit application
        /// </summary>
        private static Application _application;

        /// <summary>
        ///     The current document of the application
        /// </summary>
        private static Document _document;

        /// <summary>
        ///     The type of the ducts in the system
        /// </summary>
        private DuctType m_dtRectangle;

        /// <summary>
        ///     The type id of the duct
        /// </summary>
        private readonly ElementId m_ductTypeId = new ElementId(139191L);

        /// <summary>
        ///     The level of the duct
        /// </summary>
        private Level m_lvl;

        /// <summary>
        ///     The mechanical system that will be created
        /// </summary>
        private MechanicalSystem m_mechanicalSystem;

        /// <summary>
        ///     The system type id of the duct
        /// </summary>
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

            //get the system type id of the duct
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

                //Get the connectors and bounding boxes
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

                //Find the "Out" and "SupplyAir" connector on the base equipment
                csi = ConnectorInfo.GetConnectors(ids[0]).ForwardIterator();
                while (csi.MoveNext())
                {
                    var conn = csi.Current as Connector;
                    if (conn.Direction == FlowDirectionType.Out && conn.DuctSystemType == DuctSystemType.SupplyAir)
                        conns[0] = conn;
                }

                //Create a mechanical system with a base air supply equipment and 2 terminals.
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

                //Get the boundary of the system
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

                //Calculate the optional values for the trunk ducts
                var midX = (minX + maxX) / 2;
                var midY = (minY + maxY) / 2;
                var baseXValues = new double[3] { midX, (minX + midX) / 2, (maxX + midX) / 2 };
                var baseYValues = new double[3] { midY, (minY + midY) / 2, (maxY + midY) / 2 };

                //Get the duct type for the ducts to be created
                m_dtRectangle = _document.GetElement(m_ductTypeId) as DuctType;

                //Create the ducts and elbows that connect the base mechanical equipment
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

                //Create the vertical ducts for terminals
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

                //Connect the system by creating the trunk line of ducts and connect them to the base connectors
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

        /// <summary>
        ///     Connect the system with a trunk line of ducts on X axis
        /// </summary>
        /// <param name="baseConnectors">
        ///     the upper connectors of the vertical ducts that derived from the terminals and the base
        ///     equipment
        /// </param>
        /// <param name="baseY">the y value of the trunk line</param>
        /// <returns>
        ///     true if the system can be connected
        ///     false if the system cannot be connected
        /// </returns>
        private bool ConnectSystemOnXAxis(List<Connector> baseConnectors, double baseY)
        {
            //Check the count of the base connectors
            if (null == baseConnectors || 3 != baseConnectors.Count) return false;
            for (var i = 0; i < baseConnectors.Count; ++i)
            {
                //Check the distance of the connector from the trunk
                if (baseConnectors[i].Origin.Y != baseY &&
                    Math.Abs(baseConnectors[i].Origin.Y - baseY) < Min1Duct2FittingsLength) return false;
                //Check the distance of the connectors on X axis
                for (var j = i + 1; j < baseConnectors.Count; ++j)
                    if (baseConnectors[j].Origin.X != baseConnectors[i].Origin.X &&
                        baseConnectors[j].Origin.X - baseConnectors[i].Origin.X < Min2FittingsLength)
                        return false;
            }

            try
            {
                var baseZ = baseConnectors[0].Origin.Z + Min1FittingLength;
                //Create the ducts and elbow fittings to connect the vertical ducts and the trunk ducts
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

                    //Create the trunk
                    connectors = CreateDuct(new GXYZ(baseConnectors[0].Origin.X + Min1FittingLength, baseY, baseZ),
                        new GXYZ(baseConnectors[2].Origin.X - Min1FittingLength, baseY, baseZ));

                    //Create a tee fitting connecting the 1st and 2nd base connectors to the trunk
                    ConnectWithTeeFittingOnXAxis(baseConnectors[0], baseConnectors[1], connectors[0], true);

                    //Create an elbow fitting connection the 3rd base connector to the trunk
                    ConnectWithElbowFittingOnXAxis(baseConnectors[2], connectors[1]);
                }
                else
                {
                    //Create the segment of duct on the trunk to be connected to the 1st base connector
                    connectors = CreateDuct(new GXYZ(baseConnectors[0].Origin.X + Min1FittingLength, baseY, baseZ),
                        new GXYZ(baseConnectors[1].Origin.X - Min1FittingLength, baseY, baseZ));

                    //Create an elbow fitting connection the 1st base connector with the trunk
                    ConnectWithElbowFittingOnXAxis(baseConnectors[0], connectors[0]);

                    if (baseConnectors[1].Origin.X == baseConnectors[2].Origin.X)
                    {
                        //The 2nd and 3rd connectors are on the same side of the trunk
                        if (Math.Sign(baseConnectors[1].Origin.Y - baseY) *
                            Math.Sign(baseConnectors[2].Origin.Y - baseY) == 1) return false;
                        //Create a tee fitting connecting the 2nd and 3rd base connectors to the trunk
                        ConnectWithTeeFittingOnXAxis(baseConnectors[1], baseConnectors[2], connectors[1], true);
                    }
                    else
                    {
                        connectors.AddRange(CreateDuct(
                            new GXYZ(baseConnectors[1].Origin.X + Min1FittingLength, baseY, baseZ),
                            new GXYZ(baseConnectors[2].Origin.X - Min1FittingLength, baseY, baseZ)));
                        //Create a tee fitting connecting the 2nd base connector to the trunk
                        ConnectWithTeeFittingOnXAxis(baseConnectors[1], connectors[1], connectors[2], false);
                        //Create an elbow fitting connection the 3rd base connector to the trunk
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

        /// <summary>
        ///     Connect a base connector to a connector on the trunk with an elbow fitting
        /// </summary>
        /// <param name="baseConn">the upper connector of the vertical duct that derived from a terminal or the base equipment</param>
        /// <param name="conn">the connector of a duct on the trunk</param>
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

        /// <summary>
        ///     Connect 3 connectors on the trunk with a tee fitting
        /// </summary>
        /// <param name="conn1">the first connector</param>
        /// <param name="conn2">the second connector</param>
        /// <param name="conn3">the third connector</param>
        /// <param name="flag">a flag to indicate whether there are 2 base connectors or 1 base connector</param>
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

        /// <summary>
        ///     Sort the base connectors by their x values
        /// </summary>
        /// <param name="connectors">the connectors to be sorted</param>
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

        /// <summary>
        ///     Connect the system with a trunk line of ducts on Y axis
        /// </summary>
        /// <param name="baseConnectors">
        ///     the upper connectors of the vertical ducts that derived from the terminals and the base
        ///     equipment
        /// </param>
        /// <param name="baseX">the x value of the trunk line</param>
        /// <returns>
        ///     true if the system can be connected
        ///     false if the system cannot be connected
        /// </returns>
        private bool ConnectSystemOnYAxis(List<Connector> baseConnectors, double baseX)
        {
            //Check the count of the base connectors
            if (null == baseConnectors || 3 != baseConnectors.Count) return false;
            for (var i = 0; i < baseConnectors.Count; ++i)
            {
                //Check the distance of the connector from the trunk
                if (baseConnectors[i].Origin.X != baseX &&
                    Math.Abs(baseConnectors[i].Origin.X - baseX) < Min1Duct2FittingsLength) return false;
                //Check the distance of the connectors on Y axis
                for (var j = i + 1; j < baseConnectors.Count; ++j)
                    if (baseConnectors[j].Origin.Y != baseConnectors[i].Origin.Y &&
                        baseConnectors[j].Origin.Y - baseConnectors[i].Origin.Y < Min2FittingsLength)
                        return false;
            }

            try
            {
                var baseZ = baseConnectors[0].Origin.Z + Min1FittingLength;
                //Create the ducts and elbow fittings to connect the vertical ducts and the trunk ducts
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

                    //Create the trunk
                    connectors = CreateDuct(new GXYZ(baseX, baseConnectors[0].Origin.Y + Min1FittingLength, baseZ),
                        new GXYZ(baseX, baseConnectors[2].Origin.Y - Min1FittingLength, baseZ));

                    //Create a tee fitting connecting the 1st and 2nd base connectors to the trunk
                    ConnectWithTeeFittingOnYAxis(baseConnectors[0], baseConnectors[1], connectors[0], true);

                    //Create an elbow fitting connection the 3rd base connector to the trunk
                    ConnectWithElbowFittingOnYAxis(baseConnectors[2], connectors[1]);
                }
                else
                {
                    //Create the segment of duct on the trunk to be connected to the 1st base connector
                    connectors = CreateDuct(new GXYZ(baseX, baseConnectors[0].Origin.Y + Min1FittingLength, baseZ),
                        new GXYZ(baseX, baseConnectors[1].Origin.Y - Min1FittingLength, baseZ));

                    //Create an elbow fitting connection the 1st base connector with the trunk
                    ConnectWithElbowFittingOnYAxis(baseConnectors[0], connectors[0]);

                    if (baseConnectors[1].Origin.Y == baseConnectors[2].Origin.Y)
                    {
                        //The 2nd and 3rd connectors are on the same side of the trunk
                        if (Math.Sign(baseConnectors[1].Origin.X - baseX) *
                            Math.Sign(baseConnectors[2].Origin.X - baseX) == 1) return false;
                        //Create a tee fitting connecting the 2nd and 3rd base connectors to the trunk
                        ConnectWithTeeFittingOnYAxis(baseConnectors[1], baseConnectors[2], connectors[1], true);
                    }
                    else
                    {
                        connectors.AddRange(CreateDuct(
                            new GXYZ(baseX, baseConnectors[1].Origin.Y + Min1FittingLength, baseZ),
                            new GXYZ(baseX, baseConnectors[2].Origin.Y - Min1FittingLength, baseZ)));
                        //Create a tee fitting connecting the 2nd base connector to the trunk
                        ConnectWithTeeFittingOnYAxis(baseConnectors[1], connectors[1], connectors[2], false);
                        //Create an elbow fitting connection the 3rd base connector to the trunk
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

        /// <summary>
        ///     Connect a base connector to a connector on the trunk with an elbow fitting
        /// </summary>
        /// <param name="baseConn">the upper connector of the vertical duct that derived from a terminal or the base equipment</param>
        /// <param name="conn">the connector of a duct on the trunk</param>
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

        /// <summary>
        ///     Connect 3 connectors on the trunk with a tee fitting
        /// </summary>
        /// <param name="conn1">the first connector</param>
        /// <param name="conn2">the second connector</param>
        /// <param name="conn3">the third connector</param>
        /// <param name="flag">a flag to indicate whether there are 2 base connectors or 1 base connector</param>
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

        /// <summary>
        ///     Sort the base connectors by their y values
        /// </summary>
        /// <param name="connectors">the connectors to be sorted</param>
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

        /// <summary>
        ///     Create a duct with two points
        /// </summary>
        /// <param name="point1">the first point</param>
        /// <param name="point2">the second point</param>
        /// <returns></returns>
        private List<Connector> CreateDuct(GXYZ point1, GXYZ point2)
        {
            var connectors = new List<Connector>();

            var duct = Duct.Create(_document, m_systemTypeId, m_ductTypeId, m_lvl.Id, point1, point2);

            connectors.Add(ConnectorInfo.GetConnector(duct.Id, point1));
            connectors.Add(ConnectorInfo.GetConnector(duct.Id, point2));

            return connectors;
        }

        /// <summary>
        ///     Create a mechanical system
        /// </summary>
        /// <param name="baseConnector">the base connector of the mechanical system</param>
        /// <param name="connectors">the connectors of the mechanical system</param>
        /// <param name="systemType">the system type of the mechanical system</param>
        /// <returns>the created mechanical system</returns>
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

        /// <summary>
        ///     information of a connector
        /// </summary>
        public class ConnectorInfo
        {
            /// <summary>
            ///     The Connector object
            /// </summary>
            private Connector m_connector;

            /// <summary>
            ///     The origin of the connector
            /// </summary>
            private GXYZ m_origin;

            /// <summary>
            ///     The owner's element ID
            /// </summary>
            private ElementId m_ownerId;

            /// <summary>
            ///     The constructor that finds the connector with the owner ID and origin
            /// </summary>
            /// <param name="ownerId">the ownerID of the connector</param>
            /// <param name="origin">the origin of the connector</param>
            public ConnectorInfo(ElementId ownerId, GXYZ origin)
            {
                m_ownerId = ownerId;
                m_origin = origin;
                m_connector = GetConnector(m_ownerId, origin);
            }

            /// <summary>
            ///     The constructor that finds the connector with the owner ID and the values of the origin
            /// </summary>
            /// <param name="ownerId">the ownerID of the connector</param>
            /// <param name="x">the X value of the connector</param>
            /// <param name="y">the Y value of the connector</param>
            /// <param name="z">the Z value of the connector</param>
            public ConnectorInfo(ElementId ownerId, double x, double y, double z)
                : this(ownerId, new GXYZ(x, y, z))
            {
            }

            /// <summary>
            ///     The connector this object represents
            /// </summary>
            public Connector Connector
            {
                get => m_connector;
                set => m_connector = value;
            }

            /// <summary>
            ///     The owner ID of the connector
            /// </summary>
            public ElementId OwnerId
            {
                get => m_ownerId;
                set => m_ownerId = value;
            }

            /// <summary>
            ///     The origin of the connector
            /// </summary>
            public GXYZ Origin
            {
                get => m_origin;
                set => m_origin = value;
            }

            /// <summary>
            ///     Get the connector of the owner at the specific origin
            /// </summary>
            /// <param name="ownerId">the owner ID of the connector</param>
            /// <param name="connectorOrigin">the origin of the connector</param>
            /// <returns>if found, return the connector, or else return null</returns>
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

            /// <summary>
            ///     Get all the connectors of an element with a specific ID
            /// </summary>
            /// <param name="ownerId">the owner ID of the connector</param>
            /// <returns>the connector set which includes all the connectors found</returns>
            public static ConnectorSet GetConnectors(ElementId ownerId)
            {
                var element = _document.GetElement(ownerId);
                return GetConnectors(element);
            }

            /// <summary>
            ///     Get all the connectors of a specific element
            /// </summary>
            /// <param name="element">the owner of the connector</param>
            /// <returns>if found, return all the connectors found, or else return null</returns>
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

            /// <summary>
            ///     Find the two connectors of the specific ConnectorManager at the two locations
            /// </summary>
            /// <param name="connMgr">The ConnectorManager of the connectors to be found</param>
            /// <param name="ptn1">the location of the first connector</param>
            /// <param name="ptn2">the location of the second connector</param>
            /// <returns>The two connectors found</returns>
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
        /// <summary>
        ///     Invalid string.
        /// </summary>
        public const string InvalidString = "[!]";

        /// <summary>
        ///     Write the information of an element to the log file
        /// </summary>
        /// <param name="element">the element whose information is to be written</param>
        public static void WriteElement(Element element)
        {
            WriteElement(element, true);
        }

        /// <summary>
        ///     Write the information of an element to the log file
        /// </summary>
        /// <param name="element">the element whose information is to be written</param>
        /// <param name="writeId">whether the id will be outputted</param>
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

        /// <summary>
        ///     Write the information of a mechanical system to the log file
        /// </summary>
        /// <param name="system">the mechanical system whose information is to be written</param>
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

        /// <summary>
        ///     Get element by its id and cast it to the specified type
        /// </summary>
        /// <param name="document">the owner document of the element</param>
        /// <param name="eid">the id of the element</param>
        /// <returns>the element of the specified type</returns>
        public static T GetElement<T>(Document document, ElementId eid) where T : Element
        {
            return document.GetElement(eid) as T;
        }

        /// <summary>
        ///     Write the information of a MEPSystem to the log file.
        /// </summary>
        /// <param name="system">the MEP system whose information is to be written</param>
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

        /// <summary>
        ///     Write the information of a connector to the log file
        /// </summary>
        /// <param name="connector">the connector whose information is to be written</param>
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

        /// <summary>
        ///     Write the information of a ConnectorManager to the log file
        /// </summary>
        /// <param name="connectorManager">the ConnectorManager whose information is to be written</param>
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

        /// <summary>
        ///     Get the information string of a connector
        /// </summary>
        /// <param name="connector">the connector to be read</param>
        /// <returns>the information string of the connector</returns>
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

        /// <summary>
        ///     Get the shape information string of a connector
        /// </summary>
        /// <param name="conn">the element to be read</param>
        /// <returns>the shape information string of the connector</returns>
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

        /// <summary>
        ///     Get the location string of a connector
        /// </summary>
        /// <param name="conn">the connector to be read</param>
        /// <returns>the location information string of the connector</returns>
        private static object GetLocation(Connector conn)
        {
            if (conn.ConnectorType == ConnectorType.Logical) return InvalidString;
            var origin = conn.Origin;
            return $"{origin.X},{origin.Y},{origin.Z}";
        }

        /// <summary>
        ///     Write the information of a ConnectorSet to the log file.
        /// </summary>
        /// <param name="connectorSet">the ConnectorSet whose information is to be written</param>
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

        /// <summary>
        ///     Write the information of a ConnectorSet to the log file.
        /// </summary>
        /// <param name="connectorSet">the mechanical system whose information is to be written</param>
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

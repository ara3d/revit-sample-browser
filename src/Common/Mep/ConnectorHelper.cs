// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt


using Ara3D.RevitSampleBrowser.Common.Geometry;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Electrical;
using Autodesk.Revit.DB.Mechanical;
using Autodesk.Revit.DB.Plumbing;
using System.Collections.Generic;

namespace Ara3D.RevitSampleBrowser.Common.Mep
{
    public static class ConnectorHelper
    {
        public static Connector FindConnector(Pipe pipe, XYZ conXyz)
        {
            foreach (Connector conn in pipe.ConnectorManager.Connectors)
            {
                if (conn.Origin.IsAlmostEqualTo(conXyz))
                    return conn;
            }

            return null;
        }

        public static Vector4 ComputeCrossProduct(Edge edgeA, Edge edgeB)
        {
            var pointsA = edgeA.Tessellate() as List<XYZ>;
            var pointsB = edgeB.Tessellate() as List<XYZ>;
            return Vector4.CrossProduct(new Vector4(pointsA[1] - pointsA[0]), new Vector4(pointsB[1] - pointsB[0]));
        }

        public static bool VerifyUnusedConnectors(FamilyInstance fi)
        {
            var mepModel = fi.MEPModel;
            if (mepModel == null) return false;

            var unusedConnectors = mepModel.ConnectorManager?.UnusedConnectors;
            if (unusedConnectors == null || unusedConnectors.IsEmpty) return false;

            foreach (Connector connector in unusedConnectors)
            {
                if (connector.Domain == Domain.DomainElectrical) return true;
            }

            return false;
        }

        public static bool IsElementBelongsToCircuit(MEPModel mepModel, ElectricalSystem selectedElectricalSystem)
        {
            var ess = mepModel.GetElectricalSystems();
            return ess != null && ess.Contains(selectedElectricalSystem);
        }

        public static MEPSystem ExtractSystemFromConnectors(ConnectorSet connectors)
        {
            if (connectors == null || connectors.Size == 0)
                return null;

            List<MEPSystem> systems = [];
            foreach (Connector connector in connectors)
            {
                var tmpSystem = connector.MEPSystem;
                switch (tmpSystem)
                {
                    case null:
                        continue;
                    case MechanicalSystem ms when ms.IsWellConnected:
                        systems.Add(tmpSystem);
                        break;
                    case PipingSystem ps when ps.IsWellConnected:
                        systems.Add(tmpSystem);
                        break;
                }
            }

            MEPSystem system = null;
            if (systems.Count == 0)
                return null;

            var countOfElements = 0;
            foreach (var sys in systems)
            {
                if (sys.Elements.Size <= countOfElements)
                    continue;
                system = sys;
                countOfElements = sys.Elements.Size;
            }

            return system;
        }

        public static Connector GetConnectedConnector(Connector connector)
        {
            foreach (Connector conn in connector.AllRefs)
            {
                if (conn.ConnectorType != ConnectorType.End || conn.Owner.Id == connector.Owner.Id)
                    continue;
                return conn;
            }

            return null;
        }

    }
}
// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;
using System.Linq;


namespace BuildingCoder
{
    internal static partial class Util
    {
        #region MEP utilities

        private static ConnectorManager GetConnectorManager(
            Element e)
        {
            var mc = e as MEPCurve;
            var fi = e as FamilyInstance;

            return null == mc && null == fi
                ? throw new ArgumentException(
                    "Element is neither an MEP curve nor a fitting.")
                : null == mc
                ? fi.MEPModel.ConnectorManager
                : mc.ConnectorManager;
        }

        private static Connector GetConnectorAt(
            Element e,
            XYZ location,
            out Connector otherConnector)
        {
            otherConnector = null;

            Connector targetConnector = null;

            var cm = GetConnectorManager(e);

            var hasTwoConnectors = 2 == cm.Connectors.Size;

            foreach (Connector c in cm.Connectors)
                if (c.Origin.IsAlmostEqualTo(location))
                {
                    targetConnector = c;

                    if (!hasTwoConnectors) break;
                }
                else if (hasTwoConnectors)
                {
                    otherConnector = c;
                }

            return targetConnector;
        }

        private static Connector GetConnectorClosestTo(
            ConnectorSet connectors,
            XYZ p)
        {
            Connector targetConnector = null;
            var minDist = double.MaxValue;

            foreach (Connector c in connectors)
            {
                var d = c.Origin.DistanceTo(p);

                if (d < minDist)
                {
                    targetConnector = c;
                    minDist = d;
                }
            }
            return targetConnector;
        }

        public static Connector GetConnectorClosestTo(
            Element e,
            XYZ p)
        {
            var cm = GetConnectorManager(e);

            return null == cm
                ? null
                : GetConnectorClosestTo(cm.Connectors, p);
        }

        public static void Connect(
            XYZ p,
            Element a,
            Element b)
        {
            var cm = GetConnectorManager(a);

            if (null == cm)
                throw new ArgumentException(
                    "Element a has no connectors.");

            var ca = GetConnectorClosestTo(
                cm.Connectors, p);

            cm = GetConnectorManager(b);

            if (null == cm)
                throw new ArgumentException(
                    "Element b has no connectors.");

            var cb = GetConnectorClosestTo(
                cm.Connectors, p);

            ca.ConnectTo(cb);
            //cb.ConnectTo( ca );
        }

        public class ConnectorXyzComparer : IEqualityComparer<Connector>
        {
            public bool Equals(Connector x, Connector y)
            {
                return null != x
                       && null != y
                       && IsEqual(x.Origin, y.Origin);
            }

            public int GetHashCode(Connector x)
            {
                return HashString(x.Origin).GetHashCode();
            }
        }

        public static HashSet<Connector> GetDistinctConnectors(
            List<Connector> cons)
        {
            return cons.Distinct(new ConnectorXyzComparer())
                .ToHashSet();
        }

        #endregion
    }
}

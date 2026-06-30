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
        #region MEP utilities

        /// <summary>
        ///     Return the given element's connector manager,
        ///     using either the family instance MEPModel or
        ///     directly from the MEPCurve connector manager
        ///     for ducts and pipes.
        /// </summary>
        private static ConnectorManager GetConnectorManager(
            Element e)
        {
            var mc = e as MEPCurve;
            var fi = e as FamilyInstance;

            if (null == mc && null == fi)
                throw new ArgumentException(
                    "Element is neither an MEP curve nor a fitting.");

            return null == mc
                ? fi.MEPModel.ConnectorManager
                : mc.ConnectorManager;
        }

        /// <summary>
        ///     Return the element's connector at the given
        ///     location, and its other connector as well,
        ///     in case there are exactly two of them.
        /// </summary>
        /// <param name="e">An element, e.g. duct, pipe or family instance</param>
        /// <param name="location">The location of one of its connectors</param>
        /// <param name="otherConnector">The other connector, in case there are just two of them</param>
        /// <returns>The connector at the given location</returns>
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

        /// <summary>
        ///     Return the connector set element
        ///     closest to the given point.
        /// </summary>
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

        /// <summary>
        ///     Return the connector on the element
        ///     closest to the given point.
        /// </summary>
        public static Connector GetConnectorClosestTo(
            Element e,
            XYZ p)
        {
            var cm = GetConnectorManager(e);

            return null == cm
                ? null
                : GetConnectorClosestTo(cm.Connectors, p);
        }

        /// <summary>
        ///     Connect two MEP elements at a given point p.
        /// </summary>
        /// <exception cref="ArgumentException">
        ///     Thrown if
        ///     one of the given elements lacks connectors.
        /// </exception>
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

        /// <summary>
        ///     Compare Connector objects based on their location point.
        /// </summary>
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

        /// <summary>
        ///     Get distinct connectors from a set of MEP elements.
        /// </summary>
        public static HashSet<Connector> GetDistinctConnectors(
            List<Connector> cons)
        {
            return cons.Distinct(new ConnectorXyzComparer())
                .ToHashSet();
        }

        #endregion
    }
}

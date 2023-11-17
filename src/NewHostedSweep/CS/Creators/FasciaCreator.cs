// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using System.Collections;
using System.Collections.Generic;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Architecture;
using Autodesk.Revit.Exceptions;
using Autodesk.Revit.UI;

namespace Ara3D.RevitSampleBrowser.NewHostedSweep.CS.Creators
{
    /// <summary>
    ///     Provides functions to create Fascia.
    /// </summary>
    public class FasciaCreator : HostedSweepCreator
    {
        /// <summary>
        ///     Dictionary to store the roof=>edges for fascia creation.
        /// </summary>
        private Dictionary<Element, List<Edge>> m_roofFasciaEdges;

        /// <summary>
        ///     Constructor which take Revit.Document as parameter.
        /// </summary>
        /// <param name="rvtDoc">Revit document</param>
        public FasciaCreator(UIDocument rvtDoc)
            : base(rvtDoc)
        {
        }

        /// <summary>
        ///     A string indicates this creator just for Roof Fascia creation.
        /// </summary>
        public override string Name => "Roof Fascia";

        /// <summary>
        ///     All fascia types in Revit active document.
        /// </summary>
        public override IEnumerable AllTypes
        {
            get
            {
                var filteredElementCollector = new FilteredElementCollector(RvtDoc);
                filteredElementCollector.OfClass(typeof(FasciaType));
                return filteredElementCollector;
            }
        }

        /// <summary>
        ///     Dictionary to store all the Roof=>Edges which Fascia can be created on.
        /// </summary>
        public override Dictionary<Element, List<Edge>> SupportEdges
        {
            get
            {
                if (m_roofFasciaEdges == null)
                {
                    m_roofFasciaEdges = new Dictionary<Element, List<Edge>>();
                    var collector = new FilteredElementCollector(RvtDoc);
                    collector.OfClass(typeof(FootPrintRoof));
                    var elements = collector.ToElements();

                    collector = new FilteredElementCollector(RvtDoc);
                    collector.OfClass(typeof(ExtrusionRoof));
                    foreach (var elem in collector)
                    {
                        elements.Add(elem);
                    }

                    foreach (var elem in elements)
                    {
                        if (elem is RoofBase)
                        {
                            var solid = ExtractGeom(elem);
                            if (solid != null)
                            {
                                m_roofFasciaEdges.Add(elem, new List<Edge>());
                                ElemGeom.Add(elem, solid);
                                FilterEdgesForFascia(elem);
                            }
                        }
                    }
                }

                return m_roofFasciaEdges;
            }
        }

        /// <summary>
        ///     Filter all the edges of the given element for fascia creation.
        /// </summary>
        /// <param name="elem">Element used to filter edges which fascia can be created on</param>
        private void FilterEdgesForFascia(Element elem)
        {
            var transaction = new Transaction(RvtDocument, "FilterEdgesForFascia");
            transaction.Start();

            // Note: This method will create a Fascia with no references.
            // In the future, API may not allow to create such Fascia with 
            // no references, invoke this methods like this may throw exception.
            // 
            var fascia = RvtDoc.Create.NewFascia(null, new ReferenceArray());

            var roofEdges = m_roofFasciaEdges[elem];
            foreach (var edge in ElemGeom[elem].EdgeBindingDic.Keys)
            {
                if (edge.Reference == null) continue;
                try
                {
                    fascia.AddSegment(edge.Reference);
                    // AddSegment successfully, so this edge can be used to crate Fascia.
                    roofEdges.Add(edge);
                }
                catch (ArgumentOutOfRangeException)
                {
                    // Exception, this edge will be discard.
                }
            }

            // Delete this element, because we just use it to filter the edges.
            RvtDoc.Delete(fascia.Id);

            transaction.RollBack();
        }

        /// <summary>
        ///     Create a Fascia.
        /// </summary>
        /// <param name="symbol">Fascia type</param>
        /// <param name="refArr">Fascia reference array</param>
        /// <returns>Created Fascia</returns>
        protected override HostedSweep CreateHostedSweep(ElementType symbol, ReferenceArray refArr)
        {
            var fascia = RvtDoc.Create.NewFascia(symbol as FasciaType, refArr);

            return fascia;
        }
    }
}

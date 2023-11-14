// Copyright 2023. See https://github.com/ara3d/revit-samples/LICENSE.txt

using System.Collections;
using System.Collections.Generic;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Architecture;
using Autodesk.Revit.Exceptions;
using Autodesk.Revit.UI;

namespace Revit.SDK.Samples.NewHostedSweep.CS
{
    /// <summary>
    ///     Provides functions to create Gutter.
    /// </summary>
    public class GutterCreator : HostedSweepCreator
    {
        /// <summary>
        ///     Edges which gutter can be created on.
        /// </summary>
        private Dictionary<Element, List<Edge>> m_roofGutterEdges;

        /// <summary>
        ///     Constructor with Revit.Document as parameter.
        /// </summary>
        /// <param name="rvtDoc">Revit document</param>
        public GutterCreator(UIDocument rvtDoc)
            : base(rvtDoc)
        {
        }

        /// <summary>
        ///     A string indicates this creator just for Roof Gutter creation.
        /// </summary>
        public override string Name => "Roof Gutter";

        /// <summary>
        ///     All Gutter types in Revit active document.
        /// </summary>
        public override IEnumerable AllTypes
        {
            get
            {
                var filteredElementCollector = new FilteredElementCollector(RvtDoc);
                filteredElementCollector.OfClass(typeof(GutterType));
                return filteredElementCollector;
            }
        }

        /// <summary>
        ///     Dictionary to store all the Roof=>Edges which Gutter can be created on.
        /// </summary>
        public override Dictionary<Element, List<Edge>> SupportEdges
        {
            get
            {
                if (m_roofGutterEdges == null)
                {
                    m_roofGutterEdges = new Dictionary<Element, List<Edge>>();

                    var collector = new FilteredElementCollector(RvtDoc);
                    collector.OfClass(typeof(FootPrintRoof));
                    var elements = collector.ToElements();

                    collector = new FilteredElementCollector(RvtDoc);
                    collector.OfClass(typeof(ExtrusionRoof));
                    foreach (var elem in collector) elements.Add(elem);

                    foreach (var elem in elements)
                        if (elem is RoofBase)
                        {
                            var solid = ExtractGeom(elem);
                            if (solid != null)
                            {
                                m_roofGutterEdges.Add(elem, new List<Edge>());
                                ElemGeom.Add(elem, solid);
                                FilterEdgesForGutter(elem);
                            }
                        }
                }

                return m_roofGutterEdges;
            }
        }

        /// <summary>
        ///     Filter all the edges from the element which gutter can be created on.
        /// </summary>
        /// <param name="elem"></param>
        private void FilterEdgesForGutter(Element elem)
        {
            var transaction = new Transaction(RvtDocument, "FilterEdgesForGutter");
            transaction.Start();

            // Note: This method will create a Gutter with no reference.
            // In the future, API may not allow to create such Gutter with 
            // no references, invoke this methods like this may throw exception.
            // 
            var gutter = RvtDoc.Create.NewGutter(null, new ReferenceArray());

            var roofEdges = m_roofGutterEdges[elem];
            foreach (var edge in ElemGeom[elem].EdgeBindingDic.Keys)
            {
                if (edge.Reference == null) continue;
                try
                {
                    gutter.AddSegment(edge.Reference);
                    // AddSegment successfully, so this edge can be used to crate Gutter.
                    roofEdges.Add(edge);
                }
                catch (ArgumentOutOfRangeException)
                {
                    // Exception, this edge will be discard.
                }
            }

            // Delete this element, because we just use it to filter the edges.
            RvtDoc.Delete(gutter.Id);

            transaction.RollBack();
        }

        /// <summary>
        ///     Create a Gutter.
        /// </summary>
        /// <param name="symbol">Gutter type</param>
        /// <param name="refArr">Gutter Reference array</param>
        /// <returns>Created Gutter</returns>
        protected override HostedSweep CreateHostedSweep(ElementType symbol, ReferenceArray refArr)
        {
            var gutter = RvtDoc.Create.NewGutter(symbol as GutterType, refArr);
            return gutter;
        }
    }
}

//
// (C) Copyright 2003-2019 by Autodesk, Inc.
//
// Permission to use, copy, modify, and distribute this software in
// object code form for any purpose and without fee is hereby granted,
// provided that the above copyright notice appears in all copies and
// that both that copyright notice and the limited warranty and
// restricted rights notice below appear in all supporting
// documentation.
//
// AUTODESK PROVIDES THIS PROGRAM "AS IS" AND WITH ALL FAULTS.
// AUTODESK SPECIFICALLY DISCLAIMS ANY IMPLIED WARRANTY OF
// MERCHANTABILITY OR FITNESS FOR A PARTICULAR USE. AUTODESK, INC.
// DOES NOT WARRANT THAT THE OPERATION OF THE PROGRAM WILL BE
// UNINTERRUPTED OR ERROR FREE.
//
// Use, duplication, or disclosure by the U.S. Government is subject to
// restrictions set forth in FAR 52.227-19 (Commercial Computer
// Software - Restricted Rights) and DFAR 252.227-7013(c)(1)(ii)
// (Rights in Technical Data and Computer Software), as applicable.
//

using System.Collections;
using System.Collections.Generic;
using Autodesk.Revit.DB;
using Autodesk.Revit.Exceptions;
using Autodesk.Revit.UI;

namespace Revit.SDK.Samples.NewHostedSweep.CS
{
    /// <summary>
    ///     Provides functions to create SlabEdge.
    /// </summary>
    public class SlabEdgeCreator : HostedSweepCreator
    {
        /// <summary>
        ///     Edges which SlabEdge can be created on.
        /// </summary>
        private Dictionary<Element, List<Edge>> m_floorSlabEdges;

        /// <summary>
        ///     Constructor takes Revit.Document as parameter.
        /// </summary>
        /// <param name="rvtDoc">Revit document</param>
        public SlabEdgeCreator(UIDocument rvtDoc)
            : base(rvtDoc)
        {
        }

        /// <summary>
        ///     A string indicates this creator just for Floor SlabEdge creation.
        /// </summary>
        public override string Name => "Floor Slab Edge";

        /// <summary>
        ///     All SlabEdge types in Revit active document.
        /// </summary>
        public override IEnumerable AllTypes
        {
            get
            {
                var filteredElementCollector = new FilteredElementCollector(m_rvtDoc);
                filteredElementCollector.OfClass(typeof(SlabEdgeType));
                return filteredElementCollector;
            }
        }

        /// <summary>
        ///     Dictionary to store all the Floor=>Edges which SlabEdge can be created on.
        /// </summary>
        public override Dictionary<Element, List<Edge>> SupportEdges
        {
            get
            {
                if (m_floorSlabEdges == null)
                {
                    m_floorSlabEdges = new Dictionary<Element, List<Edge>>();

                    var collector = new FilteredElementCollector(m_rvtDoc);
                    collector.OfClass(typeof(Floor));
                    foreach (var elem in collector.ToElements())
                        if (elem is Floor)
                        {
                            var solid = ExtractGeom(elem);
                            if (solid != null)
                            {
                                m_floorSlabEdges.Add(elem, new List<Edge>());
                                m_elemGeom.Add(elem, solid);
                                FilterEdgesForSlabEdge(elem);
                            }
                        }
                }

                return m_floorSlabEdges;
            }
        }

        /// <summary>
        ///     Filter all the edges from the element which SlabEdge can be created on.
        /// </summary>
        /// <param name="elem"></param>
        private void FilterEdgesForSlabEdge(Element elem)
        {
            var transaction = new Transaction(RvtDocument, "FilterEdgesForSlabEdge");
            transaction.Start();

            // Note: This method will create a SlabEdge with no reference.
            // In the future, API may not allow to create such SlabEdge with 
            // no references, invoke this methods like this may throw exception.
            // 
            var slabEdge = m_rvtDoc.Create.NewSlabEdge(null, new ReferenceArray());

            var floorEdges = m_floorSlabEdges[elem];
            foreach (var edge in m_elemGeom[elem].EdgeBindingDic.Keys)
            {
                if (edge.Reference == null) continue;
                try
                {
                    slabEdge.AddSegment(edge.Reference);
                    // AddSegment successfully, so this edge can be used to crate SlabEdge.
                    floorEdges.Add(edge);
                }
                catch (ArgumentOutOfRangeException)
                {
                    // Exception, this edge will be discard.
                }
            }

            // Delete this element, because we just use it to filter the edges.
            m_rvtDoc.Delete(slabEdge.Id);

            transaction.RollBack();
        }

        /// <summary>
        ///     Create a SlabEdge.
        /// </summary>
        /// <param name="symbol">SlabEdge type</param>
        /// <param name="refArr">SlabEdge reference array</param>
        /// <returns>Created SlabEdge</returns>
        protected override HostedSweep CreateHostedSweep(ElementType symbol, ReferenceArray refArr)
        {
            var slabEdge = m_rvtDoc.Create.NewSlabEdge(symbol as SlabEdgeType, refArr);
            if (slabEdge != null)
                // Avoid the Revit warning, flip the direction in horizontal direction.
                slabEdge.HorizontalFlip();
            return slabEdge;
        }
    }
}
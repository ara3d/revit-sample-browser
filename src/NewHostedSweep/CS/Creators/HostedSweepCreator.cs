// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using System.Collections;
using System.Collections.Generic;
using Ara3D.RevitSampleBrowser.NewHostedSweep.CS.Data;
using Ara3D.RevitSampleBrowser.NewHostedSweep.CS.Geom;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace Ara3D.RevitSampleBrowser.NewHostedSweep.CS.Creators
{
    /// <summary>
    ///     Provides functions to create hosted sweep and preserves available edges and type.
    ///     It is the base class of FasciaCreator, GutterCreator, and SlabEdgeCreator.
    /// </summary>
    public abstract class HostedSweepCreator
    {
        /// <summary>
        ///     List of Modification to store all the created hosted-sweep by this.
        /// </summary>
        private readonly List<ModificationData> m_createdHostedSweeps;

        /// <summary>
        ///     Dictionary to store element's geometry which this creator can be used.
        /// </summary>
        protected readonly Dictionary<Element, ElementGeometry> ElemGeom;

        /// <summary>
        ///     Revit active document.
        /// </summary>
        protected readonly Document RvtDoc;

        /// <summary>
        ///     Revit UI document.
        /// </summary>
        protected readonly UIDocument RvtUiDoc;

        /// <summary>
        ///     Constructor which takes a Revit.Document as parameter.
        /// </summary>
        /// <param name="rvtDoc">Revit.Document parameter</param>
        protected HostedSweepCreator(UIDocument rvtDoc)
        {
            RvtUiDoc = rvtDoc;
            RvtDoc = rvtDoc.Document;
            ElemGeom = new Dictionary<Element, ElementGeometry>();
            m_createdHostedSweeps = new List<ModificationData>();
        }

        /// <summary>
        ///     A string indicates which type this creator can create.
        /// </summary>
        public virtual string Name => "Hosted Sweep";

        /// <summary>
        ///     A dictionary stores all the element=>edges which hosted-sweep can be created on.
        /// </summary>
        public abstract Dictionary<Element, List<Edge>> SupportEdges { get; }

        /// <summary>
        ///     All type of hosted-sweep.
        /// </summary>
        public abstract IEnumerable AllTypes { get; }

        /// <summary>
        ///     A dictionary stores all the element=>geometry which hosted-sweep can be created on.
        /// </summary>
        public Dictionary<Element, ElementGeometry> ElemGeomDic => ElemGeom;

        /// <summary>
        ///     A list to store all the created hosted-sweep by this creator.
        /// </summary>
        public List<ModificationData> CreatedHostedSweeps => m_createdHostedSweeps;

        /// <summary>
        ///     Revit active document.
        /// </summary>
        public Document RvtDocument => RvtDoc;

        /// <summary>
        ///     Revit UI document.
        /// </summary>
        public UIDocument RvtUiDocument => RvtUiDoc;

        /// <summary>
        ///     Create a hosted-sweep according to the CreationData parameter.
        /// </summary>
        /// <param name="creationData">CreationData parameter</param>
        /// <returns>ModificationData which contains the created hosted-sweep</returns>
        public ModificationData Create(CreationData creationData)
        {
            var refArr = new ReferenceArray();
            foreach (var edge in creationData.EdgesForHostedSweep)
            {
                refArr.Append(edge.Reference);
            }

            ModificationData modificationData = null;
            var transaction = new Transaction(RvtDoc, "CreateHostedSweep");
            try
            {
                transaction.Start();
                var createdHostedSweep = CreateHostedSweep(creationData.Symbol, refArr);

                if (transaction.Commit() == TransactionStatus.Committed)
                {
                    RvtUiDoc.ShowElements(createdHostedSweep);

                    // just only end transaction return true, we will create the hosted sweep.                    
                    modificationData =
                        new ModificationData(createdHostedSweep, creationData);
                    m_createdHostedSweeps.Add(modificationData);
                }
            }
            catch
            {
                transaction.RollBack();
            }

            return modificationData;
        }

        /// <summary>
        ///     Create a hosted-sweep according to the given Symbol and ReferenceArray.
        /// </summary>
        /// <param name="symbol">Hosted-sweep Symbol</param>
        /// <param name="refArr">Hosted-sweep ReferenceArray</param>
        /// <returns>Created hosted-sweep</returns>
        protected abstract HostedSweep CreateHostedSweep(ElementType symbol, ReferenceArray refArr);

        /// <summary>
        ///     Extract the geometry of the given Element.
        /// </summary>
        /// <param name="elem">Element parameter</param>
        /// <returns>Element's geometry</returns>
        protected ElementGeometry ExtractGeom(Element elem)
        {
            Solid result = null;
            var options = new Options
            {
                ComputeReferences = true
            };
            var gElement = elem.get_Geometry(options);
            //foreach (GeometryObject gObj in gElement.Objects)
            var objects = gElement.GetEnumerator();
            while (objects.MoveNext())
            {
                var gObj = objects.Current;

                result = gObj as Solid;
                if (result != null && result.Faces.Size > 0)
                    break;
            }

            var box = elem.get_BoundingBox(null);
            return new ElementGeometry(result, box);
        }
    }
}

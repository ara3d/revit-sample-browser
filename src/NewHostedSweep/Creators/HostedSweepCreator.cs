// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using Ara3D.RevitSampleBrowser.NewHostedSweep.CS.Data;
using Ara3D.RevitSampleBrowser.NewHostedSweep.CS.Geom;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System.Collections;
using System.Collections.Generic;

namespace Ara3D.RevitSampleBrowser.NewHostedSweep.CS.Creators
{
    /// <summary>
    ///     Provides functions to create hosted sweep and preserves available edges and type.
    ///     It is the base class of FasciaCreator, GutterCreator, and SlabEdgeCreator.
    /// </summary>
    public abstract class HostedSweepCreator
    {

        /// <summary>
        ///     Dictionary to store element's geometry which this creator can be used.
        /// </summary>
        protected readonly Dictionary<Element, ElementGeometry> ElemGeom;

        protected readonly Document RvtDoc;

        protected readonly UIDocument RvtUiDoc;

        /// <summary>
        ///     Constructor which takes a Revit.Document as parameter.
        /// </summary>
        /// <param name="rvtDoc">Revit.Document parameter</param>
        protected HostedSweepCreator(UIDocument rvtDoc)
        {
            RvtUiDoc = rvtDoc;
            RvtDoc = rvtDoc.Document;
            ElemGeom = [];
            CreatedHostedSweeps = [];
        }

        /// <summary>
        ///     A string indicates which type this creator can create.
        /// </summary>
        public virtual string Name => "Hosted Sweep";

        /// <summary>
        ///     A dictionary stores all the element=>edges which hosted-sweep can be created on.
        /// </summary>
        public abstract Dictionary<Element, List<Edge>> SupportEdges { get; }

        public abstract IEnumerable AllTypes { get; }

        /// <summary>
        ///     A dictionary stores all the element=>geometry which hosted-sweep can be created on.
        /// </summary>
        public Dictionary<Element, ElementGeometry> ElemGeomDic => ElemGeom;

        /// <summary>
        ///     A list to store all the created hosted-sweep by this creator.
        /// </summary>
        public List<ModificationData> CreatedHostedSweeps { get; }

        public Document RvtDocument => RvtDoc;

        public UIDocument RvtUiDocument => RvtUiDoc;

        public ModificationData Create(CreationData creationData)
        {
            ReferenceArray refArr = new();
            foreach (var edge in creationData.EdgesForHostedSweep)
            {
                refArr.Append(edge.Reference);
            }

            ModificationData modificationData = null;
            Transaction transaction = new(RvtDoc, "CreateHostedSweep");
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
                    CreatedHostedSweeps.Add(modificationData);
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
            Options options = new()
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

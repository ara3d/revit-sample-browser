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

using System;
using System.Collections.Generic;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System.Collections;

namespace Revit.SDK.Samples.NewHostedSweep.CS
{
   /// <summary>
   /// Provides functions to create hosted sweep and preserves available edges and type.
   /// It is the base class of FasciaCreator, GutterCreator, and SlabEdgeCreator.
   /// </summary>
   public abstract class HostedSweepCreator
   {
      #region Public Interfaces

      /// <summary>
      /// A string indicates which type this creator can create.
      /// </summary>
      virtual public string Name => "Hosted Sweep";

      /// <summary>
      /// A dictionary stores all the element=>edges which hosted-sweep can be created on.
      /// </summary>
      public abstract Dictionary<Element, List<Edge>> SupportEdges
      {
         get;
      }

      /// <summary>
      /// All type of hosted-sweep.
      /// </summary>
      public abstract IEnumerable AllTypes
      {
         get;
      }

      /// <summary>
      /// A dictionary stores all the element=>geometry which hosted-sweep can be created on.
      /// </summary>
      public Dictionary<Element, ElementGeometry> ElemGeomDic => m_elemGeom;

      /// <summary>
      /// Create a hosted-sweep according to the CreationData parameter.
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
         var transaction = new Transaction(m_rvtDoc, "CreateHostedSweep");
         try
         {
            transaction.Start();
            var createdHostedSweep = CreateHostedSweep(creationData.Symbol, refArr);

            if (transaction.Commit() == TransactionStatus.Committed)
            {
               m_rvtUIDoc.ShowElements(createdHostedSweep);

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
      /// A list to store all the created hosted-sweep by this creator.
      /// </summary>
      public List<ModificationData> CreatedHostedSweeps => m_createdHostedSweeps;

      /// <summary>
      /// Revit active document.
      /// </summary>
      public Document RvtDocument => m_rvtDoc;

      /// <summary>
      /// Revit UI document.
      /// </summary>
      public UIDocument RvtUIDocument => m_rvtUIDoc;

      #endregion

      #region Fields and Constructor
      /// <summary>
      /// List of Modification to store all the created hosted-sweep by this.
      /// </summary>
      private List<ModificationData> m_createdHostedSweeps;

      /// <summary>
      /// Revit active document.
      /// </summary>
      protected Document m_rvtDoc;

      /// <summary>
      /// Revit UI document.
      /// </summary>
      protected UIDocument m_rvtUIDoc;

      /// <summary>
      /// Dictionary to store element's geometry which this creator can be used.
      /// </summary>
      protected Dictionary<Element, ElementGeometry> m_elemGeom;

      /// <summary>
      /// Constructor which takes a Revit.Document as parameter.
      /// </summary>
      /// <param name="rvtDoc">Revit.Document parameter</param>
      protected HostedSweepCreator(UIDocument rvtDoc)
      {
         m_rvtUIDoc = rvtDoc;
         m_rvtDoc = rvtDoc.Document;
         m_elemGeom = new Dictionary<Element, ElementGeometry>();
         m_createdHostedSweeps = new List<ModificationData>();
      }
      #endregion

      #region Protected Methods

      /// <summary>
      /// Create a hosted-sweep according to the given Symbol and ReferenceArray.
      /// </summary>
      /// <param name="symbol">Hosted-sweep Symbol</param>
      /// <param name="refArr">Hosted-sweep ReferenceArray</param>
      /// <returns>Created hosted-sweep</returns>
      protected abstract HostedSweep CreateHostedSweep(ElementType symbol, ReferenceArray refArr);

      /// <summary>
      /// Extract the geometry of the given Element.
      /// </summary>
      /// <param name="elem">Element parameter</param>
      /// <returns>Element's geometry</returns>
      protected ElementGeometry ExtractGeom(Element elem)
      {
         Solid result = null;
         var options = new Options();
         options.ComputeReferences = true;
         var gElement = elem.get_Geometry(options);
         //foreach (GeometryObject gObj in gElement.Objects)
         var Objects = gElement.GetEnumerator();
         while (Objects.MoveNext())
         {
            var gObj = Objects.Current;

            result = gObj as Solid;
            if (result != null && result.Faces.Size > 0)
               break;
         }
         var box = elem.get_BoundingBox(null);
         return new ElementGeometry(result, box);
      }
      #endregion
   }
}

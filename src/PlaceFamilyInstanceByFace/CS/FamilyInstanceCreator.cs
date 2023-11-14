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

using GeoInstance = Autodesk.Revit.DB.GeometryInstance;
using GeoElement = Autodesk.Revit.DB.GeometryElement;
using RevitElement = Autodesk.Revit.DB.Element;

namespace Revit.SDK.Samples.PlaceFamilyInstanceByFace.CS
{
   /// <summary>
   /// This is main data class for creating family Instance by face
   /// </summary>
   public class FamilyInstanceCreator
   {
      #region Fields
      // Revit document
      private UIDocument m_revitDoc;
      // Creation application
      private Autodesk.Revit.Creation.Application m_appCreator;
      // all face names
      private List<string> m_faceNameList = new List<string>();
      // all face instances
      private List<Face> m_faceList = new List<Face>();
      // all family symbols
      private List<FamilySymbol> m_familySymbolList = new List<FamilySymbol>();
      // all family symbol names
      private List<string> m_familySymbolNameList = new List<string>();
      // the index default family symbol in family list
      private int m_defaultIndex = -1;
      #endregion

      #region Properties
      /// <summary>
      /// Store the all face names, they will be displayed in a combo box
      /// </summary>
      public List<string> FaceNameList => m_faceNameList;

      /// <summary>
      /// Revit document
      /// </summary>
      public UIDocument RevitDoc => m_revitDoc;

      /// <summary>
      /// Store all face instances for convenience to create a face-based family instance 
      /// </summary>
      public List<Face> FaceList => m_faceList;

      /// <summary>
      /// Store all family symbol in current Revit document
      /// </summary>
      public List<FamilySymbol> FamilySymbolList => m_familySymbolList;

      /// <summary>
      /// Store all family symbol names
      /// </summary>
      public List<string> FamilySymbolNameList => m_familySymbolNameList;

      /// <summary>
      /// The index of default family symbol, will set it as default value when initializing UI 
      /// For based point, its name is "Point-based"
      /// For based line, its name is "Line-based"
      /// The prepared rfa files provide them 
      /// </summary>
      public int DefaultFamilySymbolIndex => m_defaultIndex;

      #endregion

      #region Constructor
      /// <summary>
      /// Constructor, Store the Revit application
      /// </summary>
      /// <param name="app"></param>
      public FamilyInstanceCreator(UIApplication app)
      {
         m_revitDoc = app.ActiveUIDocument;
         m_appCreator = app.Application.Create;
         if (!CheckSelectedElementSet())
         {
            throw new Exception("Please select an element with face geometry.");
         }
      }
      #endregion

      #region Public methods
      /// <summary>
      /// 1. Find all family symbols in current Revit document and store them
      /// 2. Find the index of default family symbol
      /// Point("Point-based"); Line("Line-based")
      /// </summary>
      public void CheckFamilySymbol(BasedType type)
      {
         m_defaultIndex = -1;
         m_familySymbolList.Clear();

         var familySymbolItor =
             new FilteredElementCollector(m_revitDoc.Document).OfClass(typeof(FamilySymbol)).GetElementIterator();

         var defaultSymbolName = string.Empty;
         switch (type)
         {
            case BasedType.Point:
               defaultSymbolName = "Point-based";
               break;
            case BasedType.Line:
               defaultSymbolName = "Line-based";
               break;
            default:
               break;
         }

         var hasDefaultSymbol = false;
         var ii = 0;

         while (familySymbolItor.MoveNext())
         {
            var symbol = (FamilySymbol)familySymbolItor.Current;
            if (null == symbol)
            {
               continue;
            }

            if (!hasDefaultSymbol && 0 == string.Compare(defaultSymbolName, symbol.Name))
            {
               hasDefaultSymbol = true;
               m_defaultIndex = ii;
            }

            // family symbol
            m_familySymbolList.Add(symbol);

            // family symbol name
            var familyCategoryname = string.Empty;
            if (null != symbol.Family.FamilyCategory)
            {
               familyCategoryname = symbol.Family.FamilyCategory.Name + " : ";
            }
            m_familySymbolNameList.Add(string.Format("{0}{1} : {2}"
                , familyCategoryname, symbol.Family.Name, symbol.Name));
            ii++;
         }

         if (!hasDefaultSymbol)
         {
            FamilySymbol loadedfamilySymbol = null;
            try
            {
               m_revitDoc.Document.LoadFamilySymbol(string.Format(@"{0}.rfa", defaultSymbolName)
                   , defaultSymbolName
                   , out loadedfamilySymbol);
            }
            catch (Exception)
            {
               TaskDialog.Show("Revit", "Can't load the prepared rfa.");
            }


            if (null == loadedfamilySymbol)
            {
               return;
            }
            m_familySymbolList.Add(loadedfamilySymbol);

            var familyCategoryname = string.Empty;
            if (null != loadedfamilySymbol.Family.FamilyCategory)
            {
               familyCategoryname = loadedfamilySymbol.Family.FamilyCategory.Name + ": ";
            }
            m_familySymbolNameList.Add(string.Format("{0}{1}: {2}"
                , familyCategoryname, loadedfamilySymbol.Family.Name, loadedfamilySymbol.Name));
            m_defaultIndex = m_familySymbolList.Count - 1;
         }

         return;
      }

      /// <summary>
      /// Create a based-point family instance by face
      /// </summary>
      /// <param name="locationP">the location point</param>
      /// <param name="directionP">the direction</param>
      /// <param name="faceIndex">the index of the selected face</param>
      /// <param name="familySymbolIndex">the index of the selected family symbol</param>
      /// <returns></returns>
      public bool CreatePointFamilyInstance(XYZ locationP, XYZ directionP, int faceIndex
          , int familySymbolIndex)
      {
         var face = m_faceList[faceIndex];

         if (!m_familySymbolList[familySymbolIndex].IsActive)
            m_familySymbolList[familySymbolIndex].Activate();

         var instance = m_revitDoc.Document.Create.NewFamilyInstance(face
             , locationP, directionP, m_familySymbolList[familySymbolIndex]);

         var instanceId = new List<ElementId>();
         instanceId.Add(instance.Id);
         m_revitDoc.Selection.SetElementIds(instanceId);
         return true;
      }

      /// <summary>
      /// Create a based-line family instance by face
      /// </summary>
      /// <param name="startP">the start point</param>
      /// <param name="endP">the end point</param>
      /// <param name="faceIndex">the index of the selected face</param>
      /// <param name="familySymbolIndex">the index of the selected family symbol</param>
      /// <returns></returns>
      public bool CreateLineFamilyInstance(XYZ startP, XYZ endP, int faceIndex
          , int familySymbolIndex)
      {
         var face = m_faceList[faceIndex];
         var projectedStartP = Project(face.Triangulate().Vertices as List<XYZ>, startP);
         var projectedEndP = Project(face.Triangulate().Vertices as List<XYZ>, endP);

         if (projectedStartP.IsAlmostEqualTo(projectedEndP))
         {
            return false;
         }

         var line = Line.CreateBound(projectedStartP, projectedEndP);
         if (!m_familySymbolList[familySymbolIndex].IsActive)
            m_familySymbolList[familySymbolIndex].Activate();
         var instance = m_revitDoc.Document.Create.NewFamilyInstance(face, line
             , m_familySymbolList[familySymbolIndex]);

         var instanceId = new List<ElementId>();
         instanceId.Add(instance.Id);
         m_revitDoc.Selection.SetElementIds(instanceId);
         return true;
      }

      /// <summary>
      /// Judge whether the selected elementSet has face geometry
      /// </summary>
      /// <returns>true is having face geometry, false is having no face geometry</returns>
      public bool CheckSelectedElementSet()
      {
         // judge whether an or more element is selected
         var es = new ElementSet();
         foreach (var elementId in m_revitDoc.Selection.GetElementIds())
         {
            es.Insert(m_revitDoc.Document.GetElement(elementId));
         }
         if (1 != es.Size)
         {
            return false;
         }

         m_faceList.Clear();
         m_faceNameList.Clear();

         // judge whether the selected element has face geometry
         foreach (var elemId in m_revitDoc.Selection.GetElementIds())
         {
            var elem = m_revitDoc.Document.GetElement(elemId);
            CheckSelectedElement(elem);
            break;
         }

         if (0 >= m_faceList.Count)
         {
            return false;
         }

         return true;
      }
      #endregion

      #region Private Methods
      /// <summary>
      /// Get the bounding box of a face, the BoundingBoxXYZ will be set in UI as default value
      /// </summary>
      /// <param name="indexFace">the index of face</param>
      /// <returns>the bounding box</returns>
      public BoundingBoxXYZ GetFaceBoundingBox(int indexFace)
      {
         var mesh = m_faceList[indexFace].Triangulate();

         var maxP = new XYZ(double.MinValue, double.MinValue, double.MinValue);
         var minP = new XYZ(double.MaxValue, double.MaxValue, double.MaxValue);
         foreach (var tempXYZ in mesh.Vertices)
         {

            minP = new XYZ(
                Math.Min(minP.X, tempXYZ.X),
                Math.Min(minP.Y, tempXYZ.Y),
                Math.Min(minP.Z, tempXYZ.Z));

            maxP = new XYZ(
                Math.Max(maxP.X, tempXYZ.X),
                Math.Max(maxP.Y, tempXYZ.Y),
                Math.Max(maxP.Z, tempXYZ.Z));
         }

         var retBounding = new BoundingBoxXYZ();
         retBounding.Max = maxP;
         retBounding.Min = minP;
         return retBounding;
      }

      /// <summary>
      /// Judge whether an element has face geometry
      /// </summary>
      /// <param name="elem">the element to be checked</param>
      /// <returns>true is having face geometry, false is having no face geometry</returns>
      private bool CheckSelectedElement(RevitElement elem)
      {
         if (null == elem)
         {
            return false;
         }
         var opts = new Options();
         opts.View = m_revitDoc.Document.ActiveView;
         opts.ComputeReferences = true;
         // Get geometry of the element
         var geoElement = elem.get_Geometry(opts);
         InquireGeometry(geoElement, elem);

         return true;
      }

      /// <summary>
      /// Inquire an geometry element to get all face instances
      /// </summary>
      /// <param name="geoElement">the geometry element</param>
      /// <param name="elem">the element, it provides the prefix of face name</param>
      /// <returns></returns>
      private bool InquireGeometry(GeoElement geoElement, RevitElement elem)
      {
         if (null == geoElement || null == elem)
         {
            return false;
         }

         //GeometryObjectArray geoArray = null;
         var Objects = geoElement.GetEnumerator();
         //if (null != geoElement && null != geoElement.Objects)
         if (null != geoElement && Objects.MoveNext())
         {
            //geoArray = geoElement.Objects;
         }
         else
         {
            return false;
         }

         Objects.Reset();
         //foreach (GeometryObject obj in geoArray)
         while (Objects.MoveNext())
         {
            var obj = Objects.Current;

            if (obj is GeoInstance)
            {
               var instance = (GeoInstance)obj;
               InquireGeometry(instance.SymbolGeometry, elem);
            }
            else if (!(obj is Solid))
            {
               // is not Solid instance
               continue;
            }

            // continue when obj is Solid instance
            var solid = obj as Solid;
            if (null == solid)
            {
               continue;
            }
            var faces = solid.Faces;
            if (faces.IsEmpty)
            {
               continue;
            }

            // get the face name list
            var category = string.Empty;
            if (null != elem.Category && null != elem.Name)
            {
               category = elem.Category.Name;
            }

            var ii = 0;
            foreach (Face tempFace in faces)
            {
               if (tempFace is PlanarFace)
               {
                  m_faceNameList.Add(
                      string.Format("{0} : {1} ({2})", category, elem.Name, ii));
                  m_faceList.Add(tempFace);
                  ii++;
               }
            }
         }
         return true;
      }

      /// <summary>
      /// Project a point on a face
      /// </summary>
      /// <param name="xyzArray">the face points, them fix a face </param>
      /// <param name="point">the point</param>
      /// <returns>the projected point on this face</returns>
      static private XYZ Project(List<XYZ> xyzArray, XYZ point)
      {
         var a = xyzArray[0] - xyzArray[1];
         var b = xyzArray[0] - xyzArray[2];
         var c = point - xyzArray[0];

         var normal = (a.CrossProduct(b));

         try
         {
            normal = normal.Normalize();
         }
         catch (Exception)
         {
            normal = XYZ.Zero;
         }

         var retProjectedPoint = point - (normal.DotProduct(c)) * normal;
         return retProjectedPoint;
      }
      #endregion

   }
}

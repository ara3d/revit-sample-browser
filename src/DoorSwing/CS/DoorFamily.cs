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

using System.Collections.Generic;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.DB.Structure;


namespace Revit.SDK.Samples.DoorSwing.CS
{
   /// <summary>
   /// Left/Right feature based on family's actual geometry and country's standard.
   /// </summary>
   public class DoorFamily
   {
      #region "Members"

      // door family
      Family m_family;
      // opening value of one of this family's door which neither flipped nor mirrored.
      string m_basalOpeningValue;
      // one door instance of this family.
      FamilyInstance m_oneInstance;
      // Revit application
      UIApplication m_app;
      // the geometry of one of this family's door which neither flipped nor mirrored.
      DoorGeometry m_geometry;

      #endregion

      #region "Properties"

      /// <summary>
      /// Retrieval the name of this family.
      /// </summary>
      public string FamilyName => m_family.Name;

      /// <summary>
      /// Retrieve opening value of one of this family's door which neither flipped nor mirrored.
      /// </summary>
      public string BasalOpeningValue
      {
         get
         {
            if (string.IsNullOrEmpty(m_basalOpeningValue))
            {
               var paramValue = DoorSwingResource.Undefined;

               // get current opening value.  
               var fss = new List<FamilySymbol>(); 
               foreach (var elementId in m_family.GetFamilySymbolIds())
               {
                  fss.Add((FamilySymbol)(m_app.ActiveUIDocument.Document.GetElement(elementId)));
               }
               var doorSymbol = fss[0]; 
               paramValue = doorSymbol.ParametersMap.get_Item("BasalOpening").AsString();

               // deal with invalid string.
               if (!DoorSwingData.OpeningTypes.Contains(paramValue))
               {
                  paramValue = DoorSwingResource.Undefined;
               }

               m_basalOpeningValue = paramValue;
            }

            return m_basalOpeningValue;
         }
         set => m_basalOpeningValue = value;
      }

      /// <summary>
      /// Retrieve the geometry of one door which belongs to this family and 
      /// neither flipped nor mirrored.
      /// </summary>
      public DoorGeometry Geometry
      {
         get
         {
            if (null == m_geometry)
            {
               // create one instance of DoorFamilyGeometry class.
               m_geometry = new DoorGeometry(m_oneInstance);
            }

            return m_geometry;
         }
      }

      #endregion

      #region "Methods"

      /// <summary>
      ///  construct function.
      /// </summary>
      /// <param name="doorFamily"> one door family</param>
      /// <param name="app">Revit application</param>
      public DoorFamily(Family doorFamily, UIApplication app)
      {
         m_app = app;
         m_family = doorFamily;
         // one door instance which belongs to this family and neither flipped nor mirrored.
         m_oneInstance = CreateOneInstanceWithThisFamily();
      }

      /// <summary>
      /// Update Left/Right feature based on family's actual geometry and country's standard.
      /// </summary>
      public void UpdateOpeningFeature()
      {
         // get current Left/Right feature's value of this door family.
         var ffs = new List<FamilySymbol>(); 
         foreach (var elementId in m_family.GetFamilySymbolIds())
         {
            ffs.Add((FamilySymbol)(m_app.ActiveUIDocument.Document.GetElement(elementId)));
         }
         foreach (var doorSymbol in ffs)
         {
            // update the the related family shared parameter's value if user already added it.
            if (doorSymbol.ParametersMap.Contains("BasalOpening"))
            {
               var basalOpeningParam = doorSymbol.ParametersMap.get_Item("BasalOpening");
               var setResult = basalOpeningParam.Set(m_basalOpeningValue);
            }
         }
      }

      /// <summary>
      /// Delete the temporarily created door instance and its host.
      /// </summary>
      public void DeleteTempDoorInstance()
      {
         var doc = m_app.ActiveUIDocument.Document;
         var tempWall = m_oneInstance.Host;
         doc.Delete(m_oneInstance.Id); // delete temporarily created door instance with this family.
         doc.Delete(tempWall.Id); // delete the door's host.
      }

      /// <summary>
      /// Create one temporary door instance with this family.
      /// </summary>
      /// <returns>the created door.</returns>
      private FamilyInstance CreateOneInstanceWithThisFamily()
      {
         var doc = m_app.ActiveUIDocument.Document;
         var creDoc = doc.Create;
         var creApp = m_app.Application.Create;

         // get one level. A project has at least one level.
         var level = new FilteredElementCollector(doc).OfClass(typeof(Level)).FirstElement() as Level;

         // create one wall as door's host
         var wallCurve = Line.CreateBound(new XYZ(0, 0, 0), new XYZ(100, 0, 0));
         var host = Wall.Create(doc, wallCurve, level.Id, false);
         doc.Regenerate();

         // door symbol.
         var ffs = new List<FamilySymbol>(); 
         foreach (var elementId in m_family.GetFamilySymbolIds())
         {
            ffs.Add((FamilySymbol)(m_app.ActiveUIDocument.Document.GetElement(elementId)));
         }
         var doorSymbol = ffs[0];

         // create the door
         var createdFamilyInstance = creDoc.NewFamilyInstance(new XYZ(0, 0, 0), doorSymbol, host, level,
                                                StructuralType.NonStructural);
         doc.Regenerate();

         return createdFamilyInstance;
      }

      #endregion
   }
}

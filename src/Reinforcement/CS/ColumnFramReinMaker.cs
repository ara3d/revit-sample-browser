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
using System.Windows.Forms;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.DB.Structure;

namespace Revit.SDK.Samples.Reinforcement.CS
{
   /// <summary>
   /// The class derived form FramReinMaker showes how to create the rebars for a column
   /// </summary>
   public class ColumnFramReinMaker : FramReinMaker
   {
      
      ColumnGeometrySupport m_geometry; // The geometry support for column rebar creation

      double m_transverseEndSpacing;    //the space value of end transverse rebar
      double m_transverseCenterSpacing; //the space value of center transverse rebar
      int m_verticalRebarNumber;        //the number of the vertical rebar

      
      
      /// <summary>
      /// get and set the type of the end transverse rebar
      /// </summary>
      public RebarBarType TransverseEndType { get; set; }

      /// <summary>
      /// get and set the type of the center transverse rebar
      /// </summary>
      public RebarBarType TransverseCenterType { get; set; }

      /// <summary>
      /// get and set the type of the vertical rebar
      /// </summary>
      public RebarBarType VerticalRebarType { get; set; }

      /// <summary>
      /// get and set the space value of end transverse rebar
      /// </summary>
      public double TransverseEndSpacing
      {
         get => m_transverseEndSpacing;
         set
         {
            if (0 > value)  // spacing data must be above 0
            {
               throw new Exception("Transverse end spacing should be above zero");
            }
            m_transverseEndSpacing = value;
         }
      }

      /// <summary>
      /// get and set the space value of center transverse rebar
      /// </summary>
      public double TransverseCenterSpacing
      {
         get => m_transverseCenterSpacing;
         set
         {
            if (0 > value)  // spacing data must be above 0
            {
               throw new Exception("Transverse center spacing should be above zero");
            }
            m_transverseCenterSpacing = value;
         }
      }

      /// <summary>
      /// get and set the number of vertical rebar
      /// </summary>
      public int VerticalRebarNumber
      {
         get => m_verticalRebarNumber;
         set
         {
            if (4 > value)  // vertical rebar number must be above 3
            {
               throw new Exception("The minimum of vertical rebar number shouble be four.");
            }
            m_verticalRebarNumber = value;
         }
      }

      /// <summary>
      /// get and set the hook type of transverse rebar
      /// </summary>
      public RebarHookType TransverseHookType { get; set; }

      
            /// <summary>
      /// Constructor of the ColumnFramReinMaker
      /// </summary>
      /// <param name="commandData">the ExternalCommandData reference</param>
      /// <param name="hostObject">the host column</param>
      public ColumnFramReinMaker(ExternalCommandData commandData, FamilyInstance hostObject)
         : base(commandData, hostObject)
      {
         //create a new options for current project
         var geoOptions = commandData.Application.Application.Create.NewGeometryOptions();
         geoOptions.ComputeReferences = true;

         //create a ColumnGeometrySupport instance 
         m_geometry = new ColumnGeometrySupport(hostObject, geoOptions);
      }
      
      
      /// <summary>
      /// Override method to do some further checks
      /// </summary>
      /// <returns>true if the the data is right and enough, otherwise false.</returns>
      protected override bool AssertData()
      {
         return base.AssertData();
      }

      /// <summary>
      /// Display a form to collect the information for column reinforcement creation
      /// </summary>
      /// <returns>true if the informatin collection is successful, otherwise false</returns>
      protected override bool DisplayForm()
      {
         // Display ColumnFramReinMakerForm for the user input information 
         using (var displayForm = new ColumnFramReinMakerForm(this))
         {
            if (DialogResult.OK != displayForm.ShowDialog())
            {
               return false;
            }
         }
         return base.DisplayForm();
      }

      /// <summary>
      /// Override method to create rebars on the selected column
      /// </summary>
      /// <returns>true if the creation is successful, otherwise false.</returns>
      protected override bool FillWithBars()
      {
         // create the transverse rebars
         var flag = FillTransverseBars();

         // create the vertical rebars
         flag = flag && FillVerticalBars();

         return base.FillWithBars();
      }

      
      /// <summary>
      /// create the transverse rebars for the column
      /// </summary>
      /// <returns>true if the creation is successful, otherwise false</returns>
      public bool FillTransverseBars()
      {
         // create all kinds of transverse rebars according to the TransverseRebarLocation
         foreach (TransverseRebarLocation location in Enum.GetValues(
                                                     typeof(TransverseRebarLocation)))
         {
            var createdRebar = FillTransverseBar(location);
            //judge whether the transverse rebar creation is successful
            if (null == createdRebar)
            {
               return false;
            }
         }

         return true;
      }

      /// <summary>
      /// Create the transverse rebars, according to the transverse rebar location
      /// </summary>
      /// <param name="location">location of rebar which need to be created</param>
      /// <returns>the created rebar, return null if the creation is unsuccessful</returns>
      public Rebar FillTransverseBar(TransverseRebarLocation location)
      {
         // Get the geometry information which support rebar creation
         var geomInfo = new RebarGeometry();
         RebarBarType barType = null;
         switch (location)
         {
            case TransverseRebarLocation.Start: // start transverse rebar
            case TransverseRebarLocation.End:   // end transverse rebar
               geomInfo = m_geometry.GetTransverseRebar(location, m_transverseEndSpacing);
               barType = TransverseEndType;
               break;
            case TransverseRebarLocation.Center:// center transverse rebar   
               geomInfo = m_geometry.GetTransverseRebar(location, m_transverseCenterSpacing);
               barType = TransverseCenterType;
               break;
            default:
               break;
         }

         // create the rebar
         return PlaceRebars(barType, TransverseHookType, TransverseHookType,
                                         geomInfo, RebarHookOrientation.Right, RebarHookOrientation.Left);
      }


      /// <summary>
      /// Create the vertical rebar according the location
      /// </summary>
      /// <param name="location">location of rebar which need to be created</param>
      /// <returns>the created rebar, return null if the creation is unsuccessful</returns>
      public Rebar FillVerticalBar(VerticalRebarLocation location)
      {
         //calculate the rebar number in different location
         var rebarNubmer = m_verticalRebarNumber / 4;
         switch (location)
         {
            case VerticalRebarLocation.East:    // the east vertical rebar
               if (0 < m_verticalRebarNumber % 4)
               {
                  rebarNubmer++;
               }
               break;
            case VerticalRebarLocation.North:   // the north vertical rebar
               if (2 < m_verticalRebarNumber % 4)
               {
                  rebarNubmer++;
               }
               break;
            case VerticalRebarLocation.West:    // the west vertical rebar
               if (1 < m_verticalRebarNumber % 4)
               {
                  rebarNubmer++;
               }
               break;
            case VerticalRebarLocation.South:   // the south vertical rebar
               break;
         }

         // get the geometry information for rebar creation
         var geomInfo = m_geometry.GetVerticalRebar(location, rebarNubmer);

         // create the rebar
         return PlaceRebars(VerticalRebarType, null, null, geomInfo,
                                     RebarHookOrientation.Left, RebarHookOrientation.Left);
      }


      /// <summary>
      /// create the all the vertial rebar
      /// </summary>
      /// <returns>true if the creation is successful, otherwise false</returns>
      private bool FillVerticalBars()
      {
         // create all kinds of vertical rebars according to the VerticalRebarLocation
         foreach (VerticalRebarLocation location in Enum.GetValues(
                                             typeof(VerticalRebarLocation)))
         {
            var createdRebar = FillVerticalBar(location);
            //judge whether the vertical rebar creation is successful
            if (null == createdRebar)
            {
               return false;
            }
         }

         return true;
      }
   }
}

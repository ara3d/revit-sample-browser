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
using System.Collections;
using System.Windows.Forms;
using System.Linq;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.DB.Structure;

namespace Revit.SDK.Samples.CreateWallsUnderBeams.CS
{
    /// <summary>
    /// Implements the Revit add-in interface IExternalCommand
    /// </summary>
    [Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]
    [Autodesk.Revit.Attributes.Regeneration(Autodesk.Revit.Attributes.RegenerationOption.Manual)]
    [Autodesk.Revit.Attributes.Journaling(Autodesk.Revit.Attributes.JournalingMode.NoCommandData)]
    public class CreateWallsUnderBeams : IExternalCommand
    {
        // Private Members
        IList<WallType> m_wallTypeCollection;         // Store all the wall types in current document
        ArrayList m_beamCollection;             // Store the selection of beams in Revit
        WallType m_selectedWallType;            // Store the selected wall type
        Level m_level;                          // Store the level which wall create on
        bool m_isStructural;                 // Indicate whether create structural walls
        string m_errorInformation;              // Store the error information
        const double PRECISION = 0.0000000001;  // Define a precision of double data

        // Properties
        /// <summary>
        /// Inform all the wall types can be created in current document
        /// </summary>
        public IList<WallType> WallTypes => m_wallTypeCollection;

        /// <summary>
        /// Inform the wall type selected by the user
        /// </summary>
        public object SelectedWallType
        {
            set => m_selectedWallType = value as WallType;
        }

        /// <summary>
        /// Inform whether the user want to create structural or architecture walls
        /// </summary>
        public bool IsSturctual
        {
            get => m_isStructural;
            set => m_isStructural = value;
        }

        // Methods
        /// <summary>
        /// Default constructor of CreateWallsUnderBeams
        /// </summary>
        public CreateWallsUnderBeams()
        {
            m_wallTypeCollection = new List<WallType>();
            m_beamCollection = new ArrayList();
            m_isStructural = true;
        }

        #region IExternalCommand Members Implementation
        
        public Result Execute(ExternalCommandData commandData,
                                                    ref string message, ElementSet elements)
        {
            var revit = commandData.Application;
            var project = revit.ActiveUIDocument;

            // Find the selection of beams in Revit
            var selection = new ElementSet();
            foreach (var elementId in project.Selection.GetElementIds())
            {
               selection.Insert(project.Document.GetElement(elementId));
            }
            foreach (Element e in selection)
            {
                var m = e as FamilyInstance;
                if (null != m)
                {
                    if (StructuralType.Beam == m.StructuralType)
                    {
                        // Store all the beams the user selected in Revit
                        m_beamCollection.Add(e);
                    }
                }
            }
            if (0 == m_beamCollection.Count)
            {
                message = "Can not find any beams.";
                return Result.Failed;
            }

            // Make sure all the beams have horizontal analytical line
            if (!CheckBeamHorizontal())
            {
                message = m_errorInformation;
                return Result.Failed;
            }

            // Search all the wall types in the Revit
            var filteredElementCollector = new FilteredElementCollector(project.Document);
            filteredElementCollector.OfClass(typeof(WallType));
            m_wallTypeCollection = filteredElementCollector.Cast<WallType>().ToList<WallType>();

            // Show the dialog for the user select the wall style
            using (var displayForm = new CreateWallsUnderBeamsForm(this))
            {
                if (DialogResult.OK != displayForm.ShowDialog())
                {
                    return Result.Failed;
                }
            }

            // Create the walls which along and under the path of the beams.
            if (!BeginCreate(project.Document))
            {
                message = m_errorInformation;
                return Result.Failed;
            }

            // If everything goes right, return succeeded.
            return Result.Succeeded;
        }
      #endregion IExternalCommand Members Implementation

      /// <summary>
      /// Create the walls which along and under the path of the selected beams
      /// </summary>
      /// <param name="project"> A reference of current document</param>
      /// <returns>true if there is no error in process; otherwise, false.</returns>
      bool BeginCreate(Document project)
      {
         // Begin to create walls along and under each beam
         for (var i = 0; i < m_beamCollection.Count; i++)
         {
            // Get each selected beam.
            var m = m_beamCollection[i] as FamilyInstance;
            if (null == m)
            {
               m_errorInformation = "The program should not go here.";
               return false;
            }

            // the wall will be created using beam's model line as path.   
            if (!(m.Location is LocationCurve))
            {
               m_errorInformation = "The beam should have location curve.";
               return false;
            }
            var beamCurve = (m.Location as LocationCurve).Curve;

            // Get the level using the beam's reference level
            var levelId = m.get_Parameter(BuiltInParameter.INSTANCE_REFERENCE_LEVEL_PARAM).AsElementId();
            m_level = project.GetElement(levelId) as Level;
            if (null == m_level)
            {
               m_errorInformation = "The program should not go here.";
               return false;
            }

            var t = new Transaction(project, Guid.NewGuid().GetHashCode().ToString());
            t.Start();
            var createdWall = Wall.Create(project, beamCurve, m_selectedWallType.Id,
                                            m_level.Id, 10, 0, true, m_isStructural);
            if (null == createdWall)
            {
               m_errorInformation = "Can not create the walls";
               return false;
            }

            // Modify some parameters of the created wall to make it look better.
            var offset = beamCurve.GetEndPoint(0).Z - m_level.Elevation;
            createdWall.get_Parameter(BuiltInParameter.WALL_BASE_CONSTRAINT).Set(levelId);
            createdWall.get_Parameter(BuiltInParameter.WALL_BASE_OFFSET).Set(offset - 3000 / 304.8);
            createdWall.get_Parameter(BuiltInParameter.WALL_HEIGHT_TYPE).Set(levelId);
            t.Commit();
         }
         return true;
      }


      /// <summary>
      /// Check whether all the beams have horizontal analytical line 
      /// </summary>
      /// <returns>true if each beam has horizontal analytical line; otherwise, false.</returns>
      bool CheckBeamHorizontal()
      {
         for (var i = 0; i < m_beamCollection.Count; i++)
         {
            // Get the analytical curve of each selected beam.
            // And check if Z coordinate of start point and end point of the curve are same.
            var m = m_beamCollection[i] as FamilyInstance;
            var beamCurve = m.Location is LocationCurve ? (m.Location as LocationCurve).Curve : null;
            if (null == beamCurve)
            {
               m_errorInformation = "The beam should have location curve.";
               return false;
            }
            else if ((PRECISION <= beamCurve.GetEndPoint(0).Z - beamCurve.GetEndPoint(1).Z)
                || (-PRECISION >= beamCurve.GetEndPoint(0).Z - beamCurve.GetEndPoint(1).Z))
            {
               m_errorInformation = "Please only select horizontal beams.";
               return false;
            }
         }
         return true;
      }
   }
}
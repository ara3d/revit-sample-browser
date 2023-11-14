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
using System.Linq;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace Revit.Samples.DirectionCalculation
{
    /// <summary>
    /// Implementation class for utilities to find south facing exterior walls in a project.
    /// </summary>
    /// 
      [Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]
   [Autodesk.Revit.Attributes.Regeneration(Autodesk.Revit.Attributes.RegenerationOption.Manual)]
      [Autodesk.Revit.Attributes.Journaling(Autodesk.Revit.Attributes.JournalingMode.NoCommandData)]

   public class FindSouthFacingWalls : FindSouthFacingBase
    {
        /// <summary>
        /// The implementation of the FindSouthFacingWalls command.
        /// </summary>
        /// <param name="useProjectLocationNorth">true to use the active project location's north/south direction.  
        /// false to use the default coordinate system's north/south (Y-axis).</param>
        protected void Execute(bool useProjectLocationNorth)
        {
           var uiDoc = new UIDocument(Document);
           var selElements = new ElementSet();
  
            foreach (var elementId in uiDoc.Selection.GetElementIds())
            {
               selElements.Insert(uiDoc.Document.GetElement(elementId));
            }

            var walls = CollectExteriorWalls();
            foreach (var wall in walls)
            {
                var exteriorDirection = GetExteriorWallDirection(wall);

                if (useProjectLocationNorth)
                    exteriorDirection = TransformByProjectLocation(exteriorDirection);

                var isSouthFacing = IsSouthFacing(exteriorDirection);
                if (isSouthFacing)
                    selElements.Insert(wall);
            }

            // Select all walls which had the proper direction.
            var elemIdList = new List<ElementId>();
            foreach (Element element in selElements)
            {
               elemIdList.Add(element.Id);
            }
            uiDoc.Selection.SetElementIds(elemIdList);
        }

        /// <summary>
        /// Finds all exterior walls in the active document.
        /// </summary>
        /// <returns>An enumerable containing exterior walls.</returns>
        protected IEnumerable<Wall> CollectExteriorWalls()
        {
            var collector = new FilteredElementCollector(Document);
            var elementsToProcess = collector.OfClass(typeof(Wall)).ToElements();
            // Use a LINQ query to filter out only Exterior walls
            var exteriorWalls = from wall in elementsToProcess.Cast<Wall>()
                                              where IsExterior(Document.GetElement(wall.GetTypeId()) as ElementType)
                                              select wall;
            return exteriorWalls;
        }

        /// <summary>
        /// Test method to see if the wall type is exterior.
        /// </summary>
        /// <param name="wallType">The wall type.</param>
        /// <returns>true if the wall is exterior, else false.</returns>
        protected bool IsExterior(ElementType wallType)
        {
            var wallFunction = wallType.get_Parameter(BuiltInParameter.FUNCTION_PARAM);

            var value = (WallFunction)wallFunction.AsInteger();

            return (value == WallFunction.Exterior);
        }

        /// <summary>
        /// Obtains the outward direction of the exterior wall.
        /// </summary>
        /// <param name="wall">The wall.</param>
        /// <returns>A normalized XYZ direction vector.</returns>
        protected XYZ GetExteriorWallDirection(Wall wall)
        {
            var locationCurve = wall.Location as LocationCurve;
            var exteriorDirection = XYZ.BasisZ;

            if (locationCurve != null)
            {
                var curve = locationCurve.Curve;

                //Write("Wall line endpoints: ", curve);

                var direction = XYZ.BasisX;
                if (curve is Line)
                {
                    // Obtains the tangent vector of the wall.
                    direction = curve.ComputeDerivatives(0, true).BasisX.Normalize();
                }
                else
                {
                    // An assumption, for non-linear walls, that the "tangent vector" is the direction
                    // from the start of the wall to the end.
                    direction = (curve.GetEndPoint(1) - curve.GetEndPoint(0)).Normalize();
                }
                // Calculate the normal vector via cross product.
                exteriorDirection = XYZ.BasisZ.CrossProduct(direction);

                // Flipped walls need to reverse the calculated direction
                if (wall.Flipped) exteriorDirection = -exteriorDirection;
            }

            return exteriorDirection;
        }


    }
}

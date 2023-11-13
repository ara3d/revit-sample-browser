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
using System.Linq;
using System.Windows.Forms;
using System.Collections;
using System.Collections.Generic;

using Autodesk.Revit;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;


namespace Revit.SDK.Samples.GeometryCreation_BooleanOperation.CS
{
    /// <summary>
    /// Implements the Revit add-in interface IExternalCommand
    /// </summary>
    [Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]
    [Autodesk.Revit.Attributes.Journaling(Autodesk.Revit.Attributes.JournalingMode.NoCommandData)]
    public class Command : IExternalCommand
    {
        /// <summary>
        /// Implement this method as an external command for Revit.
        /// </summary>
        /// <param name="commandData">An object that is passed to the external application 
        /// which contains data related to the command, 
        /// such as the application object and active view.</param>
        /// <param name="message">A message that can be set by the external application 
        /// which will be displayed if a failure or cancellation is returned by 
        /// the external command.</param>
        /// <param name="elements">A set of elements to which the external application 
        /// can add elements that are to be highlighted in case of failure or cancellation.</param>
        /// <returns>Return the status of the external command. 
        /// A result of Succeeded means that the API external method functioned as expected. 
        /// Cancelled can be used to signify that the user cancelled the external operation 
        /// at some point. Failure should be returned if the application is unable to proceed with 
        /// the operation.</returns>
        public virtual Result Execute(ExternalCommandData commandData
            , ref string message, ElementSet elements)
        {
            try
            {
                var document = commandData.Application.ActiveUIDocument.Document;

                // Create a new transaction
                var tran = new Transaction(document, "GeometryCreation_BooleanOperation");
                tran.Start();

                // Create an object that is responsible for creating the solids
                var geometryCreation = GeometryCreation.getInstance(commandData.Application.Application);

                // Create an object that is responsible for displaying the solids
                var AVF = AnalysisVisualizationFramework.getInstance(document);

                // Create a CSG tree solid
                CSGTree(geometryCreation, AVF);

                tran.Commit();

                // Set the view which display the solid active
                commandData.Application.ActiveUIDocument.ActiveView = 
                   (((new FilteredElementCollector(document).
                   OfClass(typeof(Autodesk.Revit.DB.View))).
                   Cast<Autodesk.Revit.DB.View>()).
                   Where(e => e.Name == "CSGTree")).
                   First<Autodesk.Revit.DB.View>();

                return Result.Succeeded;
            }
            catch (Exception ex)
            {
                message = ex.Message;
                return Result.Failed;
            }
        }

        /// <summary>
        /// Prepare 5 solids materials for CSG tree
        /// </summary>
        /// <param name="geometrycreation">The object that is responsible for creating the solids</param>
        /// <returns>The solids materials list</returns>
        private List<Solid> prepareSolids(GeometryCreation geometrycreation)
        {
           var resultSolids = new List<Solid>();

           resultSolids.Add(geometrycreation.CreateCenterbasedBox(Autodesk.Revit.DB.XYZ.Zero, 25));

           resultSolids.Add(geometrycreation.CreateCenterbasedSphere(Autodesk.Revit.DB.XYZ.Zero, 20));

           resultSolids.Add(geometrycreation.CreateCenterbasedCylinder(Autodesk.Revit.DB.XYZ.Zero, 5, 40, 
              GeometryCreation.CylinderDirection.BasisX));

           resultSolids.Add(geometrycreation.CreateCenterbasedCylinder(Autodesk.Revit.DB.XYZ.Zero, 5, 40, 
              GeometryCreation.CylinderDirection.BasisY));

           resultSolids.Add(geometrycreation.CreateCenterbasedCylinder(Autodesk.Revit.DB.XYZ.Zero, 5, 40, 
              GeometryCreation.CylinderDirection.BasisZ));

           return resultSolids;
        }

        /// <summary>
        /// Create a constructive solid geometry - CSG tree
        /// http://en.wikipedia.org/wiki/Constructive_solid_geometry
        /// http://en.wikipedia.org/wiki/File:Csg_tree.png
        /// </summary>
        /// <param name="geometrycreation">The object that is responsible for creating the solids</param>
        /// <param name="avf">The object that is responsible for displaying the solids</param>
        private void CSGTree(GeometryCreation geometrycreation, AnalysisVisualizationFramework avf)
        {
           var materialSolids = prepareSolids(geometrycreation);

           // Operation 1 : Intersect
           var CSGTree_solid1 = BooleanOperation.BooleanOperation_Intersect(materialSolids[0], materialSolids[1]);

           // Operation 2 : Union
           var CSGTree_solid2 = BooleanOperation.BooleanOperation_Union(materialSolids[2], materialSolids[3]);
           // Operation 3 : Union
           BooleanOperation.BooleanOperation_Union(ref CSGTree_solid2, materialSolids[4]);

           // Operation 4 : Difference
           BooleanOperation.BooleanOperation_Difference(ref CSGTree_solid1, CSGTree_solid2);

           avf.PaintSolid(CSGTree_solid1, "CSGTree");

        }
    }
}


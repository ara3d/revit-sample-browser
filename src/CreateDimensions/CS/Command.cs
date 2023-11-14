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
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Structure;
using Autodesk.Revit.UI;

namespace Revit.SDK.Samples.CreateDimensions.CS
{
    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    [Journaling(JournalingMode.NoCommandData)]
    public class Command : IExternalCommand
    {
        private const double precision = 0.0000001; //store the precision   
        private string m_errorMessage = " "; // store error message
        private ExternalCommandData m_revit; //store external command
        private readonly ArrayList m_walls = new ArrayList(); //store the wall of selected

        public Result Execute(ExternalCommandData revit, ref string message, ElementSet elements)
        {
            try
            {
                m_revit = revit;
                var view = m_revit.Application.ActiveUIDocument.Document.ActiveView;
                var view3D = view as View3D;
                if (null != view3D)
                {
                    message += "Only create dimensions in 2D";
                    return Result.Failed;
                }

                var viewSheet = view as ViewSheet;
                if (null != viewSheet)
                {
                    message += "Only create dimensions in 2D";
                    return Result.Failed;
                }

                //try too adds a dimension from the start of the wall to the end of the wall into the project
                if (!AddDimension())
                {
                    message = m_errorMessage;
                    return Result.Failed;
                }

                return Result.Succeeded;
            }
            catch (Exception e)
            {
                message = e.Message;
                return Result.Failed;
            }
        }

        /// <summary>
        ///     find out the wall, insert it into a array list
        /// </summary>
        private bool initialize()
        {
            var selections = new ElementSet();
            foreach (var elementId in m_revit.Application.ActiveUIDocument.Selection.GetElementIds())
                selections.Insert(m_revit.Application.ActiveUIDocument.Document.GetElement(elementId));
            //nothing was selected
            if (0 == selections.Size)
            {
                m_errorMessage += "Please select Basic walls";
                return false;
            }

            //find out wall
            foreach (Element e in selections)
            {
                var wall = e as Wall;
                if (null != wall)
                {
                    if ("Basic" != wall.WallType.Kind.ToString()) continue;
                    m_walls.Add(wall);
                }
            }

            //no wall was selected
            if (0 == m_walls.Count)
            {
                m_errorMessage += "Please select Basic walls";
                return false;
            }

            return true;
        }

        /// <summary>
        ///     find out every wall in the selection and add a dimension from the start of the wall to its end
        /// </summary>
        /// <returns>if add successfully, true will be returned, else false will be returned</returns>
        public bool AddDimension()
        {
            if (!initialize()) return false;

            var transaction = new Transaction(m_revit.Application.ActiveUIDocument.Document, "Add Dimensions");
            transaction.Start();
            //get out all the walls in this array, and create a dimension from its start to its end
            for (var i = 0; i < m_walls.Count; i++)
            {
                var wallTemp = m_walls[i] as Wall;
                if (null == wallTemp) continue;

                //get location curve
                var location = wallTemp.Location;
                var locationline = location as LocationCurve;
                if (null == locationline) continue;

                //New Line

                Line newLine = null;

                //get reference
                var referenceArray = new ReferenceArray();

                AnalyticalPanel analyticalModel = null;
                var document = wallTemp.Document;
                var assocManager =
                    AnalyticalToPhysicalAssociationManager.GetAnalyticalToPhysicalAssociationManager(document);
                if (assocManager != null)
                {
                    var associatedElementId = assocManager.GetAssociatedElementId(wallTemp.Id);
                    if (associatedElementId != ElementId.InvalidElementId)
                    {
                        var associatedElement = document.GetElement(associatedElementId);
                        if (associatedElement != null && associatedElement is AnalyticalPanel)
                            analyticalModel = associatedElement as AnalyticalPanel;
                    }
                }

                IList<Curve> activeCurveList = analyticalModel.GetOuterContour().ToList();
                foreach (var aCurve in activeCurveList)
                {
                    // find non-vertical curve from analytical model
                    if (aCurve.GetEndPoint(0).Z == aCurve.GetEndPoint(1).Z)
                        newLine = aCurve as Line;
                    if (aCurve.GetEndPoint(0).Z != aCurve.GetEndPoint(1).Z)
                    {
                        var amSelector = new AnalyticalModelSelector(aCurve);
                        amSelector.CurveSelector = AnalyticalCurveSelector.StartPoint;

                        referenceArray.Append(analyticalModel.GetReference(amSelector));
                    }

                    if (2 == referenceArray.Size)
                        break;
                }

                if (referenceArray.Size != 2)
                {
                    m_errorMessage += "Did not find two references";
                    return false;
                }

                try
                {
                    //try to add new a dimension
                    var app = m_revit.Application;
                    var doc = app.ActiveUIDocument.Document;

                    var p1 = new XYZ(
                        newLine.GetEndPoint(0).X + 5,
                        newLine.GetEndPoint(0).Y + 5,
                        newLine.GetEndPoint(0).Z);
                    var p2 = new XYZ(
                        newLine.GetEndPoint(1).X + 5,
                        newLine.GetEndPoint(1).Y + 5,
                        newLine.GetEndPoint(1).Z);

                    var newLine2 = Line.CreateBound(p1, p2);
                    doc.Create.NewDimension(
                        doc.ActiveView, newLine2, referenceArray);
                }
                // catch the exceptions
                catch (Exception ex)
                {
                    m_errorMessage += ex.ToString();
                    return false;
                }
            }

            transaction.Commit();
            return true;
        }
    }
}
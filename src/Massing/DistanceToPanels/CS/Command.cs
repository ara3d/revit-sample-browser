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

namespace Revit.SDK.Samples.DistanceToPanels.CS
{
    /// <summary>
    /// Implements the Revit add-in interface IExternalCommand.
    /// This class shows how to compute the distance from divided surface panels to a user-specified point
    /// </summary>
    [Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]
    [Autodesk.Revit.Attributes.Regeneration(Autodesk.Revit.Attributes.RegenerationOption.Manual)]
    [Autodesk.Revit.Attributes.Journaling(Autodesk.Revit.Attributes.JournalingMode.NoCommandData)]
    public class SetDistanceParam : IExternalCommand
    {
        /// <summary>
        /// The Revit application instance
        /// </summary>
        UIApplication m_uiApp;
        /// <summary>
        /// The active Revit document
        /// </summary>
        UIDocument m_uiDoc;

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
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            m_uiApp = commandData.Application;
            m_uiDoc = m_uiApp.ActiveUIDocument;

            // get the target element to be used for the Distance computation
            var collection = new ElementSet();
            foreach (var elementId in m_uiDoc.Selection.GetElementIds())
            {
               collection.Insert(m_uiDoc.Document.GetElement(elementId));
            }
            Parameter param = null;
            var es = new ElementSet();
            foreach (var elementId in m_uiDoc.Selection.GetElementIds())
            {
               es.Insert(m_uiDoc.Document.GetElement(elementId));
            }
            var targetPoint = getTargetPoint(es);

            // get all the divided surfaces in the Revit document
            var dsList = GetElements<DividedSurface>();

            foreach (var ds in dsList)
            {
                var gn = new GridNode();
                var u = 0;
                while (u < ds.NumberOfUGridlines)
                {
                    gn.UIndex = u;
                    var v = 0;
                    while (v < ds.NumberOfVGridlines)
                    {
                        gn.VIndex = v;
                        if (ds.IsSeedNode(gn))
                        {
                            var familyinstance = ds.GetTileFamilyInstance(gn, 0);
                            if (familyinstance != null)
                            {
                               param = familyinstance.LookupParameter("Distance");
                                if (param == null) throw new Exception("Panel family must have a Distance instance parameter");
                                else
                                {
                                    var loc = familyinstance.Location as LocationPoint;
                                    var panelPoint = loc.Point;

                                    var d = Math.Sqrt(Math.Pow((targetPoint.X - panelPoint.X), 2) + Math.Pow((targetPoint.Y - panelPoint.Y), 2) + Math.Pow((targetPoint.Z - panelPoint.Z), 2));
                                    param.Set(d);

                                    // uncomment the following lines to create points and lines showing where the distance measurement is made
                                    //ReferencePoint rp = m_doc.FamilyCreate.NewReferencePoint(panelPoint);
                                    //Line line = m_app.Create.NewLine(targetPoint, panelPoint, true);
                                    //Plane plane = m_app.Create.NewPlane(targetPoint.Cross(panelPoint), panelPoint);
                                    //SketchPlane skplane = m_doc.FamilyCreate.NewSketchPlane(plane);
                                    //ModelCurve modelcurve = m_doc.FamilyCreate.NewModelCurve(line, skplane);
                                }
                            }
                        }
                        v = v + 1;
                    }
                    u = u + 1;
                }
            }
            return Result.Succeeded;
        }

        /// <summary>
        /// Get the Autodesk.Revit.DB.XYZ point of the selected target element
        /// </summary>
        /// <param name="collection">Selected elements</param>
        /// <returns>the Autodesk.Revit.DB.XYZ point of the selected target element</returns>
        XYZ getTargetPoint(ElementSet collection)
        {
            FamilyInstance targetElement = null;
            if (collection.Size != 1)
            {
                throw new Exception("You must select one component from which the distance to panels will be measured");
            }
            else
            {
                foreach (Element e in collection)
                {
                    targetElement = e as FamilyInstance;
                }
            }

            if (null == targetElement)
            {
                throw new Exception("You must select one family instance from which the distance to panels will be measured");
            }
            var targetLocation = targetElement.Location as LocationPoint;
            return targetLocation.Point;
        }

        protected List<T> GetElements<T>() where T : Element
        {
           var returns = new List<T>();
           var collector = new FilteredElementCollector(m_uiDoc.Document);
           ICollection<Element> founds = collector.OfClass(typeof(T)).ToElements();
           foreach (var elem in founds)
           {
              returns.Add(elem as T);
           }
           return returns;
        }


    }
}

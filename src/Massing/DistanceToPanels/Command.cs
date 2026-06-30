// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;

namespace Ara3D.RevitSampleBrowser.Massing.DistanceToPanels.CS
{
    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    [Journaling(JournalingMode.NoCommandData)]
    public class SetDistanceParam : IExternalCommand
    {
        private UIApplication m_uiApp;

        private UIDocument m_uiDoc;

        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            m_uiApp = commandData.Application;
            m_uiDoc = m_uiApp.ActiveUIDocument;

            // get the target element to be used for the Distance computation
            ElementSet collection = new();
            foreach (var elementId in m_uiDoc.Selection.GetElementIds())
            {
                collection.Insert(m_uiDoc.Document.GetElement(elementId));
            }

            ElementSet es = new();
            foreach (var elementId in m_uiDoc.Selection.GetElementIds())
            {
                es.Insert(m_uiDoc.Document.GetElement(elementId));
            }

            var targetPoint = GetTargetPoint(es);

            // get all the divided surfaces in the Revit document
            var dsList = GetElements<DividedSurface>();

            foreach (var ds in dsList)
            {
                GridNode gn = new();
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
                                var param = familyinstance.LookupParameter("Distance");
                                if (param == null)
                                {
                                    throw new Exception("Panel family must have a Distance instance parameter");
                                }

                                var loc = familyinstance.Location as LocationPoint;
                                var panelPoint = loc.Point;

                                var d = Math.Sqrt(Math.Pow(targetPoint.X - panelPoint.X, 2) +
                                                  Math.Pow(targetPoint.Y - panelPoint.Y, 2) +
                                                  Math.Pow(targetPoint.Z - panelPoint.Z, 2));
                                param.Set(d);

                                // uncomment the following lines to create points and lines showing where the distance measurement is made
                                //ReferencePoint rp = m_doc.FamilyCreate.NewReferencePoint(panelPoint);
                                //Line line = m_app.Create.NewLine(targetPoint, panelPoint, true);
                                //Plane plane = m_app.Create.NewPlane(targetPoint.Cross(panelPoint), panelPoint);
                                //SketchPlane skplane = m_doc.FamilyCreate.NewSketchPlane(plane);
                                //ModelCurve modelcurve = m_doc.FamilyCreate.NewModelCurve(line, skplane);
                            }
                        }

                        v++;
                    }

                    u++;
                }
            }

            return Result.Succeeded;
        }

        private XYZ GetTargetPoint(ElementSet collection)
        {
            FamilyInstance targetElement = null;
            if (collection.Size != 1)
                throw new Exception("You must select one component from which the distance to panels will be measured");
            foreach (Element e in collection)
            {
                targetElement = e as FamilyInstance;
            }

            if (null == targetElement)
                throw new Exception(
                    "You must select one family instance from which the distance to panels will be measured");
            var targetLocation = targetElement.Location as LocationPoint;
            return targetLocation.Point;
        }

        protected List<T> GetElements<T>() where T : Element
        {
            List<T> returns = [];
            FilteredElementCollector collector = new(m_uiDoc.Document);
            ICollection<Element> founds = collector.OfClass(typeof(T)).ToElements();
            foreach (var elem in founds)
            {
                returns.Add(elem as T);
            }

            return returns;
        }
    }
}

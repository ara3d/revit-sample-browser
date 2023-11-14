// Copyright 2023. See https://github.com/ara3d/revit-samples/LICENSE.txt

using System;
using System.Collections.Generic;
using System.Linq;
using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Analysis;
using Autodesk.Revit.DB.Events;
using Autodesk.Revit.UI;

namespace Revit.SDK.Samples.AnalysisVisualizationFramework.CS
{
    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    public class DistanceToSurfaces : IExternalApplication
    {
        public Result OnStartup(UIControlledApplication uiControlledApplication)
        {
            uiControlledApplication.ControlledApplication.DocumentOpened += docOpen;
            return Result.Succeeded;
        }

        public Result OnShutdown(UIControlledApplication application)
        {
            return Result.Succeeded;
        }

        private void docOpen(object sender, DocumentOpenedEventArgs e)
        {
            var app = sender as Application;
            var uiApp = new UIApplication(app);
            var doc = uiApp.ActiveUIDocument.Document;

            var collector = new FilteredElementCollector(doc);
            collector.WherePasses(new ElementClassFilter(typeof(FamilyInstance)));
            var sphereElements = from element in collector where element.Name == "sphere" select element;
            if (!sphereElements.Any())
            {
                TaskDialog.Show("Error", "Sphere family must be loaded");
                return;
            }

            var sphere = sphereElements.Cast<FamilyInstance>().First();
            var viewCollector = new FilteredElementCollector(doc);
            viewCollector.OfClass(typeof(View3D)).ToElements();
            var viewElements = from element in viewCollector where element.Name == "AVF" select element;
            if (!viewElements.Any())
            {
                TaskDialog.Show("Error", "A 3D view named 'AVF' must exist to run this application.");
                return;
            }

            var view = viewElements.Cast<View>().First();

            var updater = new SpatialFieldUpdater(uiApp.ActiveAddInId, sphere.Id, view.Id);
            if (!UpdaterRegistry.IsUpdaterRegistered(updater.GetUpdaterId())) UpdaterRegistry.RegisterUpdater(updater);
            var wallFilter = new ElementCategoryFilter(BuiltInCategory.OST_Walls);
            var familyFilter = new ElementClassFilter(typeof(FamilyInstance));
            var massFilter = new ElementCategoryFilter(BuiltInCategory.OST_Mass);
            IList<ElementFilter> filterList = new List<ElementFilter>();
            filterList.Add(wallFilter);
            filterList.Add(familyFilter);
            filterList.Add(massFilter);
            var filter = new LogicalOrFilter(filterList);

            UpdaterRegistry.AddTrigger(updater.GetUpdaterId(), filter, Element.GetChangeTypeGeometry());
            UpdaterRegistry.AddTrigger(updater.GetUpdaterId(), filter, Element.GetChangeTypeElementDeletion());
        }
    }

    public class SpatialFieldUpdater : IUpdater
    {
        private readonly AddInId addinID;
        private readonly ElementId sphereID;
        private readonly UpdaterId updaterID;
        private readonly ElementId viewID;

        public SpatialFieldUpdater(AddInId id, ElementId sphere, ElementId view)
        {
            addinID = id;
            sphereID = sphere;
            viewID = view;
            updaterID = new UpdaterId(addinID, new Guid("FBF2F6B2-4C06-42d4-97C1-D1B4EB593EFF"));
        }

        public void Execute(UpdaterData data)
        {
            var doc = data.GetDocument();

            var view = doc.GetElement(viewID) as View;
            var sphere = doc.GetElement(sphereID) as FamilyInstance;
            var sphereLP = sphere.Location as LocationPoint;
            var sphereXYZ = sphereLP.Point;

            var sfm = SpatialFieldManager.GetSpatialFieldManager(view) ?? SpatialFieldManager.CreateSpatialFieldManager(view, 3); // Three measurement values for each point
            sfm.Clear();

            var collector = new FilteredElementCollector(doc, view.Id);
            var wallFilter = new ElementCategoryFilter(BuiltInCategory.OST_Walls);
            var massFilter = new ElementCategoryFilter(BuiltInCategory.OST_Mass);
            var filter = new LogicalOrFilter(wallFilter, massFilter);
            ICollection<Element> elements = collector.WherePasses(filter).WhereElementIsNotElementType().ToElements();

            foreach (Face face in GetFaces(elements))
            {
                var idx = sfm.AddSpatialFieldPrimitive(face.Reference);
                var doubleList = new List<double>();
                IList<UV> uvPts = new List<UV>();
                IList<ValueAtPoint> valList = new List<ValueAtPoint>();
                var bb = face.GetBoundingBox();
                for (var u = bb.Min.U; u < bb.Max.U; u = u + (bb.Max.U - bb.Min.U) / 15)
                for (var v = bb.Min.V; v < bb.Max.V; v = v + (bb.Max.V - bb.Min.V) / 15)
                {
                    var uvPnt = new UV(u, v);
                    uvPts.Add(uvPnt);
                    var faceXYZ = face.Evaluate(uvPnt);
                    // Specify three values for each point
                    doubleList.Add(faceXYZ.DistanceTo(sphereXYZ));
                    doubleList.Add(-faceXYZ.DistanceTo(sphereXYZ));
                    doubleList.Add(faceXYZ.DistanceTo(sphereXYZ) * 10);
                    valList.Add(new ValueAtPoint(doubleList));
                    doubleList.Clear();
                }

                var pnts = new FieldDomainPointsByUV(uvPts);
                var vals = new FieldValues(valList);

                var resultSchema1 = new AnalysisResultSchema("Schema 1", "Schema 1 Description");
                IList<int> registeredResults = new List<int>();
                registeredResults = sfm.GetRegisteredResults();
                var idx1 = 0;
                if (registeredResults.Count == 0)
                    idx1 = sfm.RegisterResult(resultSchema1);
                else
                    idx1 = registeredResults.First();
                sfm.UpdateSpatialFieldPrimitive(idx, pnts, vals, idx1);
            }
        }

        public string GetAdditionalInformation()
        {
            return "Calculate distance from sphere to walls and display results";
        }

        public ChangePriority GetChangePriority()
        {
            return ChangePriority.FloorsRoofsStructuralWalls;
        }

        public UpdaterId GetUpdaterId()
        {
            return updaterID;
        }

        public string GetUpdaterName()
        {
            return "Distance to Surfaces";
        }

        private FaceArray GetFaces(ICollection<Element> elements)
        {
            var faceArray = new FaceArray();
            var options = new Options();
            options.ComputeReferences = true;
            foreach (var element in elements)
            {
                var geomElem = element.get_Geometry(options);
                if (geomElem != null)
                    foreach (var geomObj in geomElem)
                    {
                        var solid = geomObj as Solid;
                        if (solid != null)
                            foreach (Face f in solid.Faces)
                                faceArray.Append(f);
                        var inst = geomObj as GeometryInstance;
                        if (inst != null) // in-place family walls
                            foreach (object o in inst.SymbolGeometry)
                            {
                                var s = o as Solid;
                                if (s != null)
                                    foreach (Face f in s.Faces)
                                        faceArray.Append(f);
                            }
                    }
            }

            return faceArray;
        }
    }
}

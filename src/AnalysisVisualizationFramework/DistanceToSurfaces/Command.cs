// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Analysis;
using Autodesk.Revit.DB.Events;
using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Ara3D.RevitSampleBrowser.AnalysisVisualizationFramework.DistanceToSurfaces.CS
{
    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    public class DistanceToSurfaces : IExternalApplication
    {
        public Result OnStartup(UIControlledApplication uiControlledApplication)
        {
            uiControlledApplication.ControlledApplication.DocumentOpened += DocOpen;
            return Result.Succeeded;
        }

        public Result OnShutdown(UIControlledApplication application)
        {
            return Result.Succeeded;
        }

        private void DocOpen(object sender, DocumentOpenedEventArgs e)
        {
            var app = sender as Application;
            UIApplication uiApp = new(app);
            var doc = uiApp.ActiveUIDocument.Document;

            var sphere = doc.GetElements<FamilyInstance>().FirstOrDefault(s => s.Name == "Sphere");
            if (sphere == null)
            {
                TaskDialog.Show("Error", "No sphere family instance found, it must be loaded");
                return;
            }

            var view = doc.GetElements<View3D>().FirstOrDefault();
            if (view == null)
            {
                TaskDialog.Show("Error", "A 3D view named 'AVF' must exist to run this application.");
                return;
            }

            SpatialFieldUpdater updater = new(uiApp.ActiveAddInId, sphere.Id, view.Id);
            if (!UpdaterRegistry.IsUpdaterRegistered(updater.GetUpdaterId())) UpdaterRegistry.RegisterUpdater(updater);
            ElementCategoryFilter wallFilter = new(BuiltInCategory.OST_Walls);
            ElementClassFilter familyFilter = new(typeof(FamilyInstance));
            ElementCategoryFilter massFilter = new(BuiltInCategory.OST_Mass);
            List<ElementFilter> filterList =
            [
                wallFilter,
                familyFilter,
                massFilter
            ];
            LogicalOrFilter filter = new(filterList);

            UpdaterRegistry.AddTrigger(updater.GetUpdaterId(), filter, Element.GetChangeTypeGeometry());
            UpdaterRegistry.AddTrigger(updater.GetUpdaterId(), filter, Element.GetChangeTypeElementDeletion());
        }
    }

    public class SpatialFieldUpdater : IUpdater
    {
        private readonly AddInId m_addinId;
        private readonly ElementId m_sphereId;
        private readonly UpdaterId m_updaterId;
        private readonly ElementId m_viewId;

        public SpatialFieldUpdater(AddInId id, ElementId sphere, ElementId view)
        {
            m_addinId = id;
            m_sphereId = sphere;
            m_viewId = view;
            m_updaterId = new UpdaterId(m_addinId, new Guid("FBF2F6B2-4C06-42d4-97C1-D1B4EB593EFF"));
        }

        public void Execute(UpdaterData data)
        {
            var doc = data.GetDocument();

            var view = doc.GetElement(m_viewId) as View;
            var sphere = doc.GetElement(m_sphereId) as FamilyInstance;
            var sphereLp = sphere.Location as LocationPoint;
            var sphereXyz = sphereLp.Point;

            var sfm = SpatialFieldManager.GetSpatialFieldManager(view) ?? SpatialFieldManager.CreateSpatialFieldManager(view, 3); // Three measurement values for each point
            sfm.Clear();

            FilteredElementCollector collector = new(doc, view.Id);
            ElementCategoryFilter wallFilter = new(BuiltInCategory.OST_Walls);
            ElementCategoryFilter massFilter = new(BuiltInCategory.OST_Mass);
            LogicalOrFilter filter = new(wallFilter, massFilter);
            ICollection<Element> elements = collector.WherePasses(filter).WhereElementIsNotElementType().ToElements();

            foreach (Face face in GetFaces(elements))
            {
                var idx = sfm.AddSpatialFieldPrimitive(face.Reference);
                List<double> doubleList = [];
                IList<UV> uvPts = [];
                IList<ValueAtPoint> valList = [];
                var bb = face.GetBoundingBox();
                for (var u = bb.Min.U; u < bb.Max.U; u += (bb.Max.U - bb.Min.U) / 15)
                    for (var v = bb.Min.V; v < bb.Max.V; v += (bb.Max.V - bb.Min.V) / 15)
                    {
                        UV uvPnt = new(u, v);
                        uvPts.Add(uvPnt);
                        var faceXyz = face.Evaluate(uvPnt);
                        // Specify three values for each point
                        doubleList.Add(faceXyz.DistanceTo(sphereXyz));
                        doubleList.Add(-faceXyz.DistanceTo(sphereXyz));
                        doubleList.Add(faceXyz.DistanceTo(sphereXyz) * 10);
                        valList.Add(new ValueAtPoint(doubleList));
                        doubleList.Clear();
                    }

                FieldDomainPointsByUV pnts = new(uvPts);
                FieldValues vals = new(valList);

                AnalysisResultSchema resultSchema1 = new("Schema 1", "Schema 1 Description");
                IList<int> registeredResults = [];
                registeredResults = sfm.GetRegisteredResults();
                var idx1 = registeredResults.Count == 0 ? sfm.RegisterResult(resultSchema1) : registeredResults.First();
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
            return m_updaterId;
        }

        public string GetUpdaterName()
        {
            return "Distance to Surfaces";
        }

        private FaceArray GetFaces(ICollection<Element> elements)
        {
            FaceArray faceArray = new();
            Options options = new()
            {
                ComputeReferences = true
            };
            foreach (var element in elements)
            {
                var geomElem = element.get_Geometry(options);
                if (geomElem != null)
                    foreach (var geomObj in geomElem)
                    {
                        var solid = geomObj as Solid;
                        if (solid != null)
                            foreach (Face f in solid.Faces)
                            {
                                faceArray.Append(f);
                            }

                        var inst = geomObj as GeometryInstance;
                        if (inst != null) // in-place family walls
                            foreach (object o in inst.SymbolGeometry)
                            {
                                var s = o as Solid;
                                if (s != null)
                                    foreach (Face f in s.Faces)
                                    {
                                        faceArray.Append(f);
                                    }
                            }
                    }
            }

            return faceArray;
        }
    }
}

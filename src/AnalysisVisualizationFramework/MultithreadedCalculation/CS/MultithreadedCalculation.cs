// Copyright 2023. See https://github.com/ara3d/revit-samples/LICENSE.txt

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Analysis;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Events;
using Autodesk.Revit.UI.Selection;

namespace RevitMultiSample.MultithreadedCalculation.CS
{
    /// <summary>
    ///     Command to set the target element and begin the multithreaded calculation.
    /// </summary>
    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    public class MultithreadedCalculation : IExternalCommand
    {
        private static UpdaterId _sUpdaterId;
        private static int _sSpatialFieldId;
        private static int _sOldSpatialFieldId;
        private static string _sDocName;

        private static ElementId _sOldViewId;
        private static ElementId _sActiveViewId;

        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            var uiApp = commandData.Application;
            var uiDoc = uiApp.ActiveUIDocument;
            var doc = uiDoc.Document;
            _sDocName = doc.PathName;

            Element element = null;
            try
            {
                element = doc.GetElement(uiDoc.Selection.PickObject(ObjectType.Element,
                    "Select an element for the AVF demonstration."));
            }
            catch (Exception)
            {
                message = "User aborted the tool.";
                return Result.Cancelled;
            }

            // Set up SpatialFieldManager to hold results
            _sActiveViewId = doc.ActiveView.Id;
            SpatialFieldManager oldSfm = null;
            View oldView = null;
            if (_sOldViewId != null) oldView = doc.GetElement(_sOldViewId) as View;
            if (oldView != null) oldSfm = SpatialFieldManager.GetSpatialFieldManager(oldView);
            // If a previous SFM was being managed, delete it
            oldSfm?.RemoveSpatialFieldPrimitive(_sOldSpatialFieldId);

            // Setup container object for executing the calculation
            var container = CreateContainer(element);

            // Register updater to watch for geometry changes
            var updater = new SpatialFieldUpdater(container, uiApp.ActiveAddInId);
            if (!UpdaterRegistry.IsUpdaterRegistered(updater.GetUpdaterId()))
                UpdaterRegistry.RegisterUpdater(updater, doc);
            IList<ElementId> idCollection = new List<ElementId>();
            idCollection.Add(element.Id);
            UpdaterRegistry.RemoveAllTriggers(_sUpdaterId);
            UpdaterRegistry.AddTrigger(updater.GetUpdaterId(), doc, idCollection, Element.GetChangeTypeGeometry());

            // Register idling event
            uiApp.Idling += container.UpdateWhileIdling;

            // Start new thread
            var thread = new Thread(container.Run);
            thread.Start();

            return Result.Succeeded;
        }

        /// <summary>
        ///     Prepares a container object that carries out the calculations without invoking Revit API calls.
        /// </summary>
        /// <param name="element">The element for the calculations.</param>
        /// <returns>The container.</returns>
        public static MultithreadedCalculationContainer CreateContainer(Element element)
        {
            var doc = element.Document;
            var activeView = doc.GetElement(_sActiveViewId) as View;

            // Figure out which is the largest face facing the user
            var viewDirection = activeView.ViewDirection.Normalize();
            var biggestFace = GetBiggestFaceFacingUser(element, viewDirection);

            // Get or create SpatialFieldManager for AVF results
            var sfm = SpatialFieldManager.GetSpatialFieldManager(activeView) ?? SpatialFieldManager.CreateSpatialFieldManager(activeView, 1);

            // Reference the target face
            _sSpatialFieldId = sfm.AddSpatialFieldPrimitive(biggestFace.Reference);

            // Compute the range of U and V for the calculation
            var bbox = biggestFace.GetBoundingBox();

            return new MultithreadedCalculationContainer(doc.PathName, bbox.Min, bbox.Max);
        }

        /// <summary>
        ///     Gets the biggest face which faces the user.  Assumes that the element is a wall, or floor, or other "2-sided"
        ///     element, and that
        ///     one of the two biggest faces will be facing roughly towards the viewer.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <param name="viewDirection">The view direction.</param>
        /// <returns>The face.  Face.Reference will also be populated.</returns>
        private static Face GetBiggestFaceFacingUser(Element element, XYZ viewDirection)
        {
            // Holds the faces sorted by area
            var faceAreas = new SortedDictionary<double, List<Face>>();

            // Get the element geometry
            var options = new Options();
            options.ComputeReferences = true;
            var geomElem = element.get_Geometry(options);

            // Look at the faces in each solid
            foreach (var geomObj in geomElem)
            {
                var solid = geomObj as Solid;
                if (solid != null)
                    foreach (Face face in solid.Faces)
                    {
                        var area = face.Area;
                        // Save the face to the collection
                        if (faceAreas.ContainsKey(area))
                        {
                            faceAreas[area].Add(face);
                        }
                        else
                        {
                            var faces = new List<Face> { face };
                            faceAreas.Add(area, faces);
                        }
                    }
            }

            // Get biggest two faces.  There might be two faces in the last item, or one face in the last item.
            var count = faceAreas.Count;
            var faceCollection1 = faceAreas.ElementAt(count - 1);
            var faceCollection2 = faceAreas.ElementAt(count - 2);

            Face face1 = null;
            Face face2 = null;
            // Two or more equal faces.  Use the first two.
            if (faceCollection1.Value.Count > 1)
            {
                face1 = faceCollection1.Value[0];
                face2 = faceCollection1.Value[1];
            }
            // One largest face.  Use the first face from the next item for comparison.
            else
            {
                face1 = faceCollection1.Value[0];
                face2 = faceCollection2.Value[0];
            }

            // Compute face normal
            var box = face1.GetBoundingBox();
            var faceCenter = (box.Max + box.Min) / 2;
            var faceNormal = face1.ComputeNormal(faceCenter).Normalize();

            // Compute angle to the view direction.  If less than 90 degrees, keep this face.
            var angle = viewDirection.AngleTo(faceNormal);

            Face biggestFace = null;
            if (Math.Abs(angle) < Math.PI / 2)
                biggestFace = face1;
            else
                biggestFace = face2;

            return biggestFace;
        }

        /// <summary>
        ///     Updater called when wall geometry changes, so analysis results can update.
        /// </summary>
        public class SpatialFieldUpdater : IUpdater
        {
            // The old container object.
            private MultithreadedCalculationContainer m_containerOld;

            public SpatialFieldUpdater(MultithreadedCalculationContainer container, AddInId addinId)
            {
                m_containerOld = container;
                _sUpdaterId = new UpdaterId(addinId, new Guid("FBF2F6B2-4C06-42d4-97C1-D1B4EB593EFF"));
            }

            // Execution method for the updater
            public void Execute(UpdaterData data)
            {
                // Remove old idling event callback
                var uiApp = new UIApplication(data.GetDocument().Application);
                uiApp.Idling -= m_containerOld.UpdateWhileIdling;
                m_containerOld.Stop();

                // Clear the current AVF results
                var doc = data.GetDocument();
                var activeView = doc.GetElement(_sActiveViewId) as View;
                var sfm = SpatialFieldManager.GetSpatialFieldManager(activeView);
                sfm.Clear();

                // Restart the multithread calculation with a new container
                var modifiedElem = doc.GetElement(data.GetModifiedElementIds().First());
                var container = CreateContainer(modifiedElem);
                m_containerOld = container;

                // Setup the new idling callback
                uiApp.Idling += container.UpdateWhileIdling;

                // Start the thread
                var threadNew = new Thread(container.Run);
                threadNew.Start();
            }

            public string GetAdditionalInformation()
            {
                return "AVF DMU Thread sample";
            }

            public ChangePriority GetChangePriority()
            {
                return ChangePriority.FloorsRoofsStructuralWalls;
            }

            public UpdaterId GetUpdaterId()
            {
                return _sUpdaterId;
            }

            public string GetUpdaterName()
            {
                return "AVF DMU Thread";
            }
        }

        /// <summary>
        ///     Container class that manages the multithreaded calculation and idling activity.
        /// </summary>
        public class MultithreadedCalculationContainer
        {
            private const int NumberOfUPnts = 10;
            private const int NumberOfVPnts = 5;

            private readonly string m_docName;
            private readonly UV m_max;
            private readonly UV m_min;
            private volatile bool m_stop;
            private IList<UV> m_uvToCalculate = new List<UV>();
            private int m_uvToCalculateCount;
            private readonly IList<ResultsData> m_results = new List<ResultsData>();
            private readonly IList<UV> m_uvPts = new List<UV>();
            private readonly IList<ValueAtPoint> m_valList = new List<ValueAtPoint>();

            public MultithreadedCalculationContainer(string docName, UV min, UV max)
            {
                m_docName = docName;
                m_min = min;
                m_max = max;
            }

            public void Run()
            {
                m_uvToCalculate = DetermineFacePoints();
                m_uvToCalculateCount = m_uvToCalculate.Count;
                Calculate();
            }

            /// <summary>
            ///     Stops the thread/calculation and application via idling.
            /// </summary>
            public void Stop()
            {
                m_stop = true;
            }

            /// <summary>
            ///     The idling callback which adds data to the AVF results.
            /// </summary>
            /// <param name="sender"></param>
            /// <param name="e"></param>
            public void UpdateWhileIdling(object sender, IdlingEventArgs e)
            {
                var uiApp = sender as UIApplication;

                // Get SpatialFieldManager

                var resultSchema = new AnalysisResultSchema("Schema Name", "Description");
                var sfm = SpatialFieldManager.GetSpatialFieldManager(uiApp.ActiveUIDocument.Document.ActiveView) ??
                          SpatialFieldManager.CreateSpatialFieldManager(uiApp.ActiveUIDocument.Document.ActiveView, 1);

                var schemaIndex = sfm.RegisterResult(resultSchema);
                // If stopping, clear results and unset event.
                if (m_stop)
                {
                    lock (m_results)
                    {
                        m_results.Clear();
                    }

                    uiApp.Idling -= UpdateWhileIdling;
                    return;
                }

                // If document was closed and new document opened, do not run the update.
                if (uiApp.ActiveUIDocument.Document.PathName == m_docName)
                    // Lock access to current calculated results
                    lock (m_results)
                    {
                        if (m_results.Count == 0) return;

                        // Turn each result to an AVF ValueAtPoint
                        foreach (var rData in m_results)
                        {
                            m_uvPts.Add(new UV(rData.Uv.U, rData.Uv.V));
                            IList<double> doubleList = new List<double>();
                            doubleList.Add(rData.Value);
                            m_valList.Add(new ValueAtPoint(doubleList));
                        }

                        var pntsByUv = new FieldDomainPointsByUV(m_uvPts);
                        var fieldValues = new FieldValues(m_valList);

                        // Update with calculated values
                        var t = new Transaction(uiApp.ActiveUIDocument.Document);
                        t.SetName("AVF");
                        t.Start();
                        if (!m_stop)
                            sfm.UpdateSpatialFieldPrimitive(_sSpatialFieldId, pntsByUv, fieldValues, schemaIndex);
                        t.Commit();

                        // Clear results already processed.
                        m_results.Clear();

                        // If no more results to process, remove the idling event
                        if (m_uvToCalculateCount == 0)
                        {
                            uiApp.Idling -= UpdateWhileIdling;
                            _sOldViewId = _sActiveViewId;
                            _sOldSpatialFieldId = _sSpatialFieldId;
                        }
                    }
            }

            // Calculate the results in a loop 
            private void Calculate()
            {
                foreach (var uv in m_uvToCalculate)
                {
                    if (m_stop)
                    {
                        m_uvToCalculateCount = 0;
                        return;
                    }

                    // Lock access to results while the data is added
                    lock (m_results)
                    {
                        m_results.Add(new ResultsData(uv, 1000 * Math.Sin(Math.Abs(uv.U * uv.V))));
                        Thread.Sleep(500); // to simulate the effect of a complex computation
                        m_uvToCalculateCount--;
                    }
                }
            }

            // Setup the list of UV points to calculate results for
            private IList<UV> DetermineFacePoints()
            {
                IList<UV> uvList = new List<UV>();
                var upnt = m_min.U;
                var incrementU = (m_max.U - m_min.U) / (NumberOfUPnts - 1);
                var incrementV = (m_max.V - m_min.V) / (NumberOfVPnts - 1);
                while (upnt <= m_max.U)
                {
                    var vpnt = m_min.V;
                    while (vpnt <= m_max.V)
                    {
                        uvList.Add(new UV(upnt, vpnt));
                        vpnt = vpnt + incrementV;
                    }

                    upnt = upnt + incrementU;
                }

                return uvList;
            }
        }

        // Represents a set of results for the calculation
        public class ResultsData
        {
            public readonly UV Uv;
            public readonly double Value;

            public ResultsData(UV uv, double value)
            {
                Uv = uv;
                Value = value;
            }
        }
    }
}

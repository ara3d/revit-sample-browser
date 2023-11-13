//
// (C) Copyright 2003-2020 by Autodesk, Inc.
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
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;

using Autodesk.Revit;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using Autodesk.Revit.DB.Architecture;
using Autodesk.Revit.DB.Analysis;

namespace Revit.SDK.Samples.PathOfTravelCreation.CS
{
   /// <summary>
   /// The options for creating the PathOfTravel.
   /// </summary>
   public enum PathCreateOptions
   {
      /// <summary>
      /// Create from a single room's corners to single door
      /// </summary>
      SingleRoomCornersToSingleDoor,

      /// <summary>
      /// Create from all room's centerpoints to all doors
      /// </summary>
      AllRoomCenterToSingleDoor,

      /// <summary>
      /// Create from all room's corners to all doors
      /// </summary>
      AllRoomCornersToAllDoors,
   }

   /// <summary>
   /// Implements the Revit add-in interface IExternalCommand
   /// </summary>
   [Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]
   [Autodesk.Revit.Attributes.Regeneration(Autodesk.Revit.Attributes.RegenerationOption.Manual)]
   [Autodesk.Revit.Attributes.Journaling(Autodesk.Revit.Attributes.JournalingMode.NoCommandData)]
   public class Command : IExternalCommand
   {
      #region Class Interface Implementation
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
            var uiDoc = commandData.Application.ActiveUIDocument;
            var viewPlan = uiDoc.ActiveView as ViewPlan;
            if (null == viewPlan)
            {
               var td = new TaskDialog("Cannot create PathOfTravel.");
               td.MainInstruction = String.Format("PathOfTravel can only be created for plan views.");

               td.Show();

               return Result.Succeeded;
            }

            using (var createForm = new CreateForm())
            {
               if (DialogResult.OK == createForm.ShowDialog())
               {
                  if (createForm.PathCreateOption == PathCreateOptions.SingleRoomCornersToSingleDoor)
                  {
                     CreatePathsOfTravelInOneRoomMultiplePointsToOneDoor(uiDoc);
                  }
                  else if (createForm.PathCreateOption == PathCreateOptions.AllRoomCenterToSingleDoor)
                  {
                     CreatePathsOfTravelRoomCenterpointsToSingleDoor(uiDoc);
                  }
                  else
                  {
                     CreatePathsOfTravelInAllRoomsAllDoorsMultiplePointsManyToMany(uiDoc);
                  }
               }
            }

            return Result.Succeeded;
         }
         catch (Exception ex)
         {
            message = ex.Message;
            return Result.Failed;
         }
      }
      #endregion

      #region MainMethods
      /// <summary>
      /// Generates paths of travel for near-corner points in one room to a single selected door.
      /// </summary>
      private void CreatePathsOfTravelInOneRoomMultiplePointsToOneDoor(UIDocument uiDoc)
      {
         var doc = uiDoc.Document;
         var viewPlan = uiDoc.ActiveView as ViewPlan;
         var levelId = viewPlan.GenLevel.Id;

         // select room
         var reference = uiDoc.Selection.PickObject(ObjectType.Element, new RoomSelectionFilter(), "Select a room");
         var room = doc.GetElement(reference) as Room;

         // select exit door
         var roomReference = uiDoc.Selection.PickObject(ObjectType.Element, new DoorSelectionFilter(), "Select a target door");
         var doorElement = doc.GetElement(roomReference) as Instance;
         var trf = doorElement.GetTransform();
         var endPoint = trf.Origin;

         var resultsSummary = new ResultsSummary();
         resultsSummary.numDoors = 1;

         var stopwatch = Stopwatch.StartNew();

         GeneratePathsOfTravelForOneRoomOneDoor(doc, viewPlan, room, endPoint, resultsSummary);

         stopwatch.Stop();
         resultsSummary.elapsedMilliseconds = stopwatch.ElapsedMilliseconds;

         ShowResults(resultsSummary);

      }

      /// <summary>
      /// Generates paths of travel from the center points of the room to a single door using the many to many approach.   Does not collect and display results.
      /// </summary>
      private void CreatePathsOfTravelRoomCenterpointsToSingleDoor(UIDocument uiDoc)
      {
         var doc = uiDoc.Document;
         var viewPlan = uiDoc.ActiveView as ViewPlan;
         var levelId = viewPlan.GenLevel.Id;

         // select exit door
         var reference = uiDoc.Selection.PickObject(ObjectType.Element, new DoorSelectionFilter(), "Select a target door");
         var doorElement = doc.GetElement(reference) as Instance;
         var trf = doorElement.GetTransform();
         var endPoint = trf.Origin;

         // find all rooms
         var fec = new FilteredElementCollector(doc);
         fec.WherePasses(new Autodesk.Revit.DB.Architecture.RoomFilter());

         var startPoints = new List<XYZ>();

         foreach (var room in fec.Cast<Room>().Where<Room>(rm => rm.Level.Id == levelId))
         {
            var location = room.Location as LocationPoint;
            if (location == null)
               continue;
            var roomPoint = location.Point;
            startPoints.Add(roomPoint);
         }

         // generate paths           
         using (var t = new Transaction(doc, "Generate paths of travel"))
         {
            t.Start();
            IList<PathOfTravelCalculationStatus> statuses;
            PathOfTravel.CreateMapped(viewPlan, startPoints, new List<XYZ> { endPoint }, out statuses);
            t.Commit();
         }

      }

      /// <summary>
      /// Creates paths of travel using all rooms on the given floor plan, starting from the near-corner points of those rooms, to all doors in the same floor plan.
      /// This version uses Revit's many-to-many API with automatic mapping between start and endpoints.
      /// </summary>
      private void CreatePathsOfTravelInAllRoomsAllDoorsMultiplePointsManyToMany(UIDocument uiDoc)
      {
         var doc = uiDoc.Document;
         var viewPlan = uiDoc.ActiveView as ViewPlan;

         CreatePathsOfTravelInAllRoomsAllDoorsMultiplePointsManyToMany(doc, viewPlan, false);

      }
      #endregion

      #region RoomUtils
      /// <summary>
      /// A selection filter that accepts selection of door elements only.
      /// </summary>
      class DoorSelectionFilter : ISelectionFilter
      {
         public bool AllowElement(Element element)
         {
            if (element.Category.Id == new ElementId(BuiltInCategory.OST_Doors))
            {
               return true;
            }
            return false;
         }

         public bool AllowReference(Reference refer, XYZ point)
         {
            return false;
         }

      }

      /// <summary>
      /// A selection filter that accepts selection of room elements only.
      /// </summary>
      class RoomSelectionFilter : ISelectionFilter
      {
         public bool AllowElement(Element element)
         {
            if (element is Room)
            {
               return true;
            }
            return false;
         }

         public bool AllowReference(Reference refer, XYZ point)
         {
            return false;
         }

      }

      /// <summary>
      /// Appends a list of the room's near-corner points to a pre-existing list.
      /// </summary>
      /// <remarks>
      /// A near-corner point is offset from the room boundaries by 1.5 ft (18 inches). The points are calculated geometrically and some situations may not return
      /// all logical near-corner points, or may return points which are inside furniture, casework or other design elements.   Only the first boundary region of the room is
      /// currently processed.
      /// </remarks>
      /// <param name="room"></param>
      /// <param name="nearCornerPoints"></param>
      /// <returns></returns>
      private static void AppendRoomNearCornerPoints(Room room, List<XYZ> nearCornerPoints)
      {
         var segments = room.GetBoundarySegments(new SpatialElementBoundaryOptions());
         if (segments == null || segments.Count == 0)
            return;

         // First region only
         var firstSegments = segments[0];
         var numSegments = firstSegments.Count;


         for (var i = 0; i < numSegments; i++)
         {
            var seg1 = firstSegments.ElementAt(i);
            var seg2 = firstSegments.ElementAt(i == numSegments - 1 ? 0 : i + 1);

            var curve1 = seg1.GetCurve();
            var curve2 = seg2.GetCurve();

            var offsetCurve1 = curve1.CreateOffset(-1.5, XYZ.BasisZ);
            var offsetCurve2 = curve2.CreateOffset(-1.5, XYZ.BasisZ);

            IntersectionResultArray intersections = null;
            var result = offsetCurve1.Intersect(offsetCurve2, out intersections);

            // First intersection only
            if (result == SetComparisonResult.Overlap && intersections.Size == 1)
            {
               nearCornerPoints.Add(intersections.get_Item(0).XYZPoint);
            }

         }

      }

      /// <summary>
      /// Returns a list of the room's near-corner points.  
      /// </summary>
      /// <remarks>
      /// A near-corner point is offset from the room boundaries by 1.5 ft (18 inches). The points are calculated geometrically and some situations may not return
      /// all logical near-corner points, or may return points which are inside furniture, casework or other design elements.   Only the first boundary region of the room is
      /// currently processed. 
      /// </remarks>
      /// <param name="room"></param>
      /// <returns></returns>
      private static List<XYZ> GetRoomNearCornerPoints(Room room)
      {
         var nearCornerPoints = new List<XYZ>();

         AppendRoomNearCornerPoints(room, nearCornerPoints);

         return nearCornerPoints;
      }
      #endregion

      #region PathOfTravelCreationUtils
      /// <summary>
      /// Shared implementation for use of Path of Travel bulk creation routine from all near-corner room points to all doors.
      /// </summary>
      /// <param name="doc"></param>
      /// <param name="viewPlan"></param>
      /// <param name="mapAllStartsToAllEnds"></param>
      private static void CreatePathsOfTravelInAllRoomsAllDoorsMultiplePointsManyToMany(Document doc, ViewPlan viewPlan, bool mapAllStartsToAllEnds)
      {
         var levelId = viewPlan.GenLevel.Id;

         // find rooms on level
         var fec = new FilteredElementCollector(doc, viewPlan.Id);
         fec.WherePasses(new Autodesk.Revit.DB.Architecture.RoomFilter());


         // find doors on level
         var fec2 = new FilteredElementCollector(doc, viewPlan.Id);
         fec2.OfCategory(BuiltInCategory.OST_Doors);


         // setup results
         var resultsSummary = new ResultsSummary();

         var endPoints = new List<XYZ>();

         // Collect rooms
         var rooms = fec.Cast<Room>().ToList<Room>();

         // Loop on doors and collect target points (the door's origin)
         foreach (var element in fec2)
         {
            var doorElement = (Instance)element;
            var trf = doorElement.GetTransform();
            endPoints.Add(trf.Origin);
         }

         resultsSummary.numDoors = endPoints.Count;



         using (var group = new TransactionGroup(doc, "Generate all paths of travel"))
         {
            group.Start();

            GeneratePathsOfTravelForRoomsToEndpointsManyToMany(doc, viewPlan, rooms, endPoints, resultsSummary, mapAllStartsToAllEnds);

            group.Assimilate();
         }

         ShowResults(resultsSummary);
      }

      /// <summary>
      ///  Generates path of travels from room corner points to the corresponding list of end points.
      /// </summary>
      /// <param name="doc"></param>
      /// <param name="viewPlan"></param>
      /// <param name="rooms"></param>
      /// <param name="endPoints"></param>
      /// <param name="resultsSummary"></param>
      /// <param name="mapAllStartsToAllEnds"></param>
      private static void GeneratePathsOfTravelForRoomsToEndpointsManyToMany(Document doc, ViewPlan viewPlan, List<Room> rooms, List<XYZ> endPoints, ResultsSummary resultsSummary,
                                                                              bool mapAllStartsToAllEnds)
      {
         var allSourcePoints = new List<XYZ>();
         foreach (var room in rooms)
         {
            AppendRoomNearCornerPoints(room, allSourcePoints);
         }
         //            foreach (Room room in rooms)
         //            {
         //                LocationPoint location = room.Location as LocationPoint;
         //                if (location == null)
         //                    continue;
         //                XYZ roomPoint = location.Point;
         //                allSourcePoints.Add(roomPoint);
         //            }

         resultsSummary.numSourcePoints += allSourcePoints.Count;

         List<XYZ> inputStartPoints = null;
         List<XYZ> inputEndPoints = null;

         // generate full lists of start and end points mapped to one another.  
         // This is for testing purposes, the API option to do this mapping is likely more efficient for this case.
         if (mapAllStartsToAllEnds)
         {
            var allSourcePointsMappedToEnds = new List<XYZ>();
            var allEndPointsMappedToEnds = new List<XYZ>();
            foreach (var source in allSourcePoints)
            {
               foreach (var end in endPoints)
               {
                  allSourcePointsMappedToEnds.Add(source);
                  allEndPointsMappedToEnds.Add(end);
               }
            }

            inputStartPoints = allSourcePointsMappedToEnds;
            inputEndPoints = allEndPointsMappedToEnds;
         }
         else
         {
            inputStartPoints = allSourcePoints;
            inputEndPoints = endPoints;
         }

         GeneratePathsOfTravel(doc, viewPlan, inputStartPoints, inputEndPoints, resultsSummary, !mapAllStartsToAllEnds);
      }

      /// <summary>
      /// Wraps all calls to PathOfTravel.Create() with multiple start/ends.
      /// </summary>
      /// <param name="doc"></param>
      /// <param name="viewPlan"></param>
      /// <param name="startPoints"></param>
      /// <param name="endPoints"></param>
      /// <param name="resultsSummary"></param>
      /// <param name="mapAllStartsToAllEnds"></param>
      private static void GeneratePathsOfTravel(Document doc, ViewPlan viewPlan, List<XYZ> startPoints, List<XYZ> endPoints, ResultsSummary resultsSummary, bool mapAllStartsToAllEnds)
      {
         // Performance monitoring
         var stopwatch = Stopwatch.StartNew();

         using (var t = new Transaction(doc, "Generate paths of travel"))
         {
            t.Start();

            IList<PathOfTravelCalculationStatus> statuses;
            IList<PathOfTravel> pathsOfTravel;
            if (mapAllStartsToAllEnds)
               pathsOfTravel = PathOfTravel.CreateMapped(viewPlan, startPoints, endPoints, out statuses);
            else
               pathsOfTravel = PathOfTravel.CreateMultiple(viewPlan, startPoints, endPoints, out statuses);

            var i = 0;

            foreach (var pathOfTravel in pathsOfTravel)
            {
               if (pathOfTravel == null)
               {
                  resultsSummary.numFailures++;
                  resultsSummary.failuresFound.Add(statuses[i]);
               }
               else resultsSummary.numSuccesses++;
               i++;
            }

            t.Commit();
         }
         stopwatch.Stop();
         resultsSummary.elapsedMilliseconds = stopwatch.ElapsedMilliseconds;
      }

      /// <summary>
      ///  Generates paths of travel from points in one room to many target locations using the slower (one-at-a-time) method.
      /// </summary>
      /// <param name="doc"></param>
      /// <param name="viewPlan"></param>
      /// <param name="room"></param>
      /// <param name="endPoints"></param>
      /// <param name="resultsSummary"></param>
      private static void GeneratePathsOfTravelForOneRoomManyDoors(Document doc, ViewPlan viewPlan, Room room, List<XYZ> endPoints, ResultsSummary resultsSummary)
      {
         var sourcePoints = GetRoomNearCornerPoints(room);
         resultsSummary.numSourcePoints += sourcePoints.Count;

         // generate paths

         using (var t = new Transaction(doc, "Generate paths of travel"))
         {
            t.Start();
            IList<PathOfTravelCalculationStatus> statuses;
            var pathsOfTravel = PathOfTravel.CreateMapped(viewPlan, sourcePoints, endPoints, out statuses);

            foreach (var pOT in pathsOfTravel)
            {
               if (pOT == null) resultsSummary.numFailures++;
               else resultsSummary.numSuccesses++;
            }
            t.Commit();
         }
      }

      /// <summary>
      /// Generates paths of travel from points in one room to a single target location using the slower (one-at-a-time) method.
      /// </summary>
      /// <param name="doc"></param>
      /// <param name="viewPlan"></param>
      /// <param name="room"></param>
      /// <param name="endPoint"></param>
      /// <param name="resultsSummary"></param>
      private static void GeneratePathsOfTravelForOneRoomOneDoor(Document doc, ViewPlan viewPlan, Room room, XYZ endPoint, ResultsSummary resultsSummary)
      {
         GeneratePathsOfTravelForOneRoomManyDoors(doc, viewPlan, room, new List<XYZ> { endPoint }, resultsSummary);
      }
      #endregion

      #region ResultsUtils
      /// <summary>
      /// Class that aggregates the results of the path of travel creation for later display and/or logging.
      /// </summary>
      class ResultsSummary
      {
         public int numSourcePoints { get; set; }
         public int numDoors { get; set; }
         public int numSuccesses { get; set; }
         public int numFailures { get; set; }
         public long elapsedMilliseconds { get; set; }
         public List<PathOfTravelCalculationStatus> failuresFound { get; set; }

         public ResultsSummary()
         {
            failuresFound = new List<PathOfTravelCalculationStatus>();
         }
      }

      /// <summary>
      /// Displays the results from a run of path of travel creation using a TaskDialog.
      /// </summary>
      /// <param name="resultsSummary"></param>
      private static void ShowResults(ResultsSummary resultsSummary)
      {
         var ci = new CultureInfo("en-us");

         var numOfPathsToCreate = resultsSummary.numSourcePoints * resultsSummary.numDoors;

         var successRatePercent = (double)(resultsSummary.numSuccesses) / (double)(numOfPathsToCreate);

         var td = new TaskDialog("Results of PathOfTravel creation");
         td.MainInstruction = String.Format("Path of Travel succeeded on {0} of known points", successRatePercent.ToString("P01", ci));
         var details = String.Format("There were {0} room source points found in room analysis (via offsetting boundaries). " +
                                     "They would be connected to {2} door target points. {1} failed to generate a Path of Travel out of {4}  " +
                                     "Processing took {3} milliseconds.",
                                         resultsSummary.numSourcePoints, resultsSummary.numFailures, resultsSummary.numDoors, resultsSummary.elapsedMilliseconds, numOfPathsToCreate);
         if (resultsSummary.numFailures > 0)
         {
            details += "  Most likely reason for failures is an obstacle on or nearby to the source point.";
         }
         td.MainContent = details;

         td.Show();


      }
      #endregion
   }
}


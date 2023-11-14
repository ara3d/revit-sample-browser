// Copyright 2023. See https://github.com/ara3d/revit-samples/LICENSE.txt


using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Architecture;
using Autodesk.Revit.DB.Mechanical;
using Autodesk.Revit.UI;

namespace Revit.SDK.Samples.RoofsRooms.CS
{
    /// <summary>
    ///     This class inherits from IExternalCommand,
    ///     used to check if room can cut roof by geometry relationship
    /// </summary>
    [Transaction(TransactionMode.ReadOnly)]
    [Regeneration(RegenerationOption.Manual)]
    [Journaling(JournalingMode.UsingCommandData)]
    public class Command : IExternalCommand
    {
        // Revit application
        private Application m_application;

        // Current document in Revit
        private Document m_document;

        public Result Execute(ExternalCommandData commandData,
            ref string message, ElementSet elements)
        {
            var assemblyLocation = Assembly.GetExecutingAssembly().Location;
            var log = assemblyLocation + "." + DateTime.Now.ToString("yyyyMMdd") + ".log";
            if (File.Exists(log)) File.Delete(log);
            TraceListener txtListener = new TextWriterTraceListener(log);
            Trace.Listeners.Add(txtListener);
            try
            {
                // variable initialization
                m_application = commandData.Application.Application;
                m_document = commandData.Application.ActiveUIDocument.Document;
                Environment.CurrentDirectory = Path.GetDirectoryName(assemblyLocation);

                FindRoomBoundingRoofs(ref message, elements);

                // Not show TaskDialog in regression mode
                if (0 == commandData.JournalData.Count) TaskDialog.Show("Roofs Rooms", message);

                // Insert result to journal data for regression purpose.
                const string DataKey = "Results";
                if (!commandData.JournalData.ContainsKey(DataKey))
                    // In normal/recording mode 
                    commandData.JournalData.Add(DataKey, message);
                else
                    // In regression/replaying mode
                    commandData.JournalData[DataKey] = message;

                return Result.Succeeded;
            }
            catch (Exception ex)
            {
                Trace.WriteLine(ex.ToString());
                message = ex.ToString();
                return Result.Failed;
            }
            finally
            {
                Trace.Flush();
                txtListener.Close();
                Trace.Close();
                Trace.Listeners.Remove(txtListener);
            }
        }

        /// <summary>
        ///     Test whether each room has a roof to bound it.
        /// </summary>
        /// <param name="message">Error message to be dumped.</param>
        /// <param name="elements">Some elements to return.</param>
        /// <returns></returns>
        private bool FindRoomBoundingRoofs(ref string message, ElementSet elements)
        {
            // Get all rooms
            var rooms = GetRoomsElements();
            if (rooms.Count == 0)
            {
                message = "Unable to identify any rooms, please create room first!";
                return false;
            }

            // Represents the criteria for boundary elements to be considered bounding roofs
            var categoryFilter = new LogicalOrFilter(new ElementCategoryFilter(BuiltInCategory.OST_Roofs),
                new ElementCategoryFilter(BuiltInCategory.OST_RoofSoffit));

            // Calculator for room/space geometry.
            var calculator = new SpatialElementGeometryCalculator(m_document);

            // Stores the resulting room->roof relationships
            var roomsAndRoofs = new Dictionary<Element, List<ElementId>>();

            foreach (var room in rooms)
            {
                // Get room geometry & boundaries          
                var results = calculator.CalculateSpatialElementGeometry((SpatialElement)room);

                // Get solid geometry so we can examine each face
                var geometry = results.GetGeometry();

                foreach (Face face in geometry.Faces)
                {
                    // Get list of roof boundary subfaces for a given face
                    var boundaryFaces = results.GetBoundaryFaceInfo(face);
                    foreach (var boundaryFace in boundaryFaces)
                    {
                        // Get boundary element
                        var boundaryElementId = boundaryFace.SpatialBoundaryElement;

                        // Only considering local file room bounding elements
                        var localElementId = boundaryElementId.HostElementId;

                        // Evaluate if element meets criteria using PassesFilter()
                        if (localElementId != ElementId.InvalidElementId &&
                            categoryFilter.PassesFilter(m_document, localElementId))
                        {
                            // Room already has roofs, add more
                            if (roomsAndRoofs.ContainsKey(room))
                            {
                                var roofs = roomsAndRoofs[room];
                                if (!roofs.Contains(localElementId))
                                    roofs.Add(localElementId);
                            }
                            // Room found first roof
                            else
                            {
                                var roofs = new List<ElementId> { localElementId };
                                roomsAndRoofs.Add(room, roofs);
                            }

                            break;
                        }
                    }
                }
            }

            // Format results
            if (roomsAndRoofs.Count > 0)
            {
                var logs = "Rooms that have a bounding roof:";
                message += logs + "\t\r\n";
                Trace.WriteLine(logs);
                foreach (var kvp in roomsAndRoofs)
                {
                    // remove this room from all rooms list
                    rooms.Remove(kvp.Key);

                    var roofs = kvp.Value;
                    string roofsString;

                    // Single roof boundary
                    if (roofs.Count == 1)
                    {
                        var roof = m_document.GetElement(roofs[0]);
                        roofsString = $"Roof: Id = {roof.Id}, Name = {roof.Name}";
                    }
                    // Multiple roofs
                    else
                    {
                        roofsString = "Roofs ids = " +
                                      string.Join(", ", Array.ConvertAll(roofs.ToArray(), i => i.ToString()));
                    }

                    // Save results
                    logs = $"  Room: Id = {kvp.Key.Id}, Name = {kvp.Key.Name} --> {roofsString}";
                    message += logs + "\t\r\n";
                    Trace.WriteLine(logs);
                }
            }

            // Format the rooms that have no bounding roof
            Trace.WriteLine("Geometry relationship checking finished...");
            if (rooms.Count != 0)
            {
                var logs = "Below rooms don't have bounding roofs:";
                message += logs + "\t\r\n";
                Trace.WriteLine(logs);
                foreach (var room in rooms)
                {
                    elements.Insert(room);
                    logs = $"  Room Id: {room.Id}, Room Name: {room.Name}";
                    message += logs + "\t\r\n";
                    Trace.WriteLine(logs);
                }
            }

            return true;
        }

        /// <summary>
        ///     Retrieve all Rooms and Spaces elements from active document.
        /// </summary>
        /// <returns>Element list retrieved from current document.</returns>
        private List<Element> GetRoomsElements()
        {
            var array = new List<Element>();
            ElementFilter roomSpaceFilter = new LogicalOrFilter(new RoomFilter(), new SpaceFilter());
            var collector = new FilteredElementCollector(m_document);
            array.AddRange(collector.WherePasses(roomSpaceFilter).ToElements());
            return array;
        }
    }
}

// Copyright 2023. See https://github.com/ara3d/revit-samples/LICENSE.txt

using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace RevitMultiSample.ReferencePlane.CS
{
    /// <summary>
    ///     A object to manage reference plane.
    /// </summary>
    public class ReferencePlaneMgr
    {
        //A dictionary for create reference plane with different host element.
        private readonly Dictionary<Type, CreateDelegate> m_createHandler;
        private readonly UIDocument m_document; //the currently active project

        private readonly Options m_options; //User preferences for parsing of geometry.

        //The datasource for a DataGridView control.
        private DataTable m_referencePlanes;

        /// <summary>
        ///     A ReferencePlaneMgr object constructor.
        /// </summary>
        /// <param name="commandData">
        ///     The ExternalCommandData object for the active
        ///     instance of Autodesk Revit.
        /// </param>
        public ReferencePlaneMgr(ExternalCommandData commandData)
        {
            Debug.Assert(null != commandData);

            m_document = commandData.Application.ActiveUIDocument;

            //Get an instance of this class from Application. Create
            m_options = commandData.Application.Application.Create.NewGeometryOptions();
            //Set your preferences and pass it to Element.Geometry or Instance.Geometry.
            m_options.ComputeReferences = true;
            //m_options.DetailLevel = DetailLevels.Fine;
            m_options.View = m_document.Document.ActiveView;

            m_createHandler = new Dictionary<Type, CreateDelegate>
            {
                { typeof(Wall), OperateWall },
                { typeof(Floor), OperateSlab }
            };

            InitializeDataTable();
        }

        /// <summary>
        ///     The datasource for a DataGridView control.
        /// </summary>
        public DataTable ReferencePlanes
        {
            get
            {
                GetAllReferencePlanes();
                return m_referencePlanes;
            }
        }

        /// <summary>
        ///     Create reference plane with the selected element.
        ///     the selected element must be wall or slab at this sample code.
        /// </summary>
        public void Create()
        {
            foreach (var eId in m_document.Selection.GetElementIds())
            {
                var e = m_document.Document.GetElement(eId);
                try
                {
                    var createDelegate = m_createHandler[e.GetType()];
                    createDelegate(e);
                }
                catch (Exception)
                {
                }
            }
        }

        /// <summary>
        ///     Initialize a DataTable object which is datasource of a DataGridView control.
        /// </summary>
        private void InitializeDataTable()
        {
            m_referencePlanes = new DataTable("ReferencePlanes");
            // Declare variables for DataColumn and DataRow objects.

            var column =
                // Create new DataColumn, set DataType, 
                // ColumnName and add to DataTable.    
                new DataColumn();
            column.DataType = Type.GetType("System.Int32");
            column.ColumnName = "ID";
            // Add the Column to the DataColumnCollection.
            m_referencePlanes.Columns.Add(column);

            // Create second column.
            column = new DataColumn();
            column.DataType = Type.GetType("System.String");
            column.ColumnName = "BubbleEnd";
            // Add the column to the table.
            m_referencePlanes.Columns.Add(column);

            // Create third column.
            column = new DataColumn();
            column.DataType = Type.GetType("System.String");
            column.ColumnName = "FreeEnd";
            // Add the column to the table.
            m_referencePlanes.Columns.Add(column);

            // Create fourth column.
            column = new DataColumn();
            column.DataType = Type.GetType("System.String");
            column.ColumnName = "Normal";
            // Add the column to the table.
            m_referencePlanes.Columns.Add(column);

            // Make the ID column the primary key column.
            var primaryKeyColumns = new DataColumn[1];
            primaryKeyColumns[0] = m_referencePlanes.Columns["ID"];
            m_referencePlanes.PrimaryKey = primaryKeyColumns;
        }

        /// <summary>
        ///     Format the output string for a point.
        /// </summary>
        /// <param name="point">A point to show in UI.</param>
        /// <returns>The display string for a point.</returns>
        private string Format(XYZ point)
        {
            return "(" + Math.Round(point.X, 2) +
                   ", " + Math.Round(point.Y, 2) +
                   ", " + Math.Round(point.Z, 2) + ")";
        }

        /// <summary>
        ///     Get all reference planes in current revit project.
        /// </summary>
        /// <returns>The number of all reference planes.</returns>
        private int GetAllReferencePlanes()
        {
            m_referencePlanes.Clear();

            var itor = new FilteredElementCollector(m_document.Document)
                .OfClass(typeof(Autodesk.Revit.DB.ReferencePlane)).GetElementIterator();

            itor.Reset();
            while (itor.MoveNext())
            {
                if (!(itor.Current is Autodesk.Revit.DB.ReferencePlane refPlane)) continue;

                var row = m_referencePlanes.NewRow();
                row["ID"] = refPlane.Id.Value;
                row["BubbleEnd"] = Format(refPlane.BubbleEnd);
                row["FreeEnd"] = Format(refPlane.FreeEnd);
                row["Normal"] = Format(refPlane.Normal);
                m_referencePlanes.Rows.Add(row);
            }

            return m_referencePlanes.Rows.Count;
        }

        /// <summary>
        ///     Create reference plane for a wall.
        /// </summary>
        /// <param name="host">A wall element.</param>
        private void OperateWall(Element host)
        {
            var wall = host as Wall;
            var bubbleEnd = new XYZ();
            var freeEnd = new XYZ();
            var cutVec = new XYZ();

            LocateWall(wall, ref bubbleEnd, ref freeEnd, ref cutVec);
            m_document.Document.Create.NewReferencePlane(bubbleEnd, freeEnd, cutVec, m_document.Document.ActiveView);
        }

        /// <summary>
        ///     Create reference plane for a slab.
        /// </summary>
        /// <param name="host">A floor element.</param>
        private void OperateSlab(Element host)
        {
            var floor = host as Floor;
            var bubbleEnd = new XYZ();
            var freeEnd = new XYZ();
            var thirdPnt = new XYZ();
            LocateSlab(floor, ref bubbleEnd, ref freeEnd, ref thirdPnt);
            m_document.Document.Create.NewReferencePlane2(bubbleEnd, freeEnd, thirdPnt, m_document.Document.ActiveView);
        }

        /// <summary>
        ///     Located the exterior of a wall object.
        /// </summary>
        /// <param name="wall">A wall object</param>
        /// <param name="bubbleEnd">The bubble end of new reference plane.</param>
        /// <param name="freeEnd">The free end of new reference plane.</param>
        /// <param name="cutVec">The cut vector of new reference plane.</param>
        private void LocateWall(Wall wall, ref XYZ bubbleEnd, ref XYZ freeEnd, ref XYZ cutVec)
        {
            var location = wall.Location as LocationCurve;
            var locaCurve = location.Curve;

            //Not work for wall without location.
            if (null == locaCurve) throw new Exception("This wall has no location.");

            //Not work for arc wall.
            var line = locaCurve as Line;
            if (null == line) throw new Exception("Just work for straight wall.");

            //Calculate offset by law of cosines.
            var halfThickness = wall.Width / 2;
            var length = GeoHelper.GetLength(locaCurve.GetEndPoint(0), locaCurve.GetEndPoint(1));
            var xAxis = GeoHelper.GetDistance(locaCurve.GetEndPoint(0).X, locaCurve.GetEndPoint(1).X);
            var yAxis = GeoHelper.GetDistance(locaCurve.GetEndPoint(0).Y, locaCurve.GetEndPoint(1).Y);

            var xOffset = yAxis * halfThickness / length;
            var yOffset = xAxis * halfThickness / length;

            if (locaCurve.GetEndPoint(0).X < locaCurve.GetEndPoint(1).X
                && locaCurve.GetEndPoint(0).Y < locaCurve.GetEndPoint(1).Y)
                xOffset = -xOffset;
            if (locaCurve.GetEndPoint(0).X > locaCurve.GetEndPoint(1).X
                && locaCurve.GetEndPoint(0).Y > locaCurve.GetEndPoint(1).Y)
                yOffset = -yOffset;
            if (locaCurve.GetEndPoint(0).X > locaCurve.GetEndPoint(1).X
                && locaCurve.GetEndPoint(0).Y < locaCurve.GetEndPoint(1).Y)
            {
                xOffset = -xOffset;
                yOffset = -yOffset;
            }

            //Three necessary parameters for generate a reference plane.
            bubbleEnd = new XYZ(locaCurve.GetEndPoint(0).X + xOffset,
                locaCurve.GetEndPoint(0).Y + yOffset, locaCurve.GetEndPoint(0).Z);
            freeEnd = new XYZ(locaCurve.GetEndPoint(1).X + xOffset,
                locaCurve.GetEndPoint(1).Y + yOffset, locaCurve.GetEndPoint(1).Z);
            cutVec = new XYZ(0, 0, 1);
        }

        /// <summary>
        ///     Located the buttom of a slab object.
        /// </summary>
        /// <param name="floor">A floor object.</param>
        /// <param name="bubbleEnd">The bubble end of new reference plane.</param>
        /// <param name="freeEnd">The free end of new reference plane.</param>
        /// <param name="thirdPnt">The third point of new reference plane.</param>
        private void LocateSlab(Floor floor, ref XYZ bubbleEnd, ref XYZ freeEnd, ref XYZ thirdPnt)
        {
            //Obtain the geometry data of the floor.
            var geometry = floor.get_Geometry(m_options);
            Face buttomFace = null;

            //foreach (GeometryObject go in geometry.Objects)
            var objects = geometry.GetEnumerator();
            while (objects.MoveNext())
            {
                var go = objects.Current;

                var solid = go as Solid;
                if (null == solid)
                    continue;
                //Get the bottom face of this floor.
                buttomFace = GeoHelper.GetBottomFace(solid.Faces);
            }

            var mesh = buttomFace.Triangulate();
            GeoHelper.Distribute(mesh, ref bubbleEnd, ref freeEnd, ref thirdPnt);
        }

        //A delegate for create reference plane with different host element.
        private delegate void CreateDelegate(Element host);
    }
}

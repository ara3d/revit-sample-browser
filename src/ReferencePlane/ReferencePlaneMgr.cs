// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

using Ara3D.RevitSampleBrowser.Common.Geometry;
using Ara3D.RevitSampleBrowser.Common.Geometry;
using Ara3D.RevitSampleBrowser.Common.Infrastructure;
namespace Ara3D.RevitSampleBrowser.ReferencePlane.CS
{
    public class ReferencePlaneMgr
    {
        private readonly Dictionary<Type, CreateDelegate> m_createHandler;
        private readonly UIDocument m_document;

        private readonly Options m_options;

        private DataTable m_referencePlanes;

        public ReferencePlaneMgr(ExternalCommandData commandData)
        {
            Debug.Assert(null != commandData);

            m_document = commandData.Application.ActiveUIDocument;

            m_options = commandData.Application.Application.Create.NewGeometryOptions();
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

        public DataTable ReferencePlanes
        {
            get
            {
                GetAllReferencePlanes();
                return m_referencePlanes;
            }
        }

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

        private void InitializeDataTable()
        {
            m_referencePlanes = new DataTable("ReferencePlanes");

            var column =
                new DataColumn
                {
                    DataType = Type.GetType("System.Int32"),
                    ColumnName = "ID"
                };
            m_referencePlanes.Columns.Add(column);

            column = new DataColumn
            {
                DataType = Type.GetType("System.String"),
                ColumnName = "BubbleEnd"
            };
            m_referencePlanes.Columns.Add(column);

            column = new DataColumn
            {
                DataType = Type.GetType("System.String"),
                ColumnName = "FreeEnd"
            };
            m_referencePlanes.Columns.Add(column);

            column = new DataColumn
            {
                DataType = Type.GetType("System.String"),
                ColumnName = "Normal"
            };
            m_referencePlanes.Columns.Add(column);

            // Make the ID column the primary key column.
            var primaryKeyColumns = new DataColumn[1];
            primaryKeyColumns[0] = m_referencePlanes.Columns["ID"];
            m_referencePlanes.PrimaryKey = primaryKeyColumns;
        }

        private string Format(XYZ point)
        {
            return $"({Math.Round(point.X, 2)}, {Math.Round(point.Y, 2)}, {Math.Round(point.Z, 2)})";
        }

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

        private void OperateWall(Element host)
        {
            var wall = host as Wall;
            var bubbleEnd = new XYZ();
            var freeEnd = new XYZ();
            var cutVec = new XYZ();

            LocateWall(wall, ref bubbleEnd, ref freeEnd, ref cutVec);
            m_document.Document.Create.NewReferencePlane(bubbleEnd, freeEnd, cutVec, m_document.Document.ActiveView);
        }

        private void OperateSlab(Element host)
        {
            var floor = host as Floor;
            var bubbleEnd = new XYZ();
            var freeEnd = new XYZ();
            var thirdPnt = new XYZ();
            LocateSlab(floor, ref bubbleEnd, ref freeEnd, ref thirdPnt);
            m_document.Document.Create.NewReferencePlane2(bubbleEnd, freeEnd, thirdPnt, m_document.Document.ActiveView);
        }

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
            var length = XyzMath.GetLength(locaCurve.GetEndPoint(0), locaCurve.GetEndPoint(1));
            var xAxis = FaceAndSolidGeometry.GetDistance(locaCurve.GetEndPoint(0).X, locaCurve.GetEndPoint(1).X);
            var yAxis = FaceAndSolidGeometry.GetDistance(locaCurve.GetEndPoint(0).Y, locaCurve.GetEndPoint(1).Y);

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

            bubbleEnd = new XYZ(locaCurve.GetEndPoint(0).X + xOffset,
                locaCurve.GetEndPoint(0).Y + yOffset, locaCurve.GetEndPoint(0).Z);
            freeEnd = new XYZ(locaCurve.GetEndPoint(1).X + xOffset,
                locaCurve.GetEndPoint(1).Y + yOffset, locaCurve.GetEndPoint(1).Z);
            cutVec = new XYZ(0, 0, 1);
        }

        private void LocateSlab(Floor floor, ref XYZ bubbleEnd, ref XYZ freeEnd, ref XYZ thirdPnt)
        {
            var geometry = floor.get_Geometry(m_options);
            Face buttomFace = null;

            var objects = geometry.GetEnumerator();
            while (objects.MoveNext())
            {
                var go = objects.Current;

                var solid = go as Solid;
                if (null == solid)
                    continue;
                buttomFace = SampleBrowserUtils.GetBottomFace(solid.Faces);
            }

            var mesh = buttomFace.Triangulate();
            SampleBrowserUtils.Distribute(mesh, ref bubbleEnd, ref freeEnd, ref thirdPnt);
        }

        private delegate void CreateDelegate(Element host);
    }
}

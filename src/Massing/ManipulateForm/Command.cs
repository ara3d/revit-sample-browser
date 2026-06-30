// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;

namespace Ara3D.RevitSampleBrowser.Massing.ManipulateForm.CS
{
    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    [Journaling(JournalingMode.NoCommandData)]
    public class Command : IExternalCommand
    {
        private const double Epsilon = 0.000001;

        private readonly double m_bottomHeight = 0;

        private readonly double m_bottomLength = 200;

        private readonly double m_bottomWidth = 120;

        private readonly double m_profileOffset = 10;

        private Application m_revitApp;

        private Document m_revitDoc;

        private readonly double m_topHeight = 40;

        private readonly double m_topLength = 140;

        private readonly double m_topWidth = 60;

        private readonly double m_vertexOffsetOnBottomProfile = 20;

        private readonly double m_vertexOffsetOnMiddleProfile = 10;

        public virtual Result Execute(ExternalCommandData commandData
            , ref string message, ElementSet elements)
        {
            m_revitApp = commandData.Application.Application;
            m_revitDoc = commandData.Application.ActiveUIDocument.Document;
            Transaction transaction = new(m_revitDoc, "ManipulateForm");

            try
            {
                transaction.Start();

                // Create a loft form
                var form = CreateLoft();
                m_revitDoc.Regenerate();
                // Add profile to the loft form
                var profileIndex = AddProfile(form);
                m_revitDoc.Regenerate();
                // Move the edges on added profile
                MoveEdgesOnProfile(form, profileIndex);
                m_revitDoc.Regenerate();
                // Move the added profile
                MoveProfile(form, profileIndex);
                m_revitDoc.Regenerate();
                // Move the vertex on bottom profile
                MoveVertexesOnBottomProfile(form);
                m_revitDoc.Regenerate();
                // Add edge to the loft form
                var edgeReference = AddEdge(form);
                m_revitDoc.Regenerate();
                // Move the added edge
                XYZ offset = new(0, -40, 0);
                MoveSubElement(form, edgeReference, offset);
                m_revitDoc.Regenerate();
                // Move the vertex on added profile
                MoveVertexesOnAddedProfile(form, profileIndex);
                m_revitDoc.Regenerate();

                transaction.Commit();
            }
            catch (Exception ex)
            {
                message = ex.Message;
                transaction.RollBack();
                return Result.Failed;
            }

            return Result.Succeeded;
        }

        private Form CreateLoft()
        {
            // Prepare profiles for loft creation
            ReferenceArrayArray profiles = new();
            ReferenceArray bottomProfile = new();
            bottomProfile = CreateProfile(m_bottomLength, m_bottomWidth, m_bottomHeight);
            profiles.Append(bottomProfile);
            ReferenceArray topProfile = new();
            topProfile = CreateProfile(m_topLength, m_topWidth, m_topHeight);
            profiles.Append(topProfile);

            // return the created loft form
            return m_revitDoc.FamilyCreate.NewLoftForm(true, profiles);
        }

        private ReferenceArray CreateProfile(double length, double width, double height)
        {
            ReferenceArray profile = new();
            // Prepare points to create lines
            List<XYZ> points =
            [
                new XYZ(-1 * length / 2, -1 * width / 2, height),
                new XYZ(length / 2, -1 * width / 2, height),
                new XYZ(length / 2, width / 2, height),
                new XYZ(-1 * length / 2, width / 2, height)
            ];

            // Prepare sketch plane to create model line
            XYZ normal = new(0, 0, 1);
            XYZ origin = new(0, 0, height);
            var geometryPlane = Plane.CreateByNormalAndOrigin(normal, origin);
            var sketchPlane = SketchPlane.Create(m_revitDoc, geometryPlane);

            // Create model lines and get their references as the profile
            for (var i = 0; i < 4; i++)
            {
                var startPoint = points[i];
                var endPoint = i == 3 ? points[0] : points[i + 1];
                var line = Line.CreateBound(startPoint, endPoint);
                var modelLine = m_revitDoc.FamilyCreate.NewModelCurve(line, sketchPlane);
                profile.Append(modelLine.GeometryCurve.Reference);
            }

            return profile;
        }

        private int AddProfile(Form form)
        {
            // Get a connecting edge from the form
            XYZ startOfTop = new(-1 * m_topLength / 2, -1 * m_topWidth / 2, m_topHeight);
            XYZ startOfBottom = new(-1 * m_bottomLength / 2, -1 * m_bottomWidth / 2, m_bottomHeight);
            var connectingEdge = GetEdgeByEndPoints(form, startOfTop, startOfBottom);

            // Add an profile with specific parameters
            var param = 0.5;
            return form.AddProfile(connectingEdge.Reference, param);
        }

        private void MoveProfile(Form form, int profileIndex)
        {
            XYZ offset = new(0, 0, 5);
            if (form.CanManipulateProfile(profileIndex)) form.MoveProfile(profileIndex, offset);
        }

        private void MoveEdgesOnProfile(Form form, int profileIndex)
        {
            XYZ startOfTop = new(-1 * m_topLength / 2, -1 * m_topWidth / 2, m_topHeight);
            XYZ offset1 = new(m_profileOffset, 0, 0);
            XYZ offset2 = new(-m_profileOffset, 0, 0);
            Reference r1 = null;
            Reference r2 = null;
            var ra = form.get_CurveLoopReferencesOnProfile(profileIndex, 0);
            foreach (Reference r in ra)
            {
                var line = form.GetGeometryObjectFromReference(r) as Line;
                if (line == null) throw new Exception("Get curve reference on profile as line error.");
                var pnt1 = line.Evaluate(0, false);
                var pnt2 = line.Evaluate(1, false);
                if (Math.Abs(pnt1.X - pnt2.X) < Epsilon)
                {
                    if (pnt1.X < startOfTop.X)
                        r1 = r;
                    else
                        r2 = r;
                }
            }

            if (r1 == null || r2 == null) throw new Exception("Get line on profile error.");
            MoveSubElement(form, r1, offset1);
            MoveSubElement(form, r2, offset2);
        }

        private void MoveVertexesOnBottomProfile(Form form)
        {
            XYZ offset1 = new(-m_vertexOffsetOnBottomProfile, -m_vertexOffsetOnBottomProfile, 0);
            XYZ offset2 = new(m_vertexOffsetOnBottomProfile, -m_vertexOffsetOnBottomProfile, 0);

            XYZ startOfBottom = new(-1 * m_bottomLength / 2, -1 * m_bottomWidth / 2, m_bottomHeight);
            XYZ endOfBottom = new(m_bottomLength / 2, -1 * m_bottomWidth / 2, m_bottomHeight);
            var bottomEdge = GetEdgeByEndPoints(form, startOfBottom, endOfBottom);
            var pntsRef = form.GetControlPoints(bottomEdge.Reference);
            Reference r1 = null;
            Reference r2 = null;
            foreach (Reference r in pntsRef)
            {
                var pnt = form.GetGeometryObjectFromReference(r) as Point;
                if (pnt.Coord.IsAlmostEqualTo(startOfBottom))
                    r1 = r;
                else
                    r2 = r;
            }

            MoveSubElement(form, r1, offset1);
            MoveSubElement(form, r2, offset2);
        }

        private void MoveVertexesOnAddedProfile(Form form, int profileIndex)
        {
            XYZ offset = new(0, m_vertexOffsetOnMiddleProfile, 0);

            var ra = form.get_CurveLoopReferencesOnProfile(profileIndex, 0);
            foreach (Reference r in ra)
            {
                var ra2 = form.GetControlPoints(r);
                foreach (Reference r2 in ra2)
                {
                    var vertex = form.GetGeometryObjectFromReference(r2) as Point;
                    if (Math.Abs(vertex.Coord.X) < Epsilon)
                    {
                        MoveSubElement(form, r2, offset);
                        break;
                    }
                }
            }
        }

        private Reference AddEdge(Form form)
        {
            // Get two specific edges from the form
            XYZ startOfTop = new(-1 * m_topLength / 2, -1 * m_topWidth / 2, m_topHeight);
            XYZ endOfTop = new(m_topLength / 2, -1 * m_topWidth / 2, m_topHeight);
            var topEdge = GetEdgeByEndPoints(form, startOfTop, endOfTop);
            XYZ startOfBottom = new(-1 * ((m_bottomLength / 2) + m_vertexOffsetOnBottomProfile),
                -1 * ((m_bottomWidth / 2) + m_vertexOffsetOnBottomProfile), m_bottomHeight);
            XYZ endOfBottom = new((m_bottomLength / 2) + m_vertexOffsetOnBottomProfile,
                -1 * ((m_bottomWidth / 2) + m_vertexOffsetOnBottomProfile), m_bottomHeight);
            var bottomEdge = GetEdgeByEndPoints(form, startOfBottom, endOfBottom);

            // Add an edge between the two edges with specific parameters
            var topParam = 0.5;
            var bottomParam = 0.5;
            form.AddEdge(topEdge.Reference, topParam, bottomEdge.Reference, bottomParam);
            m_revitDoc.Regenerate();

            // Get the added edge and return its reference
            var startOfAddedEdge = startOfTop.Add(endOfTop.Subtract(startOfTop).Multiply(topParam));
            var endOfAddedEdge = startOfBottom.Add(endOfBottom.Subtract(startOfBottom).Multiply(bottomParam));
            return GetEdgeByEndPoints(form, startOfAddedEdge, endOfAddedEdge).Reference;
        }

        private Edge GetEdgeByEndPoints(Form form, XYZ startPoint, XYZ endPoint)
        {
            Edge edge = null;

            // Get all edges of the form
            EdgeArray edges = null;
            var geoOptions = m_revitApp.Create.NewGeometryOptions();
            geoOptions.ComputeReferences = true;
            var geoElement = form.get_Geometry(geoOptions);
            //foreach (GeometryObject geoObject in geoElement.Objects)
            var objects = geoElement.GetEnumerator();
            while (objects.MoveNext())
            {
                var geoObject = objects.Current;

                var solid = geoObject as Solid;
                if (null == solid)
                    continue;
                edges = solid.Edges;
            }

            // Traverse the edges and look for the edge with the right endpoints
            foreach (Edge ed in edges)
            {
                var rpPos1 = ed.Evaluate(0);
                var rpPos2 = ed.Evaluate(1);
                if ((startPoint.IsAlmostEqualTo(rpPos1) && endPoint.IsAlmostEqualTo(rpPos2)) ||
                    (startPoint.IsAlmostEqualTo(rpPos2) && endPoint.IsAlmostEqualTo(rpPos1)))
                {
                    edge = ed;
                    break;
                }
            }

            return edge;
        }

        private void MoveSubElement(Form form, Reference subElemReference, XYZ offset)
        {
            if (form.CanManipulateSubElement(subElemReference)) form.MoveSubElement(subElemReference, offset);
        }
    }
}

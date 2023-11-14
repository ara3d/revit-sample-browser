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
using System.Diagnostics;
using System.IO;
using System.Reflection;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Form = System.Windows.Forms.Form;

namespace Revit.SDK.Samples.RayTraceBounce.CS
{
    /// <summary>
    ///     1. This form allowing entry of a coordinate location (X, Y, Z) within the model and a coordinate direction (i, j,
    ///     k).
    ///     2. Launch a ray from this location in this direction to find the first intersection with a face
    ///     3. Calculate the reflection angle of the ray from the face and launch another ray to find the next intersection
    ///     4. For each ray/intersection, create a model line connecting the two points.  The end result should be a series of
    ///     model lines bouncing from item to item.
    ///     5. Provide a hard limit of say, 100 intersections, to prevent endless reflections within an enclosed space.
    ///     6. Write a log file of the intersection containing: the element type, id, and material of the intersected face.
    /// </summary>
    public partial class RayTraceBounceForm : Form
    {
        /// <summary>
        ///     current assembly name
        /// </summary>
        private static readonly string AssemblyName = Assembly.GetExecutingAssembly().Location;

        /// <summary>
        ///     current assembly directory
        /// </summary>
        private static string AssemblyDirectory = Path.GetDirectoryName(AssemblyName);

        /// <summary>
        ///     epsilon limit
        /// </summary>
        private static readonly double epsilon = 0.00000001;

        /// <summary>
        ///     how many bounces to run
        /// </summary>
        private static readonly int rayLimit = 100;

        /// <summary>
        ///     revit application
        /// </summary>
        private readonly UIApplication m_app;

        /// <summary>
        ///     ray direction
        /// </summary>
        private XYZ m_direction = new XYZ(0, 0, 0);

        /// <summary>
        ///     current document
        /// </summary>
        private readonly Document m_doc;

        /// <summary>
        ///     the face which the ray intersect with
        /// </summary>
        private Face m_face;

        /// <summary>
        ///     the count of line between origin/intersection and ray/intersection
        /// </summary>
        private int m_LineCount;

        /// <summary>
        ///     ray start from here
        /// </summary>
        private XYZ m_origin = new XYZ(0, 0, 0);

        /// <summary>
        ///     output string list
        /// </summary>
        private readonly List<string> m_outputInfo = new List<string>();

        /// <summary>
        ///     the count of ray between origin/intersection and ray/intersection
        /// </summary>
        private int m_RayCount;

        /// <summary>
        ///     closet geometry reference between origin/intersection and ray/intersection
        /// </summary>
        private ReferenceWithContext m_rClosest;

        /// <summary>
        ///     for time calculation
        /// </summary>
        private readonly Stopwatch m_stopWatch = new Stopwatch();

        /// <summary>
        ///     trace listener
        /// </summary>
        private readonly TraceListener m_txtListener;

        /// <summary>
        ///     3D view
        /// </summary>
        private readonly View3D m_view;

        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name="commandData">Revit application</param>
        /// <param name="v">3D View</param>
        public RayTraceBounceForm(ExternalCommandData commandData, View3D v)
        {
            InitializeComponent();

            var logFile = AssemblyName.Replace(".dll", DateTime.Now.ToString("yyyyMMddhhmmss") + ".log");
            if (File.Exists(logFile)) File.Delete(logFile);
            m_txtListener = new TextWriterTraceListener(logFile);
            Trace.Listeners.Add(m_txtListener);

            m_app = commandData.Application;
            m_doc = commandData.Application.ActiveUIDocument.Document;
            m_view = v;
            UpdateData(true);
            // Delete lines it created
            DeleteLines();
        }

        /// <summary>
        ///     OK button click event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonOK_Click(object sender, EventArgs e)
        {
            DeleteLines();
            if (!UpdateData(false)) return;
            m_outputInfo.Clear();
            m_stopWatch.Start();
            var transaction = new Transaction(m_doc, "RayTraceBounce");
            transaction.Start();
            m_LineCount = 0;
            m_RayCount = 0;
            // Start Find References By Direction
            var startpt = m_origin;
            m_outputInfo.Add("Start Find References By Direction: ");
            for (var ctr = 1; ctr <= rayLimit; ctr++)
            {
                var referenceIntersector = new ReferenceIntersector(m_view);
                var references = referenceIntersector.Find(startpt, m_direction);
                m_rClosest = null;
                FindClosestReference(references);
                if (m_rClosest == null)
                {
                    var info = "Ray " + ctr + " aborted. No closest face reference found. ";
                    m_outputInfo.Add(info);
                    if (ctr == 1) TaskDialog.Show("Revit", info);
                    break;
                }

                var reference = m_rClosest.GetReference();
                var referenceElement = m_doc.GetElement(reference);
                var referenceObject = referenceElement.GetGeometryObjectFromReference(reference);
                var endpt = reference.GlobalPoint;
                if (startpt.IsAlmostEqualTo(endpt))
                {
                    m_outputInfo.Add("Start and end points are equal. Ray " + ctr + " aborted\n" + startpt.X + ", " +
                                     startpt.Y + ", " + startpt.Z);
                    break;
                }

                {
                    MakeLine(startpt, endpt, m_direction, "bounce");
                    m_RayCount = m_RayCount + 1;
                    var info = "Intersected Element Type: [" + referenceElement.GetType() + "] ElementId: [" +
                               referenceElement.Id;
                    m_face = referenceObject as Face;
                    if (m_face.MaterialElementId != ElementId.InvalidElementId)
                    {
                        var materialElement = m_doc.GetElement(m_face.MaterialElementId) as Material;
                        info += "] Face MaterialElement Name: [" + materialElement.Name + "] Shininess: [" +
                                materialElement.Shininess;
                    }
                    else
                    {
                        info += "] Face.MaterialElement is null [" + referenceElement.Category.Name;
                    }

                    info += "]";
                    m_outputInfo.Add(info);
                    var endptUV = reference.UVPoint;
                    var FaceNormal = m_face.ComputeDerivatives(endptUV).BasisZ; // face normal where ray hits
                    FaceNormal =
                        m_rClosest.GetInstanceTransform()
                            .OfVector(
                                FaceNormal); // transformation to get it in terms of document coordinates instead of the parent symbol
                    var directionMirrored =
                        m_direction -
                        2 * m_direction.DotProduct(FaceNormal) *
                        FaceNormal; //http://www.fvastro.org/presentations/ray_tracing.htm
                    m_direction = directionMirrored; // get ready to shoot the next ray
                    startpt = endpt;
                }
            }

            transaction.Commit();
            m_stopWatch.Stop();
            var ts = m_stopWatch.Elapsed;
            var elapsedTime = string.Format("{0:00}:{1:00}:{2:00}.{3:00}", ts.Hours, ts.Minutes, ts.Seconds,
                ts.Milliseconds / 10);
            m_outputInfo.Add(elapsedTime + "\n" + "Lines = " + m_LineCount + "\n" + "Rays = " + m_RayCount);
            m_stopWatch.Reset();
            OutputInformation();
        }

        /// <summary>
        ///     Cancel button click event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonCancel_Click(object sender, EventArgs e)
        {
            Close();
        }

        /// <summary>
        ///     Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            Trace.Flush();
            m_txtListener.Close();
            Trace.Close();
            Trace.Listeners.Remove(m_txtListener);

            if (disposing && components != null) components.Dispose();
            base.Dispose(disposing);
        }

        /// <summary>
        ///     Update textbox data with member variable
        /// </summary>
        /// <param name="updateControl">if get/set date from control</param>
        public bool UpdateData(bool updateControl)
        {
            try
            {
                if (updateControl)
                {
                    textBoxLocationX.Text = Convert.ToString(0.0);
                    textBoxLocationY.Text = Convert.ToString(0.0);
                    textBoxLocationZ.Text = Convert.ToString(0.0);

                    textBoxDirectionI.Text = Convert.ToString(Math.Cos(0));
                    textBoxDirectionJ.Text = Convert.ToString(Math.Sin(0));
                    textBoxDirectionK.Text = Convert.ToString(0.0);
                }
                else
                {
                    if (string.IsNullOrEmpty(textBoxLocationX.Text) || string.IsNullOrEmpty(textBoxLocationY.Text) ||
                        string.IsNullOrEmpty(textBoxLocationZ.Text) || string.IsNullOrEmpty(textBoxDirectionI.Text) ||
                        string.IsNullOrEmpty(textBoxDirectionJ.Text) || string.IsNullOrEmpty(textBoxDirectionK.Text))
                    {
                        TaskDialog.Show("Revit", "Value cannot be empty.");
                        return false;
                    }

                    m_origin = new XYZ(
                        Convert.ToDouble(textBoxLocationX.Text),
                        Convert.ToDouble(textBoxLocationY.Text),
                        Convert.ToDouble(textBoxLocationZ.Text));

                    m_direction = new XYZ(
                        Convert.ToDouble(textBoxDirectionI.Text),
                        Convert.ToDouble(textBoxDirectionJ.Text),
                        Convert.ToDouble(textBoxDirectionK.Text));
                }

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        ///     Find the first intersection with a face
        /// </summary>
        /// <param name="references"></param>
        /// <returns></returns>
        public ReferenceWithContext FindClosestReference(IList<ReferenceWithContext> references)
        {
            var face_prox = double.PositiveInfinity;
            var edge_prox = double.PositiveInfinity;
            foreach (var r in references)
            {
                var reference = r.GetReference();
                var referenceElement = m_doc.GetElement(reference);
                var referenceGeometryObject = referenceElement.GetGeometryObjectFromReference(reference);
                m_face = null;
                m_face = referenceGeometryObject as Face;
                var edge = referenceGeometryObject as Edge;
                if (m_face != null)
                {
                    if (r.Proximity < face_prox && r.Proximity > epsilon)
                    {
                        m_rClosest = r;
                        face_prox = Math.Abs(r.Proximity);
                    }
                }
                else if (edge != null)
                {
                    if (r.Proximity < edge_prox && r.Proximity > epsilon) edge_prox = Math.Abs(r.Proximity);
                }
            }

            if (edge_prox <= face_prox)
            {
                // stop bouncing if there is an edge at least as close as the nearest face - there is no single angle of reflection for a ray striking a line
                m_outputInfo.Add(
                    "there is an edge at least as close as the nearest face - there is no single angle of reflection for a ray striking a line");
                m_rClosest = null;
            }

            return m_rClosest;
        }

        /// <summary>
        ///     Make a line from start point to end point with the direction and style
        /// </summary>
        /// <param name="startpt">start point</param>
        /// <param name="endpt">end point</param>
        /// <param name="direction">the direction which decide the plane</param>
        /// <param name="style">line style name</param>
        public void MakeLine(XYZ startpt, XYZ endpt, XYZ direction, string style)
        {
            try
            {
                m_LineCount = m_LineCount + 1;
                var line = Line.CreateBound(startpt, endpt);
                // Line must lie in the sketch plane.  Use the direction of the line to construct a plane that hosts the target line.
                var rotatedDirection = XYZ.BasisX;

                // If the direction is not vertical, cross the direction vector with Z to get a vector rotated ninety degrees.  That vector, 
                // plus the original vector, form the axes of the sketch plane.
                if (!direction.IsAlmostEqualTo(XYZ.BasisZ) && !direction.IsAlmostEqualTo(-XYZ.BasisZ))
                    rotatedDirection = direction.Normalize().CrossProduct(XYZ.BasisZ);
                var geometryPlane = Plane.CreateByOriginAndBasis(direction, rotatedDirection, startpt);
                var skplane = SketchPlane.Create(m_app.ActiveUIDocument.Document, geometryPlane);
                var mcurve = m_app.ActiveUIDocument.Document.Create.NewModelCurve(line, skplane);
                m_app.ActiveUIDocument.Document.Regenerate();
                //ElementArray lsArr = mcurve.LineStyles;
                var lsArr = mcurve.GetLineStyleIds();
                foreach (var eid in lsArr)
                {
                    var e = m_app.ActiveUIDocument.Document.GetElement(eid);

                    if (e.Name == style)
                    {
                        mcurve.LineStyle = e;
                        break;
                    }
                }

                m_app.ActiveUIDocument.Document.Regenerate();
            }
            catch (Exception ex)
            {
                m_outputInfo.Add("Failed to create lines: " + ex);
            }
        }

        /// <summary>
        ///     Delete all unnecessary lines
        /// </summary>
        public void DeleteLines()
        {
            try
            {
                var transaction = new SubTransaction(m_app.ActiveUIDocument.Document);
                transaction.Start();
                var list = new List<Element>();
                var filter = new ElementClassFilter(typeof(CurveElement));
                var collector = new FilteredElementCollector(m_app.ActiveUIDocument.Document);
                list.AddRange(collector.WherePasses(filter).ToElements());
                foreach (var e in list)
                {
                    var mc = e as ModelCurve;
                    if (mc != null)
                        if (mc.LineStyle.Name == "bounce" || mc.LineStyle.Name == "normal")
                            m_app.ActiveUIDocument.Document.Delete(e.Id);
                }

                transaction.Commit();
            }
            catch (Exception)
            {
            }
        }

        /// <summary>
        ///     Output the information to log file
        /// </summary>
        protected void OutputInformation()
        {
            foreach (var item in m_outputInfo) Trace.WriteLine(item);
            Trace.Flush();
        }
    }
}
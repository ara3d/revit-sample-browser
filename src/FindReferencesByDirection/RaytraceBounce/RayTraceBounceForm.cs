// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using Form = System.Windows.Forms.Form;

namespace Ara3D.RevitSampleBrowser.FindReferencesByDirection.RaytraceBounce.CS
{
    public partial class RayTraceBounceForm : Form
    {
        private static readonly string AssemblyName = Assembly.GetExecutingAssembly().Location;
        private static readonly string _assemblyDirectory = Path.GetDirectoryName(AssemblyName);
        private static readonly double Epsilon = 0.00000001;
        private static readonly int RayLimit = 100;

        private readonly UIApplication m_app;
        private XYZ m_direction = new(0, 0, 0);
        private readonly Document m_doc;
        private Face m_face;
        private int m_lineCount;
        private XYZ m_origin = new(0, 0, 0);
        private readonly List<string> m_outputInfo = [];
        private int m_rayCount;
        private ReferenceWithContext m_rClosest;
        private readonly Stopwatch m_stopWatch = new();
        private readonly TraceListener m_txtListener;
        private readonly View3D m_view;

        public RayTraceBounceForm(ExternalCommandData commandData, View3D v)
        {
            InitializeComponent();

            var logFile = AssemblyName.Replace(".dll", $"{DateTime.Now:yyyyMMddhhmmss}.log");
            if (File.Exists(logFile)) File.Delete(logFile);
            m_txtListener = new TextWriterTraceListener(logFile);
            Trace.Listeners.Add(m_txtListener);

            m_app = commandData.Application;
            m_doc = commandData.Application.ActiveUIDocument.Document;
            m_view = v;
            UpdateData(true);
            DeleteLines();
        }

        private void buttonOK_Click(object sender, EventArgs e)
        {
            DeleteLines();
            if (!UpdateData(false)) return;
            m_outputInfo.Clear();
            m_stopWatch.Start();
            Transaction transaction = new(m_doc, "RayTraceBounce");
            transaction.Start();
            m_lineCount = 0;
            m_rayCount = 0;
            var startpt = m_origin;
            m_outputInfo.Add("Start Find References By Direction: ");
            for (var ctr = 1; ctr <= RayLimit; ctr++)
            {
                ReferenceIntersector referenceIntersector = new(m_view);
                var references = referenceIntersector.Find(startpt, m_direction);
                m_rClosest = null;
                FindClosestReference(references);
                if (m_rClosest == null)
                {
                    var info = $"Ray {ctr} aborted. No closest face reference found. ";
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
                    m_outputInfo.Add(
                        $"Start and end points are equal. Ray {ctr} aborted\n{startpt.X}, {startpt.Y}, {startpt.Z}");
                    break;
                }

                {
                    MakeLine(startpt, endpt, m_direction, "bounce");
                    m_rayCount++;
                    var info =
                        $"Intersected Element Type: [{referenceElement.GetType()}] ElementId: [{referenceElement.Id}";
                    m_face = referenceObject as Face;
                    if (m_face.MaterialElementId != ElementId.InvalidElementId)
                    {
                        var materialElement = m_doc.GetElement(m_face.MaterialElementId) as Material;
                        info +=
                            $"] Face MaterialElement Name: [{materialElement.Name}] Shininess: [{materialElement.Shininess}";
                    }
                    else
                    {
                        info += $"] Face.MaterialElement is null [{referenceElement.Category.Name}";
                    }

                    info += "]";
                    m_outputInfo.Add(info);
                    var endptUv = reference.UVPoint;
                    var faceNormal = m_face.ComputeDerivatives(endptUv).BasisZ;
                    // Transform face normal from symbol space to document coordinates.
                    faceNormal =
                        m_rClosest.GetInstanceTransform()
                            .OfVector(
                                faceNormal);
                    var directionMirrored =
                        m_direction -
                        (2 * m_direction.DotProduct(faceNormal) *
                        faceNormal);
                    m_direction = directionMirrored;
                    startpt = endpt;
                }
            }

            transaction.Commit();
            m_stopWatch.Stop();
            var ts = m_stopWatch.Elapsed;
            var elapsedTime = string.Format("{0:00}:{1:00}:{2:00}.{3:00}", ts.Hours, ts.Minutes, ts.Seconds,
                ts.Milliseconds / 10);
            m_outputInfo.Add($"{elapsedTime}\nLines = {m_lineCount}\nRays = {m_rayCount}");
            m_stopWatch.Reset();
            OutputInformation();
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            Close();
        }

        protected override void Dispose(bool disposing)
        {
            Trace.Flush();
            m_txtListener.Close();
            Trace.Close();
            Trace.Listeners.Remove(m_txtListener);

            if (disposing && components != null) components.Dispose();
            base.Dispose(disposing);
        }

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

        public ReferenceWithContext FindClosestReference(IList<ReferenceWithContext> references)
        {
            var faceProx = double.PositiveInfinity;
            var edgeProx = double.PositiveInfinity;
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
                    if (r.Proximity < faceProx && r.Proximity > Epsilon)
                    {
                        m_rClosest = r;
                        faceProx = Math.Abs(r.Proximity);
                    }
                }
                else if (edge != null)
                {
                    if (r.Proximity < edgeProx && r.Proximity > Epsilon) edgeProx = Math.Abs(r.Proximity);
                }
            }

            if (edgeProx <= faceProx)
            {
                // No single reflection angle when a ray hits an edge as close as the nearest face.
                m_outputInfo.Add(
                    "there is an edge at least as close as the nearest face - there is no single angle of reflection for a ray striking a line");
                m_rClosest = null;
            }

            return m_rClosest;
        }

        public void MakeLine(XYZ startpt, XYZ endpt, XYZ direction, string style)
        {
            try
            {
                m_lineCount++;
                var line = Line.CreateBound(startpt, endpt);
                // Model curves must lie in the sketch plane; derive plane axes from the ray direction.
                var rotatedDirection = XYZ.BasisX;

                if (!direction.IsAlmostEqualTo(XYZ.BasisZ) && !direction.IsAlmostEqualTo(-XYZ.BasisZ))
                    rotatedDirection = direction.Normalize().CrossProduct(XYZ.BasisZ);
                var geometryPlane = Plane.CreateByOriginAndBasis(direction, rotatedDirection, startpt);
                var skplane = SketchPlane.Create(m_app.ActiveUIDocument.Document, geometryPlane);
                var mcurve = m_app.ActiveUIDocument.Document.Create.NewModelCurve(line, skplane);
                m_app.ActiveUIDocument.Document.Regenerate();
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
                m_outputInfo.Add($"Failed to create lines: {ex}");
            }
        }

        public void DeleteLines()
        {
            try
            {
                SubTransaction transaction = new(m_app.ActiveUIDocument.Document);
                transaction.Start();
                List<Element> list = new();
                ElementClassFilter filter = new(typeof(CurveElement));
                FilteredElementCollector collector = new(m_app.ActiveUIDocument.Document);
                list.AddRange(collector.WherePasses(filter).ToElements());
                foreach (var e in list)
                {
                    if (e is ModelCurve mc)
                        if (mc.LineStyle.Name is "bounce" or "normal")
                            m_app.ActiveUIDocument.Document.Delete(e.Id);
                }

                transaction.Commit();
            }
            catch (Exception)
            {
            }
        }

        protected void OutputInformation()
        {
            foreach (var item in m_outputInfo)
            {
                Trace.WriteLine(item);
            }

            Trace.Flush();
        }
    }
}

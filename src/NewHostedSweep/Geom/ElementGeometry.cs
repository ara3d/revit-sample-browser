// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using Color = System.Drawing.Color;

namespace Ara3D.RevitSampleBrowser.NewHostedSweep.CS.Geom
{
    public class ElementGeometry
    {
        private readonly XYZ m_bBoxMax;

        private readonly XYZ m_bBoxMin;

        /// <summary>
        ///     If the solid is transformed (includes translation, scale and rotation)
        ///     this flag should be true, otherwise false.
        /// </summary>
        private bool m_isDirty;

        private Transform m_rotation;

        private double m_scale;
        private XYZ m_translation;

        public ElementGeometry(Solid solid, BoundingBoxXYZ box)
        {
            Solid = solid;
            m_bBoxMin = box.Min;
            m_bBoxMax = box.Max;
            m_isDirty = true;

            // Initialize edge binding
            EdgeBindingDic = [];
            foreach (Edge edge in Solid.Edges)
            {
                EdgeBinding edgeBingding = new(edge);
                EdgeBindingDic.Add(edge, edgeBingding);
            }
        }

        public XYZ Translation
        {
            get => m_translation;
            set
            {
                m_isDirty = true;
                m_translation = value;
            }
        }

        public double Scale
        {
            get => m_scale;
            set
            {
                m_isDirty = true;
                m_scale = value;
            }
        }

        public Transform Rotation
        {
            get => m_rotation;
            set
            {
                m_isDirty = true;
                m_rotation = value;
            }
        }

        public Solid Solid { get; }

        public Dictionary<Edge, EdgeBinding> EdgeBindingDic { get; }

        public void InitializeTransform(double width, double height)
        {
            // Initialize translation and rotation transform
            var bBoxCenter = (m_bBoxMax + m_bBoxMin) / 2.0;
            m_translation = -bBoxCenter;
            m_rotation = Transform.Identity;

            // Initialize scale factor
            var bBoxWidth = m_bBoxMax.X - m_bBoxMin.X;
            var bBoxHeight = m_bBoxMax.Y - m_bBoxMin.Y;
            var widthScale = width / bBoxWidth;
            var heigthScale = height / bBoxHeight;
            m_scale = Math.Min(widthScale, heigthScale);

            // Set dirty flag
            m_isDirty = true;
        }

        public void ResetEdgeStates()
        {
            foreach (var pair in EdgeBindingDic)
            {
                pair.Value.Reset();
            }
        }

        private void Update()
        {
            if (!m_isDirty) return;

            foreach (var pair in EdgeBindingDic)
            {
                pair.Value.Update(m_rotation, m_translation, m_scale);
            }

            m_isDirty = false;
        }

        public void Draw(Graphics g)
        {
            Update();
            foreach (var pair in EdgeBindingDic)
            {
                pair.Value.Draw(g);
            }
        }
    }

    public class EdgeBinding : IDisposable
    {
        private GraphicsPath m_gdiEdge;
        private readonly Pen m_pen;

        private readonly IList<XYZ> m_points;

        private Region m_region;

        public EdgeBinding(Edge edge)
        {
            m_points = edge.Tessellate();
            m_pen = new Pen(Color.White);
            Reset();
        }

        public bool IsHighLighted { get; set; }

        public bool IsSelected { get; set; }

        public void Dispose()
        {
            m_gdiEdge.Dispose();
            m_pen.Dispose();
            m_region.Dispose();
        }

        public void Reset()
        {
            IsHighLighted = false;
            IsSelected = false;
            m_region = null;
        }

        public void Update(Transform rotation, XYZ translation, double scale)
        {
            rotation = rotation.Inverse;
            var points = new PointF[m_points.Count];
            for (var i = 0; i < m_points.Count; i++)
            {
                var tmpPt = m_points[i];
                tmpPt = rotation.OfPoint((tmpPt + translation) * scale);
                points[i] = new PointF((float)tmpPt.X, (float)tmpPt.Y);
            }

            m_gdiEdge?.Dispose();
            m_gdiEdge = new GraphicsPath();
            m_gdiEdge.AddLines(points);

            m_region?.Dispose();
            m_region = null;
        }

        public void Draw(Graphics g)
        {
            m_pen.Width = 2.0f;
            m_pen.Color = IsHighLighted ? Color.Yellow : IsSelected ? Color.Red : Color.Green;
            g.DrawPath(m_pen, m_gdiEdge);
        }

        private Region GetRegion()
        {
            if (m_region == null)
            {
                GraphicsPath tmpPath = new();
                tmpPath.AddLines(m_gdiEdge.PathPoints);
                Pen tmpPen = new(Color.White, 3.0f);
                tmpPath.Widen(tmpPen);
                m_region = new Region(tmpPath);
            }

            return m_region;
        }

        private bool HitTest(float x, float y)
        {
            return GetRegion().IsVisible(x, y);
        }

        /// <summary>
        ///     If the edge under the location (x, y), set the highlight flag to true,
        ///     otherwise false.
        /// </summary>
        /// <param name="x">X coordinate</param>
        /// <param name="y">Y coordinate</param>
        /// <returns></returns>
        public bool HighLight(float x, float y)
        {
            IsHighLighted = HitTest(x, y);
            return IsHighLighted;
        }
    }
}

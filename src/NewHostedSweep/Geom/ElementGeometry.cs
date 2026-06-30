// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using Autodesk.Revit.DB;
using Color = System.Drawing.Color;

namespace Ara3D.RevitSampleBrowser.NewHostedSweep.CS.Geom
{
    public class ElementGeometry
    {
        private readonly XYZ m_bBoxMax;

        private readonly XYZ m_bBoxMin;

        private readonly Dictionary<Edge, EdgeBinding> m_edgeBindinDic;

        /// <summary>
        ///     If the solid is transformed (includes translation, scale and rotation)
        ///     this flag should be true, otherwise false.
        /// </summary>
        private bool m_isDirty;

        private Transform m_rotation;

        private double m_scale;

        private readonly Solid m_solid;

        private XYZ m_translation;

        public ElementGeometry(Solid solid, BoundingBoxXYZ box)
        {
            m_solid = solid;
            m_bBoxMin = box.Min;
            m_bBoxMax = box.Max;
            m_isDirty = true;

            // Initialize edge binding
            m_edgeBindinDic = new Dictionary<Edge, EdgeBinding>();
            foreach (Edge edge in m_solid.Edges)
            {
                var edgeBingding = new EdgeBinding(edge);
                m_edgeBindinDic.Add(edge, edgeBingding);
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

        public Solid Solid => m_solid;

        public Dictionary<Edge, EdgeBinding> EdgeBindingDic => m_edgeBindinDic;

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
            foreach (var pair in m_edgeBindinDic)
            {
                pair.Value.Reset();
            }
        }

        private void Update()
        {
            if (!m_isDirty) return;

            foreach (var pair in m_edgeBindinDic)
            {
                pair.Value.Update(m_rotation, m_translation, m_scale);
            }

            m_isDirty = false;
        }

        public void Draw(Graphics g)
        {
            Update();
            foreach (var pair in m_edgeBindinDic)
            {
                pair.Value.Draw(g);
            }
        }
    }

    public class EdgeBinding : IDisposable
    {
        private GraphicsPath m_gdiEdge;

        private bool m_isHighLighted;

        private bool m_isSelected;

        private readonly Pen m_pen;

        private readonly IList<XYZ> m_points;

        private Region m_region;

        public EdgeBinding(Edge edge)
        {
            m_points = edge.Tessellate();
            m_pen = new Pen(Color.White);
            Reset();
        }

        public bool IsHighLighted
        {
            get => m_isHighLighted;
            set => m_isHighLighted = value;
        }

        public bool IsSelected
        {
            get => m_isSelected;
            set => m_isSelected = value;
        }

        public void Dispose()
        {
            m_gdiEdge.Dispose();
            m_pen.Dispose();
            m_region.Dispose();
        }

        public void Reset()
        {
            m_isHighLighted = false;
            m_isSelected = false;
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
            if (m_isHighLighted)
                m_pen.Color = Color.Yellow;
            else if (m_isSelected)
                m_pen.Color = Color.Red;
            else
                m_pen.Color = Color.Green;
            g.DrawPath(m_pen, m_gdiEdge);
        }

        private Region GetRegion()
        {
            if (m_region == null)
            {
                var tmpPath = new GraphicsPath();
                tmpPath.AddLines(m_gdiEdge.PathPoints);
                var tmpPen = new Pen(Color.White, 3.0f);
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
            m_isHighLighted = HitTest(x, y);
            return m_isHighLighted;
        }
    }
}

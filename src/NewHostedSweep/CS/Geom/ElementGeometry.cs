// Copyright 2023. See https://github.com/ara3d/revit-samples/LICENSE.txt

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using Autodesk.Revit.DB;
using Color = System.Drawing.Color;

namespace Revit.SDK.Samples.NewHostedSweep.CS
{
    /// <summary>
    ///     This class is intent to display element's wire-frame with C# GDI.
    ///     It contains a solid and a bounding box of an element.
    ///     It also contains transformation (translation, rotation and scale) to
    ///     transform the geometry edges.
    /// </summary>
    public class ElementGeometry
    {
        /// <summary>
        ///     Solid bounding box maximal corner.
        /// </summary>
        private readonly XYZ m_bBoxMax;

        /// <summary>
        ///     Solid bounding box minimal corner.
        /// </summary>
        private readonly XYZ m_bBoxMin;

        /// <summary>
        ///     Solid's Edge to EdgeBinding dictionary. It is intent to store all the edges
        ///     of solid.
        /// </summary>
        private readonly Dictionary<Edge, EdgeBinding> m_edgeBindinDic;

        /// <summary>
        ///     If the solid is transformed (includes translation, scale and rotation)
        ///     this flag should be true, otherwise false.
        /// </summary>
        private bool m_isDirty;

        /// <summary>
        ///     Rotation transform, it is intent to rotate the solid.
        /// </summary>
        private Transform m_rotation;

        /// <summary>
        ///     Scale transform, it is intent to scale the solid.
        /// </summary>
        private double m_scale;

        /// <summary>
        ///     Element's Solid
        /// </summary>
        private readonly Solid m_solid;

        /// <summary>
        ///     Translation transform, it is intent to translate the solid.
        ///     It is actually the center of Bounding box.
        /// </summary>
        private XYZ m_translation;

        /// <summary>
        ///     Constructor, Construct a new object with an element's geometry Solid,
        ///     and its corresponding bounding box.
        /// </summary>
        /// <param name="solid">Element's geometry Solid</param>
        /// <param name="box">Element's geometry bounding box</param>
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

        /// <summary>
        ///     Translation transform, it is intent to translate the solid.
        ///     It is actually the center of Bounding box.
        /// </summary>
        public XYZ Translation
        {
            get => m_translation;
            set
            {
                m_isDirty = true;
                m_translation = value;
            }
        }

        /// <summary>
        ///     Scale transform, it is intent to scale the solid.
        /// </summary>
        public double Scale
        {
            get => m_scale;
            set
            {
                m_isDirty = true;
                m_scale = value;
            }
        }

        /// <summary>
        ///     Rotation transform, it is intent to rotate the solid.
        /// </summary>
        public Transform Rotation
        {
            get => m_rotation;
            set
            {
                m_isDirty = true;
                m_rotation = value;
            }
        }

        /// <summary>
        ///     Element's Solid
        /// </summary>
        public Solid Solid => m_solid;

        /// <summary>
        ///     Solid's Edge to EdgeBinding dictionary. It is intent to store all the edges
        ///     of solid.
        /// </summary>
        public Dictionary<Edge, EdgeBinding> EdgeBindingDic => m_edgeBindinDic;

        /// <summary>
        ///     Initialize the transform (includes translation, scale, and rotation).
        /// </summary>
        /// <param name="width">Width of the view</param>
        /// <param name="height">Height of the view</param>
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

        /// <summary>
        ///     Reset all the edges' status to their original status.
        /// </summary>
        public void ResetEdgeStates()
        {
            foreach (var pair in m_edgeBindinDic) pair.Value.Reset();
        }

        /// <summary>
        ///     Update all the edges' transform (include translation, scale, and rotation),
        ///     reconstruct the edge's geometry info.
        /// </summary>
        private void Update()
        {
            if (!m_isDirty) return;

            foreach (var pair in m_edgeBindinDic) pair.Value.Update(m_rotation, m_translation, m_scale);
            m_isDirty = false;
        }

        /// <summary>
        ///     Draw all the edges of solid in Graphics.
        /// </summary>
        /// <param name="g">Graphics, edges will be draw in it</param>
        public void Draw(Graphics g)
        {
            Update();
            foreach (var pair in m_edgeBindinDic) pair.Value.Draw(g);
        }
    }

    /// <summary>
    ///     Binds an edge with some properties which contains its geometry information
    ///     and indicates whether the edge is selected or highlighted.
    /// </summary>
    public class EdgeBinding : IDisposable
    {
        /// <summary>
        ///     Edge geometry presentation in C# GDI.
        /// </summary>
        private GraphicsPath m_gdiEdge;

        /// <summary>
        ///     A flag to indicate the edge is highlighted or not.
        /// </summary>
        private bool m_isHighLighted;

        /// <summary>
        ///     A flag to indicate the edge is selected or not.
        /// </summary>
        private bool m_isSelected;

        /// <summary>
        ///     Pen for edge display.
        /// </summary>
        private readonly Pen m_pen;

        /// <summary>
        ///     Edge points in world coordinate system of Revit.
        /// </summary>
        private readonly IList<XYZ> m_points;

        /// <summary>
        ///     Edge bounding Region used to hit testing.
        /// </summary>
        private Region m_region;

        /// <summary>
        ///     Constructor takes Edge as parameter.
        /// </summary>
        /// <param name="edge">Edge</param>
        public EdgeBinding(Edge edge)
        {
            m_points = edge.Tessellate();
            m_pen = new Pen(Color.White);
            Reset();
        }

        /// <summary>
        ///     Gets whether the edge is highlighted or not.
        /// </summary>
        public bool IsHighLighted
        {
            get => m_isHighLighted;
            set => m_isHighLighted = value;
        }

        /// <summary>
        ///     Gets whether the edge is selected or not.
        /// </summary>
        public bool IsSelected
        {
            get => m_isSelected;
            set => m_isSelected = value;
        }


        /// <summary>
        ///     Dispose
        /// </summary>
        public void Dispose()
        {
            m_gdiEdge.Dispose();
            m_pen.Dispose();
            m_region.Dispose();
        }

        /// <summary>
        ///     Reset the status of the edge: un-highlighted, un-selected
        /// </summary>
        public void Reset()
        {
            m_isHighLighted = false;
            m_isSelected = false;
            m_region = null;
        }

        /// <summary>
        ///     Update the edge's geometry according to the transformation.
        /// </summary>
        /// <param name="rotation">Rotation transform</param>
        /// <param name="translation">Translation transform</param>
        /// <param name="scale">Scale transform</param>
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

        /// <summary>
        ///     Draw the edge in Graphics.
        /// </summary>
        /// <param name="g">Graphics</param>
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

        /// <summary>
        ///     Return the Edge Region.
        /// </summary>
        /// <returns>Region of the edge</returns>
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

        /// <summary>
        ///     Test whether or not the edge is under a specified location.
        /// </summary>
        /// <param name="x">X coordinate</param>
        /// <param name="y">Y coordinate</param>
        /// <returns></returns>
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

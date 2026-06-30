// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace Ara3D.RevitSampleBrowser.InPlaceMembers.CS
{
    /// <summary>
    ///     picturebox wich can display 3D geometry outline
    /// </summary>
    public class PictureBox3D : Button
    {
        private Matrix m_transform; //transform matrix between origin data and display data

        /// <summary>
        ///     datasource which can be any class inherited from IGraphicsData
        /// </summary>
        public IGraphicsData DataSource
        {
            get;
            set
            {
                if (null != value)
                {
                    field = value;
                    var rec = field.Region;
                    var plgpts = GetDisplayRegion();
                    m_transform = new Matrix(rec, plgpts);
                    field.UpdateViewEvent += Invalidate;
                }
            }
        }

        protected override void OnPaint(PaintEventArgs pe)
        {
            base.OnPaint(pe);

            if (null == DataSource) return;

            //prepare data
            var g = pe.Graphics;
            g.Clear(Color.White);
            Pen pen = new(Color.DarkGreen);
            GraphicsPath path = new();

            //draw curves one by one
            var curves = DataSource.PointCurves();
            foreach (var curve in curves)
            {
                path.Reset();
                path.AddLines(curve);
                path.Transform(m_transform);
                g.DrawPath(pen, path);
            }
        }

        public void Scale(bool zoomIn)
        {
            var ratio = zoomIn ? 10.0f / 11.0f : 11.0f / 10.0f;
            m_transform.Scale(ratio, ratio, MatrixOrder.Append);
            Invalidate();
        }

        public void MoveX(bool left)
        {
            var len = left ? -5.0f : 5.0f;
            m_transform.Translate(len, 0, MatrixOrder.Append);
            Invalidate();
        }

        public void MoveY(bool up)
        {
            var len = up ? 5.0f : -5.0f;
            m_transform.Translate(0, len, MatrixOrder.Append);
            Invalidate();
        }

        private PointF[] GetDisplayRegion()
        {
            var rec = DataSource.Region;
            const float margin = 8.0f;

            var realWidth = Width - (margin * 2);
            var realHeight = Height - (margin * 2);
            var minX = margin;
            var minY = margin;
            var ratioRec = rec.Height / rec.Width;
            var ratioBox = realHeight / realWidth;

            if (ratioRec > ratioBox)
            {
                var temp = realWidth;
                realWidth = realHeight * rec.Width / rec.Height;
                minX = (temp - realWidth) / 2.0f;
            }
            else
            {
                var temp = realHeight;
                realHeight = realWidth * rec.Height / rec.Width;
                minY = (temp - realHeight) / 2.0f;
            }

            if (rec.Width < GraphicsDataBase.MinedgElEngth + 1 &&
                rec.Height < GraphicsDataBase.MinedgElEngth + 1)
            {
                minX = realWidth / 2.0f;
                minY = realHeight / 2.0f;
            }

            var plgpts = new PointF[3];
            plgpts[0] = new PointF(minX, minY);
            plgpts[1] = new PointF(realWidth + minX, minY);
            plgpts[2] = new PointF(minX, realHeight + minY);

            return plgpts;
        }
    }
}

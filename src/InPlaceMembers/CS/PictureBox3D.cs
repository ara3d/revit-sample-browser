// Copyright 2023. See https://github.com/ara3d/revit-samples/LICENSE.txt

using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace Revit.SDK.Samples.InPlaceMembers.CS
{
    /// <summary>
    ///     picturebox wich can display 3D geometry outline
    /// </summary>
    public class PictureBox3D : Button
    {
        private IGraphicsData m_sourceData; //datasource
        private Matrix m_transform; //transform matrix between origin data and display data

        /// <summary>
        ///     datasource which can be any class inherited from IGraphicsData
        /// </summary>
        public IGraphicsData DataSource
        {
            get => m_sourceData;
            set
            {
                if (null != value)
                {
                    m_sourceData = value;
                    var rec = m_sourceData.Region;
                    var plgpts = GetDisplayRegion();
                    m_transform = new Matrix(rec, plgpts);
                    m_sourceData.UpdateViewEvent += Invalidate;
                }
            }
        }

        /// <summary>
        ///     paint outline
        /// </summary>
        /// <param name="pe"></param>
        protected override void OnPaint(PaintEventArgs pe)
        {
            base.OnPaint(pe);

            if (null == m_sourceData) return;

            //prepare data
            var g = pe.Graphics;
            g.Clear(Color.White);
            var pen = new Pen(Color.DarkGreen);
            var path = new GraphicsPath();

            //draw curves one by one
            var curves = m_sourceData.PointCurves();
            foreach (var curve in curves)
            {
                path.Reset();
                path.AddLines(curve);
                path.Transform(m_transform);
                g.DrawPath(pen, path);
            }
        }

        /// <summary>
        ///     scale the view by default value
        /// </summary>
        /// <param name="zoomIn">zomme in or zoom out</param>
        public void Scale(bool zoomIn)
        {
            var ratio = 1.0f;
            if (zoomIn)
                ratio = 10.0f / 11.0f;
            else
                ratio = 11.0f / 10.0f;
            m_transform.Scale(ratio, ratio, MatrixOrder.Append);
            Invalidate();
        }

        /// <summary>
        ///     move view in horizontal direction
        /// </summary>
        /// <param name="left">left or right</param>
        public void MoveX(bool left)
        {
            var len = 0.0f;
            if (left)
                len = -5.0f;
            else
                len = 5.0f;
            m_transform.Translate(len, 0, MatrixOrder.Append);
            Invalidate();
        }

        /// <summary>
        ///     move view in vertical direction
        /// </summary>
        /// <param name="up">up or down</param>
        public void MoveY(bool up)
        {
            var len = 0.0f;
            if (up)
                len = 5.0f;
            else
                len = -5.0f;
            m_transform.Translate(0, len, MatrixOrder.Append);
            Invalidate();
        }

        /// <summary>
        ///     get the display region, adjust the proportion and location
        /// </summary>
        /// <returns></returns>
        private PointF[] GetDisplayRegion()
        {
            var rec = m_sourceData.Region;
            const float MARGIN = 8.0f;

            var realWidth = Width - MARGIN * 2;
            var realHeight = Height - MARGIN * 2;
            var minX = MARGIN;
            var minY = MARGIN;
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

            if (rec.Width < GraphicsDataBase.MINEDGElENGTH + 1 &&
                rec.Height < GraphicsDataBase.MINEDGElENGTH + 1)
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

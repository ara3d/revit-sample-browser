// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using System;
using System.Drawing;
using System.Windows.Forms;

namespace Ara3D.RevitSampleBrowser.NewRoof.RoofForms.CS
{
    /// <summary>
    ///     The GraphicsControl is used to display the footprint roof lines with GDI.
    /// </summary>
    public partial class GraphicsControl : UserControl
    {
        // To store a display pen to draw the footprint roof lines.
        private readonly Pen m_displayPen;

        // To store the draw center location of the PictureBox control, it is the origin of the drawing.
        public PointF DrawCenter;

        // A reference to FootPrintRoofWrapper, It constrains the DrawFootPrint() method to 
        // draw footprint roof lines in the PictureBox control.
        private readonly FootPrintRoofWrapper m_footPrintRoofWrapper;

        // To store a highlight pen to highlight the specified footprint roof line.
        private readonly Pen m_highLightPen;

        // To store a value to decide the scale of the drawing.
        private float m_scale;

        /// <summary>
        ///     The private construct
        /// </summary>
        private GraphicsControl()
        {
            InitializeComponent();
        }

        /// <summary>
        ///     The construct of the GraphicsControl.
        /// </summary>
        /// <param name="footPrintRoofWrapper">
        ///     A reference to FootPrintRoofWrapper which will be displayed in
        ///     the picture box control.
        /// </param>
        public GraphicsControl(FootPrintRoofWrapper footPrintRoofWrapper)
        {
            InitializeComponent();
            Load += GraphicsControl_Load;

            m_displayPen = new Pen(Color.Green, 0);
            m_highLightPen = new Pen(Color.Red, 0);
            m_footPrintRoofWrapper = footPrintRoofWrapper;
        }

        /// <summary>
        ///     When the GraphicsControl was loaded, then add the picture box control to it
        ///     and initialize the draw center and scale value.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void GraphicsControl_Load(object sender, EventArgs e)
        {
            var picturebox = new PictureBox();
            picturebox.Dock = DockStyle.Fill;
            Controls.Add(picturebox);
            picturebox.Paint += picturebox_Paint;

            // initialize the draw center and scale value
            DrawCenter = new PointF(picturebox.Size.Width / 2, picturebox.Size.Height / 2);

            var size = m_footPrintRoofWrapper.Boundingbox.Max - m_footPrintRoofWrapper.Boundingbox.Min;
            var tempscale1 = (float)(0.9 * picturebox.Width / size.X);
            var tempscale2 = (float)(0.9 * picturebox.Height / size.Y);

            if (tempscale1 > tempscale2)
                m_scale = tempscale2;
            else
                m_scale = tempscale1;

            // Book the OnFootPrintRoofLineChanged event to refresh the picture box
            m_footPrintRoofWrapper.OnFootPrintRoofLineChanged += m_footPrintRoofWrapper_OnFootPrintRoofLineChanged;
        }

        /// <summary>
        ///     When the current selected FootPrintRoofLine changed in the PropertyGrid, then update the drawing.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void m_footPrintRoofWrapper_OnFootPrintRoofLineChanged(object sender, EventArgs e)
        {
            Refresh();
        }

        /// <summary>
        ///     Display the footprint roof lines in the picture box control.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void picturebox_Paint(object sender, PaintEventArgs e)
        {
            var graphics = e.Graphics;
            graphics.Clear(Color.White);
            graphics.TranslateTransform(DrawCenter.X, DrawCenter.Y);
            graphics.ScaleTransform(m_scale, m_scale);
            graphics.PageUnit = GraphicsUnit.Pixel;
            m_footPrintRoofWrapper.DrawFootPrint(graphics, m_displayPen, m_highLightPen);
        }
    }
}

// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace Ara3D.RevitSampleBrowser.DoorSwing.CS
{
    /// <summary>
    ///     A class inherit from Form is used to list all the door family exist in current project and
    ///     initialize each door type's Left/Right feature.
    /// </summary>
    public partial class InitializeForm : Form
    {
        private DoorGeometry m_currentGraphic;
        private readonly DoorSwingData m_dataBuffer;

        /// <summary>
        ///     constructor without any argument.
        /// </summary>
        private InitializeForm()
        {
            InitializeComponent();
        }

        /// <summary>
        ///     constructor overload.
        /// </summary>
        /// <param name="dataBuffer"> one reference of DoorSwingData.</param>
        public InitializeForm(DoorSwingData dataBuffer) : this()
        {
            m_dataBuffer = dataBuffer;

            // set data source of customizeDoorOpeningDataGridView.
            customizeDoorOpeningDataGridView.AutoGenerateColumns = false;
            customizeDoorOpeningDataGridView.DataSource = m_dataBuffer.DoorFamilies;
            familyNameColumn.DataPropertyName = "FamilyName";
            OpeningColumn.DataPropertyName = "BasalOpeningValue";
            OpeningColumn.DataSource = DoorSwingData.OpeningTypes;

            customizeDoorOpeningDataGridView.Focus();
            if (customizeDoorOpeningDataGridView.Rows.Count != 0)
                customizeDoorOpeningDataGridView.Rows[0].Selected = true;
        }

        /// <summary>
        ///     Preview door's geometry when user select one door family in customizeDoorOpeningDataGridView.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void customizeDoorOpeningDataGridView_RowEnter(object sender, DataGridViewCellEventArgs e)
        {
            var selectedDoorFamily = customizeDoorOpeningDataGridView.Rows[e.RowIndex].DataBoundItem as DoorFamily;
            m_currentGraphic = selectedDoorFamily.Geometry;

            // update the dialog box's display.
            previewPictureBox.Refresh();
        }

        /// <summary>
        ///     PreviewBox redraw.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void previewPictureBox_Paint(object sender, PaintEventArgs e)
        {
            // do nothing.
            if (null == m_currentGraphic) return;

            // The object of Graphics to draw sketch.
            var graphics = e.Graphics;
            // Get the element bounding box's rectangle area.
            var doorGeoRectangleF = m_currentGraphic.Bbox2D;
            // Get the display rectangle area of PreviewBox.
            RectangleF displayRectangleF = previewPictureBox.DisplayRectangle;

            // Calculate the draw area according to the size of the sketch: Adjust the shrink to change borders
            if (doorGeoRectangleF.Width * displayRectangleF.Height > doorGeoRectangleF.Height * displayRectangleF.Width)
                displayRectangleF.Inflate((float)(-0.1 * displayRectangleF.Width),
                    (float)(-1 + doorGeoRectangleF.Height * 0.8 * displayRectangleF.Width /
                        (doorGeoRectangleF.Width * displayRectangleF.Height)));
            else
                displayRectangleF.Inflate(
                    (float)(-1 + doorGeoRectangleF.Width * 0.8 * displayRectangleF.Height /
                        (doorGeoRectangleF.Height * displayRectangleF.Width)),
                    (float)(-0.1 * displayRectangleF.Height));

            // Mapping the point in sketch to point in draw area.
            var plgpts = new PointF[3];
            plgpts[0].X = displayRectangleF.Left;
            plgpts[0].Y = displayRectangleF.Bottom;
            plgpts[1].X = displayRectangleF.Right;
            plgpts[1].Y = displayRectangleF.Bottom;
            plgpts[2].X = displayRectangleF.Left;
            plgpts[2].Y = displayRectangleF.Top;

            // Get the transform matrix.
            var matrix = new Matrix(doorGeoRectangleF, plgpts);

            // Clear the object of graphics.
            graphics.Clear(previewPictureBox.BackColor);
            // Transform the object of graphics.
            graphics.Transform = matrix;
            // The pen for drawing profiles
            var drawPen = new Pen(Color.Red, (float)0.05);

            // Draw profiles.
            m_currentGraphic.DrawGraphics(graphics, drawPen);
        }
    }
}

// Copyright 2023. See https://github.com/ara3d/revit-samples/LICENSE.txt


using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace Revit.SDK.Samples.Openings.CS
{
    /// <summary>
    ///     Main form use to show the selected opening.
    /// </summary>
    public partial class OpeningForm : Form
    {
        //private member
        private readonly List<OpeningInfo> m_openingInfos; //store all the OpeningInfo class

        private OpeningInfo m_selectedOpeningInfo; //current displayed (in preview) OpeningInfo

        //constructor
        /// <summary>
        ///     The default constructor
        /// </summary>
        public OpeningForm()
        {
            InitializeComponent();
        }

        /// <summary>
        ///     constructor of OpeningForm
        /// </summary>
        /// <param name="openingInfos">a list of OpeningInFo</param>
        public OpeningForm(List<OpeningInfo> openingInfos)
        {
            InitializeComponent();

            m_openingInfos = openingInfos;
        }

        private void OpeningForm_Load(object sender, EventArgs e)
        {
            OpeningListComboBox.DataSource = m_openingInfos;
            OpeningListComboBox.DisplayMember = "NameAndId";

            m_selectedOpeningInfo = (OpeningInfo)OpeningListComboBox.SelectedItem;
            OpeningPropertyGrid.SelectedObject = m_selectedOpeningInfo.Property;
        }

        private void PreviewPictureBox_Paint(object sender, PaintEventArgs e)
        {
            var width = PreviewPictureBox.Width;
            var height = PreviewPictureBox.Height;
            if (m_selectedOpeningInfo.Sketch != null)
            {
                m_selectedOpeningInfo.Sketch.Draw2D(width,
                    height, e.Graphics);
            }
            else
            {
                //if profile is a circle (or ellipse), can not get curve from API
                //so draw an Arc according to boundingBox of the Opening
                var widthBoundBox = m_selectedOpeningInfo.BoundingBox.Width;
                var lengthBoundBox = m_selectedOpeningInfo.BoundingBox.Length;
                var scale = height * 0.8 / lengthBoundBox;
                e.Graphics.Clear(Color.Black);
                var yellowPen = new Pen(Color.Yellow, 1);
                var rect = new Rectangle((int)(width / 2 - widthBoundBox * scale / 2),
                    (int)(height / 2 - lengthBoundBox * scale / 2), (int)(widthBoundBox * scale),
                    (int)(lengthBoundBox * scale));
                // Draw circle to screen.
                e.Graphics.DrawArc(yellowPen, rect, 0, 360);
            }
        }

        private void Createbutton_Click(object sender, EventArgs e)
        {
            var optionForm =
                new CreateModelLineOptionsForm(m_openingInfos, m_selectedOpeningInfo);
            optionForm.ShowDialog();
        }

        private void OpeningListComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            m_selectedOpeningInfo = (OpeningInfo)OpeningListComboBox.SelectedItem;
            OpeningPropertyGrid.SelectedObject = m_selectedOpeningInfo.Property;
            PreviewPictureBox.Refresh();
        }

        private void OKButton_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}

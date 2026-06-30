// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using System;
using System.Drawing;
using System.Windows.Forms;
using Autodesk.Revit.DB;
using Form = System.Windows.Forms.Form;
using Point = System.Drawing.Point;

namespace Ara3D.RevitSampleBrowser.NewRoof.CS.RoofForms
{
    public partial class RoofEditorForm : Form
    {
        private readonly ExtrusionRoofWrapper m_extrusionRoofWrapper;

        private readonly FootPrintRoofWrapper m_footPrintRoofWrapper;

        private GraphicsControl m_graphicsControl;

        private readonly RoofBase m_roof;

        private readonly RoofsManager.RoofsManager m_roofsManager;

        private RoofEditorForm()
        {
            InitializeComponent();
        }

        public RoofEditorForm(RoofsManager.RoofsManager roofsManager, RoofBase roof)
        {
            m_roofsManager = roofsManager;
            m_roof = roof;
            InitializeComponent();

            m_footPrintRoofWrapper = null;
            m_extrusionRoofWrapper = null;

            if (m_roof is FootPrintRoof printRoof)
                m_footPrintRoofWrapper = new FootPrintRoofWrapper(printRoof);
            else
                m_extrusionRoofWrapper = new ExtrusionRoofWrapper(m_roof as ExtrusionRoof);
        }

        private void RoofEditorForm_Load(object sender, EventArgs e)
        {
            roofTypesComboBox.DataSource = m_roofsManager.RoofTypes;
            roofTypesComboBox.DisplayMember = "Name";
            roofTypesComboBox.ValueMember = "Id";
            roofTypesComboBox.DropDownStyle = ComboBoxStyle.DropDownList;

            roofTypesComboBox.SelectedValue = m_roof.RoofType.Id;

            if (m_roof is FootPrintRoof)
            {
                roofEditorPropertyGrid.SelectedObject = m_footPrintRoofWrapper;
                Size = new Size(814, 515);

                var label = new Label
                {
                    Text = "Footprint roof lines:",
                    AutoSize = true,
                    Location = new Point(398, 12)
                };
                Controls.Add(label);

                m_graphicsControl = new GraphicsControl(m_footPrintRoofWrapper)
                {
                    Location = new Point(398, 36),
                    Size = new Size(400, 440)
                };
                Controls.Add(m_graphicsControl);
            }
            else
            {
                roofEditorPropertyGrid.SelectedObject = m_extrusionRoofWrapper;
            }

            roofEditorPropertyGrid.ExpandAllGridItems();
        }

        private void okButton_Click(object sender, EventArgs e)
        {
            m_roof.RoofType = roofTypesComboBox.SelectedItem as RoofType;
        }
    }
}

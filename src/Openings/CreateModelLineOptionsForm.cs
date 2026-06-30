// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace Ara3D.RevitSampleBrowser.Openings.CS
{
    public partial class CreateModelLineOptionsForm : Form
    {
        private readonly List<OpeningInfo> m_openingInfos; //store all the OpeningInfo class
        private readonly OpeningInfo m_selectedOpeningInfo; //current displayed (in preview) OpeningInfo

        public CreateModelLineOptionsForm()
        {
            InitializeComponent();
        }

        public CreateModelLineOptionsForm(List<OpeningInfo> openingInfos,
            OpeningInfo selectOpening)
        {
            InitializeComponent();

            m_openingInfos = openingInfos;
            m_selectedOpeningInfo = selectOpening;
        }

        private void CreateButton_Click(object sender, EventArgs e)
        {
            if (CreateDisplayRadioButton.Checked)
                m_selectedOpeningInfo.BoundingBox.CreateLines(m_selectedOpeningInfo.Revit);
            else if (CreateAllRadioButton.Checked)
                foreach (var openingInfo in m_openingInfos)
                {
                    openingInfo.BoundingBox.CreateLines(m_selectedOpeningInfo.Revit);
                }
            else if (CreateShaftRadioButton.Checked)
                foreach (var openingInfo in m_openingInfos)
                {
                    if (openingInfo.IsShaft)
                        openingInfo.BoundingBox.CreateLines(m_selectedOpeningInfo.Revit);
                }

            Close();
        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}

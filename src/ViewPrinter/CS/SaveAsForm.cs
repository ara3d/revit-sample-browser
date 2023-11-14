// Copyright 2023. See https://github.com/ara3d/revit-samples/LICENSE.txt
using System;
using System.Windows.Forms;

namespace Revit.SDK.Samples.ViewPrinter.CS
{
    public partial class SaveAsForm : Form
    {
        private readonly ISettingNameOperation m_settingNameOperation;

        public SaveAsForm(ISettingNameOperation settingNameOperation)
        {
            InitializeComponent();
            m_settingNameOperation = settingNameOperation;
            newNameTextBox.Text = m_settingNameOperation.Prefix
                                  + m_settingNameOperation.SettingCount;
        }

        private void okButton_Click(object sender, EventArgs e)
        {
            m_settingNameOperation.SaveAs(newNameTextBox.Text);
        }
    }
}

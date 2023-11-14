using System;
using System.Windows.Forms;

namespace Revit.SDK.Samples.ViewPrinter.CS
{
    public partial class ReNameForm : Form
    {
        private readonly ISettingNameOperation m_settingWithNameOperation;

        public ReNameForm(ISettingNameOperation settingWithNameOperation)
        {
            InitializeComponent();
            m_settingWithNameOperation = settingWithNameOperation;
            previousNameTextBox.Text =
                newNameTextBox.Text =
                    m_settingWithNameOperation.SettingName;
        }

        private void okButton_Click(object sender, EventArgs e)
        {
            m_settingWithNameOperation.Rename(newNameTextBox.Text);
        }
    }
}
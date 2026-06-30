// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using System;
using System.Windows.Forms;
using Autodesk.Revit.UI;

namespace Ara3D.RevitSampleBrowser.SharedCoordinateSystem.CS
{
    /// <summary>
    ///     dupliate coordiante data form
    /// </summary>
    public partial class DuplicateForm : Form
    {
        private readonly CoordinateSystemData m_data; //the reference of the CoordinateSystemData class 
        private readonly CoordinateSystemDataForm m_dataForm; //the reference of the CoordinateSystemDataForm class
        private readonly string m_locationName; //the name of  selected location

        public DuplicateForm(CoordinateSystemData data, CoordinateSystemDataForm coorinateForm,
            string locationName)
        {
            m_data = data;
            m_dataForm = coorinateForm;
            m_locationName = locationName;
            InitializeComponent();
        }

        private void okButton_Click(object sender, EventArgs e)
        {
            foreach (var name in m_data.LocationNames)
            {
                if (name == newNameTextBox.Text)
                {
                    TaskDialog.Show("Revit", "The name entered is already in use. Enter a unique name.",
                        TaskDialogCommonButtons.Ok);
                    return;
                }
            }

            try
            {
                m_data.DuplicateLocation(m_locationName, newNameTextBox.Text);
                m_dataForm.NewLocationName = newNameTextBox.Text;
            }
            catch (ArgumentException ex)
            {
                TaskDialog.Show("Revit", ex.Message, TaskDialogCommonButtons.Ok);
                return;
            }

            DialogResult = DialogResult.OK;
            Close(); // close the form
        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close(); // close the form
        }

        private void DuplicateForm_Load(object sender, EventArgs e)
        {
            newNameTextBox.Focus();
        }
    }
}

// Copyright 2023. See https://github.com/ara3d/revit-samples/LICENSE.txt

using System;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using Autodesk.Revit.DB;
using Form = System.Windows.Forms.Form;

namespace Revit.SDK.Samples.BoundaryConditions.CS
{
    /// <summary>
    ///     UI which display the information and interact with users
    /// </summary>
    public partial class BoundaryConditionsForm : Form
    {
        // an instance of BoundaryConditionsData class which deal with the need data
        private readonly BoundaryConditionsData m_dataBuffer;

        /// <summary>
        ///     constructor
        /// </summary>
        /// <param name="dataBuffer"></param>
        public BoundaryConditionsForm(BoundaryConditionsData dataBuffer)
        {
            InitializeComponent();

            m_dataBuffer = dataBuffer;
        }

        /// <summary>
        ///     display the information about the host element and the BC parameter value
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BoundaryConditionsForm_Load(object sender, EventArgs e)
        {
            // display host element id in the text box
            hostTextBox.Text = m_dataBuffer.HostElement.Id.ToString();

            // if the selected element has not a BC, create one and display the default parameter values
            // else list the BC ids in the combox and display the first BC's parameter values
            if (0 == m_dataBuffer.BCs.Count)
            {
                bCComboBox.Visible = false;
                bCLabel.Visible = false;
                var isCreatedSuccessful = m_dataBuffer.CreateBoundaryConditions();
                m_dataBuffer.HostElement.Document.Regenerate();

                if (!isCreatedSuccessful)
                {
                    DialogResult = DialogResult.Retry;
                    return;
                }
            }
            else
            {
                bCComboBox.Visible = true;
                bCLabel.Visible = true;
            }

            // list the boundary conditions Id values to the combobox
            ICollection bCIdValues = m_dataBuffer.BCs.Keys;
            foreach (ElementId bCIdValue in bCIdValues) bCComboBox.Items.Add(bCIdValue);

            bCComboBox.SelectedIndex = 0;
        }

        /// <summary>
        ///     display the BC's property value according the selected BC ID in combobox
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void bCComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            // create a BCProperties instance according to current selected BC Id
            m_dataBuffer.BCProperties = new BCProperties(m_dataBuffer.BCs[ElementId.Parse(bCComboBox.Text)]);

            // set the display object
            bCPropertyGrid.SelectedObject = m_dataBuffer.BCProperties;

            // set the browsable attributes of the property grid
            Attribute[] attributes = null;
            switch (m_dataBuffer.BCProperties.BoundaryConditionsType)
            {
                case BCType.Point:
                    attributes = new Attribute[] { new BCTypeAttribute(new[] { BCType.Point }) };
                    break;
                case BCType.Line:
                    attributes = new Attribute[] { new BCTypeAttribute(new[] { BCType.Line }) };
                    break;
                case BCType.Area:
                    attributes = new Attribute[] { new BCTypeAttribute(new[] { BCType.Area }) };
                    break;
            }

            bCPropertyGrid.BrowsableAttributes = new AttributeCollection(attributes);
        }

        /// <summary>
        ///     deal with operations that user set these parameters with other valid value.
        /// </summary>
        /// <param name="s"></param>
        /// <param name="e"></param>
        private void bCPropertyGrid_PropertyValueChanged(object s, PropertyValueChangedEventArgs e)
        {
            if (e.ChangedItem.Label.Contains("Translation") || e.ChangedItem.Label.Contains("Rotation"))
            {
                // invoke SetSpringModulus method in BCProperties class to
                // let user enter a positive number as the SpringModulus 
                // when set any Translation/Rotation parameter to Spring 
                var value = (BCTranslationRotation)e.ChangedItem.Value;
                if (BCTranslationRotation.Spring == value)
                    m_dataBuffer.BCProperties.SetSpringModulus(e.ChangedItem.Label);
            }
        }

        /// <summary>
        ///     confirm the changed and exist the UI
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void okButton_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
        }
    }
}

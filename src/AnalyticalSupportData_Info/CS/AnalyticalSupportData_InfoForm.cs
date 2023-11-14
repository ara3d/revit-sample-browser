// Copyright 2023. See https://github.com/ara3d/revit-samples/LICENSE.txt

using System;
using System.Windows.Forms;

namespace RevitMultiSample.AnalyticalSupportData_Info.CS
{
    /// <summary>
    ///     UI which display the information
    /// </summary>
    public partial class AnalyticalSupportDataInfoForm : Form
    {
        // an instance of Command class which is prepared the displayed data.
        private readonly Command m_dataBuffer;

        /// <summary>
        ///     Default constructor
        /// </summary>
        private AnalyticalSupportDataInfoForm()
        {
            InitializeComponent();
        }

        /// <summary>
        ///     constructor
        /// </summary>
        /// <param name="dataBuffer"></param>
        public AnalyticalSupportDataInfoForm(Command dataBuffer) : this()
        {
            m_dataBuffer = dataBuffer;
            // display the elements information, which is prepared by Command class, in a grid.
            // set data source
            elementInfoDataGridView.AutoGenerateColumns = false;
            elementInfoDataGridView.DataSource = m_dataBuffer.ElementInformation;

            id.DataPropertyName = "Id";
            typeName.DataPropertyName = "Element Type";
            support.DataPropertyName = "Support Type";
            remark.DataPropertyName = "Remark";
        }

        /// <summary>
        ///     exit
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void closeButton_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}

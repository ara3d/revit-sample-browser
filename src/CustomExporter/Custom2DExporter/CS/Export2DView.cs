// Copyright 2023. See https://github.com/ara3d/revit-samples/LICENSE.txt
using System;
using System.Windows.Forms;

namespace Revit.SDK.Samples.Custom2DExporter.CS
{
    /// <summary>
    ///     UI Form presented to the user used to collection options related to the export.
    /// </summary>
    public partial class Export2DView : Form
    {
        /// <summary>
        ///     Construct a new form.
        /// </summary>
        public Export2DView()
        {
            InitializeComponent();
            ViewExportOptions = new ExportOptions
            {
                ExportAnnotationObjects = false,
                ExportPatternLines = false
            };
        }

        /// <summary>
        ///     Property containing the options chosen for export.
        /// </summary>
        public ExportOptions ViewExportOptions { get; }

        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {
            ViewExportOptions.ExportAnnotationObjects = checkBox2.Checked;
        }

        private void checkBox3_CheckedChanged(object sender, EventArgs e)
        {
            ViewExportOptions.ExportPatternLines = checkBox3.Checked;
        }

        /// <summary>
        ///     The option for creating Path of Travel.
        /// </summary>
        public class ExportOptions
        {
            /// <summary>
            ///     True if AnnotationObjects are to be included in the export, false otherwise.
            /// </summary>
            public bool ExportAnnotationObjects { get; set; }

            /// <summary>
            ///     True if Pattern Lines are to be included in the export, false otherwise.
            /// </summary>
            public bool ExportPatternLines { get; set; }
        }
    }
}

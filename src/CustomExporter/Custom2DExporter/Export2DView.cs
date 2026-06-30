// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using System;
using System.Windows.Forms;

namespace Ara3D.RevitSampleBrowser.CustomExporter.Custom2DExporter.CS
{
    public partial class Export2DView : Form
    {
        public Export2DView()
        {
            InitializeComponent();
            ViewExportOptions = new ExportOptions
            {
                ExportAnnotationObjects = false,
                ExportPatternLines = false
            };
        }

        public ExportOptions ViewExportOptions { get; }

        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {
            ViewExportOptions.ExportAnnotationObjects = checkBox2.Checked;
        }

        private void checkBox3_CheckedChanged(object sender, EventArgs e)
        {
            ViewExportOptions.ExportPatternLines = checkBox3.Checked;
        }

        public class ExportOptions
        {
            public bool ExportAnnotationObjects { get; set; }

            public bool ExportPatternLines { get; set; }
        }
    }
}

// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using Ara3D.RevitSampleBrowser.Common.Infrastructure;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using Form = System.Windows.Forms.Form;
namespace Ara3D.RevitSampleBrowser.FamilyParametersOrder.CS
{
    // Sorted output is written to an "ordered" subfolder; *.rfa files in subfolders are not processed.
    public partial class SortFamilyFilesParamsForm : Form
    {
        private readonly UIApplication m_uiApp;

        public SortFamilyFilesParamsForm(UIApplication uiApp)
        {
            m_uiApp = uiApp;
            InitializeComponent();
        }

        private void browseBtn_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog dialog = new();
            if (dialog.ShowDialog() == DialogResult.OK) directoryTxt.Text = dialog.SelectedPath;
        }

        private void SortParameters(ParametersOrder order)
        {
            var absPath = AssemblyPathHelper.ResolveDirectoryPath(directoryTxt.Text);

            DirectoryInfo dirInfo = new(absPath);
            if (!dirInfo.Exists)
            {
                MessageBox.Show("Please select a valid directory first.");
                return;
            }

            var orderedDir = Path.Combine(absPath, "ordered");
            if (!Directory.Exists(orderedDir))
                Directory.CreateDirectory(orderedDir);

            var fileInfo = dirInfo.GetFiles("*.rfa");
            foreach (var fInfo in fileInfo)
            {
                var doc = m_uiApp.Application.OpenDocumentFile(fInfo.FullName);
                using (Transaction trans = new(doc, "Sort parameters."))
                {
                    trans.Start();
                    doc.FamilyManager.SortParameters(order);
                    trans.Commit();
                }

                var destFile = Path.Combine(orderedDir, fInfo.Name);
                if (File.Exists(destFile))
                    File.Delete(destFile);

                doc.SaveAs(destFile);
                doc.Close(false);
            }

            TaskDialog.Show("Message", $"Sort completed! {fileInfo.Count()} family file(s) sorted.");
        }

        private void A_ZBtn_Click(object sender, EventArgs e)
        {
            SortParameters(ParametersOrder.Ascending);
        }

        private void Z_ABtn_Click(object sender, EventArgs e)
        {
            SortParameters(ParametersOrder.Descending);
        }

        private void closeBtn_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}

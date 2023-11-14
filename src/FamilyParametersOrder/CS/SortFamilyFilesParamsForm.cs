// Copyright 2023. See https://github.com/ara3d/revit-samples/LICENSE.txt

using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Form = System.Windows.Forms.Form;

namespace RevitMultiSample.FamilyParametersOrder.CS
{
    /// <summary>
    ///     Sort parameters' order in family files which are located in a folder:
    ///     <list type="bullet">
    ///         <item>
    ///             "Browse" button make users could choose a folder contains some families, and the selected folder will be
    ///             shown in the text box.
    ///         </item>
    ///         <item>
    ///             "A–>Z" button will update parameters in each family files located in the specific folder to alphabet
    ///             order.
    ///         </item>
    ///         <item>
    ///             "Z–>A" button will update parameters in each family files located in the specific folder to reverse
    ///             alphabet order.
    ///         </item>
    ///         <item>"Close" button will close the whole dialog.</item>
    ///     </list>
    ///     Note: we don't update the family files in sub-folders in this example.
    /// </summary>
    public partial class SortFamilyFilesParamsForm : Form
    {
        private readonly UIApplication m_uiApp;

        /// <summary>
        ///     Construct with a UIApplication.
        /// </summary>
        /// <param name="uiApp"></param>
        public SortFamilyFilesParamsForm(UIApplication uiApp)
        {
            m_uiApp = uiApp;
            InitializeComponent();
        }

        private void browseBtn_Click(object sender, EventArgs e)
        {
            var dialog = new FolderBrowserDialog();
            if (dialog.ShowDialog() == DialogResult.OK) directoryTxt.Text = dialog.SelectedPath;
        }

        /// <summary>
        ///     Sort parameters' order in family files which is located in a folder, the new files are saved in subfolder named
        ///     "ordered".
        /// </summary>
        /// <param name="order">Ascending or Descending.</param>
        private void SortParameters(ParametersOrder order)
        {
            // Convert relative path to absolute path.
            var absPath = directoryTxt.Text;
            if (!Path.IsPathRooted(absPath))
                absPath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), absPath);

            var dirInfo = new DirectoryInfo(absPath);
            if (!dirInfo.Exists)
            {
                MessageBox.Show("Please select a valid directory first.");
                return;
            }

            var orderedDir = Path.Combine(absPath, "ordered");
            if (!Directory.Exists(orderedDir))
                Directory.CreateDirectory(orderedDir);

            // Sort parameters in each family file.
            var fileInfo = dirInfo.GetFiles("*.rfa");
            foreach (var fInfo in fileInfo)
            {
                var doc = m_uiApp.Application.OpenDocumentFile(fInfo.FullName);
                using (var trans = new Transaction(doc, "Sort parameters."))
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

            TaskDialog.Show("Message", "Sort completed! " + fileInfo.Count() + " family file(s) sorted.");
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

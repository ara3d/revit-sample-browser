// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using System;
using System.IO;
using System.Reflection;
using System.Windows.Forms;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Form = System.Windows.Forms.Form;

namespace Ara3D.RevitSampleBrowser.FamilyParametersOrder.CS
{
    /// <summary>
    ///     Sort parameters' order in families which are loaded into a project or a family:
    ///     <list type="bullet">
    ///         <item>
    ///             "A–>Z" button will update parameters in each loaded family to alphabet order then reload the family to
    ///             project.
    ///         </item>
    ///         <item>
    ///             "Z–>A" button will update parameters in each loaded family to reverse alphabet order then reload the
    ///             family to project.
    ///         </item>
    ///         <item>"Close" button will close the whole dialog.</item>
    ///     </list>
    /// </summary>
    public partial class SortLoadedFamiliesParamsForm : Form
    {
        /// <summary>
        ///     Document whose loaded families parameters' order want to be sort.
        /// </summary>
        private readonly Document m_currentDoc;

        /// <summary>
        ///     Construct with a Document.
        /// </summary>
        /// <param name="currentDoc"></param>
        public SortLoadedFamiliesParamsForm(Document currentDoc)
        {
            m_currentDoc = currentDoc;
            InitializeComponent();
        }

        /// <summary>
        ///     Sort parameters of families which have been loaded into a project.
        /// </summary>
        /// <param name="order">Ascending or Descending</param>
        private void SortParameters(ParametersOrder order)
        {
            try
            {
                var coll = new FilteredElementCollector(m_currentDoc);
                var families = coll.OfClass(typeof(Family)).ToElements();

                // Edit each family->sort parameters order->save to a new file->load back to the document.
                var count = 0;
                foreach (Family fam in families)
                {
                    if (!fam.IsEditable)
                        continue;

                    var famDoc = m_currentDoc.EditFamily(fam);

                    using (var trans = new Transaction(famDoc, "Sort parameters."))
                    {
                        trans.Start();
                        famDoc.FamilyManager.SortParameters(order);
                        trans.Commit();
                    }

                    var tmpFile = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location),
                        $"{fam.Name}.rfa");

                    if (File.Exists(tmpFile))
                        File.Delete(tmpFile);

                    famDoc.SaveAs(tmpFile);
                    famDoc.Close(false);

                    using (var trans = new Transaction(m_currentDoc, "Load family."))
                    {
                        trans.Start();
                        m_currentDoc.LoadFamily(tmpFile, new FamilyLoadOptions(), out _);
                        trans.Commit();
                    }

                    File.Delete(tmpFile);
                    count++;
                }

                TaskDialog.Show("Message", $"Sort completed! {count} families sorted.");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error:{ex.Message}");
            }
        }

        /// <summary>
        ///     Sort families parameters with ascending.
        /// </summary>
        /// <param name="sender">Not used.</param>
        /// <param name="e">Not used.</param>
        private void A_ZBtn_Click(object sender, EventArgs e)
        {
            SortParameters(ParametersOrder.Ascending);
        }

        /// <summary>
        ///     Sort families parameters with descending.
        /// </summary>
        /// <param name="sender">Not used.</param>
        /// <param name="e">Not used.</param>
        private void Z_ABtn_Click(object sender, EventArgs e)
        {
            SortParameters(ParametersOrder.Descending);
        }

        /// <summary>
        ///     Close this dialog.
        /// </summary>
        /// <param name="sender">Not used.</param>
        /// <param name="e">Not used.</param>
        private void closeBtn_Click(object sender, EventArgs e)
        {
            Close();
        }

        private class FamilyLoadOptions : IFamilyLoadOptions
        {
            public bool OnFamilyFound(bool familyInUse, out bool overwriteParameterValues)
            {
                overwriteParameterValues = true;
                return true;
            }

            public bool OnSharedFamilyFound(Family sharedFamily, bool familyInUse, out FamilySource source,
                out bool overwriteParameterValues)
            {
                source = FamilySource.Family;
                overwriteParameterValues = true;
                return true;
            }
        }
    }
}

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
    public partial class SortLoadedFamiliesParamsForm : Form
    {
        private readonly Document m_currentDoc;

        public SortLoadedFamiliesParamsForm(Document currentDoc)
        {
            m_currentDoc = currentDoc;
            InitializeComponent();
        }

        private void SortParameters(ParametersOrder order)
        {
            try
            {
                var coll = new FilteredElementCollector(m_currentDoc);
                var families = coll.OfClass(typeof(Family)).ToElements();

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

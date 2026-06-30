// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using System;
using System.Windows.Forms;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Form = System.Windows.Forms.Form;

namespace Ara3D.RevitSampleBrowser.FrameBuilder.CS
{
    public partial class DuplicateTypeForm : Form
    {
        private readonly FamilySymbol m_copiedSymbol;
        private FamilySymbol m_newSymbol;
        private readonly FrameTypesMgr m_typesMgr;

        public DuplicateTypeForm(object obj, FrameTypesMgr typesMgr)
        {
            InitializeComponent();

            m_copiedSymbol = obj as FamilySymbol;
            m_typesMgr = typesMgr;
        }

        private DuplicateTypeForm()
        {
        }

        // Prompts for a name, duplicates the symbol, then shows parameter editing.
        public new DialogResult ShowDialog()
        {
            try
            {
                var initialTypeName = m_typesMgr.GenerateSymbolName(m_copiedSymbol.Name);
                using (var typeNameFrm = new EditTypeNameForm(initialTypeName))
                {
                    if (typeNameFrm.ShowDialog() != DialogResult.OK) return DialogResult.Cancel;

                    var finalTypeName = m_typesMgr.GenerateSymbolName(typeNameFrm.TypeName);
                    m_newSymbol = m_typesMgr.DuplicateSymbol(m_copiedSymbol, finalTypeName);
                }
            }
            catch
            {
                TaskDialog.Show("Revit", "Failed to duplicate Type.");
                TaskDialog.Show("Revit", "Failed to duplicate Type.");
                return DialogResult.Abort;
            }

            return base.ShowDialog();
        }

        private void DuplicateTypeForm_Load(object sender, EventArgs e)
        {
            typeNameTextBox.Text = m_newSymbol.Name;
            familyTextBox.Text = m_newSymbol.Family.Name;
            var symbolParas = FrameTypeParameters.CreateInstance(m_newSymbol);
            if (null != symbolParas)
                typeParameterPropertyGrid.SelectedObject = symbolParas;
            else
                typeParameterPropertyGrid.Enabled = false;
        }

        private void OKButton_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
            Close();
        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            var result = m_typesMgr.DeleteSymbol(m_newSymbol);
            if (result == false) throw new ErrorMessageException("can not delete the new duplicate symbol");
            DialogResult = DialogResult.Cancel;
            Close();
        }
    }
}

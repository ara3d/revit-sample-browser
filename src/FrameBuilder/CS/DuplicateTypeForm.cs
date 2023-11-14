// Copyright 2023. See https://github.com/ara3d/revit-samples/LICENSE.txt

using System;
using System.Windows.Forms;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Form = System.Windows.Forms.Form;

namespace Revit.SDK.Samples.FrameBuilder.CS
{
    /// <summary>
    ///     form to duplicate FamilySymbol and edit its name and parameters
    /// </summary>
    public partial class DuplicateTypeForm : Form
    {
        private readonly FamilySymbol m_copiedSymbol; // FamilySymbol object to be copied
        private FamilySymbol m_newSymbol; // duplicate FamilySymbol
        private readonly FrameTypesMgr m_typesMgr; // object manage FamilySymbols

        /// <summary>
        ///     constructor
        /// </summary>
        /// <param name="obj">FamilySymbol object</param>
        /// <param name="typesMgr">FamilySymbols' manager</param>
        public DuplicateTypeForm(object obj, FrameTypesMgr typesMgr)
        {
            InitializeComponent();

            m_copiedSymbol = obj as FamilySymbol;
            m_typesMgr = typesMgr;
        }

        /// <summary>
        ///     constructor without parameter is forbidden
        /// </summary>
        private DuplicateTypeForm()
        {
        }

        /// <summary>
        ///     hidden subclass' method;
        ///     display EditTypeNameForm to get type name;
        ///     then duplicate FamilySymbol
        /// </summary>
        public new DialogResult ShowDialog()
        {
            try
            {
                // generate the duplicate one's initial Name
                var initialTypeName = m_typesMgr.GenerateSymbolName(m_copiedSymbol.Name);
                // provide UI for user to edit the duplicate one's name
                using (var typeNameFrm = new EditTypeNameForm(initialTypeName))
                {
                    // cancel the command of duplicate
                    if (typeNameFrm.ShowDialog() != DialogResult.OK) return DialogResult.Cancel;

                    // generate the duplicate one's Name used to create with Name edited in EditTypeNameForm
                    var finalTypeName = m_typesMgr.GenerateSymbolName(typeNameFrm.TypeName);
                    // duplicate FamilySymbol
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

        /// <summary>
        ///     provide UI to edit is parameter
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DuplicateTypeForm_Load(object sender, EventArgs e)
        {
            // initialize controls
            typeNameTextBox.Text = m_newSymbol.Name;
            familyTextBox.Text = m_newSymbol.Family.Name;
            var symbolParas = FrameTypeParameters.CreateInstance(m_newSymbol);
            if (null != symbolParas)
                typeParameterPropertyGrid.SelectedObject = symbolParas;
            else
                typeParameterPropertyGrid.Enabled = false;
        }

        /// <summary>
        ///     to finish duplicate process
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OKButton_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
            Close();
        }

        /// <summary>
        ///     cancel the duplicate and roll back the command
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cancelButton_Click(object sender, EventArgs e)
        {
            // delete the duplicate one
            var result = m_typesMgr.DeleteSymbol(m_newSymbol);
            if (result == false) throw new ErrorMessageException("can not delete the new duplicate symbol");
            DialogResult = DialogResult.Cancel;
            Close();
        }
    }
}

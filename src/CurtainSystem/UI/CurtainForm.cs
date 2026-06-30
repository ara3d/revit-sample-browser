// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using Ara3D.RevitSampleBrowser.CurtainSystem.CS.CurtainSystem;
using Ara3D.RevitSampleBrowser.CurtainSystem.CS.Data;
using Ara3D.RevitSampleBrowser.CurtainSystem.CS.Properties;
using Autodesk.Revit.UI;

namespace Ara3D.RevitSampleBrowser.CurtainSystem.CS.UI
{
    public partial class CurtainForm : Form
    {
        // the document containing all the data used in the sample
        private readonly MyDocument m_mydocument;

        public CurtainForm(MyDocument mydoc)
        {
            m_mydocument = mydoc;

            InitializeComponent();
            // initialize some controls manually
            InitializeCustomComponent();
            // register the customized events
            RegisterEvents();
        }

        private void InitializeCustomComponent()
        {
            deleteCSButton.Enabled = false;
            addCGButton.Enabled = false;
            removeCGButton.Enabled = false;
        }

        private void RegisterEvents()
        {
            m_mydocument.FatalErrorEvent += m_document_FatalErrorEvent;
            m_mydocument.SystemData.CurtainSystemChanged += m_document_SystemData_CurtainSystemChanged;
            // moniter the sample message change status
            m_mydocument.MessageChanged += m_document_MessageChanged;
        }

        private void m_document_FatalErrorEvent(string errorMsg)
        {
            // hang the sample and shown the error hint to users
            var result = TaskDialog.Show(Resources.TXT_DialogTitle, errorMsg, TaskDialogCommonButtons.Ok,
                TaskDialogResult.Ok);
            // the user has read the hint and clicked the "OK" button, close the dialog
            if (TaskDialogResult.Ok == result) Close();
        }

        private void m_document_SystemData_CurtainSystemChanged()
        {
            // clear the out-of-date values
            csListBox.Items.Clear();
            facesCheckedListBox.Items.Clear();
            cgCheckedListBox.Items.Clear();

            var csInfos = m_mydocument.SystemData.CurtainSystemInfos;

            // no curtain system available, disable the "Delete Curtain System"
            // "Add curtain grid" and "remove curtain grid" buttons
            if (null == csInfos ||
                0 == csInfos.Count)
            {
                deleteCSButton.Enabled = false;
                addCGButton.Enabled = false;
                removeCGButton.Enabled = false;
                Show();
                return;
            }

            foreach (var info in csInfos)
            {
                csListBox.Items.Add(info);
            }

            // activate the last one
            var csInfo = csInfos[csInfos.Count - 1];
            // this will invoke the selectedIndexChanged event, then to update the other 2 list boxes
            csListBox.SetSelected(csInfos.Count - 1, true);
            // enable the buttons and show  the dialog
            deleteCSButton.Enabled = true;
            // only curtain system which created by reference array supports curtain grid operations
            if (false == csInfo.ByFaceArray)
            {
                addCGButton.Enabled = true;
                removeCGButton.Enabled = true;
            }

            Show();
        }

        private void m_document_MessageChanged()
        {
            //if it's an error / warning message, set the color of the text to red
            var message = m_mydocument.Message;
            if (message.Value)
                operationStatusLabel.ForeColor = Color.Red;
            // it's a common hint message, set the color to black
            else
                operationStatusLabel.ForeColor = Color.Black;
            operationStatusLabel.Text = message.Key;
            statusStrip.Refresh();
        }

        private void createCSButton_Click(object sender, EventArgs e)
        {
            Hide();

            // show the "create curtain system" dialog
            using (var dlg = new CreateCurtainSystemDialog(m_mydocument))
            {
                dlg.ShowDialog(this);
            }
        }

        private void deleteCSButton_Click(object sender, EventArgs e)
        {
            // no curtain system available, ask sample user to create some curtain systems first
            if (null == csListBox.Items ||
                0 == csListBox.Items.Count)
            {
                var hint = Resources.HINT_CreateCSFirst;
                m_mydocument.Message = new KeyValuePair<string, bool>(hint, true);
                return;
            }

            var checkedIndices = new List<int>();
            for (var i = 0; i < csListBox.Items.Count; i++)
            {
                var itemChecked = csListBox.GetItemChecked(i);

                if (itemChecked) checkedIndices.Add(i);
            }

            // no curtain system available or no curtain system selected for deletion
            if (null == checkedIndices ||
                0 == checkedIndices.Count)
            {
                var hint = Resources.HINT_SelectCSFirst;
                m_mydocument.Message = new KeyValuePair<string, bool>(hint, true);
                return;
            }

            m_mydocument.SystemData.DeleteCurtainSystem(checkedIndices);
        }

        private void csListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            var csInfos = m_mydocument.SystemData.CurtainSystemInfos;

            // data verification
            if (null == csInfos ||
                0 == csInfos.Count)
                return;

            // step 1: activate the selected one
            var csInfo = csInfos[csListBox.SelectedIndex];
            cgCheckedListBox.Items.Clear();
            foreach (var index in csInfo.GridFacesIndices)
            {
                var gridFaceInfo = new GridFaceInfo(index);
                cgCheckedListBox.Items.Add(gridFaceInfo);
            }

            facesCheckedListBox.Items.Clear();
            foreach (var index in csInfo.UncoverFacesIndices)
            {
                var uncoverFaceInfo = new UncoverFaceInfo(index);
                facesCheckedListBox.Items.Add(uncoverFaceInfo);
            }

            // step 2: enable/disable some buttons and refresh the status hints
            // the selected curtain system is created by face array
            // it's not allowed to modify its curtain grids data
            if (csInfo.ByFaceArray)
            {
                // disable the buttons
                addCGButton.Enabled = false;
                removeCGButton.Enabled = false;
                facesCheckedListBox.Enabled = false;
                cgCheckedListBox.Enabled = false;
                var hint = Resources.HINT_CSIsByFaceArray;
                m_mydocument.Message = new KeyValuePair<string, bool>(hint, false);
            }
            // the selected curtain system is created by references of the faces
            // it's allowed to modify its curtain grids data
            else
            {
                // enable the buttons
                if (null == facesCheckedListBox.Items ||
                    0 == facesCheckedListBox.Items.Count)
                    addCGButton.Enabled = false;
                else
                    addCGButton.Enabled = true;
                // at least one curtain grid must be kept
                if (null == cgCheckedListBox.Items ||
                    2 > cgCheckedListBox.Items.Count)
                    removeCGButton.Enabled = false;
                else
                    removeCGButton.Enabled = true;
                facesCheckedListBox.Enabled = true;
                cgCheckedListBox.Enabled = true;
                var hint = "";
                m_mydocument.Message = new KeyValuePair<string, bool>(hint, false);
            }
        }

        private void addCGButton_Click(object sender, EventArgs e)
        {
            // step 1: get the curtain system 
            var csInfos = m_mydocument.SystemData.CurtainSystemInfos;

            // no curtain system available, ask sample user to create some curtain systems first
            if (null == csInfos || 0 == csInfos.Count)
            {
                var hint = Resources.HINT_CreateCSFirst;
                m_mydocument.Message = new KeyValuePair<string, bool>(hint, true);
                return;
            }

            var csInfo = csInfos[csListBox.SelectedIndex];
            // if the curtain system is created by face array, it's forbidden to make other operations on it
            if (csInfo.ByFaceArray) return;
            // step 2: find out the faces to be covered
            var faceIndices = new List<int>();
            for (var i = 0; i < facesCheckedListBox.Items.Count; i++)
            {
                var itemChecked = facesCheckedListBox.GetItemChecked(i);
                if (itemChecked)
                {
                    var info = facesCheckedListBox.Items[i] as UncoverFaceInfo;
                    faceIndices.Add(info.Index);
                }
            }

            // no uncovered faces selected, warn the sample user
            if (null == faceIndices ||
                0 == faceIndices.Count)
            {
                var hint = Resources.HINT_SelectFaceFirst;
                m_mydocument.Message = new KeyValuePair<string, bool>(hint, true);
                return;
            }

            // step 3: cover the selected faces with curtain grids
            csInfo.AddCurtainGrids(faceIndices);
            // step 4: update the UI list boxes
            csListBox_SelectedIndexChanged(null, null);
        }

        private void removeCGButton_Click(object sender, EventArgs e)
        {
            // step 1: get the curtain system 
            var csInfos = m_mydocument.SystemData.CurtainSystemInfos;

            // no curtain system available, ask sample user to create some curtain systems first
            if (null == csInfos || 0 == csInfos.Count)
            {
                var hint = Resources.HINT_CreateCSFirst;
                m_mydocument.Message = new KeyValuePair<string, bool>(hint, true);
                return;
            }

            var csInfo = csInfos[csListBox.SelectedIndex];
            // if the curtain system is created by face array, it's forbidden to make other operations on it
            if (csInfo.ByFaceArray) return;
            // step 2: find out the curtain grids to be removed
            var faceIndices = new List<int>();
            for (var i = 0; i < cgCheckedListBox.Items.Count; i++)
            {
                var itemChecked = cgCheckedListBox.GetItemChecked(i);
                if (itemChecked)
                {
                    var info = cgCheckedListBox.Items[i] as GridFaceInfo;
                    faceIndices.Add(info.FaceIndex);
                }
            }

            // no curtain grids selected, warn the sample user
            if (null == faceIndices ||
                0 == faceIndices.Count)
            {
                var hint = Resources.HINT_SelectCGFirst;
                m_mydocument.Message = new KeyValuePair<string, bool>(hint, true);
                return;
            }

            // step 3: remove the selected curtain grids
            csInfo.RemoveCurtainGrids(faceIndices);
            // step 4: update the UI list boxes
            csListBox_SelectedIndexChanged(null, null);
        }

        private void cgCheckedListBox_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            var indices = new List<int>();
            // get all the unchecked curtain grids
            for (var i = 0; i < cgCheckedListBox.Items.Count; i++)
            {
                var itemChecked = cgCheckedListBox.GetItemChecked(i);
                if (false == itemChecked) indices.Add(i);
            }

            // for curtain system, we must keep at least one curtain grid
            // so it's not allowed to select all the curtain grids to remove
            if (indices.Count <= 1 &&
                CheckState.Unchecked == e.CurrentValue)
            {
                e.NewValue = CheckState.Unchecked;

                var hint = Resources.HINT_KeepOneCG;
                m_mydocument.Message = new KeyValuePair<string, bool>(hint, true);
            }
            else
            {
                var hint = "";
                m_mydocument.Message = new KeyValuePair<string, bool>(hint, false);
            }
        }

        private void facesCheckedListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
        }

        private void mainPanel_Paint(object sender, PaintEventArgs e)
        {
        }

        private void csLabel_Click(object sender, EventArgs e)
        {
        }
    } // end of class
}

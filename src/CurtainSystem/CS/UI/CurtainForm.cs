// Copyright 2023. See https://github.com/ara3d/revit-samples/LICENSE.txt

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using Autodesk.Revit.UI;
using Revit.SDK.Samples.CurtainSystem.CS.CurtainSystem;
using Revit.SDK.Samples.CurtainSystem.CS.Data;
using Revit.SDK.Samples.CurtainSystem.CS.Properties;

namespace Revit.SDK.Samples.CurtainSystem.CS.UI
{
    /// <summary>
    ///     the main window form for UI operations
    /// </summary>
    public partial class CurtainForm : Form
    {
        // the document containing all the data used in the sample
        private readonly MyDocument m_mydocument;

        /// <summary>
        ///     constructor
        /// </summary>
        /// <param name="mydoc">
        ///     the data used in the sample
        /// </param>
        public CurtainForm(MyDocument mydoc)
        {
            m_mydocument = mydoc;

            InitializeComponent();
            // initialize some controls manually
            InitializeCustomComponent();
            // register the customized events
            RegisterEvents();
        }

        /// <summary>
        ///     initialize some controls manually
        /// </summary>
        private void InitializeCustomComponent()
        {
            deleteCSButton.Enabled = false;
            addCGButton.Enabled = false;
            removeCGButton.Enabled = false;
        }

        /// <summary>
        ///     register the customized events
        /// </summary>
        private void RegisterEvents()
        {
            m_mydocument.FatalErrorEvent += m_document_FatalErrorEvent;
            m_mydocument.SystemData.CurtainSystemChanged += m_document_SystemData_CurtainSystemChanged;
            // moniter the sample message change status
            m_mydocument.MessageChanged += m_document_MessageChanged;
        }

        /// <summary>
        ///     Fatal error occurs, close the sample dialog directly
        /// </summary>
        /// <param name="errorMsg">
        ///     the error hint shown to user
        /// </param>
        private void m_document_FatalErrorEvent(string errorMsg)
        {
            // hang the sample and shown the error hint to users
            var result = TaskDialog.Show(Resources.TXT_DialogTitle, errorMsg, TaskDialogCommonButtons.Ok,
                TaskDialogResult.Ok);
            // the user has read the hint and clicked the "OK" button, close the dialog
            if (TaskDialogResult.Ok == result) Close();
        }

        /// <summary>
        ///     curtain system changed(added/removed), refresh the lists
        /// </summary>
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

            foreach (var info in csInfos) csListBox.Items.Add(info);

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

        /// <summary>
        ///     update the status hints in the status strip
        /// </summary>
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

        /// <summary>
        ///     "Create curtain system" button clicked, hide the main form,
        ///     pop up the create curtain system dialog to let user add a new curtain system.
        ///     After the curtain system created, close the dialog and show the main form again
        /// </summary>
        /// <param name="sender">
        ///     object who sent this event
        /// </param>
        /// <param name="e">
        ///     event args
        /// </param>
        private void createCSButton_Click(object sender, EventArgs e)
        {
            Hide();

            // show the "create curtain system" dialog
            using (var dlg = new CreateCurtainSystemDialog(m_mydocument))
            {
                dlg.ShowDialog(this);
            }
        }

        /// <summary>
        ///     delete the checked curtain systems in  the curtain system list box
        /// </summary>
        /// <param name="sender">
        ///     object who sent this event
        /// </param>
        /// <param name="e">
        ///     event args
        /// </param>
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

            // get the checked curtain sytems
            var checkedIndices = new List<int>();
            for (var i = 0; i < csListBox.Items.Count; i++)
            {
                var itemChecked = csListBox.GetItemChecked(i);

                if (itemChecked) checkedIndices.Add(i);
            }

            // no curtain system available or no curtain system selected for deletion
            // update the status hints
            if (null == checkedIndices ||
                0 == checkedIndices.Count)
            {
                var hint = Resources.HINT_SelectCSFirst;
                m_mydocument.Message = new KeyValuePair<string, bool>(hint, true);
                return;
            }

            // delete them
            m_mydocument.SystemData.DeleteCurtainSystem(checkedIndices);
        }

        /// <summary>
        ///     the selected curtain system changed, update the "Curtain grid" and
        ///     "Uncovered faces" list boxes of the selected curtain system
        /// </summary>
        /// <param name="sender">
        ///     object who sent this event
        /// </param>
        /// <param name="e">
        ///     event args
        /// </param>
        private void csListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            var csInfos = m_mydocument.SystemData.CurtainSystemInfos;

            // data verification
            if (null == csInfos ||
                0 == csInfos.Count)
                return;

            //
            // step 1: activate the selected one
            //
            var csInfo = csInfos[csListBox.SelectedIndex];
            // update the curtain grid list box
            cgCheckedListBox.Items.Clear();
            foreach (var index in csInfo.GridFacesIndices)
            {
                var gridFaceInfo = new GridFaceInfo(index);
                cgCheckedListBox.Items.Add(gridFaceInfo);
            }

            // update the uncovered face list box
            facesCheckedListBox.Items.Clear();
            foreach (var index in csInfo.UncoverFacesIndices)
            {
                var uncoverFaceInfo = new UncoverFaceInfo(index);
                facesCheckedListBox.Items.Add(uncoverFaceInfo);
            }

            //
            // step 2: enable/disable some buttons and refresh the status hints
            //
            // the selected curtain system is created by face array
            // it's not allowed to modify its curtain grids data
            if (csInfo.ByFaceArray)
            {
                // disable the buttons
                addCGButton.Enabled = false;
                removeCGButton.Enabled = false;
                facesCheckedListBox.Enabled = false;
                cgCheckedListBox.Enabled = false;
                // update the status hints
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
                // update the status hints
                var hint = "";
                m_mydocument.Message = new KeyValuePair<string, bool>(hint, false);
            }
        }

        /// <summary>
        ///     add curtain grids to the checked faces
        /// </summary>
        /// <param name="sender">
        ///     object who sent this event
        /// </param>
        /// <param name="e">
        ///     event args
        /// </param>
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

        /// <summary>
        ///     remove  the checked curtain grids from the curtain system
        ///     Note: curtain system must have at least one curtain grid
        ///     so sample users can't remove all the curtain grids away
        /// </summary>
        /// <param name="sender">
        ///     object who sent this event
        /// </param>
        /// <param name="e">
        ///     event args
        /// </param>
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

        /// <summary>
        ///     check the curtain grids to delete them, if user wants to check all the curtain
        ///     grids for deletion, prohibit it (must keep at least one curtain grid)
        /// </summary>
        /// <param name="sender">
        ///     object who sent this event
        /// </param>
        /// <param name="e">
        ///     event args
        /// </param>
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

                // update the status hints
                var hint = Resources.HINT_KeepOneCG;
                m_mydocument.Message = new KeyValuePair<string, bool>(hint, true);
            }
            else
            {
                // update the status hints
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

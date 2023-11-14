// Copyright 2023. See https://github.com/ara3d/revit-samples/LICENSE.txt
using System;
using System.Windows.Forms;

namespace Revit.SDK.Samples.DatumsModification.CS
{
    /// <summary>
    /// </summary>
    public partial class DatumStyleSetting : Form
    {
        /// <summary>
        /// </summary>
        public DatumStyleSetting()
        {
            InitializeComponent();
            datumLeftStyleListBox.SetItemChecked(0, DatumStyleModification.ShowLeftBubble);
            datumRightStyleListBox.SetItemChecked(0, DatumStyleModification.ShowRightBubble);
            datumLeftStyleListBox.SetItemChecked(1, DatumStyleModification.AddLeftElbow);
            datumRightStyleListBox.SetItemChecked(1, DatumStyleModification.AddRightElbow);
            datumLeftStyleListBox.SetItemChecked(2, DatumStyleModification.ChangeLeftEnd2D);
            datumRightStyleListBox.SetItemChecked(2, DatumStyleModification.ChangeRightEnd2D);
        }

        private void OkButtonClick(object sender, EventArgs e)
        {
            DatumStyleModification.ShowLeftBubble = datumLeftStyleListBox.GetItemChecked(0);
            DatumStyleModification.ShowRightBubble = datumRightStyleListBox.GetItemChecked(0);
            DatumStyleModification.AddLeftElbow = datumLeftStyleListBox.GetItemChecked(1);
            DatumStyleModification.AddRightElbow = datumRightStyleListBox.GetItemChecked(1);
            DatumStyleModification.ChangeLeftEnd2D = datumLeftStyleListBox.GetItemChecked(2);
            DatumStyleModification.ChangeRightEnd2D = datumRightStyleListBox.GetItemChecked(2);
            Close();
        }
    }
}

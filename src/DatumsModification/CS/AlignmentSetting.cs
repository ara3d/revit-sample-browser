// Copyright 2023. See https://github.com/ara3d/revit-samples/LICENSE.txt
using System.Windows.Forms;

namespace RevitMultiSample.DatumsModification.CS
{
    /// <summary>
    /// </summary>
    public partial class AlignmentSetting : Form
    {
        /// <summary>
        /// </summary>
        public AlignmentSetting()
        {
            InitializeComponent();
            foreach (var name in DatumAlignment.DatumDic.Keys) datumList.Items.Add(name);
        }
    }
}

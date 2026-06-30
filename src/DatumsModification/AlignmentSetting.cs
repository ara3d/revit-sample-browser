// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt
using System.Windows.Forms;

namespace Ara3D.RevitSampleBrowser.DatumsModification.CS
{
    public partial class AlignmentSetting : Form
    {
        public AlignmentSetting()
        {
            InitializeComponent();
            foreach (var name in DatumAlignment.DatumDic.Keys)
            {
                datumList.Items.Add(name);
            }
        }
    }
}

// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt
using System.Windows.Forms;

namespace Ara3D.RevitSampleBrowser.DatumsModification.CS
{
    public partial class PropogateSetting : Form
    {
        public PropogateSetting()
        {
            InitializeComponent();
            foreach (var name in DatumPropagation.ViewDic.Keys)
            {
                propagationViewList.Items.Add(name);
            }
        }
    }
}

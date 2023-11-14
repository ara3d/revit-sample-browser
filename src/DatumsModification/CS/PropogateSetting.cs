// Copyright 2023. See https://github.com/ara3d/revit-samples/LICENSE.txt
using System.Windows.Forms;

namespace Revit.SDK.Samples.DatumsModification.CS
{
    /// <summary>
    /// </summary>
    public partial class PropogateSetting : Form
    {
        /// <summary>
        /// </summary>
        public PropogateSetting()
        {
            InitializeComponent();
            foreach (var name in DatumPropagation.ViewDic.Keys) propagationViewList.Items.Add(name);
        }
    }
}

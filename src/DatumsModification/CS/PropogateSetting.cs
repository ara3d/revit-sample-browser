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
            foreach (var name in DatumPropagation.viewDic.Keys) propagationViewList.Items.Add(name);
        }
    }
}
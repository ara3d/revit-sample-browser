using System.Windows.Forms;

namespace Revit.SDK.Samples.DatumsModification.CS
{
   /// <summary>
   /// 
   /// </summary>
   public partial class AlignmentSetting : Form
   {
      /// <summary>
      /// 
      /// </summary>
      public AlignmentSetting()
      {
         InitializeComponent();
         foreach (var name in DatumAlignment.datumDic.Keys)
         {
            datumList.Items.Add(name);
         }
      }
   }
}

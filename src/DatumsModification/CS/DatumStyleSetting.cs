using System;
using System.Windows.Forms;

namespace Revit.SDK.Samples.DatumsModification.CS
{
   /// <summary>
   /// 
   /// </summary>
   public partial class DatumStyleSetting : Form
   {
      /// <summary>
      /// 
      /// </summary>
      public DatumStyleSetting()
      {
         InitializeComponent();
         datumLeftStyleListBox.SetItemChecked(0,DatumStyleModification.showLeftBubble);
         datumRightStyleListBox.SetItemChecked(0, DatumStyleModification.showRightBubble);
         datumLeftStyleListBox.SetItemChecked(1, DatumStyleModification.addLeftElbow);
         datumRightStyleListBox.SetItemChecked(1, DatumStyleModification.addRightElbow);
         datumLeftStyleListBox.SetItemChecked(2, DatumStyleModification.changeLeftEnd2D);
         datumRightStyleListBox.SetItemChecked(2, DatumStyleModification.changeRightEnd2D);

      }

      private void okButtonClick(object sender, EventArgs e)
      {
         DatumStyleModification.showLeftBubble = datumLeftStyleListBox.GetItemChecked(0);
         DatumStyleModification.showRightBubble = datumRightStyleListBox.GetItemChecked(0);
         DatumStyleModification.addLeftElbow = datumLeftStyleListBox.GetItemChecked(1);
         DatumStyleModification.addRightElbow = datumRightStyleListBox.GetItemChecked(1);
         DatumStyleModification.changeLeftEnd2D = datumLeftStyleListBox.GetItemChecked(2);
         DatumStyleModification.changeRightEnd2D = datumRightStyleListBox.GetItemChecked(2);
         Close();
      }
   }
}

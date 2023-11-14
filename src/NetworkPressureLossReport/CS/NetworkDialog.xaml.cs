﻿using System.Collections.Generic;
using System.Windows;
using System.Windows.Forms;
using Autodesk.Revit.DB;

namespace Revit.SDK.Samples.NetworkPressureLossReport
{
   /// <summary>
   /// Interaction logic for NetworkDialog.xaml
   /// </summary>
   public partial class NetworkDialog : Window
   {
      private Document m_doc;
      private IList<NetworkInfo> m_networks;
      public NetworkDialog(Document doc)
      {
         m_doc = doc;
         InitializeComponent();

         refreshNetworkList();
      }

      private void refreshNetworkList()
      {
         m_networks = NetworkInfo.FindValidNetworks(m_doc);

         NetworkList.ItemsSource = m_networks;
         if(m_networks.Count > 0)
         {
            NetworkList.SelectedIndex = 0;
         }
      }
      public void Cancel_Click(object sender, RoutedEventArgs e)
      {
         Close();
      }
      public void View_Click(object sender, RoutedEventArgs e)
      {
         if (NetworkList.SelectedItems.Count <= 0)
            return;

         using (var tran = new Transaction(m_doc))
         {
            tran.Start("Create Analysis View");

            var viewer = new AVFViewer(m_doc.ActiveView, ChxItemized.IsChecked);
            viewer.InitAVF();

            foreach (var item in NetworkList.SelectedItems)
            {
               var net = item as NetworkInfo;
               if (net != null)
               {
                  net.UpdateView(viewer);
               }
            }
            viewer.FinishDisplayStyle();

            tran.Commit();
         }

         Close();
      }
      public void Report_Click(object sender, RoutedEventArgs e)
      {
         var idx = NetworkList.SelectedIndex;
         if (idx < 0 || m_networks.Count <= 0)
            return;

         var saveFileDialog1 = new SaveFileDialog();

         saveFileDialog1.FileName = "PressureReport.csv";
         saveFileDialog1.Filter = "CSV Files | *.csv";
         saveFileDialog1.DefaultExt = "csv";
         saveFileDialog1.FilterIndex = 2;
         saveFileDialog1.RestoreDirectory = true;

         if(saveFileDialog1.ShowDialog() == System.Windows.Forms.DialogResult.OK)
         {
            using (var ex = new CSVExporter(saveFileDialog1.FileName, ChxItemized.IsChecked))
            {
               // Pass over the document and domain type to the exporter.
               var netInfo = m_networks[idx];
               ex.Document = netInfo.Document;
               ex.DomainType = netInfo.DomainType;
               netInfo.ExportCSV(ex);
            }
         }
      }
   }
}
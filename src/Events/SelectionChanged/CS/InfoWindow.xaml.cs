//
// (C) Copyright 2003-2021 by Autodesk, Inc.
//
// Permission to use, copy, modify, and distribute this software in
// object code form for any purpose and without fee is hereby granted,
// provided that the above copyright notice appears in all copies and
// that both that copyright notice and the limited warranty and
// restricted rights notice below appear in all supporting
// documentation.
//
// AUTODESK PROVIDES THIS PROGRAM "AS IS" AND WITH ALL FAULTS.
// AUTODESK SPECIFICALLY DISCLAIMS ANY IMPLIED WARRANTY OF
// MERCHANTABILITY OR FITNESS FOR A PARTICULAR USE. AUTODESK, INC.
// DOES NOT WARRANT THAT THE OPERATION OF THE PROGRAM WILL BE
// UNINTERRUPTED OR ERROR FREE.
//
// Use, duplication, or disclosure by the U.S. Government is subject to
// restrictions set forth in FAR 52.227-19 (Commercial Computer
// Software - Restricted Rights) and DFAR 252.227-7013(c)(1)(ii)
// (Rights in Technical Data and Computer Software), as applicable.
//

using System;
using System.Windows;
using System.Windows.Controls;

namespace Revit.SDK.Samples.SelectionChanged.CS
{
   /// <summary>
   /// Interaction logic for InfoWindow.xaml
   /// </summary>
   public partial class InfoWindow : Window
   {      
      /// <summary>
      /// Constructor
      /// </summary>
      /// <param name="application"></param>
      public InfoWindow()
      {
         InitializeComponent();
      }

      #region 'Event handlers for selection changed event'
      internal void RevitUIApp_SelectionChanged(object sender, Autodesk.Revit.UI.Events.SelectionChangedEventArgs e)
      {
         textBox_log.Text += e.GetInfo(true); 
      }
      #endregion


      private void textBox_log_TextChanged(object sender, TextChangedEventArgs e)
      {
         textBox_log.ScrollToEnd();
      }

      private void Window_Loaded(object sender, RoutedEventArgs e)
      {
         var desktopWorkingArea = SystemParameters.WorkArea;
         Left = desktopWorkingArea.Right - Width;
         Top = desktopWorkingArea.Bottom - Height;
      }

      private void Button_Click(object sender, RoutedEventArgs e)
      {
         textBox_log.Text = "";
      }

      private void Window_Closed(object sender, EventArgs e)
      {        
         
         SelectionChanged.InfoWindow = null;
      }
   }
}
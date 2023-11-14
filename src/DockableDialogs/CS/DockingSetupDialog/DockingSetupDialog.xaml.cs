//
// (C) Copyright 2003-2019 by Autodesk, Inc. All rights reserved.
//
// Permission to use, copy, modify, and distribute this software in
// object code form for any purpose and without fee is hereby granted
// provided that the above copyright notice appears in all copies and
// that both that copyright notice and the limited warranty and
// restricted rights notice below appear in all supporting
// documentation.

//
// AUTODESK PROVIDES THIS PROGRAM 'AS IS' AND WITH ALL ITS FAULTS.
// AUTODESK SPECIFICALLY DISCLAIMS ANY IMPLIED WARRANTY OF
// MERCHANTABILITY OR FITNESS FOR A PARTICULAR USE. AUTODESK, INC.
// DOES NOT WARRANT THAT THE OPERATION OF THE PROGRAM WILL BE
// UNINTERRUPTED OR ERROR FREE.
//
// Use, duplication, or disclosure by the U.S. Government is subject to
// restrictions set forth in FAR 52.227-19 (Commercial Computer
// Software - Restricted Rights) and DFAR 252.227-7013(c)(1)(ii)
// (Rights in Technical Data and Computer Software), as applicable. 

using System;
using System.Windows;
using Autodesk.Revit.UI;

namespace Revit.SDK.Samples.DockableDialogs.CS
{
    /// <summary>
    ///     Interaction logic for DockingSetupDialog.xaml
    /// </summary>
    public partial class DockingSetupDialog : Window
    {
        private int m_bottom;

        private int m_left;
        private string m_mainPageGuid;
        private int m_right;
        private string m_targetGuidString;
        private int m_top;

        public DockingSetupDialog()
        {
            InitializeComponent();
            tb_newGuid.Text = Globals.sm_UserDockablePaneId.Guid.ToString();
        }

        public int FloatLeft => m_left;
        public int FloatRight => m_right;
        public int FloatTop => m_top;
        public int FloatBottom => m_bottom;

        /// <summary>
        ///     The guid of the main docking page.
        /// </summary>
        public Guid MainPageGuid
        {
            get
            {
                var retval = Guid.Empty;
                if (m_mainPageGuid == "null")
                {
                    return retval;
                }

                try
                {
                    retval = new Guid(m_mainPageGuid);
                }
                catch (Exception)
                {
                }

                return retval;
            }
        }

        /// <summary>
        ///     The guid of a tab-behind system pane.
        /// </summary>
        public Guid TargetGuid
        {
            get
            {
                var retval = Guid.Empty;
                if (m_targetGuidString == "null")
                {
                    return retval;
                }

                try
                {
                    retval = new Guid(m_targetGuidString);
                }
                catch (Exception)
                {
                }

                return retval;
            }
        }

        public DockPosition DockPosition { get; private set; }

        /// <summary>
        ///     Take user-input data for docking dialog choices and attempt to parse it
        ///     into higher-level data for later use.
        /// </summary>
        private void btn_ok_Click(object sender, RoutedEventArgs e)
        {
            int.TryParse(tb_left.Text, out m_left);
            int.TryParse(tb_right.Text, out m_right);
            int.TryParse(tb_top.Text, out m_top);
            int.TryParse(tb_bottom.Text, out m_bottom);

            if (!string.IsNullOrEmpty(tb_newGuid.Text))
                m_mainPageGuid = tb_newGuid.Text;

            if (!string.IsNullOrEmpty(tb_targetGuid.Text))
                m_targetGuidString = tb_targetGuid.Text;

            else if (rb_ElementView.IsChecked == true)
                m_targetGuidString = DockablePanes.BuiltInDockablePanes.ElementView.Guid.ToString();

            else if (rb_SystemNav.IsChecked == true)
                m_targetGuidString = DockablePanes.BuiltInDockablePanes.SystemNavigator.Guid.ToString();

            else if (rb_HostByLinkNavigator.IsChecked == true)
                m_targetGuidString = DockablePanes.BuiltInDockablePanes.HostByLinkNavigator.Guid.ToString();

            else if (rb_ProjectBrowser.IsChecked == true)
                m_targetGuidString = DockablePanes.BuiltInDockablePanes.ProjectBrowser.Guid.ToString();

            else if (rb_PropertiesPalette.IsChecked == true)
                m_targetGuidString = DockablePanes.BuiltInDockablePanes.PropertiesPalette.Guid.ToString();

            else if (rb_RebarBrowser.IsChecked == true)
                m_targetGuidString = DockablePanes.BuiltInDockablePanes.RebarBrowser.Guid.ToString();

            else
                m_targetGuidString = "null";

            if (rb_bottom.IsChecked == true)
                DockPosition = DockPosition.Bottom;
            else if (rb_left.IsChecked == true)
                DockPosition = DockPosition.Left;
            else if (rb_right.IsChecked == true)
                DockPosition = DockPosition.Right;
            else if (rb_top.IsChecked == true)
                DockPosition = DockPosition.Top;
            else if (rb_tabbed.IsChecked == true)
                DockPosition = DockPosition.Tabbed;
            else
                DockPosition = DockPosition.Floating;

            DialogResult = true;
            Close();
        }

        private void SetFloatingCoordsStates(bool enabled)
        {
            tb_right.IsEnabled = enabled;
            tb_left.IsEnabled = enabled;
            tb_top.IsEnabled = enabled;
            tb_bottom.IsEnabled = enabled;
        }

        private void SetTabTargetStates(bool enabled)
        {
            rb_ElementView.IsEnabled = enabled;
            rb_HostByLinkNavigator.IsEnabled = enabled;
            rb_ProjectBrowser.IsEnabled = enabled;
            rb_PropertiesPalette.IsEnabled = enabled;
            rb_RebarBrowser.IsEnabled = enabled;
            rb_SystemNav.IsEnabled = enabled;
            rb_Null.IsEnabled = enabled;
        }

        private void rb_top_Checked(object sender, RoutedEventArgs e)
        {
            SetFloatingCoordsStates(false);
            SetTabTargetStates(false);
        }

        private void rb_tabbed_Checked(object sender, RoutedEventArgs e)
        {
            SetFloatingCoordsStates(false);
            SetTabTargetStates(true);
        }


        private void rb_left_Checked(object sender, RoutedEventArgs e)
        {
            SetFloatingCoordsStates(false);
            SetTabTargetStates(false);
        }

        private void rb_right_Checked(object sender, RoutedEventArgs e)
        {
            SetFloatingCoordsStates(false);
            SetTabTargetStates(false);
        }

        private void rb_bottom_Checked(object sender, RoutedEventArgs e)
        {
            SetFloatingCoordsStates(false);
            SetTabTargetStates(false);
        }

        private void rb_floating_Checked(object sender, RoutedEventArgs e)
        {
            SetFloatingCoordsStates(true);
            SetTabTargetStates(false);
        }
    }
}
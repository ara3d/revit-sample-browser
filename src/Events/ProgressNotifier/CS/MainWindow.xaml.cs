// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Events;
using Microsoft.Win32;
using Application = Autodesk.Revit.ApplicationServices.Application;

namespace Ara3D.RevitSampleBrowser.Events.ProgressNotifier.CS
{
    /// <summary>
    ///     Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly Application m_application;
        private bool m_cancel;
        private ProgressItem m_previousEvent;
        private readonly ProgressStack m_progressStack;

        private bool m_receivedCancelEvent;

        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name="application"></param>
        public MainWindow(Application application)
        {
            InitializeComponent();
            m_application = application;
            m_progressStack = new ProgressStack();
            m_application.ProgressChanged += RevitApp_ProgressChanged;
            m_application.DocumentOpened += RevitApp_DocumentOpened;
            m_cancel = false;
            m_receivedCancelEvent = false;
            m_previousEvent = null;
        }

        private void RevitApp_DocumentOpened(object sender, DocumentOpenedEventArgs e)
        {
            textBox_log.Text +=
                $"RevitApp_DocumentOpened: Cancellable:{e.Cancellable} , IsCancelled: {e.IsCancelled()} , Status:{e.Status}{Environment.NewLine}";
            m_receivedCancelEvent = e.Status == RevitAPIEventStatus.Cancelled;
        }

        private void RevitApp_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            if (e.Stage == ProgressStage.Unchanged || (e.Stage == ProgressStage.PositionChanged && e.Cancellable))
                button_Cancel.IsEnabled = true;
            else
                button_Cancel.IsEnabled = false;

            System.Windows.Forms.Application.DoEvents();

            if (m_cancel)
            {
                var shouldCancel = e.Cancellable;
                if (e.IsCancelled())
                    textBox_log.Text += $"!! We are already canceled!{Environment.NewLine}";

                try
                {
                    e.Cancel();
                    m_cancel = false;
                }
                catch (Exception ex)
                {
                    textBox_log.Text +=
                        $"Exception: {ex}{Environment.NewLine}'  Cancelable' value: {shouldCancel}{Environment.NewLine}";
                }
            }

            var itemReturn = m_progressStack.AddEventData(e);

            stackPanel_ProgressData.Children.Clear();
            var progressItems = m_progressStack.ToStringList(6);
            foreach (var progressItem in progressItems)
            {
                var tbProgressItem = new TextBox
                {
                    Text = progressItem
                };
                if (tbProgressItem.Text.Contains("<None>")) tbProgressItem.Foreground = Brushes.LightSlateGray;
                stackPanel_ProgressData.Children.Add(tbProgressItem);
            }

            if (itemReturn.Stage == ProgressStage.RangeChanged || itemReturn.Stage == ProgressStage.Unchanged)
                if (m_previousEvent != null)
                {
                }

            //   textBox_log.Text += "Previous: "+  previousEventData + Environment.NewLine + "Current: " + itemReturn.ToString() + Environment.NewLine;
            System.Windows.Forms.Application.DoEvents();

            m_previousEvent = itemReturn;
        }

        private void Button_Open_Click(object sender, RoutedEventArgs e)
        {
            var ofd = new OpenFileDialog
            {
                DefaultExt = ".rvt",
                Filter = "rvt files (*.rvt)|*.rvt|All files (*.*)|*.*",
                FilterIndex = 1,
                Title = "Open revit documents."
            };

            ofd.ShowDialog();
            if (!File.Exists(ofd.FileName))
                return;

            label_FileName.Content = ofd.FileName;
            System.Windows.Forms.Application.DoEvents();
            Document document = null;
            try
            {
                document = m_application.OpenDocumentFile(ofd.FileName);
                textBox_log.Text += $"Opened filename = {document.Title}{Environment.NewLine}";
            }
            catch (Exception ex)
            {
                if (m_receivedCancelEvent)
                {
                    string isNull;
                    if (document == null)
                        isNull = " is null.";
                    else
                        isNull = " is not null.";

                    textBox_log.Text += $"Open Document has thrown an exception.{Environment.NewLine}";
                    textBox_log.Text +=
                        $"We just got a cancel event, so this exception is likely from 'Open' being canceled. Returned document{isNull}{Environment.NewLine}";
                    m_receivedCancelEvent = false;
                }
                else
                {
                    textBox_log.Text += ex + Environment.NewLine;
                }
            }
        }

        private void button_Cancel_Click(object sender, RoutedEventArgs e)
        {
            m_cancel = true;
        }
    }
}

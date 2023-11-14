// Copyright 2023. See https://github.com/ara3d/revit-samples/LICENSE.txt

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing.Printing;
using System.Windows.Forms;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Control = System.Windows.Forms.Control;
using PrintRange = Autodesk.Revit.DB.PrintRange;

namespace Revit.SDK.Samples.ViewPrinter.CS
{
    /// <summary>
    ///     Exposes the print interfaces just like the Print Dialog (File->Print...) in UI.
    /// </summary>
    public class PrintMgr
    {
        private readonly ExternalCommandData m_commandData;
        private readonly PrintManager m_printMgr;

        public PrintMgr(ExternalCommandData commandData)
        {
            m_commandData = commandData;
            m_printMgr = commandData.Application.ActiveUIDocument.Document.PrintManager;
        }

        public List<string> InstalledPrinterNames
        {
            get
            {
                try
                {
                    var printers
                        = PrinterSettings.InstalledPrinters;
                    var printerNames = new string[printers.Count];
                    printers.CopyTo(printerNames, 0);

                    var names = new List<string>();
                    foreach (var name in printerNames) names.Add(name);

                    return 0 == names.Count ? null : names;
                }
                catch (Exception)
                {
                    return null; // can not get installed printer
                }
            }
        }

        public string PrinterName
        {
            get => m_printMgr.PrinterName;
            set
            {
                try
                {
                    m_printMgr.SelectNewPrintDriver(value);
                }
                catch (Exception)
                {
                    // un-available or exceptional printer
                }
            }
        }

        public string PrintSetupName
        {
            get
            {
                var setting = m_printMgr.PrintSetup.CurrentPrintSetting;
                return setting is PrintSetting printSetting ? printSetting.Name : ConstData.InSessionName;
            }
        }

        public bool IsPrintToFile
        {
            get => m_printMgr.PrintToFile;
            set
            {
                m_printMgr.PrintToFile = value;
                m_printMgr.Apply();
            }
        }

        public bool IsCombinedFile
        {
            get => m_printMgr.CombinedFile;
            set
            {
                // CombinedFile property cannot be setted to false when the Print Range is Current/Visable!
                m_printMgr.CombinedFile = value;
                m_printMgr.Apply();
            }
        }

        public string PrintToFileName => m_printMgr.PrintToFileName;

        public PrintRange PrintRange
        {
            get => m_printMgr.PrintRange;
            set
            {
                m_printMgr.PrintRange = value;
                m_printMgr.Apply();
            }
        }

        public bool Collate
        {
            get => m_printMgr.Collate;
            set
            {
                m_printMgr.Collate = value;
                m_printMgr.Apply();
            }
        }

        public int CopyNumber
        {
            get => m_printMgr.CopyNumber;
            set
            {
                m_printMgr.CopyNumber = value;
                m_printMgr.Apply();
            }
        }

        public bool PrintOrderReverse
        {
            get => m_printMgr.PrintOrderReverse;
            set
            {
                m_printMgr.PrintOrderReverse = value;
                m_printMgr.Apply();
            }
        }

        public string SelectedViewSheetSetName
        {
            get
            {
                var theSet = m_printMgr.ViewSheetSetting.CurrentViewSheetSet;
                return theSet is ViewSheetSet set ? set.Name : ConstData.InSessionName;
            }
        }

        public string DocumentTitle
        {
            get
            {
                var title = m_commandData.Application.ActiveUIDocument.Document.Title;
                if (title.Contains(".rvt"))
                    return title.Remove(title.LastIndexOf(".")) + PostFix;
                return title + PostFix;
            }
        }

        public string PostFix
        {
            get
            {
                string postfix = null;
                switch (m_printMgr.IsVirtual)
                {
                    case VirtualPrinterType.AdobePDF:
                        postfix = ".pdf";
                        break;
                    case VirtualPrinterType.DWFWriter:
                        postfix = ".dwf";
                        break;
                    case VirtualPrinterType.XPSWriter:
                        postfix = ".xps";
                        break;
                    case VirtualPrinterType.None:
                        postfix = ".prn";
                        break;
                }

                return postfix;
            }
        }

        public string ChangePrintToFileName()
        {
            using (var saveDlg = new SaveFileDialog())
            {
                string postfix = null;

                switch (m_printMgr.IsVirtual)
                {
                    case VirtualPrinterType.AdobePDF:
                        saveDlg.Filter = "pdf files (*.pdf)|*.pdf";
                        postfix = ".pdf";
                        break;
                    case VirtualPrinterType.DWFWriter:
                        saveDlg.Filter = "dwf files (*.dwf)|*.dwf";
                        postfix = ".dwf";
                        break;
                    case VirtualPrinterType.None:
                        saveDlg.Filter = "prn files (*.prn)|*.prn";
                        postfix = ".prn";
                        break;
                    case VirtualPrinterType.XPSWriter:
                        saveDlg.Filter = "XPS files (*.xps)|*.xps";
                        postfix = ".xps";
                        break;
                }

                var title = m_commandData.Application.ActiveUIDocument.Document.Title;
                if (title.Contains(".rvt"))
                    saveDlg.FileName = title.Remove(title.LastIndexOf(".")) + postfix;
                else
                    saveDlg.FileName = title + postfix;

                if (saveDlg.ShowDialog() == DialogResult.OK)
                    return m_printMgr.PrintToFileName
                        = saveDlg.FileName;
                return null;
            }
        }

        public void ChangePrintSetup()
        {
            using (var dlg = new PrintSetupForm(
                       new PrintStp(m_printMgr, m_commandData)))
            {
                dlg.ShowDialog();
            }
        }

        public void SelectViewSheetSet()
        {
            using (var dlg = new ViewSheetSetForm(
                       new ViewSheets(m_commandData.Application.ActiveUIDocument.Document)))
            {
                dlg.ShowDialog();
            }
        }

        public bool SubmitPrint()
        {
            return m_printMgr.SubmitPrint();
        }

        public bool VerifyPrintToFile(Control controlToEnableOrNot)
        {
            // Enable terms (or):
            // 1. Print to non-virtual printer.
            return controlToEnableOrNot.Enabled =
                m_printMgr.IsVirtual == VirtualPrinterType.None ? true : false;
        }

        public bool VerifyCopies(Collection<Control> controlsToEnableOrNot)
        {
            // Enable terms (or):
            // 1. Print to non-virtual priter (physical printer or OneNote), and 
            // the "Print to file" check box is NOT checked.
            // Note: SnagIt is an exception

            var enableOrNot = m_printMgr.IsVirtual == VirtualPrinterType.None
                              && !m_printMgr.PrintToFile;

            try
            {
            }
            catch (Exception)
            {
                enableOrNot = false;
                // Note: SnagIt is an exception
            }

            foreach (var control in controlsToEnableOrNot) control.Enabled = enableOrNot;

            return enableOrNot;
        }

        public bool VerifyPrintToFileName(Collection<Control> controlsToEnableOrNot)
        {
            // Enable terms (or):
            // 1. Print to virtual priter (PDF or DWF printer)
            // 2. Print to none-virtual printer (physical printer or OneNote), and the 
            // "Print to file" check box is checked.
            var enableOrNot = m_printMgr.IsVirtual != VirtualPrinterType.None
                              || (m_printMgr.IsVirtual == VirtualPrinterType.None
                                  && m_printMgr.PrintToFile);

            foreach (var control in controlsToEnableOrNot) control.Enabled = enableOrNot;

            return enableOrNot;
        }

        public bool VerifyPrintToSingleFile(Control controlToEnableOrNot)
        {
            // Enable terms (or):
            // 1. Print to virtual priter (PDF or DWF printer)
            return controlToEnableOrNot.Enabled = m_printMgr.IsVirtual != VirtualPrinterType.None;
        }

        public bool VerifyPrintToSeparateFile(Control controlToEnableOrNot)
        {
            // Enable terms (or):
            // 1. Print to virtual priter (PDF or DWF printer) and Print range is select.
            // 2. a) Print to none-virtual printer (physical printer or OneNote),  b) the 
            // "Print to file" check box is checked, and c) the Print range is select

            return controlToEnableOrNot.Enabled = (m_printMgr.IsVirtual != VirtualPrinterType.None
                                                   && m_printMgr.PrintRange == PrintRange.Select)
                                                  || (m_printMgr.IsVirtual == VirtualPrinterType.None
                                                      && m_printMgr.PrintRange == PrintRange.Select
                                                      && m_printMgr.PrintToFile);
        }

        public bool VerifyCollate(Control controlToEnableOrNot)
        {
            // Enable terms (or):
            // 1. a) Print range is select b) the copy number is more 1 c) and the Print to file
            // is not selected.
            var cn = 0;
            try
            {
                cn = m_printMgr.CopyNumber;
            }
            catch (InvalidOperationException)
            {
                //The property CopyNumber is not available.
            }

            return controlToEnableOrNot.Enabled = m_printMgr.PrintRange == PrintRange.Select
                                                  && !m_printMgr.PrintToFile
                                                  && cn > 1;
        }

        public bool VerifySelectViewSheetSet(Collection<Control> controlsToEnableOrNot)
        {
            // Enable terms (or):
            // 1. Print range is select.
            var enableOrNot = m_printMgr.PrintRange == PrintRange.Select;
            foreach (var control in controlsToEnableOrNot) control.Enabled = enableOrNot;

            return enableOrNot;
        }

        /// <summary>
        ///     global and consistent for message box with same caption
        /// </summary>
        /// <param name="text">MessageBox's text.</param>
        public static void MyMessageBox(string text)
        {
            TaskDialog.Show("View Printer", text);
        }
    }
}

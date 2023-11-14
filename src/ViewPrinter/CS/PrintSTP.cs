//
// (C) Copyright 2003-2019 by Autodesk, Inc.
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
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Control = System.Windows.Forms.Control;

namespace Revit.SDK.Samples.ViewPrinter.CS
{
    /// <summary>
    ///     Change and save printer setup setting, exposes the print parameters just
    ///     like the Print Setup Dialog (File->Print Setup...) in UI such as Printer name,
    ///     paper, zoom, options, etc.
    /// </summary>
    public class PrintSTP : ISettingNameOperation
    {
        private readonly ExternalCommandData m_commandData;
        private readonly PrintManager m_printMgr;

        public PrintSTP(PrintManager printMgr
            , ExternalCommandData commandData)
        {
            m_commandData = commandData;
            m_printMgr = printMgr;
        }

        public string PrinterName => m_printMgr.PrinterName;

        public List<string> PrintSettingNames
        {
            get
            {
                var names = new List<string>();
                //foreach (Element printSetting in m_commandData.Application.ActiveUIDocument.Document.PrintSettings)
                var printSettingIds = m_commandData.Application.ActiveUIDocument.Document.GetPrintSettingIds();
                foreach (var eid in printSettingIds)
                {
                    var printSetting = m_commandData.Application.ActiveUIDocument.Document.GetElement(eid);
                    names.Add(printSetting.Name);
                }

                names.Add(ConstData.InSessionName);
                return names;
            }
        }

        public List<string> PaperSizes
        {
            get
            {
                var names = new List<string>();
                foreach (PaperSize ps in m_printMgr.PaperSizes) names.Add(ps.Name);
                return names;
            }
        }

        public string PaperSize
        {
            get
            {
                try
                {
                    return m_printMgr.PrintSetup.CurrentPrintSetting.PrintParameters.PaperSize.Name;
                }
                catch (Exception)
                {
                    return null;
                }
            }
            set
            {
                foreach (PaperSize ps in m_printMgr.PaperSizes)
                    if (ps.Name.Equals(value))
                    {
                        m_printMgr.PrintSetup.CurrentPrintSetting.PrintParameters.PaperSize = ps;
                        break;
                    }
            }
        }

        public List<string> PaperSources
        {
            get
            {
                var names = new List<string>();
                foreach (PaperSource ps in m_printMgr.PaperSources) names.Add(ps.Name);
                return names;
            }
        }

        public string PaperSource
        {
            get => m_printMgr.PrintSetup.CurrentPrintSetting.PrintParameters.PaperSource.Name;
            set
            {
                foreach (PaperSource ps in m_printMgr.PaperSources)
                    if (ps.Name.Equals(value))
                    {
                        m_printMgr.PrintSetup.CurrentPrintSetting.PrintParameters.PaperSource = ps;
                        break;
                    }
            }
        }

        public PageOrientationType PageOrientation
        {
            get => m_printMgr.PrintSetup.CurrentPrintSetting.PrintParameters.PageOrientation;
            set => m_printMgr.PrintSetup.CurrentPrintSetting.PrintParameters.PageOrientation = value;
        }

        public PaperPlacementType PaperPlacement
        {
            get => m_printMgr.PrintSetup.CurrentPrintSetting.PrintParameters.PaperPlacement;
            set => m_printMgr.PrintSetup.CurrentPrintSetting.PrintParameters.PaperPlacement = value;
        }

        public Array MarginTypes => Enum.GetValues(typeof(MarginType));

        public MarginType SelectedMarginType
        {
            get => m_printMgr.PrintSetup.CurrentPrintSetting.PrintParameters.MarginType;
            set => m_printMgr.PrintSetup.CurrentPrintSetting.PrintParameters.MarginType = value;
        }

        public double OriginOffsetX
        {
            get => m_printMgr.PrintSetup.CurrentPrintSetting.PrintParameters.OriginOffsetX;
            set => m_printMgr.PrintSetup.CurrentPrintSetting.PrintParameters.OriginOffsetX = value;
        }

        public double OriginOffsetY
        {
            get => m_printMgr.PrintSetup.CurrentPrintSetting.PrintParameters.OriginOffsetY;
            set => m_printMgr.PrintSetup.CurrentPrintSetting.PrintParameters.OriginOffsetY = value;
        }

        public HiddenLineViewsType HiddenLineViews
        {
            get => m_printMgr.PrintSetup.CurrentPrintSetting.PrintParameters.HiddenLineViews;
            set => m_printMgr.PrintSetup.CurrentPrintSetting.PrintParameters.HiddenLineViews = value;
        }

        public int Zoom
        {
            get => m_printMgr.PrintSetup.CurrentPrintSetting.PrintParameters.Zoom;
            set => m_printMgr.PrintSetup.CurrentPrintSetting.PrintParameters.Zoom = value;
        }

        public ZoomType ZoomType
        {
            get => m_printMgr.PrintSetup.CurrentPrintSetting.PrintParameters.ZoomType;
            set => m_printMgr.PrintSetup.CurrentPrintSetting.PrintParameters.ZoomType = value;
        }

        public Array RasterQualities => Enum.GetValues(typeof(RasterQualityType));

        public RasterQualityType RasterQuality
        {
            get => m_printMgr.PrintSetup.CurrentPrintSetting.PrintParameters.RasterQuality;
            set => m_printMgr.PrintSetup.CurrentPrintSetting.PrintParameters.RasterQuality = value;
        }

        public Array Colors => Enum.GetValues(typeof(ColorDepthType));

        public ColorDepthType Color
        {
            get => m_printMgr.PrintSetup.CurrentPrintSetting.PrintParameters.ColorDepth;
            set => m_printMgr.PrintSetup.CurrentPrintSetting.PrintParameters.ColorDepth = value;
        }

        public bool ViewLinksinBlue
        {
            get => m_printMgr.PrintSetup.CurrentPrintSetting.PrintParameters.ViewLinksinBlue;
            set => m_printMgr.PrintSetup.CurrentPrintSetting.PrintParameters.ViewLinksinBlue = value;
        }

        public bool HideScopeBoxes
        {
            get => m_printMgr.PrintSetup.CurrentPrintSetting.PrintParameters.HideScopeBoxes;
            set => m_printMgr.PrintSetup.CurrentPrintSetting.PrintParameters.HideScopeBoxes = value;
        }

        public bool HideReforWorkPlanes
        {
            get => m_printMgr.PrintSetup.CurrentPrintSetting.PrintParameters.HideReforWorkPlanes;
            set => m_printMgr.PrintSetup.CurrentPrintSetting.PrintParameters.HideReforWorkPlanes = value;
        }

        public bool HideCropBoundaries
        {
            get => m_printMgr.PrintSetup.CurrentPrintSetting.PrintParameters.HideCropBoundaries;
            set => m_printMgr.PrintSetup.CurrentPrintSetting.PrintParameters.HideCropBoundaries = value;
        }

        public bool HideUnreferencedViewTags
        {
            get => m_printMgr.PrintSetup.CurrentPrintSetting.PrintParameters.HideUnreferencedViewTags;
            set => m_printMgr.PrintSetup.CurrentPrintSetting.PrintParameters.HideUnreferencedViewTags = value;
        }

        public string Prefix => "Default ";

        public int SettingCount => m_commandData.Application.ActiveUIDocument.Document.GetPrintSettingIds().Count;

        public bool SaveAs(string newName)
        {
            try
            {
                return m_printMgr.PrintSetup.SaveAs(newName);
            }
            catch (Exception ex)
            {
                PrintMgr.MyMessageBox(ex.Message);
                return false;
            }
        }

        public bool Rename(string name)
        {
            try
            {
                return m_printMgr.PrintSetup.Rename(name);
            }
            catch (Exception ex)
            {
                PrintMgr.MyMessageBox(ex.Message);
                return false;
            }
        }

        public string SettingName
        {
            get
            {
                var setting = m_printMgr.PrintSetup.CurrentPrintSetting;
                return setting is PrintSetting ? (setting as PrintSetting).Name : ConstData.InSessionName;
            }
            set
            {
                if (value == ConstData.InSessionName)
                {
                    m_printMgr.PrintSetup.CurrentPrintSetting = m_printMgr.PrintSetup.InSession;
                    return;
                }

                //foreach (Element printSetting in m_commandData.Application.ActiveUIDocument.Document.PrintSettings)
                var printSettingIds = m_commandData.Application.ActiveUIDocument.Document.GetPrintSettingIds();
                foreach (var eid in printSettingIds)
                {
                    var printSetting = m_commandData.Application.ActiveUIDocument.Document.GetElement(eid);
                    if (printSetting.Name.Equals(value))
                        m_printMgr.PrintSetup.CurrentPrintSetting = printSetting as PrintSetting;
                }
            }
        }

        public bool Save()
        {
            try
            {
                return m_printMgr.PrintSetup.Save();
            }
            catch (Exception ex)
            {
                PrintMgr.MyMessageBox(ex.Message);
                return false;
            }
        }

        public void Revert()
        {
            try
            {
                m_printMgr.PrintSetup.Revert();
            }
            catch (Exception ex)
            {
                PrintMgr.MyMessageBox(ex.Message);
            }
        }

        public bool Delete()
        {
            try
            {
                return m_printMgr.PrintSetup.Delete();
            }
            catch (Exception ex)
            {
                PrintMgr.MyMessageBox(ex.Message);
                return false;
            }
        }

        public bool VerifyMarginType(Control controlToEnableOrNot)
        {
            // Enable terms (or):
            // 1. Paper placement is LowerLeft.
            return controlToEnableOrNot.Enabled =
                m_printMgr.PrintSetup.CurrentPrintSetting.PrintParameters.PaperPlacement ==
                PaperPlacementType.LowerLeft;
        }

        public bool VerifyUserDefinedMargin(Collection<Control> controlsToEnableOrNot)
        {
            var enableOrNot =
                m_printMgr.PrintSetup.CurrentPrintSetting.PrintParameters.MarginType == MarginType.UserDefined;

            foreach (var control in controlsToEnableOrNot) control.Enabled = enableOrNot;

            return enableOrNot;
        }
    }
}
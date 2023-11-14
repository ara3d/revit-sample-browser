// Copyright 2023. See https://github.com/ara3d/revit-samples/LICENSE.txt

using System;
using System.Collections.Generic;
using System.Linq;
using Autodesk.Revit.DB;

namespace Revit.SDK.Samples.ViewPrinter.CS
{
    public enum VisibleType
    {
        VT_ViewOnly,
        VT_SheetOnly,
        VT_BothViewAndSheet,
        VT_None
    }

    public interface ISettingNameOperation
    {
        string SettingName { get; set; }

        string Prefix { get; }

        int SettingCount { get; }

        bool Rename(string name);
        bool SaveAs(string newName);
    }

    /// <summary>
    ///     Define some config data which is useful in this sample.
    /// </summary>
    public static class ConstData
    {
        /// <summary>
        ///     The const string data which is used as the name
        ///     for InSessionPrintSetting and InSessionViewSheetSet data
        /// </summary>
        public const string InSessionName = "<In-Session>";
    }

    /// <summary>
    ///     Exposes the View/Sheet Set interfaces just like
    ///     the View/Sheet Set Dialog (File->Print...; selected views/sheets->Select...) in UI.
    /// </summary>
    public class ViewSheets : ISettingNameOperation
    {
        private readonly Document m_doc;
        private readonly ViewSheetSetting m_viewSheetSetting;

        public ViewSheets(Document doc)
        {
            m_doc = doc;
            m_viewSheetSetting = doc.PrintManager.ViewSheetSetting;
        }

        public List<string> ViewSheetSetNames
        {
            get
            {
                var names = new List<string>();
                var filteredElementCollector = new FilteredElementCollector(m_doc);
                filteredElementCollector.OfClass(typeof(ViewSheetSet));
                foreach (var element in filteredElementCollector)
                {
                    var viewSheetSet = element as ViewSheetSet;
                    names.Add(viewSheetSet.Name);
                }

                names.Add(ConstData.InSessionName);

                return names;
            }
        }

        public string SettingName
        {
            get
            {
                var theSet = m_viewSheetSetting.CurrentViewSheetSet;
                return theSet is ViewSheetSet ? (theSet as ViewSheetSet).Name : ConstData.InSessionName;
            }
            set
            {
                if (value == ConstData.InSessionName)
                {
                    m_viewSheetSetting.CurrentViewSheetSet = m_viewSheetSetting.InSession;
                    return;
                }

                var filteredElementCollector = new FilteredElementCollector(m_doc);
                filteredElementCollector.OfClass(typeof(ViewSheetSet));
                var viewSheetSets = filteredElementCollector.Cast<ViewSheetSet>()
                    .Where(viewSheetSet => viewSheetSet.Name.Equals(value));
                if (viewSheetSets.Count() > 0) m_viewSheetSetting.CurrentViewSheetSet = viewSheetSets.First();
            }
        }

        public string Prefix => "Set ";

        public int SettingCount =>
            new FilteredElementCollector(m_doc).OfClass(typeof(ViewSheetSet)).ToElementIds().Count;

        public bool SaveAs(string newName)
        {
            try
            {
                return m_viewSheetSetting.SaveAs(newName);
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
                return m_viewSheetSetting.Rename(name);
            }
            catch (Exception ex)
            {
                PrintMgr.MyMessageBox(ex.Message);
                return false;
            }
        }

        public bool Save()
        {
            try
            {
                return m_viewSheetSetting.Save();
            }
            catch (Exception)
            {
                return false;
            }
        }

        public void Revert()
        {
            try
            {
                m_viewSheetSetting.Revert();
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
                return m_viewSheetSetting.Delete();
            }
            catch (Exception ex)
            {
                PrintMgr.MyMessageBox(ex.Message);
                return false;
            }
        }

        public List<View> AvailableViewSheetSet(VisibleType visibleType)
        {
            if (visibleType == VisibleType.VT_None)
                return null;

            var views = new List<View>();
            foreach (View view in m_viewSheetSetting.AvailableViews)
            {
                if (view.ViewType == ViewType.DrawingSheet
                    && visibleType == VisibleType.VT_ViewOnly)
                    continue; // filter out sheets.
                if (view.ViewType != ViewType.DrawingSheet
                    && visibleType == VisibleType.VT_SheetOnly)
                    continue; // filter out views.

                views.Add(view);
            }

            return views;
        }

        public bool IsSelected(string viewName)
        {
            foreach (View view in m_viewSheetSetting.CurrentViewSheetSet.Views)
                if (viewName.Equals(view.ViewType + ": " + view.Name))
                    return true;

            return false;
        }

        public void ChangeCurrentViewSheetSet(List<string> names)
        {
            var selectedViews = new ViewSet();

            if (null != names && 0 < names.Count)
                foreach (View view in m_viewSheetSetting.AvailableViews)
                    if (names.Contains(view.ViewType + ": " + view.Name))
                        selectedViews.Insert(view);

            var viewSheetSet = m_viewSheetSetting.CurrentViewSheetSet;
            viewSheetSet.Views = selectedViews;
            Save();
        }
    }
}

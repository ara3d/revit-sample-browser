// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using Ara3D.RevitSampleBrowser.Common.Infrastructure;
using Ara3D.RevitSampleBrowser.Common.Views;
using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;
using System.Linq;
namespace Ara3D.RevitSampleBrowser.ViewPrinter.CS
{
    public enum VisibleType
    {
        VtViewOnly,
        VtSheetOnly,
        VtBothViewAndSheet,
        VtNone
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
                List<string> names = [];
                FilteredElementCollector filteredElementCollector = new(m_doc);
                filteredElementCollector.OfClass(typeof(ViewSheetSet));
                foreach (var element in filteredElementCollector)
                {
                    var viewSheetSet = element as ViewSheetSet;
                    names.Add(viewSheetSet.Name);
                }

                names.Add(SampleBrowserUtils.InSessionName);

                return names;
            }
        }

        public string SettingName
        {
            get
            {
                var theSet = m_viewSheetSetting.CurrentViewSheetSet;
                return theSet is ViewSheetSet set ? set.Name : SampleBrowserUtils.InSessionName;
            }
            set
            {
                if (value == SampleBrowserUtils.InSessionName)
                {
                    m_viewSheetSetting.CurrentViewSheetSet = m_viewSheetSetting.InSession;
                    return;
                }

                FilteredElementCollector filteredElementCollector = new(m_doc);
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
                PrintHelper.MyMessageBox(ex.Message);
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
                PrintHelper.MyMessageBox(ex.Message);
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
                PrintHelper.MyMessageBox(ex.Message);
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
                PrintHelper.MyMessageBox(ex.Message);
                return false;
            }
        }

        public List<View> AvailableViewSheetSet(VisibleType visibleType)
        {
            if (visibleType == VisibleType.VtNone)
                return null;

            List<View> views = [];
            foreach (View view in m_viewSheetSetting.AvailableViews)
            {
                if (view.ViewType == ViewType.DrawingSheet
                    && visibleType == VisibleType.VtViewOnly)
                    continue; // filter out sheets.
                if (view.ViewType != ViewType.DrawingSheet
                    && visibleType == VisibleType.VtSheetOnly)
                    continue; // filter out views.

                views.Add(view);
            }

            return views;
        }

        public bool IsSelected(string viewName)
        {
            foreach (View view in m_viewSheetSetting.CurrentViewSheetSet.Views)
            {
                if (viewName.Equals($"{view.ViewType}: {view.Name}"))
                    return true;
            }

            return false;
        }

        public void ChangeCurrentViewSheetSet(List<string> names)
        {
            ViewSet selectedViews = new();

            if (null != names && 0 < names.Count)
                foreach (View view in m_viewSheetSetting.AvailableViews)
                {
                    if (names.Contains($"{view.ViewType}: {view.Name}"))
                        selectedViews.Insert(view);
                }

            var viewSheetSet = m_viewSheetSetting.CurrentViewSheetSet;
            viewSheetSet.Views = selectedViews;
            Save();
        }
    }
}

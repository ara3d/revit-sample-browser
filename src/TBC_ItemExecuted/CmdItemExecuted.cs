#region Header

//
// CmdItemExecuted.cs - list all ribbon panel command buttons and subscribe to Autodesk.Windows.ComponentManager.ItemExecuted
//
// Copyright (C) 2015-2021 by Jeremy Tammik,
// Autodesk Inc. All rights reserved.
//
// Keywords: The Building Coder Revit API C# .NET add-in.
//

#endregion // Header

#region Namespaces

using Autodesk.Internal.Windows;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Windows;
using System.Diagnostics;

#endregion // Namespaces

namespace BuildingCoder
{
    [Transaction(TransactionMode.ReadOnly)]
    internal class CmdItemExecuted : IExternalCommand
    {
        private static bool _subscribed;

        public Result Execute(
            ExternalCommandData commandData,
            ref string message,
            ElementSet elements)
        {
            if (_subscribed)
            {
                ComponentManager.ItemExecuted
                    -= OnItemExecuted;

                _subscribed = false;
            }
            else
            {
                var tabs
                    = ComponentManager.Ribbon.Tabs;

                foreach (var tab in tabs)
                {
                    Debug.Print("  {0} {1} '{2}'", tab,
                        tab.GetType().Name, tab.AutomationName);

                    if (tab.KeyTip == null)
                    {
                        // User-defined tabs use Autodesk.Windows.RibbonPanel, not Revit.UI.RibbonPanel.

                        foreach (var panel in tab.Panels)
                        {
                            Debug.Print("    {0} {1}",
                                panel, panel.GetType().Name);

                            foreach (var item in panel.Source.Items)
                            {
                                var ri = item;

                                var automationName = ri.AutomationName;

                                Debug.Print("      {0} {1} '{2}' {3}",
                                    item, item.GetType().Name,
                                    automationName, ri.Cookie);
                            }
                        }
                    }
                }

                ComponentManager.ItemExecuted
                    += OnItemExecuted;

                _subscribed = true;
            }

            return Result.Succeeded;
        }

        private static void OnItemExecuted(
            object sender,
            RibbonItemExecutedEventArgs e)
        {
            var s = null == sender
                ? "<nul>"
                : sender.ToString();

            var parent = e.Parent;

            var p = null == parent
                ? "<nul>"
                : parent.AutomationName;

            Debug.Print(
                "OnItemExecuted: {0} '{1}' in '{2}' cookie {3}",
                s, p, e.Item.AutomationName, e.Item.Cookie);
        }
    }
}
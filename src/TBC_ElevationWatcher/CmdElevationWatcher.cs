#region Header

//
// CmdElevationWatcher.cs - React to elevation view creation
//
// Copyright (C) 2012-2021 by Jeremy Tammik, Autodesk Inc. All rights reserved.
//
// Keywords: The Building Coder Revit API C# .NET add-in.
//

#endregion // Header

#region Namespaces

using System;
using System.Collections.Generic;
using System.Linq;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Events;
using Autodesk.Revit.UI;

#endregion // Namespaces

namespace BuildingCoder
{
    #region ElevationWatcher using DocumentChanged event

    /// <summary>
    ///     React to elevation view creation subscribing to DocumentChanged event
    /// </summary>
    [Transaction(TransactionMode.ReadOnly)]
    internal class CmdElevationWatcher : IExternalCommand
    {
        /// <summary>
        ///     Keep a reference to the handler, so we know
        ///     whether we have already registered and need
        ///     to unregister or vice versa.
        /// </summary>
        private static EventHandler<DocumentChangedEventArgs>
            _handler;

        public Result Execute(
            ExternalCommandData commandData,
            ref string message,
            ElementSet elements)
        {
            var uiapp = commandData.Application;
            var app = uiapp.Application;

            if (null == _handler)
            {
                _handler
                    = OnDocumentChanged;

                // Subscribe to DocumentChanged event

                app.DocumentChanged += _handler;
            }
            else
            {
                app.DocumentChanged -= _handler;
                _handler = null;
            }

            return Result.Succeeded;
        }

        /// <summary>
        ///     DocumentChanged event handler
        /// </summary>
        private static void OnDocumentChanged(
            object sender,
            DocumentChangedEventArgs e)
        {
            var doc = e.GetDocument();

            // To avoid reacting to family import, 
            // ignore family documents:

            if (doc.IsFamilyDocument)
            {
                var view = Util.FindElevationView(
                    doc, e.GetAddedElementIds());

                if (null != view)
                {
                    var msg =
                        $"You just created an elevation view '{view.Name}'. Are you sure you want to do that? (Elevations don't show hidden line detail, which makes them unsuitable for core wall elevations etc.)";

                    TaskDialog.Show("ElevationChecker", msg);

                    // Make sure we see this warning once only
                    // Unsubscribing to the DocumentChanged event
                    // inside the DocumentChanged event handler
                    // causes a Revit message saying "Out of
                    // memory."

                    //doc.Application.DocumentChanged
                    //  -= new EventHandler<DocumentChangedEventArgs>(
                    //    OnDocumentChanged );
                }
            }
        }
    }

    #endregion // ElevationWatcher using DocumentChanged event

    #region ElevationWatcher using DMU updater

    /// <summary>
    ///     React to elevation view creation using DMU updater
    /// </summary>


    #endregion // ElevationWatcher using DMU updater

    #region Simple Updater Sample

    // From https://forums.autodesk.com/t5/revit-api-forum/iupdater-simple-example-needed/m-p/9893248


    #endregion // Simple Updater Sample
}
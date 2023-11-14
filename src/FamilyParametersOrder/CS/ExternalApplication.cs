﻿//
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
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB.Events;
using Autodesk.Revit.UI;

namespace Revit.SDK.Samples.FamilyParametersOrder.CS
{
    /// <summary>
    ///     A class inherits IExternalApplication interface and provide an entry of the sample.
    ///     This class controls other function class and plays the bridge role in this sample.
    ///     It create a custom menu "Track Revit Events" of which the corresponding
    ///     external command is the command in this project.
    /// </summary>
    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    [Journaling(JournalingMode.NoCommandData)]
    public class ExternalApplication : IExternalApplication
    {
        /// <summary>
        ///     Implement this method to implement the external application which should be called when
        ///     Revit starts before a file or default template is actually loaded.
        /// </summary>
        /// <param name="application">
        ///     An object that is passed to the external application
        ///     which contains the controlled application.
        /// </param>
        /// <returns>
        ///     Return the status of the external application.
        ///     A result of Succeeded means that the external application successfully started.
        ///     Cancelled can be used to signify that the user cancelled the external operation at
        ///     some point.
        ///     If false is returned then Revit should inform the user that the external application
        ///     failed to load and the release the internal reference.
        /// </returns>
        public Result OnStartup(UIControlledApplication application)
        {
            try
            {
                application.ControlledApplication.DocumentOpened += SortLoadedFamiliesParams;
            }
            catch (Exception)
            {
                return Result.Failed;
            }

            return Result.Succeeded;
        }

        public Result OnShutdown(UIControlledApplication application)
        {
            return Result.Succeeded;
        }

        /// <summary>
        ///     Generic event handler can be subscribed to any events.
        ///     It will dump events information(sender and EventArgs) to log window and log file
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="args"></param>
        public void SortLoadedFamiliesParams(object obj, DocumentOpenedEventArgs args)
        {
            if (!Command.m_SortDialogIsOpened)
                return;

            using (var sortForm = new SortLoadedFamiliesParamsForm(args.Document))
            {
                sortForm.ShowDialog();
            }
        }
    }
}
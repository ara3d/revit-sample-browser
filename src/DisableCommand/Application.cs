// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

// AUTODESK PROVIDES THIS PROGRAM 'AS IS' AND WITH ALL ITS FAULTS.
// AUTODESK SPECIFICALLY DISCLAIMS ANY IMPLIED WARRANTY OF
// MERCHANTABILITY OR FITNESS FOR A PARTICULAR USE. AUTODESK, INC.
// DOES NOT WARRANT THAT THE OPERATION OF THE PROGRAM WILL BE
// UNINTERRUPTED OR ERROR FREE.
// Use, duplication, or disclosure by the U.S. Government is subject to
// restrictions set forth in FAR 52.227-19 (Commercial Computer
// Software - Restricted Rights) and DFAR 252.227-7013(c)(1)(ii)
// (Rights in Technical Data and Computer Software), as applicable. 

using System;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Events;

using Ara3D.RevitSampleBrowser.Common.Infrastructure;
namespace Ara3D.RevitSampleBrowser.DisableCommand.CS
{
    public class Application : IExternalApplication
    {
        /// <summary>
        ///     The string name of the command to disable.  To lookup a command id string, open a session of Revit,
        ///     invoke the desired command, close Revit, then look to the journal from that session.  The command
        ///     id string will be toward the end of the journal, look for the "Jrn.Command" entry that was recorded
        ///     when it was selected.
        /// </summary>
        private static readonly string SCommandToDisable = "ID_EDIT_DESIGNOPTIONS";

        /// <summary>
        ///     The command id, stored statically to allow for removal of the command binding.
        /// </summary>
        private static RevitCommandId _sCommandId;

        public Result OnStartup(UIControlledApplication application)
        {
            _sCommandId = RevitCommandId.LookupCommandId(SCommandToDisable);

            if (!_sCommandId.CanHaveBinding)
            {
                EventLoggingHelper.ShowDialog("Error",
                    $"The target command {SCommandToDisable} selected for disabling cannot be overridden");
                return Result.Failed;
            }

            // Note that you could also implement .CanExecute to override the accessibiliy of the command.
            // Doing so would allow the command to be grayed out permanently or selectively, however, 
            // no feedback would be available to the user about why the command is grayed out.
            try
            {
                var commandBinding = application.CreateAddInCommandBinding(_sCommandId);
                commandBinding.Executed += DisableEvent;
            }
            // Most likely, this is because someone else has bound this command already.
            catch (Exception)
            {
                EventLoggingHelper.ShowDialog("Error",
                    $"This add-in is unable to disable the target command {SCommandToDisable}; most likely another add-in has overridden this command.");
            }

            return Result.Succeeded;
        }

        public Result OnShutdown(UIControlledApplication application)
        {
            if (_sCommandId.HasBinding)
                application.RemoveAddInCommandBinding(_sCommandId);
            return Result.Succeeded;
        }

        private void DisableEvent(object sender, ExecutedEventArgs args)
        {
            EventLoggingHelper.ShowDialog("Disabled", "Use of this command has been disabled.");
        }
    }
}

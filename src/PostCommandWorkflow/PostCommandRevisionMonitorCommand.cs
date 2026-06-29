// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

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

using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace Ara3D.RevitSampleBrowser.PostCommandWorkflow.CS
{
    /// <summary>
    ///     The external command to set up the revision monitor and execute tasks related to it.
    /// </summary>
    [Transaction(TransactionMode.Manual)]
    public class PostCommandRevisionMonitorCommand : IExternalCommand
    {
        /// <summary>
        ///     The monitor.
        /// </summary>
        private static PostCommandRevisionMonitor _monitor;

        /// <summary>
        ///     The handle to the command's PushButton.
        /// </summary>
        private static PushButton _commandButton;

        /// <summary>
        ///     The external command callback.
        /// </summary>
        /// <param name="commandData"></param>
        /// <param name="message"></param>
        /// <param name="elements"></param>
        /// <returns></returns>
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            var doc = commandData.Application.ActiveUIDocument.Document;
            if (_monitor == null)
            {
                _monitor = new PostCommandRevisionMonitor(doc);
                _monitor.Activate();
                _commandButton.ItemText = "Remove Revision Monitor";
            }
            else
            {
                _monitor.Deactivate();
                _monitor = null;
                _commandButton.ItemText = "Setup Revision Monitor";
            }

            return Result.Succeeded;
        }

        /// <summary>
        ///     Sets the handle to the command's pushbutton.
        /// </summary>
        /// <param name="pushButton"></param>
        public static void SetPushButton(PushButton pushButton)
        {
            _commandButton = pushButton;
        }
    }
}

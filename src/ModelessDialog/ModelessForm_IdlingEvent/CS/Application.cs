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

using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Events;

namespace Ara3D.RevitSampleBrowser.ModelessForm_IdlingEvent.CS
{
    /// <summary>
    ///     Implements the Revit add-in interface IExternalApplication
    /// </summary>
    public class Application : IExternalApplication
    {
        // class instance
        internal static Application ThisApp;

        // ModelessForm instance
        private ModelessForm m_myForm;

        public Result OnShutdown(UIControlledApplication application)
        {
            if (m_myForm != null && !m_myForm.IsDisposed)
            {
                m_myForm.Dispose();
                m_myForm = null;

                // if we've had a dialog, we had subscribed
                application.Idling -= IdlingHandler;
            }

            return Result.Succeeded;
        }

        public Result OnStartup(UIControlledApplication application)
        {
            m_myForm = null; // no dialog needed yet; the command will bring it
            ThisApp = this; // static access to this application instance

            return Result.Succeeded;
        }

        /// <summary>
        ///     This method creates and shows a modeless dialog, unless it already exists.
        /// </summary>
        /// <remarks>
        ///     The external command invokes this on the end-user's request
        /// </remarks>
        public void ShowForm(UIApplication uiapp)
        {
            // If we do not have a dialog yet, create and show it
            if (m_myForm == null || m_myForm.IsDisposed)
            {
                m_myForm = new ModelessForm();
                m_myForm.Show();

                // if we have a dialog, we need Idling too
                uiapp.Idling += IdlingHandler;
            }
        }

        /// <summary>
        ///     A handler for the Idling event.
        /// </summary>
        /// <remarks>
        ///     We keep the handler very simple. First we check
        ///     if we still have the dialog. If not, we unsubscribe from Idling,
        ///     for we no longer need it and it makes Revit speedier.
        ///     If we do have the dialog around, we check if it has a request ready
        ///     and process it accordingly.
        /// </remarks>
        public void IdlingHandler(object sender, IdlingEventArgs args)
        {
            var uiapp = sender as UIApplication;

            if (m_myForm.IsDisposed)
            {
                uiapp.Idling -= IdlingHandler;
                return;
            }
            // dialog still exists
            // fetch the request from the dialog

            var request = m_myForm.Request.Take();

            if (request != RequestId.None)
                try
                {
                    // we take the request, if any was made,
                    // and pass it on to the request executor
                    RequestHandler.Execute(uiapp, request);
                }
                finally
                {
                    // The dialog may be in its waiting state;
                    // make sure we wake it up even if we get an exception.
                    m_myForm.WakeUp();
                }
        }
    }
}

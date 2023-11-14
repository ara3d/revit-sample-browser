// Copyright 2023. See https://github.com/ara3d/revit-samples/LICENSE.txt

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

namespace RevitMultiSample.ModelessForm_ExternalEvent.CS
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
            if (m_myForm != null && m_myForm.Visible) m_myForm.Close();

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
                // A new handler to handle request posting by the dialog
                var handler = new RequestHandler();

                // External Event for the dialog to use (to post requests)
                var exEvent = ExternalEvent.Create(handler);

                // We give the objects to the new dialog;
                // The dialog becomes the owner responsible fore disposing them, eventually.
                m_myForm = new ModelessForm(exEvent, handler);
                m_myForm.Show();
            }
        }

        /// <summary>
        ///     Waking up the dialog from its waiting state.
        /// </summary>
        public void WakeFormUp()
        {
            m_myForm?.WakeUp();
        }
    }
}

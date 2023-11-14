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
using System.Windows.Forms;

using Revit.SDK.Samples.NewRoof.RoofForms.CS;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace Revit.SDK.Samples.NewRoof.CS
{
    /// <summary>
    /// Implements the Revit add-in interface IExternalCommand
    /// </summary>
    [Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]
    [Autodesk.Revit.Attributes.Regeneration(Autodesk.Revit.Attributes.RegenerationOption.Manual)]
    [Autodesk.Revit.Attributes.Journaling(Autodesk.Revit.Attributes.JournalingMode.NoCommandData)]
    public class Command : IExternalCommand
    {
        // internal buffer
        private static Autodesk.Revit.DB.View m_activeView;

        /// <summary>
        /// singleton in the external application
        /// </summary>
        public static Autodesk.Revit.DB.View ActiveView => m_activeView;

        /// <summary>
        /// Implement this method as an external command for Revit.
        /// </summary>
        /// <param name="commandData">An object that is passed to the external application 
        /// which contains data related to the command, 
        /// such as the application object and active view.</param>
        /// <param name="message">A message that can be set by the external application 
        /// which will be displayed if a failure or cancellation is returned by 
        /// the external command.</param>
        /// <param name="elements">A set of elements to which the external application 
        /// can add elements that are to be highlighted in case of failure or cancellation.</param>
        /// <returns>Return the status of the external command. 
        /// A result of Succeeded means that the API external method functioned as expected. 
        /// Cancelled can be used to signify that the user cancelled the external operation 
        /// at some point. Failure should be returned if the application is unable to proceed with 
        /// the operation.</returns>
        public Result Execute(ExternalCommandData commandData,
                                               ref string message,
                                               ElementSet elements)
        {
            try
            {
                m_activeView = commandData.Application.ActiveUIDocument.Document.ActiveView;

                //// Create a new instance of class DataManager
                var roofsManager = new RoofsManager.CS.RoofsManager(commandData);
                LevelConverter.SetStandardValues(roofsManager.Levels);

                // Create a form to create and edit a roof.
                var result = DialogResult.None;
                while (result == DialogResult.None || result == DialogResult.Retry)
                {
                    if (result == DialogResult.Retry)
                    {
                        roofsManager.WindowSelect();
                    }
                    
                    using (var mainForm = new RoofForm(roofsManager))
                    {
                        result = mainForm.ShowDialog();
                    }
                }

                if (result == DialogResult.OK)
                {
                    return Result.Succeeded;
                }
                else
                {
                    return Result.Cancelled;
                }
            }
            catch (Exception ex)
            {
                // If there are something wrong, give error information and return failed
                message = ex.Message;
                return Result.Failed;
            }
        }
    }
}

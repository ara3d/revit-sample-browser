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
using System.Diagnostics;
using Autodesk.Revit;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace Revit.SDK.Samples.WindowWizard.CS
{
    /// <summary>
    /// A class inherits IExternalCommand interface.
    /// this class controls the class which subscribes handle events and the events' information UI.
    /// like a bridge between them.
    /// </summary>
    [Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]
    [Autodesk.Revit.Attributes.Regeneration(Autodesk.Revit.Attributes.RegenerationOption.Manual)]
    [Autodesk.Revit.Attributes.Journaling(Autodesk.Revit.Attributes.JournalingMode.NoCommandData)]
    public class Command : IExternalCommand
    {
        #region Class Interface Implementation
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
        public Autodesk.Revit.UI.Result Execute(ExternalCommandData commandData,
                                             ref string message,
                                             ElementSet elements)
        {
            var doc = commandData.Application.ActiveUIDocument.Document;
            //only a family document  can retrieve family manager
            if (doc.IsFamilyDocument)
            {
                if (null != doc.OwnerFamily && null != doc.OwnerFamily.FamilyCategory 
                    && doc.OwnerFamily.FamilyCategory.Name != doc.Settings.Categories.get_Item(BuiltInCategory.OST_Windows).Name)
                    // FamilyCategory.Name is not "Windows".
                {
                    message = "Please make sure you opened a template of Window.";
                    return Autodesk.Revit.UI.Result.Failed;                   
                }
                var wizard = new WindowWizard(commandData);
                var result = wizard.RunWizard();
                if (1 == result)
                {
                    return Autodesk.Revit.UI.Result.Succeeded;
                }
                else if (0 == result)
                {
                    message = "Window Creation was cancelled.";
                    return Autodesk.Revit.UI.Result.Cancelled;
                }
                else
                {
                    message = "Window Creation failed, please check your template and inputs then try again.";
                    return Autodesk.Revit.UI.Result.Failed;
                }              
            }
            else
            {
                message = "please make sure you have opened a family document!";
                return Autodesk.Revit.UI.Result.Failed;
            }
        }
        #endregion
    }
}


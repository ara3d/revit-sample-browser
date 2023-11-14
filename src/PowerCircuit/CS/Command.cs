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
using Autodesk.Revit.UI;

namespace Revit.SDK.Samples.PowerCircuit.CS
{
    /// <summary>
    /// To add an external command to Autodesk Revit 
    /// the developer should implement an object that 
    /// supports the IExternalCommand interface.
    /// </summary>
    [Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]
    [Autodesk.Revit.Attributes.Regeneration(Autodesk.Revit.Attributes.RegenerationOption.Manual)]
    [Autodesk.Revit.Attributes.Journaling(Autodesk.Revit.Attributes.JournalingMode.NoCommandData)]
    public class Command : IExternalCommand
    {
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
        ref string message, Autodesk.Revit.DB.ElementSet elements)
        {
            try
            {
                // Quit if active document is null
                if (null == commandData.Application.ActiveUIDocument.Document)
                {
                    message = Properties.Resources.ResourceManager.GetString("NullActiveDocument");
                    return Result.Failed;
                }

                // Quit if no elements selected
                if (commandData.Application.ActiveUIDocument.Selection.GetElementIds().Count == 0)
                {
                    message = Properties.Resources.ResourceManager.GetString("SelectPowerElements");
                    return Result.Failed;
                }

                // Collect information from selected elements and show operation dialog
                var optionData = new CircuitOperationData(commandData);
                using (var mainForm = new CircuitOperationForm(optionData))
                {
                    if (mainForm.ShowDialog() == DialogResult.Cancel)
                    {
                        return Result.Cancelled;
                    }
                }

                // Show the dialog for user to select a circuit if more than one circuit available
                if (optionData.Operation != Operation.CreateCircuit && 
                    optionData.ElectricalSystemCount > 1)
                {
                    using (var selectForm = new SelectCircuitForm(optionData))
                    {
                        if (selectForm.ShowDialog() == DialogResult.Cancel)
                        {
                            return Result.Cancelled;
                        }
                    }
                }

                // If user choose to edit circuit, display the circuit editing dialog
                if (optionData.Operation == Operation.EditCircuit)
                {
                    using (var editForm = new EditCircuitForm(optionData))
                    {
                        if (editForm.ShowDialog() == DialogResult.Cancel)
                        {
                            return Result.Cancelled;
                        }
                    }
                }

                // Perform the operation
                optionData.Operate();
            }
            catch (Exception ex)
            {
                message = ex.ToString();
                return Result.Failed;
            }

            return Result.Succeeded;
        }
    }
}

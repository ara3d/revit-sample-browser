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

using System.Collections.Generic;
using System.Windows.Forms;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace Revit.SDK.Samples.ValidateParameters.CS
{  
    /// <summary>
    /// A class inherits IExternalCommand interface.
    /// this class controls the class which subscribes handle events and the events' information UI.
    /// like a bridge between them.
    /// </summary>
    [Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]
    [Autodesk.Revit.Attributes.Regeneration(Autodesk.Revit.Attributes.RegenerationOption.Manual)]
    [Autodesk.Revit.Attributes.Journaling(Autodesk.Revit.Attributes.JournalingMode.NoCommandData)]
    public class Command:IExternalCommand
    {
                /// <summary>
        /// store the family manager
        /// </summary>
        FamilyManager m_familyManager;         
                
                public Result Execute(ExternalCommandData commandData,
                                             ref string message,
                                             ElementSet elements)
        {
            var document = commandData.Application.ActiveUIDocument.Document;
            // only a family document can retrieve family manager
            if (document.IsFamilyDocument)
            {                
                m_familyManager = document.FamilyManager;
                var errorMessages = ValidateParameters(m_familyManager);
                using(var msgForm = new MessageForm(errorMessages.ToArray()))
                {
                    msgForm.StartPosition = FormStartPosition.CenterParent;
                    msgForm.ShowDialog();                    
                    return Result.Succeeded;
                }
            }
            else
            {
                message = "please make sure you have opened a family document!";
                return Result.Failed;
            }
        }
        
                /// <summary>
        /// implementation of validate parameters, get all family types and parameters, 
        /// use the function FamilyType.HasValue() to make sure if the parameter needs to
        /// validate. Then along to the storage type to validate the parameters.
        /// </summary>
        /// <returns>error information list</returns>
        public static List<string> ValidateParameters(FamilyManager familyManager)
        {
            var errorInfo = new List<string>();
            // go though all parameters
            foreach (FamilyType type in familyManager.Types)
            {
                var right = true;
                foreach (FamilyParameter para in familyManager.Parameters)
                {
                    try
                    { 
                        if (type.HasValue(para))
                        {
                            switch (para.StorageType)
                            {
                                case StorageType.Double:
                                    if (!(type.AsDouble(para) is double))
                                        right = false;
                                    break;
                                case StorageType.ElementId:
                                    try
                                    {
                                        type.AsElementId(para);
                                    }
                                    catch
                                    {
                                        right = false;
                                    }                                    
                                    break;
                                case StorageType.Integer:
                                    if (!(type.AsInteger(para) is int))
                                        right = false;
                                    break;
                                case StorageType.String:
                                    if (!(type.AsString(para) is string))
                                        right = false;
                                    break;
                                default:
                                    break;
                            }
                        }
                    } 
                    // output the parameters which failed during validating.
                    catch
                    {
                        errorInfo.Add("Family Type:" + type.Name + "   Family Parameter:" 
                            + para.Definition.Name + "   validating failed!");                       
                    }
                    if (!right)
                    {
                        errorInfo.Add("Family Type:" + type.Name + "   Family Parameter:"
                            + para.Definition.Name + "   validating failed!");                       
                    }                  
                } 
            }           
            return errorInfo;
        }       
            }   
}

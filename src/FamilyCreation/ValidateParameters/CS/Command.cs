// Copyright 2023. See https://github.com/ara3d/revit-samples/LICENSE.txt

using System.Collections.Generic;
using System.Windows.Forms;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace Revit.SDK.Samples.ValidateParameters.CS
{
    /// <summary>
    ///     A class inherits IExternalCommand interface.
    ///     this class controls the class which subscribes handle events and the events' information UI.
    ///     like a bridge between them.
    /// </summary>
    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    [Journaling(JournalingMode.NoCommandData)]
    public class Command : IExternalCommand
    {
        /// <summary>
        ///     store the family manager
        /// </summary>
        private FamilyManager m_familyManager;

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
                using (var msgForm = new MessageForm(errorMessages.ToArray()))
                {
                    msgForm.StartPosition = FormStartPosition.CenterParent;
                    msgForm.ShowDialog();
                    return Result.Succeeded;
                }
            }

            message = "please make sure you have opened a family document!";
            return Result.Failed;
        }

        /// <summary>
        ///     implementation of validate parameters, get all family types and parameters,
        ///     use the function FamilyType.HasValue() to make sure if the parameter needs to
        ///     validate. Then along to the storage type to validate the parameters.
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
                            }
                    }
                    // output the parameters which failed during validating.
                    catch
                    {
                        errorInfo.Add("Family Type:" + type.Name + "   Family Parameter:"
                                      + para.Definition.Name + "   validating failed!");
                    }

                    if (!right)
                        errorInfo.Add("Family Type:" + type.Name + "   Family Parameter:"
                                      + para.Definition.Name + "   validating failed!");
                }
            }

            return errorInfo;
        }
    }
}

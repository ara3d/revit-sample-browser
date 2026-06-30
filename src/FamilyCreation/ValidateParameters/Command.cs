// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System.Collections.Generic;
using System.Windows.Forms;

namespace Ara3D.RevitSampleBrowser.FamilyCreation.ValidateParameters.CS
{
    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    [Journaling(JournalingMode.NoCommandData)]
    public class Command : IExternalCommand
    {
        private FamilyManager m_familyManager;

        public Result Execute(ExternalCommandData commandData,
            ref string message,
            ElementSet elements)
        {
            var document = commandData.Application.ActiveUIDocument.Document;
            // FamilyManager is only available in family documents.
            if (document.IsFamilyDocument)
            {
                m_familyManager = document.FamilyManager;
                var errorMessages = ValidateParameters(m_familyManager);
                using MessageForm msgForm = new(errorMessages.ToArray());
                msgForm.StartPosition = FormStartPosition.CenterParent;
                msgForm.ShowDialog();
                return Result.Succeeded;
            }

            message = "please make sure you have opened a family document!";
            return Result.Failed;
        }

        public static List<string> ValidateParameters(FamilyManager familyManager)
        {
            List<string> errorInfo = new();
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
                                    if (type.AsDouble(para) is not double)
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
                                    if (type.AsInteger(para) is not int)
                                        right = false;
                                    break;
                                case StorageType.String:
                                    if (type.AsString(para) is null)
                                        right = false;
                                    break;
                            }
                    }
                    catch
                    {
                        errorInfo.Add(
                            $"Family Type:{type.Name}   Family Parameter:{para.Definition.Name}   validating failed!");
                    }

                    if (!right)
                        errorInfo.Add(
                            $"Family Type:{type.Name}   Family Parameter:{para.Definition.Name}   validating failed!");
                }
            }

            return errorInfo;
        }
    }
}

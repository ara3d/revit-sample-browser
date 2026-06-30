// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace Ara3D.RevitSampleBrowser.FamilyCreation.WindowWizard.CS
{
    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    [Journaling(JournalingMode.NoCommandData)]
    public class Command : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData,
            ref string message,
            ElementSet elements)
        {
            var doc = commandData.Application.ActiveUIDocument.Document;
            if (doc.IsFamilyDocument)
            {
                if (null != doc.OwnerFamily && null != doc.OwnerFamily.FamilyCategory
                                            && doc.OwnerFamily.FamilyCategory.Name != doc.Settings.Categories
                                                .get_Item(BuiltInCategory.OST_Windows).Name)
                {
                    message = "Please make sure you opened a template of Window.";
                    return Result.Failed;
                }

                WindowWizard wizard = new(commandData);
                var result = wizard.RunWizard();
                switch (result)
                {
                    case 1:
                        return Result.Succeeded;
                    case 0:
                        message = "Window Creation was cancelled.";
                        return Result.Cancelled;
                    default:
                        message = "Window Creation failed, please check your template and inputs then try again.";
                        return Result.Failed;
                }
            }

            message = "please make sure you have opened a family document!";
            return Result.Failed;
        }
    }
}

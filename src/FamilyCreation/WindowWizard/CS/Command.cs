// Copyright 2023. See https://github.com/ara3d/revit-samples/LICENSE.txt

using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace RevitMultiSample.WindowWizard.CS
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
        public Result Execute(ExternalCommandData commandData,
            ref string message,
            ElementSet elements)
        {
            var doc = commandData.Application.ActiveUIDocument.Document;
            //only a family document  can retrieve family manager
            if (doc.IsFamilyDocument)
            {
                if (null != doc.OwnerFamily && null != doc.OwnerFamily.FamilyCategory
                                            && doc.OwnerFamily.FamilyCategory.Name != doc.Settings.Categories
                                                .get_Item(BuiltInCategory.OST_Windows).Name)
                    // FamilyCategory.Name is not "Windows".
                {
                    message = "Please make sure you opened a template of Window.";
                    return Result.Failed;
                }

                var wizard = new WindowWizard(commandData);
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

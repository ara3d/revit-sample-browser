// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System;
using System.Windows.Forms;

namespace Ara3D.RevitSampleBrowser.PlaceFamilyInstanceByFace.CS
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
            if (null == commandData.Application.ActiveUIDocument.Document)
            {
                message = "Active document is null.";
                return Result.Failed;
            }

            try
            {
                FamilyInstanceCreator creator = new(commandData.Application);

                BasedTypeForm baseTypeform = new();
                if (DialogResult.OK == baseTypeform.ShowDialog())
                {
                    PlaceFamilyInstanceForm placeForm = new(creator
                        , baseTypeform.BaseType);
                    placeForm.ShowDialog();
                }

                return Result.Succeeded;
            }
            catch (Exception ex)
            {
                message = ex.Message;
                return Result.Failed;
            }
        }
    }
}

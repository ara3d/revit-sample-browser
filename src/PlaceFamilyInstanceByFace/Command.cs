// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using System;
using System.Windows.Forms;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

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
                var creator = new FamilyInstanceCreator(commandData.Application);

                var baseTypeform = new BasedTypeForm();
                if (DialogResult.OK == baseTypeform.ShowDialog())
                {
                    var placeForm = new PlaceFamilyInstanceForm(creator
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

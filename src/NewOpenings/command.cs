// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using System;
using System.Linq;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace Ara3D.RevitSampleBrowser.NewOpenings.CS
{
    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    [Journaling(JournalingMode.NoCommandData)]
    public class Command : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData,
            ref string message, ElementSet elements)
        {
            var transaction = new Transaction(commandData.Application.ActiveUIDocument.Document, "External Tool");
            try
            {
                transaction.Start();

                var selectedId = commandData.Application.ActiveUIDocument.Selection.GetElementIds().FirstOrDefault();
                if (selectedId == null)
                {
                    message = "please selected one Object (Floor or Wall) to create Opening.";
                    return Result.Cancelled;
                }

                var selectElem = commandData.Application.ActiveUIDocument.Document.GetElement(selectedId);

                if (!(selectElem is Wall) && !(selectElem is Floor))
                {
                    message = "please selected one Object (Floor or Wall) to create Opening.";
                    return Result.Cancelled;
                }

                try
                {
                    switch (selectElem)
                    {
                        case Wall wall:
                        {
                            var profileWall = new ProfileWall(wall, commandData);
                            var newOpeningsForm = new NewOpeningsForm(profileWall);
                            newOpeningsForm.ShowDialog();
                            break;
                        }
                        case Floor floor:
                        {
                            var profileFloor = new ProfileFloor(floor, commandData);
                            var newOpeningsForm = new NewOpeningsForm(profileFloor);
                            newOpeningsForm.ShowDialog();
                            break;
                        }
                    }
                }
                catch (Exception ex)
                {
                    message = ex.Message;
                    return Result.Cancelled;
                }

                return Result.Succeeded;
            }
            catch (Exception e)
            {
                message = e.Message;
                return Result.Failed;
            }
            finally
            {
                transaction.Commit();
            }
        }
    }
}

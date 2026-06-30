// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System;
using System.Linq;

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
            Transaction transaction = new(commandData.Application.ActiveUIDocument.Document, "External Tool");
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

                if (selectElem is not Wall and not Floor)
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
                                ProfileWall profileWall = new(wall, commandData);
                                NewOpeningsForm newOpeningsForm = new(profileWall);
                                newOpeningsForm.ShowDialog();
                                break;
                            }
                        case Floor floor:
                            {
                                ProfileFloor profileFloor = new(floor, commandData);
                                NewOpeningsForm newOpeningsForm = new(profileFloor);
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

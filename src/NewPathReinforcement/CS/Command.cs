// Copyright 2023. See https://github.com/ara3d/revit-samples/LICENSE.txt


using System;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace Revit.SDK.Samples.NewPathReinforcement.CS
{
    /// <summary>
    ///     The entrance of this example, implement the Execute method of IExternalCommand
    /// </summary>
    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    [Journaling(JournalingMode.NoCommandData)]
    public class Command : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData,
            ref string message, ElementSet elements)
        {
            try
            {
                Wall wall = null;
                Floor floor = null;

                var elems = new ElementSet();
                foreach (var elementId in commandData.Application.ActiveUIDocument.Selection.GetElementIds())
                    elems.Insert(commandData.Application.ActiveUIDocument.Document.GetElement(elementId));
                //if user had some wrong selection, give user an Error message
                var errorMessage =
                    "Please select one Slab (or Structure Wall) to create PathReinforcement.";
                if (1 != elems.Size)
                {
                    message = errorMessage;
                    return Result.Cancelled;
                }

                Element selectElem = null;
                var iter = elems.GetEnumerator();
                iter.Reset();
                if (iter.MoveNext()) selectElem = (Element)iter.Current;

                if (selectElem is Wall)
                {
                    wall = selectElem as Wall;
                }
                else if (selectElem is Floor)
                {
                    floor = selectElem as Floor;
                }
                else
                {
                    message = errorMessage;
                    return Result.Cancelled;
                }

                try
                {
                    if (null != wall)
                    {
                        var profileWall = new ProfileWall(wall, commandData);
                        var newPathReinforcementForm =
                            new NewPathReinforcementForm(profileWall);
                        newPathReinforcementForm.ShowDialog();
                    }
                    else if (null != floor)
                    {
                        var profileFloor = new ProfileFloor(floor, commandData);
                        var newPathReinforcementForm =
                            new NewPathReinforcementForm(profileFloor);
                        newPathReinforcementForm.ShowDialog();
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
        }
    }
}

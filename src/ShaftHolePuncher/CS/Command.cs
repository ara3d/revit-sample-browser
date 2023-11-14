// Copyright 2023. See https://github.com/ara3d/revit-samples/LICENSE.txt

using System;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Structure;
using Autodesk.Revit.UI;

namespace Revit.SDK.Samples.ShaftHolePuncher.CS
{
    /// <summary>
    ///     The entrance of this example, implements the Execute method of IExternalCommand
    /// </summary>
    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    [Journaling(JournalingMode.NoCommandData)]
    public class Command : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData,
            ref string message, ElementSet elements)
        {
            var trans = new Transaction(commandData.Application.ActiveUIDocument.Document,
                "Revit.SDK.Samples.ShaftHolePuncher");
            trans.Start();
            try
            {
                Wall wall = null;
                Floor floor = null;
                FamilyInstance familyInstance = null;
                var elems = new ElementSet();
                foreach (var elementId in commandData.Application.ActiveUIDocument.Selection.GetElementIds())
                    elems.Insert(commandData.Application.ActiveUIDocument.Document.GetElement(elementId));

                //if user had some wrong selection, give user an Error message
                var errorMessage =
                    "Please select one Floor (Beam or Wall) to create opening or select nothing to create Shaft Opening";
                if (elems.Size > 1)
                {
                    message = errorMessage;
                    trans.RollBack();
                    return Result.Cancelled;
                }

                Element selectElem = null;
                if (1 == elems.Size)
                {
                    var iter = elems.GetEnumerator();
                    iter.Reset();
                    if (iter.MoveNext()) selectElem = (Element)iter.Current;

                    switch (selectElem)
                    {
                        case Wall elem:
                            wall = elem;
                            break;
                        case Floor elem1:
                            floor = elem1;
                            break;
                        case FamilyInstance instance:
                        {
                            familyInstance = instance;
                            if (familyInstance.StructuralType !=
                                StructuralType.Beam)
                            {
                                message = errorMessage;
                                trans.RollBack();
                                return Result.Cancelled;
                            }

                            break;
                        }
                        default:
                            message = errorMessage;
                            trans.RollBack();
                            return Result.Cancelled;
                    }
                }

                try
                {
                    if (null != wall)
                    {
                        var profileWall = new ProfileWall(wall, commandData);
                        var shaftHolePuncherForm =
                            new ShaftHolePuncherForm(profileWall);
                        shaftHolePuncherForm.ShowDialog();
                    }
                    else if (null != floor)
                    {
                        var profileFloor = new ProfileFloor(floor, commandData);
                        var shaftHolePuncherForm =
                            new ShaftHolePuncherForm(profileFloor);
                        shaftHolePuncherForm.ShowDialog();
                    }
                    else if (null != familyInstance)
                    {
                        var profileBeam = new ProfileBeam(familyInstance, commandData);
                        var shaftHolePuncherForm =
                            new ShaftHolePuncherForm(profileBeam);
                        shaftHolePuncherForm.ShowDialog();
                    }
                    else
                    {
                        var profileNull = new ProfileNull(commandData);
                        var shaftHolePuncherForm =
                            new ShaftHolePuncherForm(profileNull);
                        shaftHolePuncherForm.ShowDialog();
                    }
                }
                catch (Exception ex)
                {
                    message = ex.Message;
                    trans.RollBack();
                    return Result.Cancelled;
                }

                trans.Commit();
                return Result.Succeeded;
            }
            catch (Exception e)
            {
                message = e.Message;
                trans.RollBack();
                return Result.Failed;
            }
        }
    }
}

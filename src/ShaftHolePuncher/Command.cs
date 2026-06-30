// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Structure;
using Autodesk.Revit.UI;
using System;
using System.Collections;

namespace Ara3D.RevitSampleBrowser.ShaftHolePuncher.CS
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
            Transaction trans = new(commandData.Application.ActiveUIDocument.Document,
                "Ara3D.RevitSampleBrowser.ShaftHolePuncher");
            trans.Start();
            try
            {
                Wall wall = null;
                Floor floor = null;
                FamilyInstance familyInstance = null;
                ElementSet elems = new();
                foreach (var elementId in commandData.Application.ActiveUIDocument.Selection.GetElementIds())
                {
                    elems.Insert(commandData.Application.ActiveUIDocument.Document.GetElement(elementId));
                }

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
                        ProfileWall profileWall = new(wall, commandData);
                        ShaftHolePuncherForm shaftHolePuncherForm =
                            new(profileWall);
                        shaftHolePuncherForm.ShowDialog();
                    }
                    else if (null != floor)
                    {
                        ProfileFloor profileFloor = new(floor, commandData);
                        ShaftHolePuncherForm shaftHolePuncherForm =
                            new(profileFloor);
                        shaftHolePuncherForm.ShowDialog();
                    }
                    else if (null != familyInstance)
                    {
                        ProfileBeam profileBeam = new(familyInstance, commandData);
                        ShaftHolePuncherForm shaftHolePuncherForm =
                            new(profileBeam);
                        shaftHolePuncherForm.ShowDialog();
                    }
                    else
                    {
                        ProfileNull profileNull = new(commandData);
                        ShaftHolePuncherForm shaftHolePuncherForm =
                            new(profileNull);
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

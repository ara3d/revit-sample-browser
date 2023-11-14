//
// (C) Copyright 2003-2019 by Autodesk, Inc.
//
// Permission to use, copy, modify, and distribute this software in
// object code form for any purpose and without fee is hereby granted,
// provided that the above copyright notice appears in all copies and
// that both that copyright notice and the limited warranty and
// restricted rights notice below appear in all supporting
// documentation.
//
// AUTODESK PROVIDES THIS PROGRAM "AS IS" AND WITH ALL FAULTS.
// AUTODESK SPECIFICALLY DISCLAIMS ANY IMPLIED WARRANTY OF
// MERCHANTABILITY OR FITNESS FOR A PARTICULAR USE. AUTODESK, INC.
// DOES NOT WARRANT THAT THE OPERATION OF THE PROGRAM WILL BE
// UNINTERRUPTED OR ERROR FREE.
//
// Use, duplication, or disclosure by the U.S. Government is subject to
// restrictions set forth in FAR 52.227-19 (Commercial Computer
// Software - Restricted Rights) and DFAR 252.227-7013(c)(1)(ii)
// (Rights in Technical Data and Computer Software), as applicable.
//

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

                    if (selectElem is Wall)
                    {
                        wall = selectElem as Wall;
                    }
                    else if (selectElem is Floor)
                    {
                        floor = selectElem as Floor;
                    }
                    else if (selectElem is FamilyInstance)
                    {
                        familyInstance = selectElem as FamilyInstance;
                        if (familyInstance.StructuralType !=
                            StructuralType.Beam)
                        {
                            message = errorMessage;
                            trans.RollBack();
                            return Result.Cancelled;
                        }
                    }
                    else
                    {
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
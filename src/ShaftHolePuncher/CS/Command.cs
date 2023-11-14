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
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace Revit.SDK.Samples.ShaftHolePuncher.CS
{
    /// <summary>
    /// The entrance of this example, implements the Execute method of IExternalCommand
    /// </summary>
    [Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]
    [Autodesk.Revit.Attributes.Regeneration(Autodesk.Revit.Attributes.RegenerationOption.Manual)]
    [Autodesk.Revit.Attributes.Journaling(Autodesk.Revit.Attributes.JournalingMode.NoCommandData)]
    public class Command : IExternalCommand
    {
        #region IExternalCommand Members Implementation
        ///<summary>
        /// Implement this method as an external command for Revit.
        /// </summary>
        /// <param name="commandData">An object that is passed to the external application 
        /// which contains data related to the command, 
        /// such as the application object and active view.</param>
        /// <param name="message">A message that can be set by the external application 
        /// which will be displayed if a failure or cancellation is returned by 
        /// the external command.</param>
        /// <param name="elements">A set of elements to which the external application 
        /// can add elements that are to be highlighted in case of failure or cancellation.</param>
        /// <returns>Return the status of the external command. 
        /// A result of Succeeded means that the API external method functioned as expected. 
        /// Cancelled can be used to signify that the user cancelled the external operation 
        /// at some point. Failure should be returned if the application is unable to proceed with 
        /// the operation.</returns>
        public Result Execute(ExternalCommandData commandData,
            ref string message, ElementSet elements)
        {
            var trans = new Transaction(commandData.Application.ActiveUIDocument.Document, "Revit.SDK.Samples.ShaftHolePuncher");
            trans.Start();
            try
            {
                Wall wall = null;
                Floor floor = null;
                FamilyInstance familyInstance = null;
                var elems = new ElementSet();
                foreach (var elementId in commandData.Application.ActiveUIDocument.Selection.GetElementIds())
                {
                   elems.Insert(commandData.Application.ActiveUIDocument.Document.GetElement(elementId));
                }

                #region selection handle -- select one floor, wall, beam or nothing
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
                    if (iter.MoveNext())
                    {
                        selectElem = (Element)iter.Current;
                    }

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
                            Autodesk.Revit.DB.Structure.StructuralType.Beam)
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
                #endregion

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
        #endregion
    }
}

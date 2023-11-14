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

namespace Revit.SDK.Samples.NewPathReinforcement.CS
{
    /// <summary>
    /// The entrance of this example, implement the Execute method of IExternalCommand
    /// </summary>
    [Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]
    [Autodesk.Revit.Attributes.Regeneration(Autodesk.Revit.Attributes.RegenerationOption.Manual)]
    [Autodesk.Revit.Attributes.Journaling(Autodesk.Revit.Attributes.JournalingMode.NoCommandData)]
    public class Command : IExternalCommand
    {
        #region IExternalCommand Members Implementation
        public Result Execute(ExternalCommandData commandData,
            ref string message, ElementSet elements)
      {
         try
         {
            Wall wall = null;
            Floor floor = null;

            var elems = new ElementSet();
            foreach (var elementId in commandData.Application.ActiveUIDocument.Selection.GetElementIds())
            {
               elems.Insert(commandData.Application.ActiveUIDocument.Document.GetElement(elementId));
            }
            #region selection handle -- select one Slab (or Structure Wall)
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
            else
            {
               message = errorMessage;
               return Result.Cancelled;
            }
            #endregion
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
      #endregion
   }
}

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
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;

namespace Revit.SDK.Samples.FabricationPartLayout.CS
{
    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    public class DetachRods : IExternalCommand
    {
        public virtual Result Execute(ExternalCommandData commandData
            , ref string message, ElementSet elements)
        {
            try
            {
                // check user selection
                var uiDoc = commandData.Application.ActiveUIDocument;
                var doc = uiDoc.Document;
                var refObj = uiDoc.Selection.PickObject(ObjectType.Element, "Pick a fabrication part hanger to start.");
                var part = doc.GetElement(refObj) as FabricationPart;

                if (part == null || part.IsAHanger() == false)
                {
                    message = "The selected element is not a fabrication part hanger.";
                    return Result.Failed;
                }

                var rodInfo = part.GetRodInfo();
                using (var trans = new Transaction(doc, "Detach Rods"))
                {
                    trans.Start();

                    rodInfo.CanRodsBeHosted = false;

                    trans.Commit();
                }

                message = "Detach successful";
                return Result.Succeeded;
            }
            catch (Exception ex)
            {
                message = ex.Message;
                return Result.Failed;
            }
        }
    }

    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    public class DoubleRodLength : IExternalCommand
    {
        public virtual Result Execute(ExternalCommandData commandData
            , ref string message, ElementSet elements)
        {
            try
            {
                // check user selection
                var uiDoc = commandData.Application.ActiveUIDocument;
                var doc = uiDoc.Document;
                var refObj = uiDoc.Selection.PickObject(ObjectType.Element, "Pick a fabrication part hanger to start.");
                var part = doc.GetElement(refObj) as FabricationPart;

                if (part == null || part.IsAHanger() == false)
                {
                    message = "The selected element is not a fabrication part hanger.";
                    return Result.Failed;
                }

                var rodInfo = part.GetRodInfo();
                if (rodInfo.IsAttachedToStructure)
                {
                    message = "The hanger rods must be detached from their host first";
                    return Result.Failed;
                }

                using (var trans = new Transaction(doc, "Double Rod Length"))
                {
                    trans.Start();

                    for (var i = 0; i < rodInfo.RodCount; i++)
                    {
                        var originalLength = rodInfo.GetRodLength(i);
                        rodInfo.SetRodLength(i, originalLength * 2.0);
                    }

                    trans.Commit();
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

    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    public class HalveRodLength : IExternalCommand
    {
        public virtual Result Execute(ExternalCommandData commandData
            , ref string message, ElementSet elements)
        {
            try
            {
                // check user selection
                var uiDoc = commandData.Application.ActiveUIDocument;
                var doc = uiDoc.Document;
                var refObj = uiDoc.Selection.PickObject(ObjectType.Element, "Pick a fabrication part hanger to start.");
                var part = doc.GetElement(refObj) as FabricationPart;

                if (part == null || part.IsAHanger() == false)
                {
                    message = "The selected element is not a fabrication part hanger.";
                    return Result.Failed;
                }

                var rodInfo = part.GetRodInfo();
                if (rodInfo.IsAttachedToStructure)
                {
                    message = "The hanger rods must be detached from their host first";
                    return Result.Failed;
                }

                using (var trans = new Transaction(doc, "Halve Rod Length"))
                {
                    trans.Start();

                    for (var i = 0; i < rodInfo.RodCount; i++)
                    {
                        var originalLength = rodInfo.GetRodLength(i);
                        rodInfo.SetRodLength(i, originalLength / 2.0);
                    }

                    trans.Commit();
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

    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    public class IncreaseRodStructureExtension : IExternalCommand
    {
        public virtual Result Execute(ExternalCommandData commandData
            , ref string message, ElementSet elements)
        {
            try
            {
                // check user selection
                var uiDoc = commandData.Application.ActiveUIDocument;
                var doc = uiDoc.Document;
                var refObj = uiDoc.Selection.PickObject(ObjectType.Element, "Pick a fabrication part hanger to start.");
                var part = doc.GetElement(refObj) as FabricationPart;

                if (part == null || part.IsAHanger() == false)
                {
                    message = "The selected element is not a fabrication part hanger.";
                    return Result.Failed;
                }

                var rodInfo = part.GetRodInfo();
                if (rodInfo.IsAttachedToStructure == false)
                {
                    message = "The hanger rods must be attached to structure.";
                    return Result.Failed;
                }

                using (var trans = new Transaction(doc, "Increase Rod Structure Extension"))
                {
                    trans.Start();

                    for (var i = 0; i < rodInfo.RodCount; i++)
                    {
                        var originalExtension = rodInfo.GetRodStructureExtension(i);
                        rodInfo.SetRodStructureExtension(i, originalExtension + 1.0);
                    }

                    trans.Commit();
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

    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    public class DecreaseRodStructureExtension : IExternalCommand
    {
        public virtual Result Execute(ExternalCommandData commandData
            , ref string message, ElementSet elements)
        {
            try
            {
                // check user selection
                var uiDoc = commandData.Application.ActiveUIDocument;
                var doc = uiDoc.Document;
                var refObj = uiDoc.Selection.PickObject(ObjectType.Element, "Pick a fabrication part hanger to start.");
                var part = doc.GetElement(refObj) as FabricationPart;

                if (part == null || part.IsAHanger() == false)
                {
                    message = "The selected element is not a fabrication part hanger.";
                    return Result.Failed;
                }

                var rodInfo = part.GetRodInfo();
                if (rodInfo.IsAttachedToStructure == false)
                {
                    message = "The hanger rods must be attached to structure.";
                    return Result.Failed;
                }

                using (var trans = new Transaction(doc, "Increase Rod Structure Extension"))
                {
                    trans.Start();

                    for (var i = 0; i < rodInfo.RodCount; i++)
                    {
                        var originalExtension = rodInfo.GetRodStructureExtension(i);
                        rodInfo.SetRodStructureExtension(i, originalExtension - 1.0);
                    }

                    trans.Commit();
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
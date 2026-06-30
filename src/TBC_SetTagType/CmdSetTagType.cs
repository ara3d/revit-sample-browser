#region Header

//
// CmdSetTagType.cs - create a wall, door, door tag, then create and set a new door tag type
//
// Copyright (C) 2010-2021 by Jeremy Tammik,
// Autodesk Inc. All rights reserved.
//
// Keywords: The Building Coder Revit API C# .NET add-in.
//

#endregion // Header

#region Namespaces

using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Structure;
using Autodesk.Revit.UI;

#endregion // Namespaces

namespace BuildingCoder
{
    [Transaction(TransactionMode.Manual)]
    class CmdSetTagType : IExternalCommand
    {
        const double MeterToFeet = 3.2808399;

        public Result Execute(
            ExternalCommandData commandData,
            ref string message,
            ElementSet elements)
        {
            var app = commandData.Application;
            var doc = app.ActiveUIDocument.Document;

            var createApp
                = app.Application.Create;

            var createDoc
                = doc.Create;

            var length = 5 * MeterToFeet;

            var pts = new XYZ[2];

            pts[0] = XYZ.Zero;
            pts[1] = new XYZ(length, 0, 0);

            Level levelBottom = null;
            Level levelTop = null;

            if (!Util.GetBottomAndTopLevels(doc,
                ref levelBottom, ref levelTop))
            {
                message = "Unable to determine "
                          + "wall bottom and top levels";

                return Result.Failed;
            }

            using var t = new Transaction(doc);
            t.Start("Create Wall, Door and Tag");

            var topLevelParam
                = BuiltInParameter.WALL_HEIGHT_TYPE;

            var topLevelId = levelTop.Id;

            var line = Line.CreateBound(pts[0], pts[1]);

            var wall = Wall.Create(
                doc, line, levelBottom.Id, false);

            var param = wall.get_Parameter(
                topLevelParam);

            param.Set(topLevelId);

            var wallThickness = wall.WallType.GetCompoundStructure().GetLayers()[0].Width;

            var doorSymbol = Util.GetFirstFamilySymbol(
                doc, BuiltInCategory.OST_Doors);

            if (null == doorSymbol)
            {
                message = "No door symbol found.";
                return Result.Failed;
            }

            var midpoint = Util.Midpoint(pts[0], pts[1]);

            var door = createDoc
                .NewFamilyInstance(
                    midpoint, doorSymbol, wall, levelBottom,
                    StructuralType.NonStructural);

            var view = doc.ActiveView;

            var tagOffset = 3 * wallThickness;

            midpoint += tagOffset * XYZ.BasisY;

            var tag = IndependentTag.Create(
                doc, view.Id, new Reference(door),
                false, TagMode.TM_ADDBY_CATEGORY,
                TagOrientation.Horizontal, midpoint);

            var doorTagType
                = Util.GetFirstFamilySymbol(
                    doc, BuiltInCategory.OST_DoorTags);

            doorTagType = doorTagType.Duplicate(
                "New door tag type") as FamilySymbol;

            tag.ChangeTypeId(doorTagType.Id);

            door.Name = $"{door.Name} modified";
            door.Symbol.Name = $"{door.Symbol.Name} modified";

            t.Commit();

            return Result.Succeeded;
        }
    }
}

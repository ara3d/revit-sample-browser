// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Structure;
using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace Ara3D.RevitSampleBrowser.CreateSimpleAreaRein.CS
{
    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    [Journaling(JournalingMode.NoCommandData)]
    public class Command : IExternalCommand
    {
        private UIDocument m_currentDoc;

        public static ExternalCommandData CommandData { get; private set; }

        public Result Execute(ExternalCommandData revit,
            ref string message, ElementSet elements)
        {
            Transaction trans = new(revit.Application.ActiveUIDocument.Document,
                "Ara3D.RevitSampleBrowser.CreateSimpleAreaRein");
            trans.Start();
            //initialize necessary data
            CommandData = revit;
            m_currentDoc = revit.Application.ActiveUIDocument;

            //create AreaReinforcement
            try
            {
                if (Create())
                {
                    trans.Start();
                    return Result.Succeeded;
                }
            }
            catch (ApplicationException appEx)
            {
                TaskDialog.Show("Revit", appEx.Message);
                trans.RollBack();
                return Result.Failed;
            }
            catch
            {
                TaskDialog.Show("Revit", "Unknow Errors.");
                trans.RollBack();
                return Result.Failed;
            }

            trans.RollBack();
            return Result.Cancelled;
        }

        private bool Create()
        {
            ElementSet elems = new();
            foreach (var elementId in m_currentDoc.Selection.GetElementIds())
            {
                elems.Insert(m_currentDoc.Document.GetElement(elementId));
            }

            //selected 0 or more than 1 element
            if (elems.Size != 1)
            {
                TaskDialog.Show("Error", "Please select exactly one wall or floor.");
                return false;
            }

            foreach (var o in elems)
            {
                switch (o)
                {
                    //create on floor
                    case Floor floor:
                        {
                            var flag = CreateAreaReinOnFloor(floor);
                            return flag;
                        }
                    //create on wall
                    case Wall wall:
                        {
                            var flag = CreateAreaReinOnWall(wall);
                            return flag;
                        }
                    default:
                        //selected element is neither wall nor floor
                        TaskDialog.Show("Error", "Please select exactly one wall or floor.");
                        break;
                }
            }

            return false;
        }

        private bool CreateAreaReinOnFloor(Floor floor)
        {
            GeomHelper helper = new();
            Reference refer = null;
            IList<Curve> curves = [];

            //and prepare necessary to create AreaReinforcement
            if (!helper.GetFloorGeom(floor, ref refer, ref curves))
            {
                ApplicationException appEx = new(
                    "Your selection is not a horizontal rectangular slab.");
                throw appEx;
            }

            AreaReinDataOnFloor dataOnFloor = new();
            CreateSimpleAreaReinForm createForm =
                new(dataOnFloor);

            //allow use select parameters to create
            if (createForm.ShowDialog() == DialogResult.OK)
            {
                //we get direction of first Line on the Floor as the Major Direction
                var firstLine = (Line)curves[0];
                XYZ majorDirection = new(
                    firstLine.GetEndPoint(1).X - firstLine.GetEndPoint(0).X,
                    firstLine.GetEndPoint(1).Y - firstLine.GetEndPoint(0).Y,
                    firstLine.GetEndPoint(1).Z - firstLine.GetEndPoint(0).Z);

                //Create AreaReinforcement
                var areaReinforcementTypeId =
                    AreaReinforcementType.CreateDefaultAreaReinforcementType(CommandData.Application.ActiveUIDocument
                        .Document);
                var rebarBarTypeId =
                    RebarBarType.CreateDefaultRebarBarType(CommandData.Application.ActiveUIDocument.Document);
                var rebarHookTypeId =
                    RebarHookType.CreateDefaultRebarHookType(CommandData.Application.ActiveUIDocument.Document);
                var areaRein = AreaReinforcement.Create(CommandData.Application.ActiveUIDocument.Document, floor,
                    curves, majorDirection, areaReinforcementTypeId, rebarBarTypeId, rebarHookTypeId);

                //set AreaReinforcement and it's AreaReinforcementCurves parameters
                dataOnFloor.FillIn(areaRein);
                return true;
            }

            return false;
        }

        private bool CreateAreaReinOnWall(Wall wall)
        {
            //make sure selected is basic wall
            if (wall.WallType.Kind != WallKind.Basic)
            {
                TaskDialog.Show("Revit", "Selected wall is not a basic wall.");
                return false;
            }

            GeomHelper helper = new();
            Reference refer = null;
            IList<Curve> curves = [];
            if (!helper.GetWallGeom(wall, ref refer, ref curves))
            {
                ApplicationException appEx = new(
                    "Your selection is not a structural straight rectangular wall.");
                throw appEx;
            }

            AreaReinDataOnWall dataOnWall = new();
            CreateSimpleAreaReinForm createForm = new
(dataOnWall);

            //allow use select parameters to create
            if (createForm.ShowDialog() == DialogResult.OK)
            {
                //we get direction of first Line on the Floor as the Major Direction
                var firstLine = (Line)curves[0];
                XYZ majorDirection = new(
                    firstLine.GetEndPoint(1).X - firstLine.GetEndPoint(0).X,
                    firstLine.GetEndPoint(1).Y - firstLine.GetEndPoint(0).Y,
                    firstLine.GetEndPoint(1).Z - firstLine.GetEndPoint(0).Z);

                //create AreaReinforcement
                IList<Curve> curveList = [.. curves];

                var areaReinforcementTypeId =
                    AreaReinforcementType.CreateDefaultAreaReinforcementType(CommandData.Application.ActiveUIDocument
                        .Document);
                var rebarBarTypeId =
                    RebarBarType.CreateDefaultRebarBarType(CommandData.Application.ActiveUIDocument.Document);
                var rebarHookTypeId =
                    RebarHookType.CreateDefaultRebarHookType(CommandData.Application.ActiveUIDocument.Document);
                var areaRein = AreaReinforcement.Create(CommandData.Application.ActiveUIDocument.Document, wall,
                    curveList, majorDirection, areaReinforcementTypeId, rebarBarTypeId, rebarHookTypeId);
                dataOnWall.FillIn(areaRein);
                return true;
            }

            return false;
        }
    }
}

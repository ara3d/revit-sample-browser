// Copyright 2023. See https://github.com/ara3d/revit-samples/LICENSE.txt

using System;
using System.Collections.Generic;
using System.Windows.Forms;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Structure;
using Autodesk.Revit.UI;
using Document = Autodesk.Revit.Creation.Document;

namespace Revit.SDK.Samples.CreateSimpleAreaRein.CS
{
    using DocCreator = Document;


    /// <summary>
    ///     main class to create simple AreaReinforcement on selected wall or floor
    /// </summary>
    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    [Journaling(JournalingMode.NoCommandData)]
    public class Command : IExternalCommand
    {
        private UIDocument m_currentDoc;

        /// <summary>
        ///     ExternalCommandData
        /// </summary>
        public static ExternalCommandData CommandData { get; private set; }

        public Result Execute(ExternalCommandData revit,
            ref string message, ElementSet elements)
        {
            var trans = new Transaction(revit.Application.ActiveUIDocument.Document,
                "Revit.SDK.Samples.CreateSimpleAreaRein");
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

        /// <summary>
        ///     create simple AreaReinforcement on selected wall or floor
        /// </summary>
        /// <returns></returns>
        private bool Create()
        {
            var elems = new ElementSet();
            foreach (var elementId in m_currentDoc.Selection.GetElementIds())
                elems.Insert(m_currentDoc.Document.GetElement(elementId));

            //selected 0 or more than 1 element
            if (elems.Size != 1)
            {
                TaskDialog.Show("Error", "Please select exactly one wall or floor.");
                return false;
            }

            foreach (var o in elems)
            {
                //create on floor
                var floor = o as Floor;
                if (null != floor)
                {
                    var flag = CreateAreaReinOnFloor(floor);
                    return flag;
                }

                //create on wall
                var wall = o as Wall;
                if (null != wall)
                {
                    var flag = CreateAreaReinOnWall(wall);
                    return flag;
                }

                //selected element is neither wall nor floor
                TaskDialog.Show("Error", "Please select exactly one wall or floor.");
            }

            return false;
        }

        /// <summary>
        ///     create simple AreaReinforcement on horizontal floor
        /// </summary>
        /// <param name="floor"></param>
        /// <returns>is successful</returns>
        private bool CreateAreaReinOnFloor(Floor floor)
        {
            var helper = new GeomHelper();
            Reference refer = null;
            IList<Curve> curves = new List<Curve>();

            //check whether floor is horizontal rectangular 
            //and prepare necessary to create AreaReinforcement
            if (!helper.GetFloorGeom(floor, ref refer, ref curves))
            {
                var appEx = new ApplicationException(
                    "Your selection is not a horizontal rectangular slab.");
                throw appEx;
            }

            var dataOnFloor = new AreaReinDataOnFloor();
            var createForm =
                new CreateSimpleAreaReinForm(dataOnFloor);

            //allow use select parameters to create
            if (createForm.ShowDialog() == DialogResult.OK)
            {
                //define the Major Direction of AreaReinforcement,
                //we get direction of first Line on the Floor as the Major Direction
                var firstLine = (Line)curves[0];
                var majorDirection = new XYZ(
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

        /// <summary>
        ///     create simple AreaReinforcement on vertical straight rectangular wall
        /// </summary>
        /// <param name="wall"></param>
        /// <returns>is successful</returns>
        private bool CreateAreaReinOnWall(Wall wall)
        {
            //make sure selected is basic wall
            if (wall.WallType.Kind != WallKind.Basic)
            {
                TaskDialog.Show("Revit", "Selected wall is not a basic wall.");
                return false;
            }

            var helper = new GeomHelper();
            Reference refer = null;
            IList<Curve> curves = new List<Curve>();
            //check whether wall is vertical rectangular and analytical model shape is line
            if (!helper.GetWallGeom(wall, ref refer, ref curves))
            {
                var appEx = new ApplicationException(
                    "Your selection is not a structural straight rectangular wall.");
                throw appEx;
            }

            var dataOnWall = new AreaReinDataOnWall();
            var createForm = new
                CreateSimpleAreaReinForm(dataOnWall);

            //allow use select parameters to create
            if (createForm.ShowDialog() == DialogResult.OK)
            {
                //define the Major Direction of AreaReinforcement,
                //we get direction of first Line on the Floor as the Major Direction
                var firstLine = (Line)curves[0];
                var majorDirection = new XYZ(
                    firstLine.GetEndPoint(1).X - firstLine.GetEndPoint(0).X,
                    firstLine.GetEndPoint(1).Y - firstLine.GetEndPoint(0).Y,
                    firstLine.GetEndPoint(1).Z - firstLine.GetEndPoint(0).Z);

                //create AreaReinforcement
                IList<Curve> curveList = new List<Curve>();
                foreach (var curve in curves) curveList.Add(curve);
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

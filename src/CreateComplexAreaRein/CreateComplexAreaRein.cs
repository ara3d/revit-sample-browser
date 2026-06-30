// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Structure;
using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace Ara3D.RevitSampleBrowser.CreateComplexAreaRein.CS
{
    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    [Journaling(JournalingMode.NoCommandData)]
    public class Command : IExternalCommand
    {
        private UIDocument m_currentDoc;
        private AreaReinData m_data;

        public static ExternalCommandData CommandData { get; private set; }

        public Result Execute(ExternalCommandData revit,
            ref string message, ElementSet elements)
        {
            Transaction trans = new(revit.Application.ActiveUIDocument.Document,
                "Ara3D.RevitSampleBrowser.CreateComplexAreaRein");
            trans.Start();
            //initialize members
            CommandData = revit;
            m_currentDoc = revit.Application.ActiveUIDocument;
            m_data = new AreaReinData(revit.Application.ActiveUIDocument.Document);

            try
            {
                //check precondition and prepare necessary data to create AreaReinforcement.
                Reference refer = null;
                IList<Curve> curves = [];
                var floor = InitFloor(ref refer, ref curves);

                //ask for user's input
                AreaReinData dataOnFloor = new(revit.Application.ActiveUIDocument.Document);
                CreateComplexAreaReinForm createForm = new
(dataOnFloor);
                if (createForm.ShowDialog() == DialogResult.OK)
                {
                    //we get direction of first Line on the Floor as the Major Direction
                    var firstLine = (Line)curves[0];
                    XYZ majorDirection = new(
                        firstLine.GetEndPoint(1).X - firstLine.GetEndPoint(0).X,
                        firstLine.GetEndPoint(1).Y - firstLine.GetEndPoint(0).Y,
                        firstLine.GetEndPoint(1).Z - firstLine.GetEndPoint(0).Z);

                    //create AreaReinforcement by AreaReinforcement.Create() function
                    var areaReinforcementTypeId =
                        AreaReinforcementType.CreateDefaultAreaReinforcementType(revit.Application.ActiveUIDocument
                            .Document);
                    var rebarBarTypeId =
                        RebarBarType.CreateDefaultRebarBarType(revit.Application.ActiveUIDocument.Document);
                    var rebarHookTypeId =
                        RebarHookType.CreateDefaultRebarHookType(revit.Application.ActiveUIDocument.Document);
                    var areaRein = AreaReinforcement.Create(revit.Application.ActiveUIDocument.Document, floor, curves,
                        majorDirection, areaReinforcementTypeId, rebarBarTypeId, rebarHookTypeId);

                    //set AreaReinforcement and it's AreaReinforcementCurves parameters
                    dataOnFloor.FillIn(areaRein);
                    trans.Commit();
                    return Result.Succeeded;
                }
            }
            catch (ApplicationException appEx)
            {
                message = appEx.Message;
                trans.RollBack();
                return Result.Failed;
            }
            catch
            {
                message = "Unknow Errors.";
                trans.RollBack();
                return Result.Failed;
            }

            trans.RollBack();
            return Result.Cancelled;
        }

        private Floor InitFloor(ref Reference refer, ref IList<Curve> curves)
        {
            ElementSet elems = new();
            foreach (var elementId in m_currentDoc.Selection.GetElementIds())
            {
                elems.Insert(m_currentDoc.Document.GetElement(elementId));
            }

            //selected 0 or more than 1 element
            if (elems.Size != 1)
            {
                var msg = "Please select exactly one slab.";
                ApplicationException appEx = new(msg);
                throw appEx;
            }

            Floor floor = null;
            foreach (var o in elems)
            {
                //selected one floor
                floor = o as Floor;
                if (null == floor)
                {
                    var msg = "Please select exactly one slab.";
                    ApplicationException appEx = new(msg);
                    throw appEx;
                }
            }

            //check the shape is rectangular and get its edges
            GeomHelper helper = new();
            if (!helper.GetFloorGeom(floor, ref refer, ref curves))
            {
                ApplicationException appEx = new
(
                        "Your selection is not a structural rectangular horizontal slab.");
                throw appEx;
            }

            return floor;
        }
    }
}

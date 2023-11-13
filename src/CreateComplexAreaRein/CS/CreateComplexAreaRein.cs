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

using Autodesk.Revit.UI;

namespace Revit.SDK.Samples.CreateComplexAreaRein.CS
{
    using System;
    using System.Collections.Generic;
    using System.Collections;
    using System.Text;
    using System.Windows.Forms;

    using Autodesk.Revit;
    using Autodesk.Revit.DB;
    using Autodesk.Revit.DB.Structure;

    using DocCreator = Autodesk.Revit.Creation.Document;


    [Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]
    [Autodesk.Revit.Attributes.Regeneration(Autodesk.Revit.Attributes.RegenerationOption.Manual)]
    [Autodesk.Revit.Attributes.Journaling(Autodesk.Revit.Attributes.JournalingMode.NoCommandData)]
    public class Command : IExternalCommand
    {
        private UIDocument m_currentDoc;
        private static ExternalCommandData m_revit;
        private AreaReinData m_data;

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
        public Autodesk.Revit.UI.Result Execute(ExternalCommandData revit,
            ref string message, Autodesk.Revit.DB.ElementSet elements)
        {
            var trans = new Transaction(revit.Application.ActiveUIDocument.Document, "Revit.SDK.Samples.CreateComplexAreaRein");
            trans.Start();
            //initialize members
            m_revit = revit;
            m_currentDoc = revit.Application.ActiveUIDocument;
            m_data = new AreaReinData(revit.Application.ActiveUIDocument.Document);

            try
            {
                //check precondition and prepare necessary data to create AreaReinforcement.
                Reference refer = null;
                IList<Curve> curves = new List<Curve>();
                var floor = InitFloor(ref refer, ref curves);

                //ask for user's input
                var dataOnFloor = new AreaReinData(revit.Application.ActiveUIDocument.Document);
                var createForm = new
                    CreateComplexAreaReinForm(dataOnFloor);
                if (createForm.ShowDialog() == DialogResult.OK)
                {
                    //define the Major Direction of AreaReinforcement,
                    //we get direction of first Line on the Floor as the Major Direction
                    var firstLine = (Line)(curves[0]);
                    var majorDirection = new Autodesk.Revit.DB.XYZ(
                        firstLine.GetEndPoint(1).X - firstLine.GetEndPoint(0).X,
                        firstLine.GetEndPoint(1).Y - firstLine.GetEndPoint(0).Y,
                        firstLine.GetEndPoint(1).Z - firstLine.GetEndPoint(0).Z);

                    //create AreaReinforcement by AreaReinforcement.Create() function
                    var creator = m_revit.Application.ActiveUIDocument.Document.Create;
                    var areaReinforcementTypeId = AreaReinforcementType.CreateDefaultAreaReinforcementType(revit.Application.ActiveUIDocument.Document);
                    var rebarBarTypeId = RebarBarType.CreateDefaultRebarBarType(revit.Application.ActiveUIDocument.Document);
                    var rebarHookTypeId = RebarHookType.CreateDefaultRebarHookType(revit.Application.ActiveUIDocument.Document);
                    var areaRein = AreaReinforcement.Create(revit.Application.ActiveUIDocument.Document, floor, curves, majorDirection, areaReinforcementTypeId, rebarBarTypeId, rebarHookTypeId);

                    //set AreaReinforcement and it's AreaReinforcementCurves parameters
                    dataOnFloor.FillIn(areaRein);
                    trans.Commit();
                    return Autodesk.Revit.UI.Result.Succeeded;
                }
            }
            catch (ApplicationException appEx)
            {
                message = appEx.Message;
                trans.RollBack();
                return Autodesk.Revit.UI.Result.Failed;
            }
            catch
            {
                message = "Unknow Errors.";
                trans.RollBack();
                return Autodesk.Revit.UI.Result.Failed;
            }
            trans.RollBack();
            return Autodesk.Revit.UI.Result.Cancelled;
        }

        /// <summary>
        /// ExternalCommandData
        /// </summary>
        public static ExternalCommandData CommandData
        {
            get
            {
                return m_revit;
            }
        }

        /// <summary>
        /// initialize member data, judge simple precondition
        /// </summary>
        private Floor InitFloor(ref Reference refer, ref IList<Curve> curves)
        {
           var elems = new ElementSet();
            foreach (var elementId in m_currentDoc.Selection.GetElementIds())
            {
               elems.Insert(m_currentDoc.Document.GetElement(elementId));
            }
            //selected 0 or more than 1 element
            if (elems.Size != 1)
            {
                var msg = "Please select exactly one slab.";
                var appEx = new ApplicationException(msg);
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
                    var appEx = new ApplicationException(msg);
                    throw appEx;
                }
            }
            //check the shape is rectangular and get its edges
            var helper = new GeomHelper();
            if (!helper.GetFloorGeom(floor, ref refer, ref curves))
            {
                var appEx = new
                    ApplicationException(
                    "Your selection is not a structural rectangular horizontal slab.");
                throw appEx;
            }

            return floor;
        }
    }
}

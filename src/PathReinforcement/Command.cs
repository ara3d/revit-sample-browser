// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Structure;
using Autodesk.Revit.UI;
using System;
using System.Collections;

namespace Ara3D.RevitSampleBrowser.PathReinforcement.CS
{
    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    [Journaling(JournalingMode.NoCommandData)]
    public class Command : IExternalCommand
    {
        /// <summary>
        ///     static property corresponding to s_rebarBarTypes field.
        /// </summary>
        public static Hashtable BarTypes { get; } = [];

        public Result Execute(ExternalCommandData commandData,
            ref string message, ElementSet elements)
        {
            Transaction transaction = new(commandData.Application.ActiveUIDocument.Document, "External Tool");
            try
            {
                transaction.Start();
                ElementSet elems = new();
                foreach (var elementId in commandData.Application.ActiveUIDocument.Selection.GetElementIds())
                {
                    elems.Insert(commandData.Application.ActiveUIDocument.Document.GetElement(elementId));
                }

                //if user have some wrong selection, give user an Error message
                if (1 != elems.Size)
                {
                    message = "please select one PathReinforcement.";
                    return Result.Cancelled;
                }

                Element selectElem = null;
                foreach (Element e in elems)
                {
                    selectElem = e;
                }

                if (selectElem is not Autodesk.Revit.DB.Structure.PathReinforcement pathRein)
                {
                    message = "please select one PathReinforcement.";
                    return Result.Cancelled;
                }

                //clear all rebar bar type.
                if (BarTypes.Count > 0) BarTypes.Clear();

                //get all bar type.
                FilteredElementCollector collector = new(commandData.Application.ActiveUIDocument.Document);
                var itor = collector.OfClass(typeof(RebarBarType)).GetElementIterator();
                itor.Reset();
                while (itor.MoveNext())
                {
                    if (itor.Current is RebarBarType bartype)
                    {
                        var id = bartype.Id;
                        var name = bartype.Name;
                        BarTypes.Add(name, id);
                    }
                }

                //Create a form to view the path reinforcement.
                using PathReinforcementForm form = new(pathRein, commandData);
                form.ShowDialog();
            }
            catch (Exception e)
            {
                transaction.RollBack();
                message = e.Message;
                return Result.Failed;
            }
            finally
            {
                transaction.Commit();
            }

            return Result.Succeeded;
        }
    }
}

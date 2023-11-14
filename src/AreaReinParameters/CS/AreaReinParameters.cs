// Copyright 2023. See https://github.com/ara3d/revit-samples/LICENSE.txt

using System;
using System.Collections;
using System.Windows.Forms;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Structure;
using Autodesk.Revit.UI;

namespace Revit.SDK.Samples.AreaReinParameters.CS
{
    /// <summary>
    ///     Entry point and main command class
    /// </summary>
    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    [Journaling(JournalingMode.NoCommandData)]
    internal class Command : IExternalCommand
    {
        private AreaReinforcement m_areaRein;

        /// <summary>
        ///     it is convenient for other class to get
        /// </summary>
        public static ExternalCommandData CommandData { get; private set; }

        /// <summary>
        ///     all hook types in current project
        ///     it is static because of IConverter limitation
        /// </summary>
        public static Hashtable HookTypes { get; private set; }

        /// <summary>
        ///     all hook types in current project
        ///     it is static because of IConverter limitation
        /// </summary>
        public static Hashtable BarTypes { get; private set; }

        public Result Execute(
            ExternalCommandData revit,
            ref string message,
            ElementSet elements)
        {
            var trans = new Transaction(revit.Application.ActiveUIDocument.Document,
                "Revit.SDK.Samples.AreaReinParameters");
            trans.Start();
            CommandData = revit;
            if (!PreData())
            {
                message = "Please select only one AreaReinforcement and ";
                message += "make sure there are Hook Types and Bar Types in current project.";
                trans.RollBack();
                return Result.Failed;
            }

            IAreaReinData data = new WallAreaReinData();
            if (!data.FillInData(m_areaRein))
            {
                data = new FloorAreaReinData();
                if (!data.FillInData(m_areaRein))
                {
                    message = "Failed to get properties of selected AreaReinforcement.";
                    trans.RollBack();
                    return Result.Failed;
                }
            }

            var form = new AreaReinParametersForm(data);
            if (form.ShowDialog() == DialogResult.Cancel)
            {
                trans.RollBack();
                return Result.Cancelled;
            }

            trans.Commit();
            return Result.Succeeded;
        }

        /// <summary>
        ///     check whether the selected is expected, find all hooktypes in current project
        /// </summary>
        /// <param name="selected">selected elements</param>
        /// <returns>whether the selected AreaReinforcement is expected</returns>
        private bool PreData()
        {
            var selected = new ElementSet();
            foreach (var elementId in CommandData.Application.ActiveUIDocument.Selection.GetElementIds())
                selected.Insert(CommandData.Application.ActiveUIDocument.Document.GetElement(elementId));

            //selected is not only one AreaReinforcement
            if (selected.Size != 1) return false;
            foreach (var o in selected) m_areaRein = o as AreaReinforcement;
            if (null == m_areaRein) return false;

            //make sure hook type and bar type exist in current project and get them
            HookTypes = new Hashtable();
            BarTypes = new Hashtable();

            var activeDoc = CommandData.Application.ActiveUIDocument.Document;

            var itor = new FilteredElementCollector(activeDoc).OfClass(typeof(RebarHookType)).GetElementIterator();
            itor.Reset();
            while (itor.MoveNext())
            {
                if (itor.Current is RebarHookType hookType)
                {
                    var hookTypeName = hookType.Name;
                    HookTypes.Add(hookTypeName, hookType.Id);
                }
            }

            itor = new FilteredElementCollector(activeDoc).OfClass(typeof(RebarBarType)).GetElementIterator();
            itor.Reset();
            while (itor.MoveNext())
            {
                if (itor.Current is RebarBarType barType)
                {
                    var barTypeName = barType.Name;
                    BarTypes.Add(barTypeName, barType.Id);
                }
            }

            return HookTypes.Count != 0 && BarTypes.Count != 0;
        }
    }

    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    internal class RebarParas : IExternalCommand
    {
        public Result Execute(
            ExternalCommandData revit,
            ref string message,
            ElementSet elements)
        {
            try
            {
                // Get the active document and view
                var revitDoc = revit.Application.ActiveUIDocument;
                foreach (var elemId in revitDoc.Selection.GetElementIds())
                {
                    //if( elem.GetType() == typeof( Autodesk.Revit.DB.Structure.Rebar ) )
                    var elem = revitDoc.Document.GetElement(elemId);
                    if (elem is Rebar rebar)
                    {
                        var str = "";
                        var pars = rebar.Parameters;
                        foreach (Parameter param in pars)
                        {
                            var val = "";
                            var name = param.Definition.Name;
                            var type = param.StorageType;
                            switch (type)
                            {
                                case StorageType.Double:
                                    val = param.AsDouble().ToString();
                                    break;
                                case StorageType.ElementId:
                                    var id = param.AsElementId();
                                    var paraElem = revitDoc.Document.GetElement(id);
                                    if (paraElem != null)
                                        val = paraElem.Name;
                                    break;
                                case StorageType.Integer:
                                    val = param.AsInteger().ToString();
                                    break;
                                case StorageType.String:
                                    val = param.AsString();
                                    break;
                            }

                            str = str + name + ": " + val + "\r\n";
                        }

                        TaskDialog.Show("Rebar parameters", str);
                        return Result.Succeeded;
                    }
                }

                message = "No rebar selected!";
                return Result.Failed;
            }
            catch (Exception e)
            {
                message = e.Message;
                return Result.Failed;
            }
        }
    }
}

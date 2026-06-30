// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Structure;
using Autodesk.Revit.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace Ara3D.RevitSampleBrowser.AreaReinParameters.CS
{
    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    [Journaling(JournalingMode.NoCommandData)]
    public class Command : IExternalCommand
    {
        private AreaReinforcement m_areaRein;

        public static ExternalCommandData CommandData { get; private set; }

        public static Hashtable HookTypes { get; private set; }

        public static Hashtable BarTypes { get; private set; }

        public Result Execute(
            ExternalCommandData revit,
            ref string message,
            ElementSet elements)
        {
            Transaction trans = new(revit.Application.ActiveUIDocument.Document,
                "Ara3D.RevitSampleBrowser.AreaReinParameters");
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

            AreaReinParametersForm form = new(data);
            if (form.ShowDialog() == DialogResult.Cancel)
            {
                trans.RollBack();
                return Result.Cancelled;
            }

            trans.Commit();
            return Result.Succeeded;
        }

        private bool PreData()
        {
            var selectedIds = CommandData.Application.ActiveUIDocument.Selection.GetElementIds();
            if (selectedIds.Count != 1) return false;

            var doc = CommandData.Application.ActiveUIDocument.Document;
            m_areaRein = doc.GetElement(selectedIds.First()) as AreaReinforcement;
            if (m_areaRein == null) return false;

            HookTypes = new Hashtable(
                doc.GetElements<RebarHookType>().ToDictionary(ht => ht.Name, ht => ht.Id));
            BarTypes = new Hashtable(
                doc.GetElements<RebarBarType>().ToDictionary(bt => bt.Name, bt => bt.Id));

            return HookTypes.Count != 0 && BarTypes.Count != 0;
        }
    }

    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    public class RebarParas : IExternalCommand
    {
        public Result Execute(
            ExternalCommandData revit,
            ref string message,
            ElementSet elements)
        {
            try
            {
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

                            str = $"{str}{name}: {val}\r\n";
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

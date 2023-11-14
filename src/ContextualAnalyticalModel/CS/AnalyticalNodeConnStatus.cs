using System;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Structure;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;

namespace ContextualAnalyticalModel
{
    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    internal class AnalyticalNodeConnStatus : IExternalCommand
    {
        public virtual Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            try
            {
                var uiDoc = commandData.Application.ActiveUIDocument;
                var document = uiDoc.Document;

                var refNode = uiDoc.Selection.PickObject(ObjectType.Element, "Please select an Anlytical Node");
                if (refNode != null)
                {
                    var analyticalNode = document.GetElement(refNode.ElementId);
                    if (analyticalNode == null)
                        return Result.Failed;

                    var analyticalNodeData = AnalyticalNodeData.GetAnalyticalNodeData(analyticalNode);

                    if (analyticalNodeData != null) analyticalNodeData.GetConnectionStatus();
                }
            }
            catch (Exception ex)
            {
                message = ex.Message;
                return Result.Failed;
            }

            return Result.Succeeded;
        }
    }
}
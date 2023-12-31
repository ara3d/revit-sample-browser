// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using System;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Structure;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;

namespace Ara3D.RevitSampleBrowser.ContextualAnalyticalModel.CS
{
    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    public class AnalyticalNodeConnStatus : IExternalCommand
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

                    analyticalNodeData?.GetConnectionStatus();
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

using System;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.DB.Structure;

namespace ContextualAnalyticalModel
{
   /// <summary>
   /// Implements the Revit add-in interface IExternalCommand
   /// </summary>
   [Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]
   [Autodesk.Revit.Attributes.Regeneration(Autodesk.Revit.Attributes.RegenerationOption.Manual)]
   class AnalyticalNodeConnStatus : IExternalCommand
   {
      public virtual Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
      {
         try
         {
            var uiDoc = commandData.Application.ActiveUIDocument;
            var document = uiDoc.Document;

            var refNode = uiDoc.Selection.PickObject(Autodesk.Revit.UI.Selection.ObjectType.Element, "Please select an Anlytical Node");
            if(refNode != null)
            {
               var analyticalNode = document.GetElement(refNode.ElementId);
               if (analyticalNode == null)
                  return Result.Failed;

               var analyticalNodeData = AnalyticalNodeData.GetAnalyticalNodeData(analyticalNode);

               AnalyticalNodeConnectionStatus nodeStatus;
               if(analyticalNodeData != null)
               {
                  nodeStatus = analyticalNodeData.GetConnectionStatus();
               }
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

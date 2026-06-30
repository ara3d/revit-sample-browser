// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using Ara3D.RevitSampleBrowser.N3P_Shared.CS;
using Autodesk.Revit.Attributes;
using Nice3point.Revit.Toolkit;
using ToolkitExternalCommand = Nice3point.Revit.Toolkit.External.ExternalCommand;

namespace Ara3D.RevitSampleBrowser.N3P_ExternalCommand.CS
{
    [Transaction(TransactionMode.ReadOnly)]
    public class N3P_ExternalCommand : ToolkitExternalCommand
    {
        public override void Execute()
        {
            N3POutput.Header("Nice3point RevitToolkit ExternalCommand");

            N3POutput.Line("IsRevitInApiMode", RevitContext.IsRevitInApiMode);
            N3POutput.Line("Active document", RevitContext.ActiveDocument?.Title ?? "(none)");
            N3POutput.Line("Active view", RevitContext.ActiveView?.Name ?? "(none)");
            N3POutput.Line("Command view", View?.Name ?? "(none)");
        }
    }
}

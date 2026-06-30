// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using Ara3D.RevitSampleBrowser.Common.Infrastructure;
using Autodesk.Revit.UI;
namespace Ara3D.RevitSampleBrowser.ModelessDialog.ModelessForm_IdlingEvent.CS
{
    public static class RequestHandler
    {
        public static void Execute(UIApplication uiapp, RequestId reqest)
        {
            switch (reqest)
            {
                case RequestId.None:
                    return;
                case RequestId.Delete:
                    DialogHelper.ModifySelectedDoors(uiapp, "Delete doors", e => e.Document.Delete(e.Id));
                    break;
                case RequestId.FlipLeftRight:
                    DialogHelper.ModifySelectedDoors(uiapp, "Flip door Hand", e => e.flipHand());
                    break;
                case RequestId.FlipInOut:
                    DialogHelper.ModifySelectedDoors(uiapp, "Flip door Facing", e => e.flipFacing());
                    break;
                case RequestId.MakeLeft:
                    DialogHelper.ModifySelectedDoors(uiapp, "Make door Left", DialogHelper.MakeLeft);
                    break;
                case RequestId.MakeRight:
                    DialogHelper.ModifySelectedDoors(uiapp, "Make door Right", DialogHelper.MakeRight);
                    break;
                case RequestId.TurnOut:
                    DialogHelper.ModifySelectedDoors(uiapp, "Place door Out", DialogHelper.TurnOut);
                    break;
                case RequestId.TurnIn:
                    DialogHelper.ModifySelectedDoors(uiapp, "Place door In", DialogHelper.TurnIn);
                    break;
                case RequestId.Rotate:
                    DialogHelper.ModifySelectedDoors(uiapp, "Rotate door", DialogHelper.FlipHandAndFace);
                    break;
            }
        }
    }
}

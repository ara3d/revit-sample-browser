using System;
using System.Threading.Tasks;
using System.Windows.Forms;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Events;

namespace BuildingCoder
{
    internal static partial class Util
    {
        internal static void CloseDocProc(object stateInfo)
        {
            try
            {
                SendKeys.SendWait("^{F4}");
            }
            catch (Exception ex)
            {
                ErrorMsg(ex.Message);
            }
        }

        internal static void CloseDocByCommand(UIApplication uiapp)
        {
            var closeDoc
                = RevitCommandId.LookupPostableCommandId(
                    PostableCommand.Close);

            uiapp.PostCommand(closeDoc);
        }

        internal static void OnDialogBoxShowing(
            object sender,
            DialogBoxShowingEventArgs args)
        {
            var e2 = args
                as TaskDialogShowingEventArgs;

            e2.OverrideResult((int) TaskDialogResult.Ok);
        }

        internal static async void RunCommands(
            UIApplication uiapp,
            RevitCommandId id_addin)
        {
            uiapp.PostCommand(id_addin);
            await Task.Delay(400);
            SendKeys.Send("{ENTER}");
            await Task.Delay(400);
            SendKeys.Send("{ENTER}");
            await Task.Delay(400);
            SendKeys.Send("{ENTER}");
            await Task.Delay(400);
            SendKeys.Send("{ESCAPE}");
            await Task.Delay(400);
            SendKeys.Send("{ESCAPE}");
        }

        internal static void TwinMotionExportFbx(Document doc)
        {
            var app = doc.Application;
            var uiapp = new UIApplication(app);

            try
            {
                var name = "CustomCtrl_%CustomCtrl_%"
                           + "Twinmotion 2020%Twinmotion Direct Link%"
                           + "ExportButton";

                var id_addin = RevitCommandId
                    .LookupCommandId(name);

                if (id_addin != null)
                {
                    uiapp.DialogBoxShowing += OnDialogBoxShowing;

                    RunCommands(uiapp, id_addin);
                }
            }

            catch
            {
                TaskDialog.Show("Test", "error");
            }
            finally
            {
                uiapp.DialogBoxShowing
                    -= OnDialogBoxShowing;
            }
        }
    }
}

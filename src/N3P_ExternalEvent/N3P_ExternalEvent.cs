// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using Ara3D.RevitSampleBrowser.Common.Infrastructure;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using ToolkitExternalCommand = Nice3point.Revit.Toolkit.External.ExternalCommand;
using WinFormsForm = System.Windows.Forms.Form;

namespace Ara3D.RevitSampleBrowser.N3P_ExternalEvent.CS
{
    [Transaction(TransactionMode.Manual)]
    public class N3P_ExternalEvent : ToolkitExternalCommand
    {
        private static N3PExternalEventForm _form;

        public override void Execute()
        {
            if (_form == null || _form.IsDisposed)
            {
                RevitActionEvent actionEvent = new();
                _form = new N3PExternalEventForm(Application, actionEvent);
                _form.Show();
            }
            else
            {
                _form.BringToFront();
            }
        }
    }

    internal sealed class N3PExternalEventForm : WinFormsForm
    {
        private readonly UIApplication _uiApplication;
        private readonly RevitActionEvent _actionEvent;

        public N3PExternalEventForm(UIApplication uiApplication, RevitActionEvent actionEvent)
        {
            _uiApplication = uiApplication;
            _actionEvent = actionEvent;

            Text = "N3P External Event";
            Width = 320;
            Height = 140;

            Button button = new()
            {
                Text = "Tag selection (N3P Toolkit event)",
                Dock = DockStyle.Fill
            };
            button.Click += OnTagSelection;
            Controls.Add(button);
        }

        private void OnTagSelection(object sender, EventArgs e)
        {
            _actionEvent.Raise(TagSelection);
        }

        private void TagSelection(UIApplication app)
        {
            var uidoc = app.ActiveUIDocument;
            if (uidoc == null)
                return;

            var ids = uidoc.Selection.GetElementIds();
            if (ids.Count == 0)
            {
                TaskDialog.Show("N3P External Event", "Select one or more elements first.");
                return;
            }

            using (Transaction transaction = new(uidoc.Document, "N3P tag selection"))
            {
                transaction.Start();
                foreach (var id in ids.Take(10))
                {
                    var element = uidoc.Document.GetElement(id);
                    element?.get_Parameter(BuiltInParameter.ALL_MODEL_INSTANCE_COMMENTS)
                        ?.Set("Tagged via RevitToolkitEvents");
                }

                transaction.Commit();
            }

            TaskDialog.Show("N3P External Event", $"Tagged {Math.Min(ids.Count, 10)} element(s).");
        }
    }
}

// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace Ara3D.RevitSampleBrowser
{
    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    [Journaling(JournalingMode.UsingCommandData)]
    public class MultiSampleCommand : IExternalCommand
    {
        public MultiSampleMainForm Form { get; private set; }
        public ExternalCommandData CommandData { get; private set; }
        public GenericExternalEventHandler EventHandler { get; } = new GenericExternalEventHandler();

        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            try
            {
                CommandData = commandData;

                Form = new MultiSampleMainForm();

                var samples = Assembly.GetExecutingAssembly()
                    .GetTypes()
                    .Where(t => typeof(IExternalCommand).IsAssignableFrom(t)
                                || typeof(IExternalApplication).IsAssignableFrom(t))
                    .Select(t => new SampleData(t))
                    .OrderBy(t => t.Name);

                var header1 = new ColumnHeader
                {
                    Text = "Name",
                    Width = 400
                };

                var header2 = new ColumnHeader
                {
                    Text = "Type",
                    Width = 200
                };

                var header3 = new ColumnHeader
                {
                    Text = "Readme",
                    Width = 400
                };

                Form.listView1.Columns.Add(header1);
                Form.listView1.Columns.Add(header2);
                Form.listView1.Columns.Add(header3);

                Form.listView1.Items.Clear();

                foreach (var sample in samples)
                {
                    var item = Form.listView1.Items.Add(sample.Name);
                    item.SubItems.Add(sample.IsCommand ? "Command" : "Application");
                    item.SubItems.Add(sample.ReadmePath);
                    item.Tag = sample;
                }

                Form.listView1.SelectedIndexChanged += ListView1_SelectedIndexChanged;
                Form.listView1.FullRowSelect = true;
                Form.listView1.MouseDoubleClick += ListView1_MouseDoubleClick;
                Form.Show();
                return Result.Succeeded;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                return Result.Failed;
            }
        }

        private void ListView1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            try
            {
                var info = Form.listView1.HitTest(e.X, e.Y);
                var item = info.Item;

                if (item?.Tag is SampleData sample)
                {
                    EventHandler.Raise(() => 
                        sample.Activate(CommandData, MultiSampleApplication.Application),
                        $"Activate sample {item.Name}");
                }
                else
                {
                    MessageBox.Show($"Internal error: couldn't find sample data");
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
        }

        private void ListView1_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                Form.richTextBox1.ResetText();
                if (Form.listView1.SelectedItems.Count > 0)
                {
                    var item = Form.listView1.SelectedItems[0];
                    if (!(item.Tag is SampleData data))
                        return;
                    if (File.Exists(data.ReadmePath))
                        Form.richTextBox1.LoadFile(data.ReadmePath);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
        }
    }
}

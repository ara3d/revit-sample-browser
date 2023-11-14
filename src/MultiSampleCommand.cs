// Copyright 2023. See https://github.com/ara3d/revit-samples/LICENSE.txt

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Windows.Forms;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace RevitMultiSample
{
    public class SampleData
    {
        public Type Type { get; }
        public string Namespace { get; } 
        public string Name { get; }
        public bool IsCommand => typeof(IExternalCommand).IsAssignableFrom(Type);
        public bool IsApplication => typeof(IExternalApplication).IsAssignableFrom(Type);
        public string FolderPath { get; }
        public string ReadmePath { get; }
        public bool FolderPathExists => Directory.Exists(FolderPath);

        public void Activate()
        {
            if (IsCommand)
            {
                var command = Activator.CreateInstance(Type) as IExternalCommand;
                var message = "";
                command?.Execute(null, ref message, null);
            }
            else if (IsApplication)
            {
                // TODO: this requires that we have a UIExternalApplication and store the data somewhere to make it available.
                //var app = Activator.CreateInstance(Type) as IExternalApplication;
                //app?.OnStartup()
            }
        }

        public SampleData(Type type)
        {
            Type = type ?? throw new ArgumentNullException(nameof(type));
            Namespace = type.Namespace ?? "";
            var prefix = "RevitMultiSample.";
            if (Namespace.StartsWith(prefix))
                Namespace = Namespace.Substring(prefix.Length);
            var suffix = ".CS";
            if (Namespace.EndsWith(suffix))
                Namespace = Namespace.Substring(0, Namespace.Length - suffix.Length);
            Name = Namespace + "." + Type.Name;

            var folderRelPath = Namespace.Replace(".", "\\");
            FolderPath = ThisFolderPath + "\\" + folderRelPath + @"\CS";
            if (Directory.Exists(FolderPath))
                ReadmePath = Directory.GetFiles(FolderPath, "*.rtf").FirstOrDefault();
        }

        private static readonly string ThisFolderPath = GetThisFolder();

        /// <summary>
        /// Fancy C# trick for getting the source file path. The "callerFilePath" parameter
        /// is automatically supplied by the compiler. 
        /// </summary>
        private static string GetThisFolder([CallerFilePathAttribute] string callerFilePath = null)
        {
            return new FileInfo(callerFilePath ?? "").DirectoryName;
        }
    }

    [Transaction(TransactionMode.ReadOnly)]
    [Regeneration(RegenerationOption.Manual)]
    [Journaling(JournalingMode.NoCommandData)]
    public class MultiSampleCommand : IExternalCommand
    {
        private MultiSampleMainForm m_form;

        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            try
            {
                m_form = new MultiSampleMainForm();

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

                m_form.listView1.Columns.Add(header1);
                m_form.listView1.Columns.Add(header2);
                m_form.listView1.Columns.Add(header3);

                m_form.listView1.Items.Clear();

                foreach (var sample in samples)
                {
                    var item = m_form.listView1.Items.Add(sample.Name);
                    item.SubItems.Add(sample.IsCommand ? "Command" : "Application");
                    item.SubItems.Add(sample.ReadmePath);
                    item.Tag = sample;
                }

                m_form.listView1.SelectedIndexChanged += ListView1_SelectedIndexChanged;
                m_form.listView1.FullRowSelect = true;
                m_form.listView1.MouseDoubleClick += ListView1_MouseDoubleClick;
                m_form.Show();
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
                var info = m_form.listView1.HitTest(e.X, e.Y);
                var item = info.Item;

                if (item?.Tag is SampleData sample)
                {
                    sample.Activate();
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
                m_form.richTextBox1.ResetText();
                if (m_form.listView1.SelectedItems.Count > 0)
                {
                    var item = m_form.listView1.SelectedItems[0];
                    var data = item.Tag as SampleData;
                    if (data == null)
                        return;
                    if (File.Exists(data.ReadmePath))
                        m_form.richTextBox1.LoadFile(data.ReadmePath);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
        }
    }
}

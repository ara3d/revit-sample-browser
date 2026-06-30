using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Threading;
using System.Windows;
using Autodesk.Revit.DB;
using ExcelExporterImporter.ViewModels;

namespace ExcelExporterImporter.Views
{
    public partial class ImportWindow
    {
        private readonly ConsoleTraceListener consoleTraceListener;

        public ImportWindow(Document doc)
        {
            var application = doc.Application;
            var lang = application.Language;
            if (lang.ToString().Contains("French"))
            {
                var cultureInfo = new CultureInfo("fr-FR");
                Thread.CurrentThread.CurrentCulture = Thread.CurrentThread.CurrentUICulture = cultureInfo;
            }

            InitializeComponent();

            var viewModel = new ImportViewModel(this, doc);
            DataContext = viewModel;
            if (viewModel.CloseAction == null)
                viewModel.CloseAction = Close;

            consoleTraceListener = new ConsoleTraceListener();
            PresentationTraceSources.Refresh();
            PresentationTraceSources.DataBindingSource.Listeners.Add(consoleTraceListener);
            PresentationTraceSources.DataBindingSource.Switch.Level = SourceLevels.Warning | SourceLevels.Error;
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            base.OnClosing(e);
            if (!e.Cancel) Settings.Default.Save();
        }

        protected override void OnClosed(EventArgs e)
        {
            PresentationTraceSources.DataBindingSource.Listeners.Remove(consoleTraceListener);
            PresentationTraceSources.Refresh();
            base.OnClosed(e);
        }
    }
}

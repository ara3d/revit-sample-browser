// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using Ara3D.RevitSampleBrowser.Common.Infrastructure;
using Ara3D.RevitSampleBrowser.ExtensibleStorageManager.SchemaWrapperTools.CS;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.ExtensibleStorage;
using Microsoft.Win32;
using System;
using System.ComponentModel;
using System.IO;
using System.Reflection;
using System.Windows;
namespace Ara3D.RevitSampleBrowser.ExtensibleStorageManager.ExtensibleStorageManager.CS.User
{
    public partial class UiCommand : Window
    {
        private SchemaWrapper m_schemaWrapper;

        public UiCommand(Document doc, string applicationId)
        {
            InitializeComponent();
            Closing += UICommand_Closing;
            Document = doc;

            m_schemaWrapper = SchemaWrapper.NewSchema(Guid.Empty, AccessLevel.Public, AccessLevel.Public, "adsk",
                applicationId, "schemaName", "Schema documentation");
            m_label_applicationAppId.Content = applicationId;
            UpdateUi();
        }

        public Document Document { get; set; }

        private string GetStartingXmlPath()
        {
            var currentAssemby = Assembly.GetAssembly(GetType()).Location;
            return Path.Combine(Path.GetDirectoryName(currentAssemby), "schemas");
        }

        private void UpdateUi()
        {
            m_textBox_SchemaApplicationId.Text = m_schemaWrapper.Data.ApplicationId;
            m_textBox_SchemaVendorId.Text = m_schemaWrapper.Data.VendorId;
            m_textBox_SchemaPath.Content = m_schemaWrapper.GetXmlPath();
            m_textBox_SchemaName.Text = m_schemaWrapper.Data.Name;
            m_textBox_SchemaDocumentation.Text = m_schemaWrapper.Data.Documentation;
            m_textBox_SchemaId.Text = m_schemaWrapper.Data.SchemaId;
            if (m_textBox_SchemaId.Text == Guid.Empty.ToString())
                m_textBox_SchemaId.Text = Application.Application.LastGuid;

            switch (m_schemaWrapper.Data.ReadAccess)
            {
                case AccessLevel.Application:
                    {
                        m_rb_ReadAccess_Application.IsChecked = true;
                        break;
                    }
                case AccessLevel.Public:
                    {
                        m_rb_ReadAccess_Public.IsChecked = true;
                        break;
                    }
                case AccessLevel.Vendor:
                    {
                        m_rb_ReadAccess_Vendor.IsChecked = true;
                        break;
                    }
            }

            switch (m_schemaWrapper.Data.WriteAccess)
            {
                case AccessLevel.Application:
                    {
                        m_rb_WriteAccess_Application.IsChecked = true;
                        break;
                    }
                case AccessLevel.Public:
                    {
                        m_rb_WriteAccess_Public.IsChecked = true;
                        break;
                    }
                case AccessLevel.Vendor:
                    {
                        m_rb_WriteAccess_Vendor.IsChecked = true;
                        break;
                    }
            }
        }

        private void GetUiAccessLevels(out AccessLevel read, out AccessLevel write)
        {
            read = AccessLevel.Public;
            write = AccessLevel.Public;

            read = m_rb_ReadAccess_Application.IsChecked == true
                ? AccessLevel.Application
                : m_rb_ReadAccess_Public.IsChecked == true ? AccessLevel.Public : AccessLevel.Vendor;

            write = m_rb_WriteAccess_Application.IsChecked == true
                ? AccessLevel.Application
                : m_rb_WriteAccess_Public.IsChecked == true ? AccessLevel.Public : AccessLevel.Vendor;
        }

        private bool ValidateGuids()
        {
            var retval = true;
            try
            {
                new Guid(m_textBox_SchemaId.Text);
                new Guid(m_textBox_SchemaApplicationId.Text);
            }
            catch (Exception)
            {
                retval = false;
            }

            return retval;
        }

        // Persist last-used schema Guid for the next dialog session.
        private void UICommand_Closing(object sender, CancelEventArgs e)
        {
            Application.Application.LastGuid = m_textBox_SchemaId.Text;
        }

        private void m_button_NewSchemaId_Click(object sender, RoutedEventArgs e)
        {
            m_textBox_SchemaId.Text = ExtensibleStorageHelper.NewGuid().ToString();
        }

        private void m_button_CreateSetSaveSimple_Click(object sender, RoutedEventArgs e)
        {
            CreateSetSave(SampleSchemaComplexity.SimpleExample);
        }

        private void m_button_CreateSetSaveComplex_Click(object sender, RoutedEventArgs e)
        {
            CreateSetSave(SampleSchemaComplexity.ComplexExample);
        }

        private void CreateSetSave(SampleSchemaComplexity schemaComplexity)
        {
            AccessLevel read;
            AccessLevel write;
            GetUiAccessLevels(out read, out write);
            if (!ValidateGuids())
            {
                TaskDialog.Show("ExtensibleStorage Manager", "Invalid Schema or ApplicationId Guid.");
                return;
            }

            SaveFileDialog sfd = new()
            {
                DefaultExt = ".xml",
                Filter = "SchemaWrapper Xml files (*.xml)|*.xml",
                InitialDirectory = GetStartingXmlPath(),
                FileName =
                    $"{m_textBox_SchemaName.Text}_{m_textBox_SchemaVendorId.Text}___{m_textBox_SchemaId.Text.Substring(31)}.xml"
            };

            var result = sfd.ShowDialog();

            if (result.HasValue && result == true)
            {
                try
                {
                    m_schemaWrapper = StorageCommand.CreateSetAndExport(Document.ProjectInformation, sfd.FileName,
                        new Guid(m_textBox_SchemaId.Text), read, write, m_textBox_SchemaVendorId.Text,
                        m_textBox_SchemaApplicationId.Text, m_textBox_SchemaName.Text,
                        m_textBox_SchemaDocumentation.Text, schemaComplexity);
                }
                catch (Exception ex)
                {
                    TaskDialog.Show("ExtensibleStorage Manager", $"Could not Create Schema.  {ex}");
                    return;
                }

                UpdateUi();

                UiData dataDialog = new();
                var schemaData = m_schemaWrapper.ToString();
                var entityData =
                    m_schemaWrapper.GetSchemaEntityData(
                        Document.ProjectInformation.GetEntity(m_schemaWrapper.GetSchema()));
                var allData =
                    $"Schema: {Environment.NewLine}{schemaData}{Environment.NewLine}{Environment.NewLine}Entity{Environment.NewLine}{entityData}";
                dataDialog.SetData(allData);
                dataDialog.ShowDialog();
            }
        }

        private void m_button_CreateWrapperFromSchema_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                StorageCommand.CreateWrapperFromSchema(new Guid(m_textBox_SchemaId.Text), out m_schemaWrapper);
                UpdateUi();
            }

            catch (Exception ex)
            {
                TaskDialog.Show("ExtensibleStorage Manager", $"Could not Create SchemaWrapper from Schema.  {ex}");
                return;
            }

            UiData dataDialog = new();
            dataDialog.SetData(m_schemaWrapper.ToString());
            dataDialog.ShowDialog();
        }

        private void m_button_LookupExtract_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                StorageCommand.LookupAndExtractData(Document.ProjectInformation, new Guid(m_textBox_SchemaId.Text),
                    out m_schemaWrapper);
            }
            catch (Exception ex)
            {
                TaskDialog.Show("ExtensibleStorage Manager", $"Could not extract data from Schema.  {ex}");
                return;
            }

            UpdateUi();
            UiData dataDialog = new();

            var schemaData = m_schemaWrapper.ToString();
            var entityData =
                m_schemaWrapper.GetSchemaEntityData(Document.ProjectInformation.GetEntity(m_schemaWrapper.GetSchema()));
            var allData =
                $"Schema: {Environment.NewLine}{schemaData}{Environment.NewLine}{Environment.NewLine}Entity{Environment.NewLine}{entityData}";

            dataDialog.SetData(allData);
            dataDialog.ShowDialog();
        }

        private void m_button_CreateSchemaFromXml_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog ofd = new()
            {
                InitialDirectory = GetStartingXmlPath(),
                DefaultExt = ".xml",
                Filter = "SchemaWrapper Xml files (*.xml)|*.xml"
            };
            var result = ofd.ShowDialog();

            if (result.HasValue && result == true)
            {
                try
                {
                    StorageCommand.ImportSchemaFromXml(ofd.FileName, out m_schemaWrapper);
                }

                catch (Exception ex)
                {
                    TaskDialog.Show("ExtensibleStorage Manager", $"Could not import Schema from Xml.  {ex}");
                    return;
                }

                UpdateUi();

                UiData dataDialog = new();
                dataDialog.SetData(m_schemaWrapper.ToString());
                dataDialog.ShowDialog();
            }
        }

        private void m_button_EditExistingSimple_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Requires matching schema Guid and field layout in the current document.
                StorageCommand.EditExistingData(Document.ProjectInformation, new Guid(m_textBox_SchemaId.Text),
                    out m_schemaWrapper);
            }
            catch (Exception ex)
            {
                TaskDialog.Show("ExtensibleStorage Manager", $"Could not extract data from Schema.  {ex}");
                return;
            }

            UpdateUi();

            UiData dataDialog = new();
            var schemaData = m_schemaWrapper.ToString();
            var entityData =
                m_schemaWrapper.GetSchemaEntityData(Document.ProjectInformation.GetEntity(m_schemaWrapper.GetSchema()));
            var allData =
                $"Schema: {Environment.NewLine}{schemaData}{Environment.NewLine}{Environment.NewLine}Entity{Environment.NewLine}{entityData}";

            dataDialog.SetData(allData);
            dataDialog.ShowDialog();
        }
    }
}

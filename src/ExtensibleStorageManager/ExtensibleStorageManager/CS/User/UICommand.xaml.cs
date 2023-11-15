// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt
using System;
using System.ComponentModel;
using System.IO;
using System.Reflection;
using System.Windows;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.ExtensibleStorage;
using Autodesk.Revit.UI;
using Microsoft.Win32;
using SchemaWrapperTools;

namespace ExtensibleStorageManager
{
    /// <summary>
    ///     The main user dialog class for issuing sample commands to the a SchemaWrapper
    /// </summary>
    public partial class UiCommand : Window
    {
        /// <summary>
        ///     The object that provides high level serialization access to an Autodesk.Revit.DB.ExtensibleStorage.Schema
        /// </summary>
        private SchemaWrapper m_schemaWrapper;

        /// <summary>
        ///     Create a new dialog object and store a reference to the active document and applicationID of this addin.
        /// </summary>
        public UiCommand(Document doc, string applicationId)
        {
            InitializeComponent();
            Closing += UICommand_Closing;
            Document = doc;

            //Create a new empty schemaWrapper.
            m_schemaWrapper = SchemaWrapper.NewSchema(Guid.Empty, AccessLevel.Public, AccessLevel.Public, "adsk",
                applicationId, "schemaName", "Schema documentation");
            m_label_applicationAppId.Content = applicationId;
            UpdateUi();
        }

        /// <summary>
        ///     The active document in Revit that the dialog queries for Schema and Entity data.
        /// </summary>
        public Document Document { get; set; }

        /// <summary>
        ///     Return a convenient recommended path to save schema files in.
        /// </summary>
        private string GetStartingXmlPath()
        {
            var currentAssemby = Assembly.GetAssembly(GetType()).Location;
            return Path.Combine(Path.GetDirectoryName(currentAssemby), "schemas");
        }

        /// <summary>
        ///     Synchronize all UI controls in the dialog with the data in m_SchemaWrapper.
        /// </summary>
        private void UpdateUi()
        {
            m_textBox_SchemaApplicationId.Text = m_schemaWrapper.Data.ApplicationId;
            m_textBox_SchemaVendorId.Text = m_schemaWrapper.Data.VendorId;
            m_textBox_SchemaPath.Content = m_schemaWrapper.GetXmlPath();
            m_textBox_SchemaName.Text = m_schemaWrapper.Data.Name;
            m_textBox_SchemaDocumentation.Text = m_schemaWrapper.Data.Documentation;
            m_textBox_SchemaId.Text = m_schemaWrapper.Data.SchemaId;
            if (m_textBox_SchemaId.Text == Guid.Empty.ToString())
                m_textBox_SchemaId.Text = Application.LastGuid;

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

        /// <summary>
        ///     Retrieve AccessLevel enums for read and write permissions from the UI
        /// </summary>
        private void GetUiAccessLevels(out AccessLevel read, out AccessLevel write)
        {
            read = AccessLevel.Public;
            write = AccessLevel.Public;

            if (m_rb_ReadAccess_Application.IsChecked == true)
                read = AccessLevel.Application;
            else if (m_rb_ReadAccess_Public.IsChecked == true)
                read = AccessLevel.Public;
            else
                read = AccessLevel.Vendor;

            if (m_rb_WriteAccess_Application.IsChecked == true)
                write = AccessLevel.Application;
            else if (m_rb_WriteAccess_Public.IsChecked == true)
                write = AccessLevel.Public;
            else
                write = AccessLevel.Vendor;
        }

        /// <summary>
        ///     Ensure that the values in the two text fields in the dialogs meant for Guids evaluate
        ///     to valid Guids.
        /// </summary>
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

        //Store the Guid of the last-used schema in the Application object for convenient access
        //later if the user re-creates and displays this dialog again.
        private void UICommand_Closing(object sender, CancelEventArgs e)
        {
            Application.LastGuid = m_textBox_SchemaId.Text;
        }

        //Put a new, arbitrary Guid in the schema text box.
        private void m_button_NewSchemaId_Click(object sender, RoutedEventArgs e)
        {
            m_textBox_SchemaId.Text = StorageCommand.NewGuid().ToString();
        }

        /// <summary>
        ///     Handler for the "Create a simple schema" button.
        /// </summary>
        private void m_button_CreateSetSaveSimple_Click(object sender, RoutedEventArgs e)
        {
            CreateSetSave(SampleSchemaComplexity.SimpleExample);
        }

        /// <summary>
        ///     Handler for the "Create a complex schema" button.
        /// </summary>
        private void m_button_CreateSetSaveComplex_Click(object sender, RoutedEventArgs e)
        {
            CreateSetSave(SampleSchemaComplexity.ComplexExample);
        }

        /// <summary>
        ///     Creates a sample schema, populates it with sample data, and saves it to an XML file
        /// </summary>
        /// <param name="schemaComplexity">The example schema to create</param>
        private void CreateSetSave(SampleSchemaComplexity schemaComplexity)
        {
            //Get read-write access levels and schema and application Ids from the active dialog
            AccessLevel read;
            AccessLevel write;
            GetUiAccessLevels(out read, out write);
            if (!ValidateGuids())
            {
                TaskDialog.Show("ExtensibleStorage Manager", "Invalid Schema or ApplicationId Guid.");
                return;
            }

            //Get a pathname for an XML file from the user.
            var sfd = new SaveFileDialog
            {
                DefaultExt = ".xml",
                Filter = "SchemaWrapper Xml files (*.xml)|*.xml",
                InitialDirectory = GetStartingXmlPath(),
                FileName = m_textBox_SchemaName.Text + "_" + m_textBox_SchemaVendorId.Text + "___" +
                           m_textBox_SchemaId.Text.Substring(31) + ".xml"
            };

            var result = sfd.ShowDialog();

            if (result.HasValue && result == true)
            {
                try
                {
                    //Create a new sample SchemaWrapper, schema, and Entity and store it in the current document's ProjectInformation element.
                    m_schemaWrapper = StorageCommand.CreateSetAndExport(Document.ProjectInformation, sfd.FileName,
                        new Guid(m_textBox_SchemaId.Text), read, write, m_textBox_SchemaVendorId.Text,
                        m_textBox_SchemaApplicationId.Text, m_textBox_SchemaName.Text,
                        m_textBox_SchemaDocumentation.Text, schemaComplexity);
                }
                catch (Exception ex)
                {
                    TaskDialog.Show("ExtensibleStorage Manager", "Could not Create Schema.  " + ex);
                    return;
                }

                UpdateUi();

                //Display the schema fields and sample data we just created in a dialog.
                var dataDialog = new UiData();
                var schemaData = m_schemaWrapper.ToString();
                var entityData =
                    m_schemaWrapper.GetSchemaEntityData(
                        Document.ProjectInformation.GetEntity(m_schemaWrapper.GetSchema()));
                var allData = "Schema: " + Environment.NewLine + schemaData + Environment.NewLine +
                              Environment.NewLine + "Entity" + Environment.NewLine + entityData;
                dataDialog.SetData(allData);
                dataDialog.ShowDialog();
            }
        }

        /// <summary>
        ///     Handler for the "Create Wrapper from Schema" button
        /// </summary>
        private void m_button_CreateWrapperFromSchema_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                //Given a Guid that corresponds to a schema that already exists in a document, create a SchemaWrapper
                //from it and display its top-level data in the dialog.
                StorageCommand.CreateWrapperFromSchema(new Guid(m_textBox_SchemaId.Text), out m_schemaWrapper);
                UpdateUi();
            }

            catch (Exception ex)
            {
                TaskDialog.Show("ExtensibleStorage Manager", "Could not Create SchemaWrapper from Schema.  " + ex);
                return;
            }

            //Display all of the schema's field data in a separate dialog.
            var dataDialog = new UiData();
            dataDialog.SetData(m_schemaWrapper.ToString());
            dataDialog.ShowDialog();
        }

        /// <summary>
        ///     Handler for the "Look up and extract" button
        /// </summary>
        private void m_button_LookupExtract_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                //Given a Guid that corresponds to a schema that already exists in a document, create a SchemaWrapper
                //from it and display its top-level data in the dialog.
                StorageCommand.LookupAndExtractData(Document.ProjectInformation, new Guid(m_textBox_SchemaId.Text),
                    out m_schemaWrapper);
            }
            catch (Exception ex)
            {
                TaskDialog.Show("ExtensibleStorage Manager", "Could not extract data from Schema.  " + ex);
                return;
            }

            UpdateUi();
            var dataDialog = new UiData();

            //Get and display the schema field data and the actual entity data in a separate dialog.
            var schemaData = m_schemaWrapper.ToString();
            var entityData =
                m_schemaWrapper.GetSchemaEntityData(Document.ProjectInformation.GetEntity(m_schemaWrapper.GetSchema()));
            var allData = "Schema: " + Environment.NewLine + schemaData + Environment.NewLine + Environment.NewLine +
                          "Entity" + Environment.NewLine + entityData;

            dataDialog.SetData(allData);
            dataDialog.ShowDialog();
        }

        /// <summary>
        ///     Handler for the "Create Schema from XML" button
        /// </summary>
        private void m_button_CreateSchemaFromXml_Click(object sender, RoutedEventArgs e)
        {
            //Prompt the user for an xml file containing a serialized schema.
            var ofd = new OpenFileDialog
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
                    //Given an xml file containing schema data, create a new SchemaWrapper object
                    StorageCommand.ImportSchemaFromXml(ofd.FileName, out m_schemaWrapper);
                }

                catch (Exception ex)
                {
                    TaskDialog.Show("ExtensibleStorage Manager", "Could not import Schema from Xml.  " + ex);
                    return;
                }

                //Display the top level schema data in the dialog.
                UpdateUi();

                //Display the field data of the schema in a separate dialog.
                var dataDialog = new UiData();
                dataDialog.SetData(m_schemaWrapper.ToString());
                dataDialog.ShowDialog();
            }
        }

        /// <summary>
        ///     Handler for the "Edit Exisiting Data" button
        /// </summary>
        private void m_button_EditExistingSimple_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                ///Given a guid corresponding to a Schema with Entity data in the current document's ProjectInformation element,
                ///change the entity data to new data, (assuming that the schemas and schema guids are identical).
                StorageCommand.EditExistingData(Document.ProjectInformation, new Guid(m_textBox_SchemaId.Text),
                    out m_schemaWrapper);
            }
            catch (Exception ex)
            {
                TaskDialog.Show("ExtensibleStorage Manager", "Could not extract data from Schema.  " + ex);
                return;
            }

            UpdateUi();

            ///Display the schema fields and new data in a separate dialog.
            var dataDialog = new UiData();
            var schemaData = m_schemaWrapper.ToString();
            var entityData =
                m_schemaWrapper.GetSchemaEntityData(Document.ProjectInformation.GetEntity(m_schemaWrapper.GetSchema()));
            var allData = "Schema: " + Environment.NewLine + schemaData + Environment.NewLine + Environment.NewLine +
                          "Entity" + Environment.NewLine + entityData;

            dataDialog.SetData(allData);
            dataDialog.ShowDialog();
        }
    }
}

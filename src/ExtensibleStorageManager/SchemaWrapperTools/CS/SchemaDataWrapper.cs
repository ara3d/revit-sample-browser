// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

//
// AUTODESK PROVIDES THIS PROGRAM 'AS IS' AND WITH ALL ITS FAULTS.
// AUTODESK SPECIFICALLY DISCLAIMS ANY IMPLIED WARRANTY OF
// MERCHANTABILITY OR FITNESS FOR A PARTICULAR USE. AUTODESK, INC.
// DOES NOT WARRANT THAT THE OPERATION OF THE PROGRAM WILL BE
// UNINTERRUPTED OR ERROR FREE.
//
// Use, duplication, or disclosure by the U.S. Government is subject to
// restrictions set forth in FAR 52.227-19 (Commercial Computer
// Software - Restricted Rights) and DFAR 252.227-7013(c)(1)(ii)
// (Rights in Technical Data and Computer Software), as applicable. 

using System;
using System.Collections.Generic;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.ExtensibleStorage;

namespace Ara3D.RevitSampleBrowser.ExtensibleStorageManager.SchemaWrapperTools.CS
{
    /// <summary>
    ///     A class to store a list of FieldData objects as well as the top level data (name, access levels, SchemaId, etc..)
    ///     of an Autodesk.Revit.DB.ExtensibleStorage.Schema
    /// </summary>
    [Serializable]
    public class SchemaDataWrapper
    {
        private string m_applicationId;
        private List<FieldData> m_dataList;
        private string m_documentation;
        private string m_name;

        private AccessLevel m_readAccess;
        private string m_schemaId;
        private string m_vendorId;
        private AccessLevel m_writeAccess;

        /// <summary>
        ///     For serialization only -- Do not use.
        /// </summary>
        public SchemaDataWrapper()
        {
        }

        /// <summary>
        ///     Create a new SchemaDataWrapper
        /// </summary>
        /// <param name="schemaId">The Guid of the Schema</param>
        /// <param name="readAccess">The access level for read permission</param>
        /// <param name="writeAccess">The access level for write permission</param>
        /// <param name="vendorId">The user-registered vendor ID string</param>
        /// <param name="applicationId">The application ID from the application manifest</param>
        /// <param name="name">The name of the schema</param>
        /// <param name="documentation">Descriptive details on the schema</param>
        public SchemaDataWrapper(Guid schemaId, AccessLevel readAccess, AccessLevel writeAccess, string vendorId,
            string applicationId, string name, string documentation)
        {
            DataList = new List<FieldData>();
            SchemaId = schemaId.ToString();
            ReadAccess = readAccess;
            WriteAccess = writeAccess;
            VendorId = vendorId;
            ApplicationId = applicationId;
            Name = name;
            Documentation = documentation;
        }

        /// <summary>
        ///     The list of FieldData objects in the wrapper
        /// </summary>
        public List<FieldData> DataList
        {
            get => m_dataList;
            set => m_dataList = value;
        }

        /// <summary>
        ///     The schemaId Guid of the Schema
        /// </summary>
        public string SchemaId
        {
            get => m_schemaId;
            set => m_schemaId = value;
        }

        /// <summary>
        ///     The read access of the Schema
        /// </summary>
        public AccessLevel ReadAccess
        {
            get => m_readAccess;
            set => m_readAccess = value;
        }

        /// <summary>
        ///     The write access of the Schema
        /// </summary>
        public AccessLevel WriteAccess
        {
            get => m_writeAccess;
            set => m_writeAccess = value;
        }

        /// <summary>
        ///     Vendor Id
        /// </summary>
        public string VendorId
        {
            get => m_vendorId;
            set => m_vendorId = value;
        }

        /// <summary>
        ///     Application Id
        /// </summary>
        public string ApplicationId
        {
            get => m_applicationId;
            set => m_applicationId = value;
        }

        /// <summary>
        ///     The documentation string for the schema
        /// </summary>
        public string Documentation
        {
            get => m_documentation;
            set => m_documentation = value;
        }

        /// <summary>
        ///     The name of the schema
        /// </summary>
        public string Name
        {
            get => m_name;
            set => m_name = value;
        }

        /// <summary>
        ///     Adds a new field to the wrapper's list of fields.
        /// </summary>
        /// <param name="name">the name of the field</param>
        /// <param name="typeIn">the data type of the field</param>
        /// <param name="spec">The unit type of the Field (set to UT_Undefined for non-floating point types</param>
        /// <param name="subSchema">The SchemaWrapper of the field's subSchema, if the field is of type "Entity"</param>
        public void AddData(string name, Type typeIn, ForgeTypeId spec, SchemaWrapper subSchema)
        {
            m_dataList.Add(new FieldData(name, typeIn.FullName, spec.TypeId, subSchema));
        }
    }
}

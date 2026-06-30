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

        /// <summary>For serialization only — do not use.</summary>
        public SchemaDataWrapper()
        {
        }

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

        public List<FieldData> DataList
        {
            get => m_dataList;
            set => m_dataList = value;
        }

        public string SchemaId
        {
            get => m_schemaId;
            set => m_schemaId = value;
        }

        public AccessLevel ReadAccess
        {
            get => m_readAccess;
            set => m_readAccess = value;
        }

        public AccessLevel WriteAccess
        {
            get => m_writeAccess;
            set => m_writeAccess = value;
        }

        public string VendorId
        {
            get => m_vendorId;
            set => m_vendorId = value;
        }

        public string ApplicationId
        {
            get => m_applicationId;
            set => m_applicationId = value;
        }

        public string Documentation
        {
            get => m_documentation;
            set => m_documentation = value;
        }

        public string Name
        {
            get => m_name;
            set => m_name = value;
        }

        /// <param name="spec">Unit type; use UT_Undefined for non-floating-point types.</param>
        public void AddData(string name, Type typeIn, ForgeTypeId spec, SchemaWrapper subSchema)
        {
            m_dataList.Add(new FieldData(name, typeIn.FullName, spec.TypeId, subSchema));
        }
    }
}

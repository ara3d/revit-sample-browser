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
using System.Text;

namespace Ara3D.RevitSampleBrowser.ExtensibleStorageManager.SchemaWrapperTools.CS
{
    /// <summary>
    ///     A class to store schema field information
    /// </summary>
    [Serializable]
    public class FieldData
    {
        private string m_name;
        private string m_spec;

        private SchemaWrapper m_subSchema;
        private string m_type;

        /// <summary>
        ///     For serialization only -- Do not use.
        /// </summary>
        public FieldData()
        {
        }

        /// <summary>
        ///     Create a new FieldData object
        /// </summary>
        /// <param name="name">The name of the field</param>
        /// <param name="typeIn">The AssemblyQualifiedName of the Field's data type</param>
        /// <param name="spec">The unit type of the Field (set to UT_Undefined for non-floating point types</param>
        public FieldData(string name, string typeIn, string spec) : this(name, typeIn, spec, null)
        {
        }

        /// <summary>
        ///     Create a new FieldData object
        /// </summary>
        /// <param name="name">The name of the field</param>
        /// <param name="typeIn">The AssemblyQualifiedName of the Field's data type</param>
        /// <param name="spec">The unit type of the Field (set to UT_Undefined for non-floating point types</param>
        /// <param name="subSchema">The SchemaWrapper of the field's subSchema, if the field is of type "Entity"</param>
        public FieldData(string name, string typeIn, string spec, SchemaWrapper subSchema)
        {
            m_name = name;
            m_type = typeIn;
            m_spec = spec;
            m_subSchema = subSchema;
        }

        /// <summary>
        ///     The name of a schema field
        /// </summary>
        public string Name
        {
            get => m_name;
            set => m_name = value;
        }

        /// <summary>
        ///     The string representation of a schema field type (e.g. System.Int32)
        /// </summary>
        public string Type
        {
            get => m_type;
            set => m_type = value;
        }

        /// <summary>
        ///     The Unit type of the field
        /// </summary>
        public string Spec
        {
            get => m_spec;
            set => m_spec = value;
        }

        /// <summary>
        ///     The SchemaWrapper of the field's sub-Schema, if is of type "Entity"
        /// </summary>
        public SchemaWrapper SubSchema
        {
            get => m_subSchema;
            set => m_subSchema = value;
        }

        public override string ToString()
        {
            var strBuilder = new StringBuilder();
            strBuilder.Append("   Field: ");
            strBuilder.Append(Name);
            strBuilder.Append(", ");
            strBuilder.Append(Type);
            strBuilder.Append(", ");
            strBuilder.Append(Spec);

            if (SubSchema != null) strBuilder.Append($"{Environment.NewLine}   {SubSchema}");
            return strBuilder.ToString();
        }
    }
}

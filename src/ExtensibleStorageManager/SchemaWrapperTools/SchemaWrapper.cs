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
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Text;
using System.Xml.Serialization;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.ExtensibleStorage;

namespace Ara3D.RevitSampleBrowser.ExtensibleStorageManager.SchemaWrapperTools.CS
{
    /// <summary>
    /// Wraps ExtensibleStorage Schema/SchemaBuilder and supports XML serialization of schema definitions.
    /// </summary>
    [Serializable]
    public class SchemaWrapper
    {
        [NonSerialized] private Assembly m_assembly;

        [NonSerialized] private Schema m_schema;

        [NonSerialized] private SchemaBuilder m_schemaBuilder;

        private SchemaDataWrapper m_schemaDataWrapper;

        [NonSerialized] private string m_xmlPath;

        /// <summary>For serialization only — do not use.</summary>
        public SchemaWrapper()
        {
        }

        private SchemaWrapper(Guid schemaId, AccessLevel readAccess, AccessLevel writeAccess, string vendorId,
            string applicationId, string schemaName, string schemaDescription)
        {
            m_schemaDataWrapper = new SchemaDataWrapper(schemaId, readAccess, writeAccess, vendorId, applicationId,
                schemaName, schemaDescription);
            SetRevitAssembly();
        }

        private SchemaWrapper(Schema schema) : this(schema.GUID, schema.ReadAccessLevel, schema.WriteAccessLevel,
            schema.VendorId, schema.ApplicationGUID.ToString(), schema.SchemaName, schema.Documentation)
        {
            SetSchema(schema);
        }

        /// <summary>Set is for serialization only.</summary>
        public SchemaDataWrapper Data
        {
            get => m_schemaDataWrapper;
            set => m_schemaDataWrapper = value;
        }

        public static SchemaWrapper FromSchema(Schema schema)
        {
            var swReturn = new SchemaWrapper(schema);

            foreach (var currentField in schema.ListFields())
            {
                // AddField is generic; resolve type parameters from the source field and invoke via reflection.
                var addFieldmethod = typeof(SchemaWrapper).GetMethod("AddField",
                    new[] { typeof(string), typeof(ForgeTypeId), typeof(SchemaWrapper) });
                Type[] methodGenericParameters = null;

                switch (currentField.ContainerType)
                {
                    case ContainerType.Simple:
                        methodGenericParameters = new[] { currentField.ValueType };
                        break;
                    case ContainerType.Array:
                        methodGenericParameters = new[]
                            { typeof(IList<int>).GetGenericTypeDefinition().MakeGenericType(currentField.ValueType) };
                        break;
                    default:
                        methodGenericParameters = new[]
                        {
                            typeof(Dictionary<int, int>).GetGenericTypeDefinition()
                                .MakeGenericType(currentField.KeyType, currentField.ValueType)
                        };
                        break;
                }

                var genericAddFieldMethodInstantiated = addFieldmethod.MakeGenericMethod(methodGenericParameters);
                SchemaWrapper swSub = null;
                if (currentField.ValueType == typeof(Entity))
                {
                    var subSchema = Schema.Lookup(currentField.SubSchemaGUID);
                    swSub = FromSchema(subSchema);
                }

                genericAddFieldMethodInstantiated.Invoke(swReturn,
                    new object[] { currentField.FieldName, currentField.GetSpecTypeId(), swSub });
            }

            // Schema is already registered — populate the wrapper only; do not call FinishSchema.
            return swReturn;
        }

        public static SchemaWrapper NewSchema(Guid schemaId, AccessLevel readAccess, AccessLevel writeAccess,
            string vendorId, string applicationId, string name, string description)
        {
            return new SchemaWrapper(schemaId, readAccess, writeAccess, vendorId, applicationId, name, description);
        }

        public static SchemaWrapper FromXml(string xmlDataPath)
        {
            var sampleSchemaInXml = new XmlSerializer(typeof(SchemaWrapper));
            Stream streamXmlIn = new FileStream(xmlDataPath, FileMode.Open, FileAccess.Read, FileShare.Read);
            SchemaWrapper wrapperIn = null;
            try
            {
                wrapperIn = sampleSchemaInXml.Deserialize(streamXmlIn) as SchemaWrapper;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Could not deserialize schema file.{ex}");
                return null;
            }

            wrapperIn.SetRevitAssembly();
            streamXmlIn.Close();
            try
            {
                wrapperIn.FinishSchema();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Could not create schema.{ex}");
                return null;
            }

            return wrapperIn;
        }

        /// <typeparam name="TYpeName">
        /// Supported: int, short, float, double, bool, string, ElementId, XYZ, UV, Guid, Entity,
        /// IDictionary&lt;&gt;, IList&lt;&gt;. IDictionary&lt;&gt; keys cannot be floating point, XYZ, UV, or Entity.
        /// </typeparam>
        public void AddField<TYpeName>(string name, ForgeTypeId spec, SchemaWrapper subSchema)
        {
            m_schemaDataWrapper.AddData(name, typeof(TYpeName), spec, subSchema);
        }

        public void FinishSchema()
        {
            m_schemaBuilder = new SchemaBuilder(new Guid(m_schemaDataWrapper.SchemaId));

            foreach (var currentFieldData in m_schemaDataWrapper.DataList)
            {
                // System types resolve via Type.GetType; Revit API types require the RevitAPI assembly.
                Type fieldType = null;
                try
                {
                    fieldType = Type.GetType(currentFieldData.Type, true, true);
                }

                catch (Exception ex)
                {
                    Debug.WriteLine(ex.ToString());
                    try
                    {
                        fieldType = m_assembly.GetType(currentFieldData.Type);
                    }

                    catch (Exception exx)
                    {
                        Debug.WriteLine($"Error getting type: {exx}");
                        throw;
                    }
                }

                FieldBuilder currentFieldBuilder = null;
                var subSchemaId = Guid.Empty;
                Type[] genericParams = null;

                if (currentFieldData.SubSchema != null)
                    subSchemaId = new Guid(currentFieldData.SubSchema.Data.SchemaId);

                if (fieldType.IsGenericType)
                {
                    var tGeneric = fieldType.GetGenericTypeDefinition();
                    var iDictionaryType = typeof(IDictionary<int, int>).GetGenericTypeDefinition();
                    var iListType = typeof(IList<int>).GetGenericTypeDefinition();

                    genericParams = fieldType.GetGenericArguments();
                    if (tGeneric == iDictionaryType)
                        currentFieldBuilder =
                            m_schemaBuilder.AddMapField(currentFieldData.Name, genericParams[0], genericParams[1]);
                    else if (tGeneric == iListType)
                        currentFieldBuilder = m_schemaBuilder.AddArrayField(currentFieldData.Name, genericParams[0]);
                    else
                        throw new Exception("Generic type is neither IList<> nor IDictionary<>, cannot process.");
                }
                else
                {
                    currentFieldBuilder = m_schemaBuilder.AddSimpleField(currentFieldData.Name, fieldType);
                }

                if (
                    fieldType == typeof(Entity)
                    ||
                    (fieldType.IsGenericType && (genericParams[0] == typeof(Entity) ||
                                                 (genericParams.Length > 1 && genericParams[1] == typeof(Entity))))
                   )
                {
                    currentFieldBuilder.SetSubSchemaGUID(subSchemaId);
                    currentFieldData.SubSchema.FinishSchema();
                }

                if (!string.IsNullOrEmpty(currentFieldData.Spec))
                    currentFieldBuilder.SetSpec(new ForgeTypeId(currentFieldData.Spec));
            }

            m_schemaBuilder.SetReadAccessLevel(Data.ReadAccess);
            m_schemaBuilder.SetWriteAccessLevel(Data.WriteAccess);
            m_schemaBuilder.SetVendorId(Data.VendorId);
            m_schemaBuilder.SetApplicationGUID(new Guid(Data.ApplicationId));
            m_schemaBuilder.SetDocumentation(Data.Documentation);
            m_schemaBuilder.SetSchemaName(Data.Name);

            m_schema = m_schemaBuilder.Finish();
        }

        public void ToXml(string xmlDataPath)
        {
            var sampleSchemaOutXml = new XmlSerializer(typeof(SchemaWrapper));
            Stream streamXmlOut = new FileStream(xmlDataPath, FileMode.OpenOrCreate, FileAccess.ReadWrite,
                FileShare.ReadWrite);
            sampleSchemaOutXml.Serialize(streamXmlOut, this);
            streamXmlOut.Close();
        }

        public override string ToString()
        {
            var strBuilder = new StringBuilder();
            strBuilder.AppendLine(
                $"--Start Schema--   Name: {Data.Name}, Description: {Data.Documentation}, Id: {Data.SchemaId}, ReadAccess: {Data.ReadAccess}, WriteAccess: {Data.WriteAccess}");
            foreach (var fd in Data.DataList)
            {
                strBuilder.AppendLine(fd.ToString());
            }

            strBuilder.AppendLine("--End Schema--");
            return strBuilder.ToString();
        }

        public string GetSchemaEntityData(Entity entity)
        {
            var swBuilder = new StringBuilder();
            DumpAllSchemaEntityData(entity, entity.Schema, swBuilder);
            return swBuilder.ToString();
        }

        private void DumpAllSchemaEntityData<TEntityType>(TEntityType storageEntity, Schema schema,
            StringBuilder strBuilder)
        {
            strBuilder.AppendLine(
                $"Schema/Entity Name: , Description: {schema.Documentation}, Id: {schema.GUID}, Read Access: {schema.ReadAccessLevel}, Write Access: {schema.WriteAccessLevel}");
            foreach (var currentField in schema.ListFields())
            {
                // GetFieldDataAsString is generic; field types are known only at runtime.
                var pmodifiers = Array.Empty<ParameterModifier>();

                var getFieldDataAsStringMethod = typeof(SchemaWrapper).GetMethod("GetFieldDataAsString",
                    BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.InvokeMethod, Type.DefaultBinder,
                    new[] { typeof(Field), typeof(Entity), typeof(StringBuilder) }, pmodifiers);

                // Key type is only used for maps; pass int as placeholder for simple/array fields.
                Type[] methodGenericParameters = null;
                switch (currentField.ContainerType)
                {
                    case ContainerType.Simple:
                    case ContainerType.Array:
                        methodGenericParameters =
                            new[] { typeof(int), currentField.ValueType };
                        break;
                    default:
                        methodGenericParameters = new[] { currentField.KeyType, currentField.ValueType };
                        break;
                }

                var genericGetFieldDataAsStringmethodInstantiated =
                    getFieldDataAsStringMethod.MakeGenericMethod(methodGenericParameters);
                genericGetFieldDataAsStringmethodInstantiated.Invoke(this,
                    new object[] { currentField, storageEntity, strBuilder });
            }

            strBuilder.AppendLine("---------------------------------------------------------");
        }

        private void GetFieldDataAsString<TKeyType, TFieldType>(Field field, Entity entity, StringBuilder strBuilder)
        {
            var fieldType = field.ValueType;
            var fieldUnit = field.GetSpecTypeId();
            Type[] methodGenericParameters = null;
            object[] invokeParams = null;
            Type[] methodOverloadSelectionParams = null;
            switch (field.ContainerType)
            {
                case ContainerType.Simple:
                    methodGenericParameters = new[] { field.ValueType };
                    break;
                case ContainerType.Array:
                    methodGenericParameters = new[]
                        { typeof(IList<int>).GetGenericTypeDefinition().MakeGenericType(field.ValueType) };
                    break;
                //map
                default:
                    methodGenericParameters = new[]
                    {
                        typeof(IDictionary<int, int>).GetGenericTypeDefinition()
                            .MakeGenericType(field.KeyType, field.ValueType)
                    };
                    break;
            }

            if (fieldUnit.Empty())
            {
                methodOverloadSelectionParams = new[] { typeof(Field) };
                invokeParams = new object[] { field };
            }
            else
            {
                methodOverloadSelectionParams = new[] { typeof(Field), typeof(ForgeTypeId) };
                invokeParams = new object[] { field, UnitTypeId.Meters };
            }

            var instantiatedGenericGetMethod = entity.GetType().GetMethod("Get", methodOverloadSelectionParams)
                .MakeGenericMethod(methodGenericParameters);
            switch (field.ContainerType)
            {
                case ContainerType.Simple:
                {
                    var retval = (TFieldType)instantiatedGenericGetMethod.Invoke(entity, invokeParams);
                    if (fieldType == typeof(Entity))
                    {
                        var subSchema = Schema.Lookup(field.SubSchemaGUID);
                        strBuilder.AppendLine(
                            $"Field: {field.FieldName}, Type: {field.ValueType}, Value:  {{SubEntity}} , Unit: {field.GetSpecTypeId().TypeId}, ContainerType: {field.ContainerType}");
                        DumpAllSchemaEntityData(retval, subSchema, strBuilder);
                    }
                    else
                    {
                        strBuilder.AppendLine(
                            $"Field: {field.FieldName}, Type: {field.ValueType}, Value: {retval}, Unit: {field.GetSpecTypeId().TypeId}, ContainerType: {field.ContainerType}");
                    }

                    break;
                }
                case ContainerType.Array:
                {
                    var listRetval = (IList<TFieldType>)instantiatedGenericGetMethod.Invoke(entity, invokeParams);
                    if (fieldType == typeof(Entity))
                    {
                        strBuilder.AppendLine(
                            $"Field: {field.FieldName}, Type: {field.ValueType}, Value:  {{SubEntity}} , Unit: {field.GetSpecTypeId().TypeId}, ContainerType: {field.ContainerType}");

                        foreach (var fa in listRetval)
                        {
                            strBuilder.Append("  Array Value: ");
                            DumpAllSchemaEntityData(fa, Schema.Lookup(field.SubSchemaGUID), strBuilder);
                        }
                    }
                    else
                    {
                        strBuilder.AppendLine(
                            $"Field: {field.FieldName}, Type: {field.ValueType}, Value:  {{Array}} , Unit: {field.GetSpecTypeId().TypeId}, ContainerType: {field.ContainerType}");
                        foreach (var fa in listRetval)
                        {
                            strBuilder.AppendLine($"  Array value: {fa}");
                        }
                    }

                    break;
                }
                //Map
                default:
                {
                    strBuilder.AppendLine(
                        $"Field: {field.FieldName}, Type: {field.ValueType}, Value:  {{Map}} , Unit: {field.GetSpecTypeId().TypeId}, ContainerType: {field.ContainerType}");
                    var mapRetval =
                        (IDictionary<TKeyType, TFieldType>)instantiatedGenericGetMethod.Invoke(entity, invokeParams);
                    if (fieldType == typeof(Entity))
                    {
                        strBuilder.AppendLine(
                            $"Field: {field.FieldName}, Type: {field.ValueType}, Value:  {{SubEntity}} , Unit: {field.GetSpecTypeId().TypeId}, ContainerType: {field.ContainerType}");
                        foreach (var fa in mapRetval.Values)
                        {
                            strBuilder.Append("  Map Value: ");
                            DumpAllSchemaEntityData(fa, Schema.Lookup(field.SubSchemaGUID), strBuilder);
                        }
                    }
                    else
                    {
                        strBuilder.AppendLine(
                            $"Field: {field.FieldName}, Type: {field.ValueType}, Value:  {{Map}} , Unit: {field.GetSpecTypeId().TypeId}, ContainerType: {field.ContainerType}");
                        foreach (var fa in mapRetval.Values)
                        {
                            strBuilder.AppendLine($"  Map value: {fa}");
                        }
                    }

                    break;
                }
            }
        }

        private void SetRevitAssembly()
        {
            m_assembly = Assembly.GetAssembly(typeof(XYZ));
        }

        public Schema GetSchema()
        {
            return m_schema;
        }

        public void SetSchema(Schema schema)
        {
            m_schema = schema;
        }

        public string GetXmlPath()
        {
            return m_xmlPath;
        }

        public void SetXmlPath(string path)
        {
            m_xmlPath = path;
        }
    }
}

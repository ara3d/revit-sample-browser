// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using System;
using System.Collections.Generic;
using Ara3D.RevitSampleBrowser.ExtensibleStorageManager.SchemaWrapperTools.CS;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.ExtensibleStorage;

namespace Ara3D.RevitSampleBrowser.ExtensibleStorageManager.ExtensibleStorageManager.CS.User
{
    /// <summary>
    ///     An enum to select which sample schema to create.
    /// </summary>
    public enum SampleSchemaComplexity
    {
        SimpleExample = 1,
        ComplexExample = 2
    }

    /// <summary>
    ///     A static class that issues sample commands to a SchemaWrapper to demonstrate
    ///     schema and data storage.
    /// </summary>
    public static class StorageCommand
    {
        //A counter field used to assist in creating pseudorandom Guids
        private static int _sCounter = DateTime.Now.Second;

        //Field names and schema guids used in sample schemas
        private static readonly string Int0Name = "int0Name";
        private static readonly string Double0Name = "double0Name";
        private static readonly string Bool0Name = "bool0Name";
        private static readonly string String0Name = "string0Name";
        private static readonly string Id0Name = "id0Name";
        private static readonly string Point0Name = "point0Name";
        private static readonly string Uv0Name = "uv0Name";
        private static readonly string Float0Name = "float0Name";
        private static readonly string Short0Name = "short0Name";
        private static readonly string Guid0Name = "guid0Name";
        private static readonly string Map0Name = "map0Name";
        private static readonly string Array0Name = "array0Name";

        private static readonly Guid SubEntityGuid0 = NewGuid();
        private static readonly string Entity0Name = "entity0Name";

        private static readonly Guid SubEntityGuidMap1 = NewGuid();
        private static readonly string Entity1NameMap = "entity1Name_Map";

        private static readonly Guid SubEntityGuidArray2 = NewGuid();
        private static readonly string Entity2NameArray = "entity2Name_Array";

        private static readonly string Array1Name = Entity2NameArray;
        private static readonly string Map1Name = Entity1NameMap;

        /// <summary>
        ///     Creates a new sample Schema, creates an instance of that Schema (an Entity) in the given element,
        ///     sets data on that element's entity, and exports the schema to a given XML file.
        /// </summary>
        /// <returns>A new SchemaWrapper</returns>
        public static SchemaWrapper CreateSetAndExport(Element storageElement, string xmlPathOut, Guid schemaId,
            AccessLevel readAccess, AccessLevel writeAccess, string vendorId, string applicationId, string name,
            string documentation, SampleSchemaComplexity schemaComplexity)
        {
            if (Schema.Lookup(schemaId) != null)
                throw new Exception(
                    "A Schema with this Guid already exists in this document -- another one cannot be created.");
            var storageWrite = new Transaction(storageElement.Document, "storageWrite");
            storageWrite.Start();

            //Create a new schema.
            var mySchemaWrapper = SchemaWrapper.NewSchema(schemaId, readAccess, writeAccess, vendorId, applicationId,
                name, documentation);
            mySchemaWrapper.SetXmlPath(xmlPathOut);

            Entity storageElementEntityWrite = null;

            //Create some sample schema fields.  There are two sample schemas hard coded here, "simple" and "complex."
            switch (schemaComplexity)
            {
                case SampleSchemaComplexity.SimpleExample:
                    SimpleSchemaAndData(mySchemaWrapper, out storageElementEntityWrite);
                    break;
                case SampleSchemaComplexity.ComplexExample:
                    ComplexSchemaAndData(mySchemaWrapper, storageElement, xmlPathOut, schemaId, readAccess, writeAccess,
                        vendorId, applicationId, name, documentation, out storageElementEntityWrite);
                    break;
            }

            storageElement.SetEntity(storageElementEntityWrite);
            var storageResult = storageWrite.Commit();
            if (storageResult != TransactionStatus.Committed)
            {
                throw new Exception("Error storing Schema.  Transaction status: " + storageResult);
            }

            mySchemaWrapper.ToXml(xmlPathOut);
            return mySchemaWrapper;
        }

        /// <summary>
        ///     Adds several small, simple fields to a SchemaWrapper and Entity
        /// </summary>
        private static void SimpleSchemaAndData(SchemaWrapper mySchemaWrapper, out Entity storageElementEntityWrite)
        {
            //Create some sample fields.
            mySchemaWrapper.AddField<int>(Int0Name, new ForgeTypeId(), null);
            mySchemaWrapper.AddField<short>(Short0Name, new ForgeTypeId(), null);
            mySchemaWrapper.AddField<double>(Double0Name, SpecTypeId.Length, null);
            mySchemaWrapper.AddField<float>(Float0Name, SpecTypeId.Length, null);
            mySchemaWrapper.AddField<bool>(Bool0Name, new ForgeTypeId(), null);
            mySchemaWrapper.AddField<string>(String0Name, new ForgeTypeId(), null);

            //Generate the Autodesk.Revit.DB.ExtensibleStorage.Schema.
            mySchemaWrapper.FinishSchema();

            //Get the  fields
            var fieldInt0 = mySchemaWrapper.GetSchema().GetField(Int0Name);
            var fieldShort0 = mySchemaWrapper.GetSchema().GetField(Short0Name);
            var fieldDouble0 = mySchemaWrapper.GetSchema().GetField(Double0Name);
            var fieldFloat0 = mySchemaWrapper.GetSchema().GetField(Float0Name);
            var fieldBool0 = mySchemaWrapper.GetSchema().GetField(Bool0Name);
            var fieldString0 = mySchemaWrapper.GetSchema().GetField(String0Name);

            storageElementEntityWrite = null;
            //Create a new entity of the given Schema
            storageElementEntityWrite = new Entity(mySchemaWrapper.GetSchema());

            //Set data in the entity.
            storageElementEntityWrite.Set(fieldInt0, 5);
            storageElementEntityWrite.Set<short>(fieldShort0, 2);
            storageElementEntityWrite.Set(fieldDouble0, 7.1, UnitTypeId.Meters);
            storageElementEntityWrite.Set(fieldFloat0, 3.1f, UnitTypeId.Meters);
            storageElementEntityWrite.Set(fieldBool0, false);
            storageElementEntityWrite.Set(fieldString0, "hello");
        }

        /// <summary>
        ///     Adds a simple fields, arrays, maps, subEntities, and arrays and maps of subEntities to a SchemaWrapper and Entity
        /// </summary>
        private static void ComplexSchemaAndData(SchemaWrapper mySchemaWrapper, Element storageElement,
            string xmlPathOut, Guid schemaId, AccessLevel readAccess, AccessLevel writeAccess, string vendorId,
            string applicationId, string name, string documentation, out Entity storageElementEntityWrite)
        {
            mySchemaWrapper.AddField<int>(Int0Name, new ForgeTypeId(), null);
            mySchemaWrapper.AddField<short>(Short0Name, new ForgeTypeId(), null);
            mySchemaWrapper.AddField<double>(Double0Name, SpecTypeId.Length, null);
            mySchemaWrapper.AddField<float>(Float0Name, SpecTypeId.Length, null);
            mySchemaWrapper.AddField<bool>(Bool0Name, new ForgeTypeId(), null);
            mySchemaWrapper.AddField<string>(String0Name, new ForgeTypeId(), null);
            mySchemaWrapper.AddField<ElementId>(Id0Name, new ForgeTypeId(), null);
            mySchemaWrapper.AddField<XYZ>(Point0Name, SpecTypeId.Length, null);
            mySchemaWrapper.AddField<UV>(Uv0Name, SpecTypeId.Length, null);
            mySchemaWrapper.AddField<Guid>(Guid0Name, new ForgeTypeId(), null);

            //Note that we use IDictionary<> for map types and IList<> for array types
            mySchemaWrapper.AddField<IDictionary<string, string>>(Map0Name, new ForgeTypeId(), null);
            mySchemaWrapper.AddField<IList<bool>>(Array0Name, new ForgeTypeId(), null);

            //Create a sample subEntity
            var mySubSchemaWrapper0 = SchemaWrapper.NewSchema(SubEntityGuid0, readAccess, writeAccess, vendorId,
                applicationId, Entity0Name, "A sub entity");
            mySubSchemaWrapper0.AddField<int>("subInt0", new ForgeTypeId(), null);
            mySubSchemaWrapper0.FinishSchema();
            var subEnt0 = new Entity(mySubSchemaWrapper0.GetSchema());
            subEnt0.Set(mySubSchemaWrapper0.GetSchema().GetField("subInt0"), 11);
            mySchemaWrapper.AddField<Entity>(Entity0Name, new ForgeTypeId(), mySubSchemaWrapper0);

            //
            //Create a sample map of subEntities (An IDictionary<> with key type "int" and value type "Entity")
            //
            //Create a new sample schema.
            var mySubSchemaWrapper1Map = SchemaWrapper.NewSchema(SubEntityGuidMap1, readAccess, writeAccess, vendorId,
                applicationId, Map1Name, "A map of int to Entity");
            mySubSchemaWrapper1Map.AddField<int>("subInt1", new ForgeTypeId(), null);
            mySubSchemaWrapper1Map.FinishSchema();
            //Create a new sample Entity.
            var subEnt1 = new Entity(mySubSchemaWrapper1Map.GetSchema());
            //Set data in that entity.
            subEnt1.Set(mySubSchemaWrapper1Map.GetSchema().GetField("subInt1"), 22);
            //Add a new map field to the top-level Schema.  We will add the entity we just created after all top-level
            //fields are created.
            mySchemaWrapper.AddField<IDictionary<int, Entity>>(Map1Name, new ForgeTypeId(), mySubSchemaWrapper1Map);

            //
            //Create a sample array of subentities (An IList<> of type "Entity")
            //
            //Create a new sample schema
            var mySubSchemaWrapper2Array = SchemaWrapper.NewSchema(SubEntityGuidArray2, readAccess, writeAccess,
                vendorId, applicationId, Array1Name, "An array of Entities");
            mySubSchemaWrapper2Array.AddField<int>("subInt2", new ForgeTypeId(), null);
            mySubSchemaWrapper2Array.FinishSchema();
            //Create a new sample Entity.
            var subEnt2 = new Entity(mySubSchemaWrapper2Array.GetSchema());
            //Set the data in that Entity.
            subEnt2.Set(mySubSchemaWrapper2Array.GetSchema().GetField("subInt2"), 33);
            //Add a new array field to the top-level Schema We will add the entity we just crated after all top-level fields
            //are created.
            mySchemaWrapper.AddField<IList<Entity>>(Array1Name, new ForgeTypeId(), mySubSchemaWrapper2Array);

            mySchemaWrapper.FinishSchema();

            storageElementEntityWrite = null;

            storageElementEntityWrite = new Entity(mySchemaWrapper.GetSchema());

            var fieldInt0 = mySchemaWrapper.GetSchema().GetField(Int0Name);
            var fieldShort0 = mySchemaWrapper.GetSchema().GetField(Short0Name);
            var fieldDouble0 = mySchemaWrapper.GetSchema().GetField(Double0Name);
            var fieldFloat0 = mySchemaWrapper.GetSchema().GetField(Float0Name);

            var fieldBool0 = mySchemaWrapper.GetSchema().GetField(Bool0Name);
            var fieldString0 = mySchemaWrapper.GetSchema().GetField(String0Name);

            var fieldId0 = mySchemaWrapper.GetSchema().GetField(Id0Name);
            var fieldPoint0 = mySchemaWrapper.GetSchema().GetField(Point0Name);
            var fieldUv0 = mySchemaWrapper.GetSchema().GetField(Uv0Name);
            var fieldGuid0 = mySchemaWrapper.GetSchema().GetField(Guid0Name);

            var fieldMap0 = mySchemaWrapper.GetSchema().GetField(Map0Name);
            var fieldArray0 = mySchemaWrapper.GetSchema().GetField(Array0Name);

            var fieldEntity0 = mySchemaWrapper.GetSchema().GetField(Entity0Name);

            var fieldMap1 = mySchemaWrapper.GetSchema().GetField(Map1Name);
            var fieldArray1 = mySchemaWrapper.GetSchema().GetField(Array1Name);

            storageElementEntityWrite.Set(fieldInt0, 5);
            storageElementEntityWrite.Set<short>(fieldShort0, 2);

            storageElementEntityWrite.Set(fieldDouble0, 7.1, UnitTypeId.Meters);
            storageElementEntityWrite.Set(fieldFloat0, 3.1f, UnitTypeId.Meters);

            storageElementEntityWrite.Set(fieldBool0, false);
            storageElementEntityWrite.Set(fieldString0, "hello");
            storageElementEntityWrite.Set(fieldId0, storageElement.Id);
            storageElementEntityWrite.Set(fieldPoint0, new XYZ(1, 2, 3), UnitTypeId.Meters);
            storageElementEntityWrite.Set(fieldUv0, new UV(1, 2), UnitTypeId.Meters);
            storageElementEntityWrite.Set(fieldGuid0, new Guid("D8301329-F207-43B8-8AA1-634FD344F350"));

            //Note that we must pass an IDictionary<>, not a Dictionary<> to Set().
            IDictionary<string, string> myMap0 = new Dictionary<string, string>
            {
                { "mykeystr", "myvalstr" }
            };
            storageElementEntityWrite.Set(fieldMap0, myMap0);

            //Note that we must pass an IList<>, not a List<> to Set().
            IList<bool> myBoolArrayList0 = new List<bool>
            {
                true,
                false
            };
            storageElementEntityWrite.Set(fieldArray0, myBoolArrayList0);
            storageElementEntityWrite.Set(fieldEntity0, subEnt0);

            //Create a map of Entities
            IDictionary<int, Entity> myMap1 = new Dictionary<int, Entity>
            {
                { 5, subEnt1 }
            };
            //Set the map of Entities.
            storageElementEntityWrite.Set(fieldMap1, myMap1);

            //Create a list of entities
            IList<Entity> myEntArrayList1 = new List<Entity>
            {
                subEnt2,
                subEnt2
            };
            //Set the list of entities.
            storageElementEntityWrite.Set(fieldArray1, myEntArrayList1);
        }

        /// <summary>
        ///     Given an Autodesk.Revit.DB.ExtensibleStorage.Schema that already exists,
        ///     create a SchemaWrapper containing that Schema's data.
        /// </summary>
        /// <param name="schemaId">The Guid of the existing Schema</param>
        public static void CreateWrapperFromSchema(Guid schemaId, out SchemaWrapper schemaWrapper)
        {
            var toLookup = Schema.Lookup(schemaId);
            if (toLookup == null)
                throw new Exception("Schema not found in current document.");
            schemaWrapper = SchemaWrapper.FromSchema(toLookup);
        }

        /// <summary>
        ///     Create a SchemaWrapper from a Schema Guid and try to find an Entity of a matching Guid
        ///     in a given Element.  If successfull, try to change the data in that Entity.
        /// </summary>
        /// <param name="storageElement"></param>
        /// <param name="schemaId"></param>
        /// <param name="schemaWrapper"></param>
        public static void EditExistingData(Element storageElement, Guid schemaId, out SchemaWrapper schemaWrapper)
        {
            //Try to find the schema in the active document.
            var schemaLookup = Schema.Lookup(schemaId);
            if (schemaLookup == null) throw new Exception("Schema not found in current document.");

            //Create a SchemaWrapper.
            schemaWrapper = SchemaWrapper.FromSchema(schemaLookup);

            //Try to get an Entity of the given Schema
            var storageElementEntityWrite = storageElement.GetEntity(schemaLookup);
            if (storageElementEntityWrite.SchemaGUID != schemaId)
                throw new Exception("SchemaID of found entity does not match the SchemaID passed to GetEntity.");

            if (storageElementEntityWrite == null) throw new Exception("Entity of given Schema not found.");

            //Get the fields of the schema
            var fieldInt0 = schemaWrapper.GetSchema().GetField(Int0Name);
            var fieldShort0 = schemaWrapper.GetSchema().GetField(Short0Name);
            var fieldDouble0 = schemaWrapper.GetSchema().GetField(Double0Name);
            var fieldFloat0 = schemaWrapper.GetSchema().GetField(Float0Name);
            var fieldBool0 = schemaWrapper.GetSchema().GetField(Bool0Name);
            var fieldString0 = schemaWrapper.GetSchema().GetField(String0Name);

            //Edit the fields.
            var tStore = new Transaction(storageElement.Document, "tStore");
            tStore.Start();
            storageElementEntityWrite = null;
            storageElementEntityWrite = new Entity(schemaWrapper.GetSchema());

            storageElementEntityWrite.Set(fieldInt0, 10);
            storageElementEntityWrite.Set<short>(fieldShort0, 20);
            storageElementEntityWrite.Set(fieldDouble0, 14.2, UnitTypeId.Meters);
            storageElementEntityWrite.Set(fieldFloat0, 6.12f, UnitTypeId.Meters);
            storageElementEntityWrite.Set(fieldBool0, true);
            storageElementEntityWrite.Set(fieldString0, "goodbye");
            //Set the entity back into the storage element.
            storageElement.SetEntity(storageElementEntityWrite);
            tStore.Commit();
        }

        /// <summary>
        ///     Given an element, try to find an entity containing instance data from a given Schema Guid.
        /// </summary>
        /// <param name="storageElement">The element to query</param>
        /// <param name="schemaId">The id of the Schema to query</param>
        public static void LookupAndExtractData(Element storageElement, Guid schemaId, out SchemaWrapper schemaWrapper)
        {
            var schemaLookup = Schema.Lookup(schemaId);
            if (schemaLookup == null) throw new Exception("Schema not found in current document.");
            schemaWrapper = SchemaWrapper.FromSchema(schemaLookup);

            var storageElementEntityRead = storageElement.GetEntity(schemaLookup);
            if (storageElementEntityRead.SchemaGUID != schemaId)
                throw new Exception("SchemaID of found entity does not match the SchemaID passed to GetEntity.");

            if (storageElementEntityRead == null) throw new Exception("Entity of given Schema not found.");
        }

        /// <summary>
        ///     Given an xml path containing serialized schema data, create a new Schema and SchemaWrapper
        /// </summary>
        public static void ImportSchemaFromXml(string path, out SchemaWrapper sWrapper)
        {
            sWrapper = SchemaWrapper.FromXml(path);
            sWrapper.SetXmlPath(path);
        }

        /// <summary>
        ///     Create a new pseudorandom Guid
        /// </summary>
        /// <returns></returns>
        public static Guid NewGuid()
        {
            var guidBytes = new byte[16];
            var randomGuidBytes = new Random(_sCounter);
            randomGuidBytes.NextBytes(guidBytes);
            _sCounter++;
            return new Guid(guidBytes);
        }
    }
}

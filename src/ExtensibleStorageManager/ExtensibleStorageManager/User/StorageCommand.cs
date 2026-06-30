// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using Ara3D.RevitSampleBrowser.Common.Infrastructure;
using Ara3D.RevitSampleBrowser.ExtensibleStorageManager.SchemaWrapperTools.CS;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.ExtensibleStorage;
using System;
using System.Collections.Generic;
namespace Ara3D.RevitSampleBrowser.ExtensibleStorageManager.ExtensibleStorageManager.CS.User
{
    public enum SampleSchemaComplexity
    {
        SimpleExample = 1,
        ComplexExample = 2
    }

    public static class StorageCommand
    {
        // Field names and schema guids used in sample schemas.
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

        private static readonly Guid SubEntityGuid0 = ExtensibleStorageHelper.NewGuid();
        private static readonly string Entity0Name = "entity0Name";

        private static readonly Guid SubEntityGuidMap1 = ExtensibleStorageHelper.NewGuid();
        private static readonly string Entity1NameMap = "entity1Name_Map";

        private static readonly Guid SubEntityGuidArray2 = ExtensibleStorageHelper.NewGuid();
        private static readonly string Entity2NameArray = "entity2Name_Array";

        private static readonly string Array1Name = Entity2NameArray;
        private static readonly string Map1Name = Entity1NameMap;

        public static SchemaWrapper CreateSetAndExport(Element storageElement, string xmlPathOut, Guid schemaId,
            AccessLevel readAccess, AccessLevel writeAccess, string vendorId, string applicationId, string name,
            string documentation, SampleSchemaComplexity schemaComplexity)
        {
            if (Schema.Lookup(schemaId) != null)
                throw new Exception(
                    "A Schema with this Guid already exists in this document -- another one cannot be created.");
            Transaction storageWrite = new(storageElement.Document, "storageWrite");
            storageWrite.Start();

            var mySchemaWrapper = SchemaWrapper.NewSchema(schemaId, readAccess, writeAccess, vendorId, applicationId,
                name, documentation);
            mySchemaWrapper.SetXmlPath(xmlPathOut);

            Entity storageElementEntityWrite = null;

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
                throw new Exception($"Error storing Schema.  Transaction status: {storageResult}");
            }

            mySchemaWrapper.ToXml(xmlPathOut);
            return mySchemaWrapper;
        }

        private static void SimpleSchemaAndData(SchemaWrapper mySchemaWrapper, out Entity storageElementEntityWrite)
        {
            mySchemaWrapper.AddField<int>(Int0Name, new ForgeTypeId(), null);
            mySchemaWrapper.AddField<short>(Short0Name, new ForgeTypeId(), null);
            mySchemaWrapper.AddField<double>(Double0Name, SpecTypeId.Length, null);
            mySchemaWrapper.AddField<float>(Float0Name, SpecTypeId.Length, null);
            mySchemaWrapper.AddField<bool>(Bool0Name, new ForgeTypeId(), null);
            mySchemaWrapper.AddField<string>(String0Name, new ForgeTypeId(), null);

            mySchemaWrapper.FinishSchema();

            var fieldInt0 = mySchemaWrapper.GetSchema().GetField(Int0Name);
            var fieldShort0 = mySchemaWrapper.GetSchema().GetField(Short0Name);
            var fieldDouble0 = mySchemaWrapper.GetSchema().GetField(Double0Name);
            var fieldFloat0 = mySchemaWrapper.GetSchema().GetField(Float0Name);
            var fieldBool0 = mySchemaWrapper.GetSchema().GetField(Bool0Name);
            var fieldString0 = mySchemaWrapper.GetSchema().GetField(String0Name);

            storageElementEntityWrite = null;
            storageElementEntityWrite = new Entity(mySchemaWrapper.GetSchema());

            storageElementEntityWrite.Set(fieldInt0, 5);
            storageElementEntityWrite.Set<short>(fieldShort0, 2);
            storageElementEntityWrite.Set(fieldDouble0, 7.1, UnitTypeId.Meters);
            storageElementEntityWrite.Set(fieldFloat0, 3.1f, UnitTypeId.Meters);
            storageElementEntityWrite.Set(fieldBool0, false);
            storageElementEntityWrite.Set(fieldString0, "hello");
        }

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

            // Use IDictionary<> for maps and IList<> for arrays.
            mySchemaWrapper.AddField<IDictionary<string, string>>(Map0Name, new ForgeTypeId(), null);
            mySchemaWrapper.AddField<IList<bool>>(Array0Name, new ForgeTypeId(), null);

            var mySubSchemaWrapper0 = SchemaWrapper.NewSchema(SubEntityGuid0, readAccess, writeAccess, vendorId,
                applicationId, Entity0Name, "A sub entity");
            mySubSchemaWrapper0.AddField<int>("subInt0", new ForgeTypeId(), null);
            mySubSchemaWrapper0.FinishSchema();
            Entity subEnt0 = new(mySubSchemaWrapper0.GetSchema());
            subEnt0.Set(mySubSchemaWrapper0.GetSchema().GetField("subInt0"), 11);
            mySchemaWrapper.AddField<Entity>(Entity0Name, new ForgeTypeId(), mySubSchemaWrapper0);

            var mySubSchemaWrapper1Map = SchemaWrapper.NewSchema(SubEntityGuidMap1, readAccess, writeAccess, vendorId,
                applicationId, Map1Name, "A map of int to Entity");
            mySubSchemaWrapper1Map.AddField<int>("subInt1", new ForgeTypeId(), null);
            mySubSchemaWrapper1Map.FinishSchema();
            Entity subEnt1 = new(mySubSchemaWrapper1Map.GetSchema());
            subEnt1.Set(mySubSchemaWrapper1Map.GetSchema().GetField("subInt1"), 22);
            mySchemaWrapper.AddField<IDictionary<int, Entity>>(Map1Name, new ForgeTypeId(), mySubSchemaWrapper1Map);

            var mySubSchemaWrapper2Array = SchemaWrapper.NewSchema(SubEntityGuidArray2, readAccess, writeAccess,
                vendorId, applicationId, Array1Name, "An array of Entities");
            mySubSchemaWrapper2Array.AddField<int>("subInt2", new ForgeTypeId(), null);
            mySubSchemaWrapper2Array.FinishSchema();
            Entity subEnt2 = new(mySubSchemaWrapper2Array.GetSchema());
            subEnt2.Set(mySubSchemaWrapper2Array.GetSchema().GetField("subInt2"), 33);
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

            // Entity.Set requires IDictionary<>, not Dictionary<>.
            IDictionary<string, string> myMap0 = new Dictionary<string, string>
            {
                { "mykeystr", "myvalstr" }
            };
            storageElementEntityWrite.Set(fieldMap0, myMap0);

            // Entity.Set requires IList<>, not List<>.
            IList<bool> myBoolArrayList0 =
            [
                true,
                false
            ];
            storageElementEntityWrite.Set(fieldArray0, myBoolArrayList0);
            storageElementEntityWrite.Set(fieldEntity0, subEnt0);

            IDictionary<int, Entity> myMap1 = new Dictionary<int, Entity>
            {
                { 5, subEnt1 }
            };
            storageElementEntityWrite.Set(fieldMap1, myMap1);

            IList<Entity> myEntArrayList1 =
            [
                subEnt2,
                subEnt2
            ];
            storageElementEntityWrite.Set(fieldArray1, myEntArrayList1);
        }

        public static void CreateWrapperFromSchema(Guid schemaId, out SchemaWrapper schemaWrapper)
        {
            var toLookup = Schema.Lookup(schemaId);
            if (toLookup == null)
                throw new Exception("Schema not found in current document.");
            schemaWrapper = SchemaWrapper.FromSchema(toLookup);
        }

        public static void EditExistingData(Element storageElement, Guid schemaId, out SchemaWrapper schemaWrapper)
        {
            var schemaLookup = Schema.Lookup(schemaId);
            if (schemaLookup == null) throw new Exception("Schema not found in current document.");

            schemaWrapper = SchemaWrapper.FromSchema(schemaLookup);

            var storageElementEntityWrite = storageElement.GetEntity(schemaLookup);
            if (storageElementEntityWrite.SchemaGUID != schemaId)
                throw new Exception("SchemaID of found entity does not match the SchemaID passed to GetEntity.");

            if (storageElementEntityWrite == null) throw new Exception("Entity of given Schema not found.");

            var fieldInt0 = schemaWrapper.GetSchema().GetField(Int0Name);
            var fieldShort0 = schemaWrapper.GetSchema().GetField(Short0Name);
            var fieldDouble0 = schemaWrapper.GetSchema().GetField(Double0Name);
            var fieldFloat0 = schemaWrapper.GetSchema().GetField(Float0Name);
            var fieldBool0 = schemaWrapper.GetSchema().GetField(Bool0Name);
            var fieldString0 = schemaWrapper.GetSchema().GetField(String0Name);

            Transaction tStore = new(storageElement.Document, "tStore");
            tStore.Start();
            storageElementEntityWrite = null;
            storageElementEntityWrite = new Entity(schemaWrapper.GetSchema());

            storageElementEntityWrite.Set(fieldInt0, 10);
            storageElementEntityWrite.Set<short>(fieldShort0, 20);
            storageElementEntityWrite.Set(fieldDouble0, 14.2, UnitTypeId.Meters);
            storageElementEntityWrite.Set(fieldFloat0, 6.12f, UnitTypeId.Meters);
            storageElementEntityWrite.Set(fieldBool0, true);
            storageElementEntityWrite.Set(fieldString0, "goodbye");
            storageElement.SetEntity(storageElementEntityWrite);
            tStore.Commit();
        }

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

        public static void ImportSchemaFromXml(string path, out SchemaWrapper sWrapper)
        {
            sWrapper = SchemaWrapper.FromXml(path);
            sWrapper.SetXmlPath(path);
        }

    }
}

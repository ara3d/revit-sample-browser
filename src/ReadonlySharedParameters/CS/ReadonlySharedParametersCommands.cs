// Copyright 2023. See https://github.com/ara3d/revit-samples/LICENSE.txt

using System;
using System.Collections.Generic;
using System.IO;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace RevitMultiSample.ReadonlySharedParameters.CS
{
    [Transaction(TransactionMode.Manual)]
    internal class SetReadonlyCost1 : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            var doc = commandData.View.Document;

            ReadonlyCostSetter.SetReadonlyCosts1(doc);

            return Result.Succeeded;
        }
    }

    [Transaction(TransactionMode.Manual)]
    internal class SetReadonlyCost2 : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            var doc = commandData.View.Document;

            ReadonlyCostSetter.SetReadonlyCosts2(doc);

            return Result.Succeeded;
        }
    }

    internal class ReadonlyCostSetter
    {
        public static void SetReadonlyCosts1(Document doc)
        {
            SetReadonlyCosts(doc, GetReadonlyCostFromId);
        }

        public static void SetReadonlyCosts2(Document doc)
        {
            SetReadonlyCosts(doc, GetReadonlyCostFromIncrements);
        }

        private static double GetReadonlyCostFromId(Element elem, int seed)
        {
            var costRoot = elem.Id.Value % 100;
            return costRoot * 100.0 + 0.99;
        }

        private static double GetReadonlyCostFromIncrements(Element elem, int seed)
        {
            return seed * 100.0 + 0.88;
        }

        private static void SetReadonlyCosts(Document doc, Func<Element, int, double> valueGetter)
        {
            var collector = new FilteredElementCollector(doc);
            collector.WhereElementIsElementType();
            var rule = ParameterFilterRuleFactory.CreateSharedParameterApplicableRule("ReadonlyCost");
            var filter = new ElementParameterFilter(rule);
            collector.WherePasses(filter);

            var increment = 1;

            using (var t = new Transaction(doc, "Apply ReadonlyCost"))
            {
                t.Start();
                foreach (var elem in collector)
                {
                    var p = elem.LookupParameter("ReadonlyCost");
                    p?.Set(valueGetter(elem, increment));
                    increment++;
                }

                t.Commit();
            }
        }
    }

    [Transaction(TransactionMode.Manual)]
    internal class SetReadonlyId1 : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            var doc = commandData.View.Document;

            ReadonlyIdSetter.SetReadonlyIds1(doc);

            return Result.Succeeded;
        }
    }

    [Transaction(TransactionMode.Manual)]
    internal class SetReadonlyId2 : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            var doc = commandData.View.Document;

            ReadonlyIdSetter.SetReadonlyIds2(doc);

            return Result.Succeeded;
        }
    }

    internal class ReadonlyIdSetter
    {
        private static string GetReadonlyIdUniqueId(Element elem)
        {
            return elem.UniqueId;
        }

        private static string GetReadonlyIdFromElementId(Element elem)
        {
            var eType = elem.Document.GetElement(elem.GetTypeId());

            return eType.Name.Substring(0, 2) + elem.Id;
        }

        public static void SetReadonlyIds1(Document doc)
        {
            SetReadonlyIds(doc, GetReadonlyIdUniqueId);
        }

        public static void SetReadonlyIds2(Document doc)
        {
            SetReadonlyIds(doc, GetReadonlyIdFromElementId);
        }

        private static void SetReadonlyIds(Document doc, Func<Element, string> idGetter)
        {
            var collector = new FilteredElementCollector(doc);
            var rule = ParameterFilterRuleFactory.CreateSharedParameterApplicableRule("ReadonlyId");
            var filter = new ElementParameterFilter(rule);
            collector.WherePasses(filter);

            using (var t = new Transaction(doc, "Apply ReadonlyId"))
            {
                t.Start();
                foreach (var elem in collector)
                {
                    var p = elem.LookupParameter("ReadonlyId");
                    p?.Set(idGetter(elem));
                }

                t.Commit();
            }
        }
    }

    [Transaction(TransactionMode.Manual)]
    internal class BindNewReadonlySharedParametersToDocument : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            var doc = commandData.View.Document;

            AddSetOfSharedParameters(doc);

            return Result.Succeeded;
        }

        private List<SharedParameterBindingManager> BuildSharedParametersToCreate()
        {
            var sharedParametersToCreate =
                new List<SharedParameterBindingManager>();

            var manager = new SharedParameterBindingManager
            {
                Name = "ReadonlyId",
                Type = SpecTypeId.String.Text,
                UserModifiable = false,
                Description = "A read-only instance parameter used for coordination with external content.",
                Instance = true
            };
            manager.AddCategory(BuiltInCategory.OST_Walls);
            manager.AddCategory(BuiltInCategory.OST_Floors);
            manager.AddCategory(BuiltInCategory.OST_Ceilings);
            manager.AddCategory(BuiltInCategory.OST_Roofs);
            manager.ParameterGroup = GroupTypeId.IdentityData;

            sharedParametersToCreate.Add(manager); // Look up syntax for this automatic initialization.

            manager = new SharedParameterBindingManager
            {
                Name = "ReadonlyCost",
                Type = SpecTypeId.Currency,
                UserModifiable = false,
                Description = "A read-only type parameter used to list the cost of a type.",
                Instance = false
            };

            manager.AddCategory(BuiltInCategory.OST_Furniture);
            manager.AddCategory(BuiltInCategory.OST_Planting);
            manager.ParameterGroup = GroupTypeId.Materials;

            sharedParametersToCreate.Add(manager);

            return sharedParametersToCreate;
        }

        public void AddSetOfSharedParameters(Document doc)
        {
            var app = doc.Application;

            var filePath = GetRandomSharedParameterFileName();

            app.SharedParametersFilename = filePath;

            var dFile = app.OpenSharedParameterFile();
            var dGroup = dFile.Groups.Create("Demo group");
            var managers = BuildSharedParametersToCreate();
            using (var t = new Transaction(doc, "Bind parameters"))
            {
                t.Start();
                foreach (var manager in managers)
                {
                    manager.Definition = dGroup.Definitions.Create(manager.GetCreationOptions());
                    manager.AddBindings(doc);
                }

                t.Commit();
            }
        }

        private string GetRandomSharedParameterFileName()
        {
            var randomFileName = Path.GetRandomFileName();
            var spFile = Path.ChangeExtension(randomFileName, "txt");
            var filePath = Path.Combine(@"c:\tmp\Meridian\", spFile);
            var writer = File.CreateText(filePath);
            writer.Close();
            return filePath;
        }
    }
}

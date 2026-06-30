// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt


using Ara3D.RevitSampleBrowser.Common.Geometry;
using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Architecture;
using Autodesk.Revit.DB.Plumbing;
using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using Document = Autodesk.Revit.DB.Document;

namespace Ara3D.RevitSampleBrowser.Common.Parameters
{
    public static class ParameterAccess
    {
        private const double Tolerance = 0.0001d;
        public const double RadiansToDegrees = 180 / Math.PI;

        public static bool SetParaInt(Element elem, string paraName, int value)
        {
            var findPara = FindParaByName(elem.Parameters, paraName);
            if (findPara == null || findPara.IsReadOnly)
                return false;
            findPara.Set(value);
            return true;
        }

        public static bool SetParaInt(Element elem, BuiltInParameter paraIndex, int value)
        {
            var para = elem.get_Parameter(paraIndex);
            if (para == null || para.IsReadOnly)
                return false;
            para.Set(value);
            return true;
        }

        public static Parameter FindParaByName(ParameterSet paras, string name)
        {
            foreach (Parameter para in paras)
            {
                if (para.Definition.Name == name)
                    return para;
            }

            return null;
        }

        public static bool SetParaNullId(Parameter para)
        {
            if (para.IsReadOnly)
                return false;
            para.Set(ElementId.InvalidElementId);
            return true;
        }

        public static void CopyParameters(Pipe source, Pipe target)
        {
            var diameter = source.get_Parameter(BuiltInParameter.RBS_PIPE_DIAMETER_PARAM).AsDouble();
            target.get_Parameter(BuiltInParameter.RBS_PIPE_DIAMETER_PARAM).Set(diameter);
        }

        public static bool IsParallel(Face face, Line line)
        {
            var points = XyzMath.GetPoints(face);
            if (points.Count < 3) return false;
            var cross = XyzMath.CrossMatrix(XyzMath.SubXyz(points[0], points[1]), XyzMath.SubXyz(points[1], points[2]));
            return XyzMath.DotMatrix(cross, XyzMath.SubXyz(line.GetEndPoint(0), line.GetEndPoint(1))) < XyzMath.Precision;
        }

        public static Line GetXyParallelLine(Line inLine, double distance)
        {
            var direct = XyzMath.SubXyz(inLine.GetEndPoint(1), inLine.GetEndPoint(0));
            var length = Math.Sqrt((direct.Y * direct.Y) + (direct.X * direct.X));
            var temp = distance / length;
            var dPerp = new XYZ(-direct.Y * temp, direct.X * temp, 0.0);
            return Line.CreateBound(XyzMath.AddXyz(inLine.GetEndPoint(0), dPerp), XyzMath.AddXyz(inLine.GetEndPoint(1), dPerp));
        }

        public static bool IsLinesParallel(Vector4 vec4A, Vector4 vec4B)
        {
            var aa = vec4A.X * vec4B.Y;
            var bb = vec4A.Y * vec4B.X;
            var cc = vec4A.Y * vec4B.Z;
            var dd = vec4A.Z * vec4B.Y;
            var ee = vec4A.X * vec4B.Z;
            var ff = vec4A.Z * vec4B.X;
            return Math.Abs(aa - bb) < Tolerance && Math.Abs(cc - dd) < Tolerance && Math.Abs(ee - ff) < Tolerance;
        }

        public static string FormatParameterValue(Parameter parameter)
        {
            return parameter.StorageType switch
            {
                StorageType.Double => parameter.AsDouble().ToString(),
                StorageType.ElementId => parameter.AsElementId().ToString(),
                StorageType.String => parameter.AsString(),
                StorageType.Integer => parameter.AsInteger().ToString(),
                _ => ""
            };
        }

        public static void AddSharedParameters(UIApplication app)
        {
            var categories = app.Application.Create.NewCategorySet();
            var doorCategory = app.ActiveUIDocument.Document.Settings.Categories.get_Item(BuiltInCategory.OST_Doors);
            categories.Insert(doorCategory);

            var instanceBinding = app.Application.Create.NewInstanceBinding(categories);
            var typeBinding = app.Application.Create.NewTypeBinding(categories);
            var bindingMap = app.ActiveUIDocument.Document.ParameterBindings;

            var defFile = AccessOrCreateSharedParameterFile(app.Application);
            if (defFile == null) return;

            var defGroups = defFile.Groups;
            var defGroup = defGroups.get_Item("DoorProjectSharedParameters") ?? defGroups.Create("DoorProjectSharedParameters");
            var doc = app.ActiveUIDocument.Document;

            if (!AlreadyAddedSharedParameter(doc, "BasalOpening", BuiltInCategory.OST_Doors))
            {
                var basalOpening = defGroup.Definitions.get_Item("BasalOpening")
                                   ?? defGroup.Definitions.Create(new ExternalDefinitionCreationOptions("BasalOpening", SpecTypeId.String.Text));
                bindingMap.Insert(basalOpening, typeBinding, GroupTypeId.Geometry);
            }

            if (!AlreadyAddedSharedParameter(doc, "InstanceOpening", BuiltInCategory.OST_Doors))
            {
                var instanceOpening = defGroup.Definitions.get_Item("InstanceOpening")
                                      ?? defGroup.Definitions.Create(new ExternalDefinitionCreationOptions("InstanceOpening", SpecTypeId.String.Text));
                bindingMap.Insert(instanceOpening, instanceBinding, GroupTypeId.Geometry);
            }

            if (!AlreadyAddedSharedParameter(doc, "public Door", BuiltInCategory.OST_Doors))
            {
                var internalDoorFlag = defGroup.Definitions.get_Item("public Door")
                                       ?? defGroup.Definitions.Create(new ExternalDefinitionCreationOptions("public Door", SpecTypeId.Boolean.YesNo));
                bindingMap.Insert(internalDoorFlag, instanceBinding);
            }
        }

        private static DefinitionFile AccessOrCreateSharedParameterFile(Application app)
        {
            var sharedParameterFilePath = Path.Combine(
                Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) ?? "",
                "MySharedParameterFile.txt");

            if (!new FileInfo(sharedParameterFilePath).Exists)
            {
                using (File.Create(sharedParameterFilePath)) { }
            }

            app.SharedParametersFilename = sharedParameterFilePath;
            return app.OpenSharedParameterFile();
        }

        private static bool AlreadyAddedSharedParameter(Document doc, string paraName, BuiltInCategory boundCategory)
        {
            try
            {
                var bindingMapIter = doc.ParameterBindings.ForwardIterator();
                while (bindingMapIter.MoveNext())
                {
                    if (!bindingMapIter.Key.Name.Equals(paraName)) continue;
                    if (bindingMapIter.Current is ElementBinding binding)
                    {
                        foreach (Category category in binding.Categories)
                        {
                            if (category.BuiltInCategory == boundCategory) return true;
                        }
                    }
                }
            }
            catch (Exception)
            {
                return false;
            }

            return false;
        }

        public static DefinitionFile GetSharedParameterFile(Application revitApp)
        {
            DefinitionFile file = null;
            var count = 0;
            while (file == null && count < 100)
            {
                file = revitApp.OpenSharedParameterFile();
                if (file != null)
                    continue;

                var shapeFile =
                    $"{Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)}\\MultiplanarParameterFiles.txt";
                var contents = new StringBuilder();
                contents.AppendLine("# This is a Revit shared parameter file.");
                contents.AppendLine("# Do not edit manually.");
                contents.AppendLine("*META	VERSION	MINVERSION");
                contents.AppendLine("META	2	1");
                contents.AppendLine("*GROUP	ID	NAME");
                contents.AppendLine("*PARAM	GUID	NAME	DATATYPE	DATACATEGORY	GROUP	VISIBLE");
                File.WriteAllText(shapeFile, contents.ToString());
                revitApp.SharedParametersFilename = shapeFile;
                ++count;
            }

            return file;
        }

        public static bool ShareParameterExists(Document doc, string paramName)
        {
            var bindingMap = doc.ParameterBindings;
            var iter = bindingMap.ForwardIterator();
            iter.Reset();

            while (iter.MoveNext())
            {
                var tempDefinition = iter.Key;
                if (string.Compare(tempDefinition.Name, paramName) != 0)
                    continue;

                if (bindingMap.get_Item(tempDefinition) is ElementBinding binding)
                {
                    foreach (Category category in binding.Categories)
                    {
                        if (category.Name == doc.Settings.Categories.get_Item(BuiltInCategory.OST_Rebar).Name)
                            return true;
                    }
                }
            }

            return false;
        }

        public static bool ShareParameterExists(Room roomObj, string paramName, ref Parameter sharedParam)
        {
            sharedParam = roomObj.LookupParameter(paramName);
            return sharedParam != null;
        }

        public static bool AddSharedTestParameter(ExternalCommandData commandData, string paramName,
                    ForgeTypeId paramType, bool userModifiable)
        {
            try
            {
                var doc = commandData.Application.ActiveUIDocument.Document;
                if (ShareParameterExists(doc, paramName))
                    return true;

                var modulePath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
                var paramFile = $"{modulePath}\\RebarTestParameters.txt";
                if (File.Exists(paramFile))
                    File.Delete(paramFile);
                File.Create(paramFile).Close();

                var revitApp = commandData.Application.Application;
                revitApp.SharedParametersFilename = paramFile;
                var parafile = revitApp.OpenSharedParameterFile();
                var apiGroup = parafile.Groups.Create("RebarTestParamGroup");
                var extDefinitionCreationOptions = new ExternalDefinitionCreationOptions(paramName, paramType)
                {
                    HideWhenNoValue = true,
                    UserModifiable = userModifiable
                };
                var rebarSharedParamDef = apiGroup.Definitions.Create(extDefinitionCreationOptions);
                var rebarCat = doc.Settings.Categories.get_Item(BuiltInCategory.OST_Rebar);
                var categories = revitApp.Create.NewCategorySet();
                categories.Insert(rebarCat);
                var binding = revitApp.Create.NewInstanceBinding(categories);
                doc.ParameterBindings.Insert(rebarSharedParamDef, binding);
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to create shared parameter: {ex.Message}");
            }
        }

        public static string FindParameter(string parameterName, FamilyInstance familyInstance)
        {
            foreach (Parameter familyAttribute in familyInstance.Parameters)
            {
                if (familyAttribute.Definition.Name != parameterName)
                    continue;

                switch (familyAttribute.StorageType)
                {
                    case StorageType.Double:
                        return parameterName.Equals("Cross-Section Rotation")
                            ? Math.Round(familyAttribute.AsDouble() * RadiansToDegrees, 3).ToString()
                            : familyAttribute.AsDouble().ToString();
                    case StorageType.ElementId:
                        return familyAttribute.AsElementId().ToString();
                    case StorageType.Integer:
                        return familyAttribute.AsInteger().ToString();
                    case StorageType.String:
                        return familyAttribute.AsString();
                    case StorageType.None:
                        return familyAttribute.AsValueString();
                }
            }

            return null;
        }

        private static readonly BuiltInParameter[] SkipParameters = { BuiltInParameter.ALL_MODEL_MARK };

        public static bool ShouldSkip(ElementId parameterId)
        {
            foreach (var bip in SkipParameters)
            {
                if (new ElementId(bip) == parameterId)
                    return true;
            }

            return false;
        }

        public static FamilySymbol FindFamilySymbol(Document doc, string familyName, string symbolName)
        {
            foreach (var family in new FilteredElementCollector(doc).OfClass(typeof(Family)).Cast<Family>())
            {
                if (family.Name != familyName)
                    continue;

                foreach (var symbolId in family.GetFamilySymbolIds())
                {
                    if (doc.GetElement(symbolId) is FamilySymbol symbol && symbol.Name == symbolName)
                        return symbol;
                }
            }

            return null;
        }

    }
}
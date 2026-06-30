// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt
// Portions Copyright Revit Database Explorer (Apache-2.0)
// https://github.com/NeVeSpl/RevitDBExplorer @ 6929da81491a7f9ef69ed4c346afa1c582b830b5

using Autodesk.Revit.DB;
using System.Collections.Generic;


namespace Ara3D.RevitSampleBrowser.Common.Documents.Query.Parser.Commands
{
    public class ParameterArgument : CommandArgument<ElementId>
    {
        private static readonly Dictionary<ElementId, StorageType> storageTypeCache = [];
        private static readonly Dictionary<ElementId, ForgeTypeId> dataTypeCache = [];
        public bool IsBuiltInParameter { get; }
        public BuiltInParameter BuiltInParameter { get; init; }
        public StorageType StorageType { get; private set; } = StorageType.None;
        public ForgeTypeId DataType { get; private set; } = SpecTypeId.Custom;

        public ParameterArgument(BuiltInParameter value) : base(new ElementId(value))
        {
            IsBuiltInParameter = true;
            BuiltInParameter = value;
            Name = $"BuiltInParameter.{value}";
            Label = LabelUtils.GetLabelFor(value);
        }
        public ParameterArgument(ElementId value, string name) : base(value)
        {

            IsBuiltInParameter = false;
            Name = $"new ElementId({value})";
            Label = name;
        }


        public void ResolveStorageType(Document document)
        {
            if (storageTypeCache.TryGetValue(Value, out var storageType))
            {
                StorageType = storageType;
                if (dataTypeCache.TryGetValue(Value, out var dataType))
                {
                    DataType = dataType;
                    return;
                }
            }

            var collector = new FilteredElementCollector(document)
                     .WherePasses(new LogicalOrFilter(new ElementIsElementTypeFilter(true), new ElementIsElementTypeFilter(false)))
                     .WherePasses(new LogicalOrFilter(new ElementParameterFilter(ParameterFilterRuleFactory.CreateHasValueParameterRule(Value)), new ElementParameterFilter(ParameterFilterRuleFactory.CreateHasNoValueParameterRule(Value))));
            var first = collector.FirstElement();

            if (IsBuiltInParameter)
            {
                StorageType = document.get_TypeOfStorage(BuiltInParameter);
                storageTypeCache[Value] = StorageType;

                var parameter = first?.get_Parameter(BuiltInParameter);
                if (parameter != null)
                {
                    DataType = parameter.Definition.GetDataType();
                    dataTypeCache[Value] = DataType;
                }
            }
            else
            {
                if (document.GetElement(Value) is ParameterElement parameterElement)
                {
                    var definition = parameterElement.GetDefinition();
                    DataType = definition.GetDataType();
                    dataTypeCache[Value] = DataType;
                    var parameter = first?.get_Parameter(definition);
                    if (parameter != null)
                    {
                        StorageType = parameter.StorageType;
                        storageTypeCache[Value] = StorageType;
                    }
                }
            }
        }
    }
}
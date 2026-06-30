// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using Ara3D.RevitSampleBrowser.ProjectInfo.CS.Wrappers;
using Autodesk.Revit.DB.Analysis;

namespace Ara3D.RevitSampleBrowser.ProjectInfo.CS.Converters
{
    public class ConstructionWrapperConverter : TypeConverter
    {
        public override bool GetStandardValuesSupported(ITypeDescriptorContext context)
        {
            return true;
        }

        public override bool GetStandardValuesExclusive(ITypeDescriptorContext context)
        {
            return true;
        }

        public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
        {
            var list = new List<ConstructionWrapper>();
            // convert property name to ConstructionType
            var constructionType =
                (ConstructionType)Enum.Parse(typeof(ConstructionType), context.PropertyDescriptor.Name);

            // convert instance to MEPBuildingConstructionWrapper
            var mEpBuildingConstruction = context.Instance as MepBuildingConstructionWrapper;

            // get all Constructions from MEPBuildingConstructionWrapper and add them to a list
            foreach (var con in mEpBuildingConstruction.GetConstructions(constructionType))
            {
                list.Add(new ConstructionWrapper(con));
            }

            // sort the list
            list.Sort();
            return new StandardValuesCollection(list);
        }

        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            return sourceType == typeof(string) || base.CanConvertFrom(context, sourceType);
        }

        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            return destinationType.Equals(typeof(string)) || base.CanConvertTo(context, destinationType);
        }

        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            var text = value as string;
            if (!string.IsNullOrEmpty(text))
                foreach (ConstructionWrapper con in GetStandardValues(context))
                {
                    if (con.Name == text)
                        return con;
                }

            return base.ConvertFrom(context, culture, value);
        }

        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value,
            Type destinationType)
        {
            if (destinationType == null) throw new ArgumentNullException(nameof(destinationType));
            if (destinationType == typeof(string))
            {
                switch (value)
                {
                    case null:
                        return string.Empty;
                    case ConstructionWrapper construction:
                        return construction.Name;
                }
            }

            return base.ConvertTo(context, culture, value, destinationType);
        }
    }
}

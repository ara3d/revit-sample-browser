// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using Autodesk.Revit.DB;
using System;
using System.ComponentModel;
using System.Globalization;
using System.Linq;

namespace Ara3D.RevitSampleBrowser.ProjectInfo.CS.Converters
{
    public class ElementIdConverter<T> : TypeConverter where T : Element
    {
        public override bool GetStandardValuesSupported(ITypeDescriptorContext context)
        {
            return true;
        }

        public override bool GetStandardValuesExclusive(ITypeDescriptorContext context)
        {
            return true;
        }

        /// <summary>
        ///     Returns a collection of standard values from the default context for the
        ///     data type this type converter is designed for.
        /// </summary>
        /// <returns>Element collection retrieved through filtering current Revit document elements</returns>
        public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
        {
            // using type filter to get the target type objects
            //Autodesk.Revit.DB.TypeFilter typeFilter = RevitStartInfo.RevitApp.Create.Filter.NewTypeFilter(targetType, true);
            //ElementIterator elementIterator = RevitStartInfo.RevitDoc.get_Elements(typeFilter);

            //List<Element> list = new List<Element>();
            //elementIterator.Reset();
            //while (elementIterator.MoveNext())
            //{
            //    list.Add(elementIterator.Current as Element);
            //}
            var list = new FilteredElementCollector(RevitStartInfo.RevitDoc).OfClass(typeof(T));

            return new StandardValuesCollection(list.ToElementIds().ToList());
        }

        /// <summary>
        ///     Converts from string.
        /// </summary>
        /// <returns>Converted string</returns>
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            return sourceType == typeof(string) || base.CanConvertFrom(context, sourceType);
        }

        /// <summary>
        ///     Converts to string.
        /// </summary>
        /// <returns>Converted string</returns>
        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            return destinationType.Equals(typeof(string)) || base.CanConvertTo(context, destinationType);
        }

        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            var text = value as string;
            if (!string.IsNullOrEmpty(text))
            {
                var svc = GetStandardValues(context);
                foreach (ElementId elementId in svc)
                {
                    var element = RevitStartInfo.GetElement(elementId);
                    if (element.Name == text)
                        return element.Id;
                }
            }

            return base.ConvertFrom(context, culture, value);
        }

        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value,
            Type destinationType)
        {
            if (destinationType == null) throw new ArgumentNullException(nameof(destinationType));
            if (destinationType == typeof(string))
            {
                if (value == null) return string.Empty;
                var elementId = value as ElementId;
                if (elementId != null)
                {
                    var element = RevitStartInfo.GetElement(elementId);
                    if (element != null)
                    {
                        var elementName = string.Empty;
                        try
                        {
                            elementName = element.Name;
                        }
                        catch
                        {
                        }

                        return elementName;
                    }
                }
            }

            return base.ConvertTo(context, culture, value, destinationType);
        }
    }
}

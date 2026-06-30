// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using System;
using System.Collections;
using System.ComponentModel;
using System.Globalization;
using Autodesk.Revit.DB;

namespace Ara3D.RevitSampleBrowser.AreaReinParameters.CS
{
    public abstract class ParameterConverter : TypeConverter
    {
        protected Hashtable Hash;

        protected ParameterConverter()
        {
            Hash = new Hashtable();
            GetConvertHash();
        }

        public abstract void GetConvertHash();

        public override bool GetStandardValuesSupported(ITypeDescriptorContext context)
        {
            return true;
        }

        public override StandardValuesCollection GetStandardValues(
            ITypeDescriptorContext context)
        {
            var ids = new ElementId[Hash.Values.Count];
            var i = 0;

            foreach (DictionaryEntry de in Hash)
            {
                ids[i++] = (ElementId)de.Value;
            }

            var standardValues = new StandardValuesCollection(ids);

            return standardValues;
        }

        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            return sourceType == typeof(string) || base.CanConvertFrom(context, sourceType);
        }

        public override object ConvertFrom(ITypeDescriptorContext context,
            CultureInfo culture, object v)
        {
            if (v is string)
                foreach (DictionaryEntry de in Hash)
                {
                    if (de.Key.Equals(v.ToString()))
                        return de.Value;
                }

            return base.ConvertFrom(context, culture, v);
        }

        public override object ConvertTo(ITypeDescriptorContext context,
            CultureInfo culture, object v, Type destinationType)
        {
            if (destinationType == typeof(string))
            {
                foreach (DictionaryEntry de in Hash)
                {
                    var tmpId = (ElementId)de.Value;
                    var cmpId = (ElementId)v;

                    if (tmpId == cmpId) return de.Key.ToString();
                }

                return "";
            }

            return base.ConvertTo(context, culture, v, destinationType);
        }

        public override bool GetStandardValuesExclusive(ITypeDescriptorContext context)
        {
            return true;
        }
    }

    public class HookTypeItem : ParameterConverter
    {
        public override void GetConvertHash()
        {
            Hash = Command.HookTypes;
            var id = ElementId.InvalidElementId;

            if (!Hash.ContainsKey("None")) Hash.Add("None", id);
        }
    }

    public class BarTypeItem : ParameterConverter
    {
        public override void GetConvertHash()
        {
            Hash = Command.BarTypes;
        }
    }
}

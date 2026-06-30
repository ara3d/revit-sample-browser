// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using Autodesk.Revit.DB;

namespace Ara3D.RevitSampleBrowser.CreateBeamSystem.CS
{
    public abstract class ParameterConverter : TypeConverter
    {
        protected Dictionary<string, FamilySymbol> Hash;

        protected ParameterConverter()
        {
            GetConvertHash();
        }

        public abstract void GetConvertHash();

        public override bool GetStandardValuesSupported(ITypeDescriptorContext context)
        {
            return true;
        }

        public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
        {
            return new StandardValuesCollection(Hash.Values);
        }

        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            return sourceType == typeof(string) || base.CanConvertFrom(context, sourceType);
        }

        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            return Hash[value.ToString()];
        }

        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object v,
            Type destinationType)
        {
            if (destinationType == typeof(string))
            {
                if (!(v is FamilySymbol symbol)) return "";

                foreach (var kvp in Hash)
                {
                    if (kvp.Value.Id == symbol.Id)
                        return kvp.Key;
                }

                return "";
            }

            return base.ConvertTo(context, culture, v, destinationType);
        }

        public override bool GetStandardValuesExclusive(ITypeDescriptorContext context)
        {
            return false;
        }
    }

    public class BeamTypeItem : ParameterConverter
    {
        /// <summary>
        ///     override the base type's GetConvertHash method
        /// </summary>
        public override void GetConvertHash()
        {
            Hash = BeamSystemData.GetBeamTypes();
        }
    }
}

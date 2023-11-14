// Copyright 2023. See https://github.com/ara3d/revit-samples/LICENSE.txt

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using Autodesk.Revit.DB;

namespace Revit.SDK.Samples.CreateBeamSystem.CS
{
    /// <summary>
    ///     base class of converting types of FamilySymbol to string
    ///     Code here have nothing with Revit API
    ///     it's only for PropertyGrid and its SelectedObject
    /// </summary>
    public abstract class ParameterConverter : TypeConverter
    {
        /// <summary>
        ///     hashtable of FamilySymbol and its Name
        /// </summary>
        protected Dictionary<string, FamilySymbol> Hash;

        /// <summary>
        ///     constructor initialize m_hash
        /// </summary>
        protected ParameterConverter()
        {
            GetConvertHash();
        }

        /// <summary>
        ///     subclass must implement to initialize m_hash
        /// </summary>
        public abstract void GetConvertHash();

        /// <summary>
        ///     returns whether this object supports a standard set of values that can be picked from a list
        /// </summary>
        /// <param name="context">provides a format context</param>
        /// <returns>
        ///     true if GetStandardValues should be called to find a common set of values the object supports;
        ///     otherwise, false
        /// </returns>
        public override bool GetStandardValuesSupported(ITypeDescriptorContext context)
        {
            return true;
        }

        /// <summary>
        ///     returns a collection of FamilySymbol
        /// </summary>
        /// <param name="context">provides a format context</param>
        /// <returns>collection of FamilySymbol</returns>
        public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
        {
            return new StandardValuesCollection(Hash.Values);
        }

        /// <summary>
        ///     whether this converter can convert an object of the given type to string
        /// </summary>
        /// <param name="context">provides a format context</param>
        /// <param name="sourceType">a Type that represents the type you want to convert from</param>
        /// <returns></returns>
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            return sourceType == typeof(string) || base.CanConvertFrom(context, sourceType);
        }

        /// <summary>
        ///     converts the Name to corresponding FamilySymbol
        /// </summary>
        /// <param name="context">provides a format context</param>
        /// <param name="culture">the CultureInfo to use as the current culture</param>
        /// <param name="value">the Object to convert</param>
        /// <returns>an FamilySymbol object</returns>
        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            return Hash[value.ToString()];
        }

        /// <summary>
        ///     converts the given FamilySymbol to the Name, using the specified context and culture information
        /// </summary>
        /// <param name="context">provides a format context</param>
        /// <param name="culture">the CultureInfo to use as the current culture</param>
        /// <param name="v">the Object to convert</param>
        /// <param name="destinationType">should be string</param>
        /// <returns></returns>
        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object v,
            Type destinationType)
        {
            if (destinationType == typeof(string))
            {
                if (!(v is FamilySymbol symbol)) return "";

                foreach (var kvp in Hash)
                    if (kvp.Value.Id == symbol.Id)
                        return kvp.Key;
                return "";
            }

            return base.ConvertTo(context, culture, v, destinationType);
        }

        /// <summary>
        ///     whether the collection of standard values returned
        ///     from GetStandardValues is an exclusive list of possible values
        /// </summary>
        /// <param name="context">provides a format context</param>
        /// <returns></returns>
        public override bool GetStandardValuesExclusive(ITypeDescriptorContext context)
        {
            return false;
        }
    }

    /// <summary>
    ///     converting types of FamilySymbol to string
    ///     Code here have nothing with Revit API
    ///     it's only for PropertyGrid and its SelectedObject
    /// </summary>
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

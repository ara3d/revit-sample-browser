// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using System;
using System.Collections;
using System.ComponentModel;
using System.Globalization;
using Autodesk.Revit.DB;

namespace Ara3D.RevitSampleBrowser.PathReinforcement.CS
{
    /// <summary>
    ///     Converter between Autodesk.Revit.DB.ElementId and Element's name
    /// </summary>
    public class BartypeConverter : TypeConverter
    {
        /// <summary>
        ///     hash table
        /// </summary>
        protected Hashtable Hash;

        /// <summary>
        ///     initialize m_hash
        /// </summary>
        public BartypeConverter()
        {
            Hash = new Hashtable();
            GetConvertHash();
        }

        /// <summary>
        ///     fill hash table
        /// </summary>
        public void GetConvertHash()
        {
            Hash = Command.BarTypes;
        }

        /// <summary>
        ///     Get supported standard value
        /// </summary>
        /// <param name="context"></param>
        /// <returns>always return true</returns>
        public override bool GetStandardValuesSupported(ITypeDescriptorContext context)
        {
            return true;
        }

        /// <summary>
        ///     provide Autodesk.Revit.DB.ElementId collection
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
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

        /// <summary>
        ///     whether conversion is allowed
        /// </summary>
        /// <param name="context"></param>
        /// <param name="sourceType"></param>
        /// <returns></returns>
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            return sourceType == typeof(string) || base.CanConvertFrom(context, sourceType);
        }

        /// <summary>
        ///     convert from Name to ElementId
        /// </summary>
        /// <param name="context"></param>
        /// <param name="culture"></param>
        /// <param name="v">Name</param>
        /// <returns>ElementId</returns>
        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object v)
        {
            if (v is string)
                foreach (DictionaryEntry de in Hash)
                {
                    if (de.Key.Equals(v.ToString()))
                        return de.Value;
                }

            return base.ConvertFrom(context, culture, v);
        }

        /// <summary>
        ///     convert from Autodesk.Revit.DB.ElementId to Name
        /// </summary>
        /// <param name="context"></param>
        /// <param name="culture"></param>
        /// <param name="v">ElementId</param>
        /// <param name="destinationType">String</param>
        /// <returns>Name</returns>
        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object v,
            Type destinationType)
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

        /// <summary>
        ///     always return true so that user can't input unexpected text
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public override bool GetStandardValuesExclusive(ITypeDescriptorContext context)
        {
            return true;
        }
    }
}

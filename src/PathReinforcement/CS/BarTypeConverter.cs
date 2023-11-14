// Copyright 2023. See https://github.com/ara3d/revit-samples/LICENSE.txt

using System;
using System.Collections;
using System.ComponentModel;
using System.Globalization;
using Autodesk.Revit.DB;

namespace Revit.SDK.Samples.PathReinforcement.CS
{
    /// <summary>
    ///     Converter between Autodesk.Revit.DB.ElementId and Element's name
    /// </summary>
    public class BartypeConverter : TypeConverter
    {
        /// <summary>
        ///     hash table
        /// </summary>
        protected Hashtable m_hash;

        /// <summary>
        ///     initialize m_hash
        /// </summary>
        public BartypeConverter()
        {
            m_hash = new Hashtable();
            GetConvertHash();
        }

        /// <summary>
        ///     fill hash table
        /// </summary>
        public void GetConvertHash()
        {
            m_hash = Command.BarTypes;
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
            var Ids = new ElementId[m_hash.Values.Count];
            var i = 0;

            foreach (DictionaryEntry de in m_hash) Ids[i++] = (ElementId)de.Value;
            var standardValues = new StandardValuesCollection(Ids);

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
            if (sourceType == typeof(string)) return true;
            return base.CanConvertFrom(context, sourceType);
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
                foreach (DictionaryEntry de in m_hash)
                    if (de.Key.Equals(v.ToString()))
                        return de.Value;
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
                foreach (DictionaryEntry de in m_hash)
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

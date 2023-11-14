// Copyright 2023. See https://github.com/ara3d/revit-samples/LICENSE.txt
using System;
using System.ComponentModel;
using Autodesk.Revit.DB;

namespace RevitMultiSample.ProjectInfo.CS
{
    /// <summary>
    ///     Wrapper class for Construction
    /// </summary>
    [TypeConverter(typeof(ConstructionWrapperConverter))]
    public class ConstructionWrapper : IComparable, IWrapper
    {
        /// <summary>
        ///     Construction
        /// </summary>
        private readonly Construction m_construction;

        /// <summary>
        ///     Initializes private variables.
        /// </summary>
        /// <param name="construction">Construction</param>
        public ConstructionWrapper(Construction construction)
        {
            m_construction = construction;
        }

        /// <summary>
        ///     Compares the names of Constructions.
        /// </summary>
        /// <param name="obj">ConstructionWrapper used to compare</param>
        /// <returns>
        ///     A 32-bit signed integer that indicates the relative order of the objects
        ///     being compared. The return value has these meanings:
        ///     Value Condition Less than zero This instance is less than value.
        ///     Zero This instance is equal to value. Greater than zero This instance is
        ///     greater than value.-or- value is null.
        /// </returns>
        public int CompareTo(object obj)
        {
            return obj is ConstructionWrapper wrapper ? Name.CompareTo(wrapper.Name) : 1;
        }

        /// <summary>
        ///     Gets the handle object.
        /// </summary>
        [Browsable(false)]
        public object Handle => m_construction;

        /// <summary>
        ///     Gets the name of the handle.
        /// </summary>
        [Browsable(false)]
        public string Name => m_construction.Name;
    }
}

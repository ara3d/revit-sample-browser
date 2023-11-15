// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using System;
using System.ComponentModel;

namespace Ara3D.RevitSampleBrowser.ProjectInfo.CS
{
    /// <summary>
    /// </summary>
    public class WrapperCustomDescriptor : ICustomTypeDescriptor, IWrapper
    {
        /// <summary>
        ///     Handle object
        /// </summary>
        private readonly object m_handle;

        /// <summary>
        ///     Initializes handle object
        /// </summary>
        /// <param name="handle">Handle object</param>
        public WrapperCustomDescriptor(object handle)
        {
            m_handle = handle;
        }

        /// <summary>
        ///     Returns a collection of custom attributes for this instance of a component.
        /// </summary>
        /// <returns>Handle's attributes</returns>
        public AttributeCollection GetAttributes()
        {
            return TypeDescriptor.GetAttributes(m_handle, false);
        }

        /// <summary>
        ///     Returns the class name of this instance of a component.
        /// </summary>
        /// <returns>Handle's class name</returns>
        public string GetClassName()
        {
            return TypeDescriptor.GetClassName(m_handle, false);
        }

        /// <summary>
        ///     Returns the name of this instance of a component.
        /// </summary>
        /// <returns>The name of handle object</returns>
        public string GetComponentName()
        {
            return TypeDescriptor.GetComponentName(m_handle, false);
        }

        /// <summary>
        ///     Returns a type converter for this instance of a component.
        /// </summary>
        /// <returns>The converter of the handle</returns>
        public TypeConverter GetConverter()
        {
            return TypeDescriptor.GetConverter(m_handle, false);
        }

        /// <summary>
        ///     Returns the default event for this instance of a component.
        /// </summary>
        /// <returns>
        ///     An EventDescriptor that represents the default event for this object,
        ///     or null if this object does not have events.
        /// </returns>
        public EventDescriptor GetDefaultEvent()
        {
            return TypeDescriptor.GetDefaultEvent(m_handle, false);
        }

        /// <summary>
        ///     Returns the default property for this instance of a component.
        /// </summary>
        /// <returns>
        ///     A PropertyDescriptor that represents the default property for this object,
        ///     or null if this object does not have properties.
        /// </returns>
        public PropertyDescriptor GetDefaultProperty()
        {
            return TypeDescriptor.GetDefaultProperty(m_handle, false);
        }

        /// <summary>
        ///     Returns an editor of the specified type for this instance of a component.
        /// </summary>
        /// <param name="editorBaseType">A Type that represents the editor for this object. </param>
        /// <returns>
        ///     An Object of the specified type that is the editor for this object,
        ///     or null if the editor cannot be found.
        /// </returns>
        public object GetEditor(Type editorBaseType)
        {
            return TypeDescriptor.GetEditor(m_handle, editorBaseType, false);
        }

        /// <summary>
        ///     Returns the events for this instance of a component using the specified attribute array as a filter.
        /// </summary>
        /// <param name="attributes">An array of type Attribute that is used as a filter. </param>
        /// <returns>An EventDescriptorCollection that represents the filtered events for this component instance.</returns>
        public EventDescriptorCollection GetEvents(Attribute[] attributes)
        {
            return TypeDescriptor.GetEvents(m_handle, attributes, false);
        }

        /// <summary>
        ///     Returns the events for this instance of a component.
        /// </summary>
        /// <returns>An EventDescriptorCollection that represents the events for this component instance.</returns>
        public EventDescriptorCollection GetEvents()
        {
            return TypeDescriptor.GetEvents(m_handle, false);
        }

        /// <summary>
        ///     Returns the properties for this instance of a component using the attribute array as a filter.
        /// </summary>
        /// <param name="attributes">An array of type Attribute that is used as a filter.</param>
        /// <returns>
        ///     A PropertyDescriptorCollection that
        ///     represents the filtered properties for this component instance.
        /// </returns>
        public PropertyDescriptorCollection GetProperties(Attribute[] attributes)
        {
            // get handle's properties
            var collection = TypeDescriptor.GetProperties(m_handle, attributes, false);
            // create empty collection
            var collection2 = new PropertyDescriptorCollection(Array.Empty<PropertyDescriptor>());

            // filter properties by RevitVersionAttribute.
            // if there is RevitVersionAttribute specified and the designated names does not 
            // contain current Revit version, the property will not be exposed.
            foreach (PropertyDescriptor pd in collection)
            {
                var matchRevitVersion = true;
                foreach (Attribute att in pd.Attributes)
                {
                    if (att is RevitVersionAttribute pfa)
                    {
                        if (!pfa.Names.Contains(RevitStartInfo.RevitProduct))
                            matchRevitVersion = false;
                        break;
                    }
                }

                if (matchRevitVersion)
                    collection2.Add(pd);
            }

            return collection2;
        }

        /// <summary>
        ///     Returns the properties for this instance of a component.
        /// </summary>
        /// <returns>
        ///     A PropertyDescriptorCollection that represents the properties
        ///     for this component instance.
        /// </returns>
        public PropertyDescriptorCollection GetProperties()
        {
            return TypeDescriptor.GetProperties(m_handle, false);
        }

        /// <summary>
        ///     Returns an object that contains the property described by the specified property descriptor.
        /// </summary>
        /// <param name="pd">A PropertyDescriptor that represents the property whose owner is to be found. </param>
        /// <returns>Handle object</returns>
        public object GetPropertyOwner(PropertyDescriptor pd)
        {
            return m_handle;
        }

        /// <summary>
        ///     Gets handle object
        /// </summary>
        public object Handle => m_handle;

        /// <summary>
        ///     Gets the name of the handle object if it has the Name property,
        ///     otherwise returns Handle.ToString().
        /// </summary>
        public string Name
        {
            get
            {
                var mi = Handle.GetType().GetMethod("get_Name", Type.EmptyTypes);
                if (mi != null)
                {
                    var name = mi.Invoke(Handle, Array.Empty<object>());
                    return name != null ? name.ToString() : string.Empty;
                }

                return Handle.ToString();
            }
        }

        /// <summary>
        ///     overrides ToString method
        /// </summary>
        /// <returns>The name of the handle object</returns>
        public override string ToString()
        {
            return Name;
        }
    }
}

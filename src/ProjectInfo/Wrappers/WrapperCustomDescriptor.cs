// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using System;
using System.ComponentModel;

namespace Ara3D.RevitSampleBrowser.ProjectInfo.CS.Wrappers
{
    public class WrapperCustomDescriptor : ICustomTypeDescriptor, IWrapper
    {
        private readonly object m_handle;

        public WrapperCustomDescriptor(object handle)
        {
            m_handle = handle;
        }

        public AttributeCollection GetAttributes()
        {
            return TypeDescriptor.GetAttributes(m_handle, false);
        }

        public string GetClassName()
        {
            return TypeDescriptor.GetClassName(m_handle, false);
        }

        public string GetComponentName()
        {
            return TypeDescriptor.GetComponentName(m_handle, false);
        }

        public TypeConverter GetConverter()
        {
            return TypeDescriptor.GetConverter(m_handle, false);
        }

        public EventDescriptor GetDefaultEvent()
        {
            return TypeDescriptor.GetDefaultEvent(m_handle, false);
        }

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

        public EventDescriptorCollection GetEvents(Attribute[] attributes)
        {
            return TypeDescriptor.GetEvents(m_handle, attributes, false);
        }

        public EventDescriptorCollection GetEvents()
        {
            return TypeDescriptor.GetEvents(m_handle, false);
        }

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

        public PropertyDescriptorCollection GetProperties()
        {
            return TypeDescriptor.GetProperties(m_handle, false);
        }

        public object GetPropertyOwner(PropertyDescriptor pd)
        {
            return m_handle;
        }

        public object Handle => m_handle;

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

        public override string ToString()
        {
            return Name;
        }
    }
}

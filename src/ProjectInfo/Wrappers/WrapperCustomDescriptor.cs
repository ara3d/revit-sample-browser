// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using System;
using System.ComponentModel;
using System.Reflection;

namespace Ara3D.RevitSampleBrowser.ProjectInfo.CS.Wrappers
{
    public class WrapperCustomDescriptor : ICustomTypeDescriptor, IWrapper
    {
        public WrapperCustomDescriptor(object handle)
        {
            Handle = handle;
        }

        public AttributeCollection GetAttributes()
        {
            return TypeDescriptor.GetAttributes(Handle, false);
        }

        public string GetClassName()
        {
            return TypeDescriptor.GetClassName(Handle, false);
        }

        public string GetComponentName()
        {
            return TypeDescriptor.GetComponentName(Handle, false);
        }

        public TypeConverter GetConverter()
        {
            return TypeDescriptor.GetConverter(Handle, false);
        }

        public EventDescriptor GetDefaultEvent()
        {
            return TypeDescriptor.GetDefaultEvent(Handle, false);
        }

        public PropertyDescriptor GetDefaultProperty()
        {
            return TypeDescriptor.GetDefaultProperty(Handle, false);
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
            return TypeDescriptor.GetEditor(Handle, editorBaseType, false);
        }

        public EventDescriptorCollection GetEvents(Attribute[] attributes)
        {
            return TypeDescriptor.GetEvents(Handle, attributes, false);
        }

        public EventDescriptorCollection GetEvents()
        {
            return TypeDescriptor.GetEvents(Handle, false);
        }

        public PropertyDescriptorCollection GetProperties(Attribute[] attributes)
        {
            // get handle's properties
            var collection = TypeDescriptor.GetProperties(Handle, attributes, false);
            // create empty collection
            PropertyDescriptorCollection collection2 = new(Array.Empty<PropertyDescriptor>());

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
            return TypeDescriptor.GetProperties(Handle, false);
        }

        public object GetPropertyOwner(PropertyDescriptor pd)
        {
            return Handle;
        }

        public object Handle { get; }

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

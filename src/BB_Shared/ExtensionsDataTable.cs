using System;
using System.Collections.Generic;
using System.Reflection;

namespace Ara3D.Bowerbird.RevitSamples
{
    public static class ExtensionsDataTable
    {
        public static Type GetUnderlyingType(this Type type)
        {
            return Nullable.GetUnderlyingType(type) ?? type;
        }

        public static Type GetValueType(this MemberInfo mi)
        {
            if (mi is FieldInfo f) return f.FieldType.GetUnderlyingType();
            return mi is PropertyInfo p
                ? p.PropertyType.GetUnderlyingType()
                : mi is MethodInfo m ? m.ReturnType.GetUnderlyingType() : throw new Exception("Not invokable member");
        }

        public static object GetValue(this MemberInfo mi, object obj)
        {
            if (mi is FieldInfo f) return f.GetValue(obj);
            return mi is PropertyInfo p
                ? p.GetValue(obj)
                : mi is MethodInfo m ? m.Invoke(obj, Array.Empty<object>()) : throw new Exception("Not invokable member");
        }

        public static bool CanGetValue(this MemberInfo mi)
        {
            return mi is FieldInfo
                       || (mi is PropertyInfo pi && pi.CanRead && pi.GetIndexParameters().Length == 0)
                       || (mi is MethodInfo method && method.GetParameters().Length == 0);
        }

        public static DataTableBuilder BuildDataTable(this Type self, DataTableBuilder.DataTableBuilderOptions options = null)
        {
            return new DataTableBuilder(self, options);
        }

        public static System.Data.DataTable ToDataTable<T>(this IEnumerable<T> self, DataTableBuilder.DataTableBuilderOptions options = null)
        {
            return BuildDataTable(typeof(T), options).AddRows(self).DataTable;
        }
    }
}
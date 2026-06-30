// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt
//
// Adapted from RoomVolumeDirectShape by Jeremy Tammik (MIT).
// https://github.com/jeremytammik/RoomVolumeDirectShape

using System;
using System.Collections.Generic;
using System.Linq;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Architecture;

namespace Ara3D.RevitSampleBrowser.RoomVolumeDirectShape.CS
{
    internal static class RoomPropertyHelper
    {
        public static string GetRoomPropertiesJson(Room room)
        {
            return FormatDictAsJson(GetParamValues(room));
        }

        static Dictionary<string, string> GetParamValues(Element element)
        {
            var values = new Dictionary<string, string>(element.Parameters.Size);

            foreach (Parameter parameter in element.Parameters)
            {
                var key = string.Format(
                    "{0}({1})",
                    parameter.Definition.Name,
                    ParameterStorageTypeChar(parameter));

                var val = ParameterToString(parameter);

                if (values.TryGetValue(key, out var existing))
                {
                    if (existing != val)
                    {
                        values[key] = existing + " | " + val;
                    }
                }
                else
                {
                    values.Add(key, val);
                }
            }

            return values;
        }

        static string FormatDictAsJson(Dictionary<string, string> dictionary)
        {
            var keys = dictionary.Keys.ToList();
            keys.Sort();

            var pairs = keys.Select(
                key => string.Format(
                    "\"{0}\" : \"{1}\"",
                    key,
                    dictionary[key]));

            return "{" + string.Join(", ", pairs) + "}";
        }

        static char ParameterStorageTypeChar(Parameter parameter)
        {
            return parameter.StorageType switch
            {
                StorageType.Double => 'r',
                StorageType.Integer => 'n',
                StorageType.String => 's',
                StorageType.ElementId => 'e',
                _ => throw new ArgumentOutOfRangeException(
                    nameof(parameter),
                    "expected valid parameter storage type, not 'None'")
            };
        }

        static string ParameterToString(Parameter parameter)
        {
            if (parameter == null)
            {
                return "null";
            }

            return parameter.StorageType switch
            {
                StorageType.Double => parameter.AsDouble().ToString("0.##"),
                StorageType.Integer => parameter.AsInteger().ToString(),
                StorageType.String => parameter.AsString(),
                StorageType.ElementId => parameter.AsElementId().Value.ToString(),
                StorageType.None => "none",
                _ => "null"
            };
        }
    }
}

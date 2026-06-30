// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt
// Portions Copyright Revit Database Explorer (Apache-2.0)
// https://github.com/NeVeSpl/RevitDBExplorer @ 6929da81491a7f9ef69ed4c346afa1c582b830b5

using System;
using System.Collections.Generic;


namespace Ara3D.RevitSampleBrowser.Common.Infrastructure
{
    public static class StringExtensions
    {
        /// <summary>
        /// source: https://www.dotnetperls.com/remove-html-tags
        /// </summary>
        public static string StripTags(this string source)
        {
            var array = new char[source.Length];
            var arrayIndex = 0;
            var inside = false;

            for (var i = 0; i < source.Length; i++)
            {
                var let = source[i];
                if (let == '<')
                {
                    inside = true;
                    continue;
                }
                if (let == '>')
                {
                    inside = false;
                    continue;
                }
                if (!inside)
                {
                    array[arrayIndex] = let;
                    arrayIndex++;
                }
            }
            return new string(array, 0, arrayIndex);
        }

        /// <summary>
        /// source: https://stackoverflow.com/questions/6219454/efficient-way-to-remove-all-whitespace-from-string/30732794#30732794
        /// </summary>
        public static string RemoveWhitespace(this string str)
        {
            return string.Join("", str.Split(default(string[]), StringSplitOptions.RemoveEmptyEntries));
        }


        /// <summary>
        /// source: https://stackoverflow.com/questions/5284591/how-to-remove-a-suffix-from-end-of-string
        /// </summary>
        public static string RemoveFromEnd(this string s, string suffix)
        {
            return s.EndsWith(suffix) ? s.Substring(0, s.Length - suffix.Length) : s;
        }

        public static string RemovePrefix(this string input, string prefix)
        {
            return input.StartsWith(prefix, StringComparison.OrdinalIgnoreCase) ? input.Remove(0, prefix.Length) : input;
        }


        public static string Truncate(this string value, int maxChars)
        {
            return value == null ? null : maxChars <= 3 ? value : value.Length <= maxChars ? value : value.Substring(0, maxChars - 3) + "...";
        }

        public static string ReplaceMany(this string input, IEnumerable<(string, string)> replacements)
        {
            var result = input;
            foreach (var replacement in replacements)
            {
                result = result.Replace(replacement.Item1, replacement.Item2);
            }
            return result;
        }
    }
}
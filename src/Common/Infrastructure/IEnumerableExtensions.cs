// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media.Imaging;
using System.Xml.Linq;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Architecture;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using Color = System.Drawing.Color;
using OperationCanceledException = Autodesk.Revit.Exceptions.OperationCanceledException;
using Rectangle = System.Drawing.Rectangle;
using WinForms = System.Windows.Forms;


namespace BuildingCoder
{
    public static class IEnumerableExtensions
        {
            // (C) Jonathan Skeet
            // from https://github.com/morelinq/MoreLINQ/blob/master/MoreLinq/MinBy.cs
            public static TSource MinBy<TSource, TKey>(
                this IEnumerable<TSource> source,
                Func<TSource, TKey> selector)
            {
                return source.MinBy(selector, Comparer<TKey>.Default);
            }
    
            public static TSource MinBy<TSource, TKey>(
                this IEnumerable<TSource> source,
                Func<TSource, TKey> selector,
                IComparer<TKey> comparer)
            {
                if (source == null)
                    throw new ArgumentNullException(nameof(source));
                if (selector == null)
                    throw new ArgumentNullException(nameof(selector));
                if (comparer == null)
                    throw new ArgumentNullException(nameof(comparer));
                using var sourceIterator = source.GetEnumerator();
                if (!sourceIterator.MoveNext())
                    throw new InvalidOperationException("Sequence was empty");
                var min = sourceIterator.Current;
                var minKey = selector(min);
                while (sourceIterator.MoveNext())
                {
                    var candidate = sourceIterator.Current;
                    var candidateProjected = selector(candidate);
                    if (comparer.Compare(candidateProjected, minKey) < 0)
                    {
                        min = candidate;
                        minKey = candidateProjected;
                    }
                }
    
                return min;
            }
    
            public static HashSet<TElement> ToHashSet<TSource, TElement>(
                this IEnumerable<TSource> source,
                Func<TSource, TElement> elementSelector,
                IEqualityComparer<TElement> comparer)
            {
                if (source == null)
                    throw new ArgumentNullException("source");
                if (elementSelector == null)
                    throw new ArgumentNullException("elementSelector");
    
                // you can unroll this into a foreach if you want efficiency gain, but for brevity...
                return new HashSet<TElement>(
                    source.Select(elementSelector), comparer);
            }
    
            public static HashSet<TSource> ToHashSet<TSource>(
                this IEnumerable<TSource> source)
            {
                // key selector is identity fxn and null is default comparer
                return source.ToHashSet(
                    item => item, null);
            }
    
            public static HashSet<TSource> ToHashSet<TSource>(
                this IEnumerable<TSource> source,
                IEqualityComparer<TSource> comparer)
            {
                return source.ToHashSet(
                    item => item, comparer);
            }
    
            public static HashSet<TElement> ToHashSet<TSource, TElement>(
                this IEnumerable<TSource> source,
                Func<TSource, TElement> elementSelector)
            {
                return source.ToHashSet(
                    elementSelector, null);
            }
        }
}

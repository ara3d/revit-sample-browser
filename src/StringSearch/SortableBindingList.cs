#region Namespaces
using System;
using System.Collections.Generic;
using System.ComponentModel;
#endregion // Namespaces

namespace Ara3D.RevitSampleBrowser.StringSearch.CS
{
    /// <summary>
    /// The code below is by Tim Van Wassenhove, 2008.
    /// http://www.timvw.be/2008/08/02/presenting-the-sortablebindinglistt-take-two/
    /// If the above link fails for you, please search for 
    /// "Presenting the SortableBindingList (take two)". 
    /// </summary>
    public class SortableBindingList<T> : BindingList<T>
    {
        private readonly Dictionary<Type, PropertyComparer<T>> comparers;
        private bool isSorted;
        private ListSortDirection listSortDirection;
        private PropertyDescriptor propertyDescriptor;

        public SortableBindingList()
          : base([])
        {
            this.comparers = [];
        }

        public SortableBindingList(IEnumerable<T> enumeration)
          : base([.. enumeration])
        {
            this.comparers = [];
        }

        protected override bool SupportsSortingCore => true;

        protected override bool IsSortedCore => this.isSorted;

        protected override PropertyDescriptor SortPropertyCore => this.propertyDescriptor;

        protected override ListSortDirection SortDirectionCore => this.listSortDirection;

        protected override bool SupportsSearchingCore => true;

        protected override void ApplySortCore(PropertyDescriptor property, ListSortDirection direction)
        {
            var itemsList = (List<T>)this.Items;

            var propertyType = property.PropertyType;
            PropertyComparer<T> comparer;
            if (!this.comparers.TryGetValue(propertyType, out comparer))
            {
                comparer = new PropertyComparer<T>(property, direction);
                this.comparers.Add(propertyType, comparer);
            }

            comparer.SetPropertyAndDirection(property, direction);
            itemsList.Sort(comparer);

            this.propertyDescriptor = property;
            this.listSortDirection = direction;
            this.isSorted = true;

            this.OnListChanged(new ListChangedEventArgs(ListChangedType.Reset, -1));
        }

        protected override void RemoveSortCore()
        {
            this.isSorted = false;
            this.propertyDescriptor = base.SortPropertyCore;
            this.listSortDirection = base.SortDirectionCore;

            this.OnListChanged(new ListChangedEventArgs(ListChangedType.Reset, -1));
        }

        protected override int FindCore(PropertyDescriptor property, object key)
        {
            var count = this.Count;
            for (var i = 0; i < count; ++i)
            {
                var element = this[i];
                if (property.GetValue(element).Equals(key))
                {
                    return i;
                }
            }
            return -1;
        }
    }
}

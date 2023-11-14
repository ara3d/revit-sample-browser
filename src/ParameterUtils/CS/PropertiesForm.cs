// Copyright 2023. See https://github.com/ara3d/revit-samples/LICENSE.txt

using System.Windows.Forms;

namespace Revit.SDK.Samples.ParameterUtils.CS
{
    public partial class PropertiesForm : Form
    {
        /// <summary>
        ///     Default constructor, initialize all controls
        /// </summary>
        private PropertiesForm()
        {
            InitializeComponent();
        }

        /// <summary>
        ///     This Form is used to display the properties that exist upon an element.
        ///     It consists of a list view and the ok, cancel buttons.
        /// </summary>
        /// <param name="information">A string array that will be loaded into the list view</param>
        public PropertiesForm(string[] information)
            : this()
        {
            // we need to add each string in to each row of the list view, and split the string
            // into substrings delimited by '\t' then put them into the columns of the row.

            // create three columns with "Name", "Type" and "Value"
            propertyListView.Columns.Add("Name");
            propertyListView.Columns.Add("Type");
            propertyListView.Columns.Add("Value");

            // loop all the strings, split them, and add them to rows of the list view
            foreach (var row in information)
            {
                if (row == null) continue;
                var lvi = new ListViewItem(row.Split('\t'));
                propertyListView.Items.Add(lvi);
            }

            // The following code is used to sort and resize the columns within the list view 
            // so that the data can be viewed better.

            // sort the items in the list view ordered by ascending.
            propertyListView.Sorting = SortOrder.Ascending;

            // make the column width fit the content
            propertyListView.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent);

            // increase the width of columns by 40, make them a litter wider
            var span = 40;
            foreach (ColumnHeader ch in propertyListView.Columns) ch.Width += span;

            // the last column fit the rest of the list view
            propertyListView.Columns[propertyListView.Columns.Count - 1].Width = -2;
        }
    }
}

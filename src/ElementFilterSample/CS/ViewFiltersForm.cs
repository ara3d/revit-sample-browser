// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using System;
using System.Collections.Generic;
using System.Windows.Forms;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using ComboBox = System.Windows.Forms.ComboBox;
using Form = System.Windows.Forms.Form;

namespace Ara3D.RevitSampleBrowser.ElementFilterSample.CS
{
    /// <summary>
    ///     UI form to display the view filters information
    ///     Some controls provide interfaces to create or modify filters and rules.
    /// </summary>
    public partial class ViewFiltersForm : Form
    {
        /// <summary>
        ///     Const name for invalid parameter
        /// </summary>
        private const string NoneParam = "(none)";

        /// <summary>
        ///     Sample custom rule name prefix, filter rules will be displayed with this name + #(1, 2, ...)
        /// </summary>
        private const string RuleNamePrefix = "Filter Rule ";

        /// <summary>
        ///     Indicates if categories were changed by programming,
        ///     It's used to suppress ItemCheck event for Categories controls
        /// </summary>
        private bool m_catChangedEventSuppress;

        /// <summary>
        ///     Current filter data maps active Revit filter.
        /// </summary>
        private FilterData m_currentFilterData;

        /// <summary>
        ///     Dictionary of filter and its filter data(categories, filter rules)
        /// </summary>
        private readonly Dictionary<string, FilterData> m_dictFilters = new Dictionary<string, FilterData>();

        /// <summary>
        ///     Revit active document
        /// </summary>
        private readonly Document m_doc;

        /// <summary>
        ///     Overload the constructor
        /// </summary>
        /// <param name="commandData">an instance of Data class</param>
        public ViewFiltersForm(ExternalCommandData commandData)
        {
            InitializeComponent();
            m_doc = commandData.Application.ActiveUIDocument.Document;
        }

        /// <summary>
        ///     When the form was loaded, display the information of existing filters
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ViewFiltersForm_Load(object sender, EventArgs e)
        {
            SuspendLayout();
            // Get all existing filters and initialize class members
            InitializeFilterData();
            //
            // Get all applicable categories and fill in category CheckListBox
            AddAppliableCategories();
            //
            // Set 1st item selected, then track according event
            if (filtersListBox.Items.Count > 0)
                filtersListBox.SetSelected(0, true); // set first item to be selected to raise control event
            else
                ResetControls_NoFilter();
            ResumeLayout();
        }

        /// <summary>
        ///     Select one filter, it will reset categories and accordingly rules.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void filtersListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            // if no item yet do nothing
            if (filtersListBox.Items.Count == 0 || filtersListBox.SelectedItems.Count == 0)
                return;
            //
            // Get current selected filter, use 1st item because only one item can be selected for control.
            var filterName = filtersListBox.SelectedItems[0] as string;
            if (!m_dictFilters.TryGetValue(filterName, out m_currentFilterData))
                return;
            //
            // Reset categoryCheckedListBox:
            // Show all categories of document, also check categories belong to this filter
            m_catChangedEventSuppress = true; // suppress some events when checking categories during reset 
            ResetCategoriesControl(false); // don't need to re-add all items
            m_catChangedEventSuppress = false;
            //
            // Initialize all supported parameters for selected categories
            ResetRule_CategoriesChanged();
        }

        /// <summary>
        ///     Create new filter
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void newButton_Click(object sender, EventArgs e)
        {
            // Show new name for filter, unique name is required for creating filters
            var inUseKeys = new List<string>();
            inUseKeys.AddRange(m_dictFilters.Keys);
            var newForm = new NewFilterForm(inUseKeys);
            var result = newForm.ShowDialog();
            if (result != DialogResult.OK)
                return;
            //
            // Create new filter data now(the filter data will be reflected to Revit filter when Ok button is clicked).
            var newFilterName = newForm.FilterName;
            m_currentFilterData = new FilterData(m_doc, new List<BuiltInCategory>(), new List<FilterRuleBuilder>());
            m_dictFilters.Add(newFilterName, m_currentFilterData);
            filtersListBox.Items.Add(newFilterName);
            filtersListBox.SetSelected(filtersListBox.Items.Count - 1, true);
            ResetControls_HasFilter();
        }

        /// <summary>
        ///     Delete one filter
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void deleteButton_Click(object sender, EventArgs e)
        {
            // delete current selected filter
            if (filtersListBox.Items.Count == 0 || filtersListBox.SelectedItems.Count == 0)
                return;
            // 
            // Remove selected items
            var curFilter = filtersListBox.Items[filtersListBox.SelectedIndex] as string;
            m_dictFilters.Remove(curFilter);
            filtersListBox.Items.Remove(curFilter);
            if (filtersListBox.Items.Count > 0)
                filtersListBox.SetSelected(0, true);
            else
                ResetControls_NoFilter();
        }

        /// <summary>
        ///     Categories for filter were changed, clean all old filter rules.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void categoryCheckedListBox_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            // if item is checked by programming, suppress this event
            if (m_catChangedEventSuppress)
                return;
            //
            // Get all selected categories, include one which is going to be checked
            var selCats = new List<BuiltInCategory>();
            var itemCount = categoryCheckedListBox.Items.Count;
            for (var ii = 0; ii < itemCount; ii++)
            {
                // Skip current check item if it's going to be unchecked;
                // add current check item it's going to be checked
                var addItemToChecked = false;
                if (null != e && e.Index == ii)
                {
                    addItemToChecked = e.NewValue == CheckState.Checked;
                    if (!addItemToChecked)
                        continue;
                }

                //
                // add item checked and item is going to be checked
                if (addItemToChecked || categoryCheckedListBox.GetItemChecked(ii))
                {
                    var curCat = categoryCheckedListBox.GetItemText(categoryCheckedListBox.Items[ii]);
                    var param = EnumParseUtility<BuiltInCategory>.Parse(curCat);
                    selCats.Add(param);
                }
            }

            //
            // Reset accordingly controls
            var changed = m_currentFilterData.SetNewCategories(selCats);
            if (!changed)
                return;
            //
            // Update rules controls
            ResetRule_CategoriesChanged();
        }

        /// <summary>
        ///     Check all items of categories
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void checkAllButton_Click(object sender, EventArgs e)
        {
            m_catChangedEventSuppress = true;
            for (var ii = 0; ii < categoryCheckedListBox.Items.Count; ii++)
                categoryCheckedListBox.SetItemChecked(ii, true);
            m_catChangedEventSuppress = false;
            //
            // force call event handler to update accordingly
            categoryCheckedListBox_ItemCheck(null, null);
        }

        /// <summary>
        ///     Cancel check for all items of categories
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void checkNoneButton_Click(object sender, EventArgs e)
        {
            m_catChangedEventSuppress = true;
            for (var ii = 0; ii < categoryCheckedListBox.Items.Count; ii++)
                categoryCheckedListBox.SetItemChecked(ii, false);
            m_catChangedEventSuppress = false;
            categoryCheckedListBox_ItemCheck(null, null);
        }

        /// <summary>
        ///     Hide or show un-checked categories
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void hideUnCheckCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            if (!HasFilterData()) return;

            // reset categories controls, re-add all applicable categories if needed
            ResetCategoriesControl(true);
            //
            // Call event handler to refresh filter rules
            categoryCheckedListBox_ItemCheck(null, null);
        }

        /// <summary>
        ///     Select one filter rule
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void rulesListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            // do nothing if no any filter yet
            if (!HasFilterData())
                return;
            //
            // Change parameter for selected filter rule
            var currentRule = GetCurrentRuleData();
            if (null == currentRule) return;
            var paramName = EnumParseUtility<BuiltInParameter>.Parse(currentRule.Parameter);
            paramerComboBox.SelectedItem = paramName;
        }

        /// <summary>
        ///     Select parameter for current filter rule
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void paramerComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Get criteria for selected parameter and reset rule criteria and rule values
            var selectItem = paramerComboBox.SelectedItem.ToString();
            var isNone = selectItem == NoneParam;
            criteriaComboBox.Enabled = ruleValueComboBox.Enabled = newRuleButton.Enabled = !isNone;
            if (isNone) // is (none) selected
            {
                ResetControlByParamType(StorageType.None);
                return;
            }

            //
            // Check to see if this parameter is in use:
            // Switch to this parameter if parameter is in use, and reset controls with criteria and value for this parameter.
            var curParam = EnumParseUtility<BuiltInParameter>.Parse(selectItem);
            var paramType = m_doc.get_TypeOfStorage(curParam);
            ResetControlByParamType(paramType);
            ParameterInUse(curParam);
            //
            // New parameter was selected, reset controls with new criteria
            var possibleCritias = RuleCriteraNames.Criterions(paramType);
            criteriaComboBox.Items.Clear();
            foreach (var criteria in possibleCritias)
            {
                criteriaComboBox.Items.Add(criteria);
            }

            // 
            // Display parameter values for current filter rule, 
            // If current selected parameter equal to current filter rule's, reset controls with rule data
            var currentRule = GetCurrentRuleData();
            if (null != currentRule && currentRule.Parameter == curParam)
            {
                criteriaComboBox.SelectedItem = currentRule.RuleCriteria;
                ruleValueComboBox.Text = currentRule.RuleValue;
                epsilonTextBox.Text = $"{currentRule.Epsilon:N6}";
            }
            else
            {
                // set with default value
                criteriaComboBox.SelectedIndex = 0;
                ruleValueComboBox.Text = string.Empty;
                epsilonTextBox.Text = "0.0";
            }
        }

        /// <summary>
        ///     Create one new rule
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void newRuleButton_Click(object sender, EventArgs e)
        {
            if (!HasFilterData() || paramerComboBox.SelectedText == NoneParam)
                return;
            //
            // check if rule value is specified or exist
            var curParam = EnumParseUtility<BuiltInParameter>.Parse(paramerComboBox.SelectedItem as string);
            if (ParameterAlreadyExist(curParam))
            {
                MyMessageBox(
                    "Filter rule for this parameter already exists, no sense to add new rule for this parameter again.");
                return;
            }

            var newRule = CreateNewFilterRule(curParam);
            if (null == newRule)
                return;
            // 
            // Create and reserve this rule and reset controls
            m_currentFilterData.RuleData.Add(newRule);
            var ruleName = $"{RuleNamePrefix} {rulesListBox.Items.Count + 1}";
            rulesListBox.Items.Add(ruleName);
            rulesListBox.SelectedIndex = rulesListBox.Items.Count - 1;
            rulesListBox.Enabled = true;
            ViewFiltersForm_MouseMove(null, null);
        }

        /// <summary>
        ///     Delete some filter rule
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void deleRuleButton_Click(object sender, EventArgs e)
        {
            if (rulesListBox.SelectedItems.Count == 0)
            {
                MyMessageBox("Please select filter rule you want to delete.");
                return;
            }

            //
            // Remove the selected item and set the 1st item to be selected auto
            var ruleIndex = rulesListBox.SelectedIndex;
            m_currentFilterData.RuleData.RemoveAt(ruleIndex);
            rulesListBox.Items.RemoveAt(ruleIndex);
            if (rulesListBox.Items.Count > 0)
                rulesListBox.SetSelected(0, true);
        }

        /// <summary>
        ///     Update change rule criteria and values for current filter rule
        ///     Filter criteria and values won't be changed until you click this button.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void updateButton_Click(object sender, EventArgs e)
        {
            var currentRule = GetCurrentRuleData();
            if (null == currentRule) return;
            var selParam = currentRule.Parameter;
            var newRule = CreateNewFilterRule(selParam);
            if (null == newRule) return;
            var oldRuleIndex = GetCurrentRuleDataIndex();
            if (oldRuleIndex > m_currentFilterData.RuleData.Count) return;
            //
            // Update rule value
            m_currentFilterData.RuleData[oldRuleIndex] = newRule;
        }

        /// <summary>
        ///     Enable or disable some buttons according to current status/values of controls
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ViewFiltersForm_MouseMove(object sender, MouseEventArgs e)
        {
            // Enable and disable some buttons:
            // . If current selected parameter in rulesListBox equals to parameter in Parameter ComboBox:
            // Update/Delete is allowed; Otherwise, New and delete is allowed.
            // . If rulesListBox has no any rule, only New button is allowed
            var currentRule = GetCurrentRuleData();
            if (null == currentRule)
            {
                updateButton.Enabled = deleRuleButton.Enabled = false;
                if (!HasFilterData() || null == paramerComboBox.SelectedItem ||
                    paramerComboBox.SelectedItem as string == NoneParam)
                    newRuleButton.Enabled = false;
                else
                    newRuleButton.Enabled = true;
            }
            else
            {
                var selParam = currentRule.Parameter;
                var selComboxParam = EnumParseUtility<BuiltInParameter>.Parse(paramerComboBox.SelectedItem as string);
                var paramEquals = selParam == selComboxParam;
                //
                // new button is available only when user select new parameter
                newRuleButton.Enabled = !paramEquals;
                updateButton.Enabled = deleRuleButton.Enabled = paramEquals;
            }
        }

        /// <summary>
        ///     Update ParameterFilterElements in the Revit document with data from the dialog box.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void okButton_Click(object sender, EventArgs e)
        {
            // Reserve how many Revit filters need to be updated/removed
            ICollection<string> updatedFilters = new List<string>();
            ICollection<ElementId> deleteElemIds = new List<ElementId>();
            var collector = new FilteredElementCollector(m_doc);
            ICollection<Element> oldFilters = collector.OfClass(typeof(ParameterFilterElement)).ToElements();
            //
            // Start transaction to update filters now
            var tran = new Transaction(m_doc, "Update View Filter");
            tran.Start();
            try
            {
                // 1. Update existing filters
                foreach (ParameterFilterElement filter in oldFilters)
                {
                    FilterData filterData;
                    var bExist = m_dictFilters.TryGetValue(filter.Name, out filterData);
                    if (!bExist)
                    {
                        deleteElemIds.Add(filter.Id);
                        continue;
                    }

                    //
                    // Update Filter categories for this filter
                    ICollection<ElementId> newCatIds = filterData.GetCategoryIds();
                    if (!ListCompareUtility<ElementId>.Equals(filter.GetCategories(), newCatIds))
                        filter.SetCategories(newCatIds);

                    // Update filter rules for this filter
                    IList<FilterRule> newRules = new List<FilterRule>();
                    foreach (var ruleData in filterData.RuleData)
                    {
                        newRules.Add(ruleData.AsFilterRule());
                    }

                    var elemFilter = FiltersUtil.CreateElementFilterFromFilterRules(newRules);
                    // Set this filter's list of rules.
                    filter.SetElementFilter(elemFilter);

                    // Remember that we updated this filter so that we do not try to create it again below.
                    updatedFilters.Add(filter.Name);
                }

                //
                // 2. Delete some filters
                if (deleteElemIds.Count > 0)
                    m_doc.Delete(deleteElemIds);
                //
                // 3. Create new filters(if have)
                foreach (var myFilter in m_dictFilters)
                {
                    // If this filter was updated in the previous step, do nothing.
                    if (updatedFilters.Contains(myFilter.Key))
                        continue;

                    // Create a new filter.
                    // Collect the FilterRules, create an ElementFilter representing the
                    // conjunction ("ANDing together") of the FilterRules, and use the ElementFilter
                    // to create a ParameterFilterElement
                    IList<FilterRule> rules = new List<FilterRule>();
                    foreach (var ruleData in myFilter.Value.RuleData)
                    {
                        rules.Add(ruleData.AsFilterRule());
                    }

                    var elemFilter = FiltersUtil.CreateElementFilterFromFilterRules(rules);

                    // Check that the ElementFilter is valid for use by a ParameterFilterElement.
                    var categoryIdList = myFilter.Value.GetCategoryIds();
                    ISet<ElementId> categoryIdSet = new HashSet<ElementId>(categoryIdList);
                    if (!ParameterFilterElement.ElementFilterIsAcceptableForParameterFilterElement(
                            m_doc, categoryIdSet, elemFilter))
                        // In case the UI allowed invalid rules, issue a warning to the user.
                        MyMessageBox("The combination of filter rules is not acceptable for a View Filter.");
                    else
                        ParameterFilterElement.Create(m_doc, myFilter.Key, categoryIdSet, elemFilter);
                }

                // 
                // Commit change now
                tran.Commit();
            }
            catch (Exception ex)
            {
                var failMsg = string.Format("Revit filters update failed and was aborted: " + ex);
                MyMessageBox(failMsg);
                tran.RollBack();
            }
        }

        /// <summary>
        ///     Dynamically adjust the width of parameter so that all parameters can be visible
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void paramerComboBox_DropDown(object sender, EventArgs e)
        {
            var senderComboBox = (ComboBox)sender;
            var width = senderComboBox.DropDownWidth;
            var g = senderComboBox.CreateGraphics();
            var font = senderComboBox.Font;
            var vertScrollBarWidth =
                senderComboBox.Items.Count > senderComboBox.MaxDropDownItems
                    ? SystemInformation.VerticalScrollBarWidth
                    : 0;

            foreach (string s in ((ComboBox)sender).Items)
            {
                var newWidth = (int)g.MeasureString(s, font).Width
                               + vertScrollBarWidth;
                if (width < newWidth) width = newWidth;
            }

            senderComboBox.DropDownWidth = width;
        }

        /// <summary>
        ///     Initialize filter data with existing view filters
        /// </summary>
        private void InitializeFilterData()
        {
            // Get all existing filters
            var filters = FiltersUtil.GetViewFilters(m_doc);
            foreach (var filter in filters)
            {
                // Get all data of the current filter and create my FilterData
                var catIds = filter.GetCategories();

                // Get the ElementFilter representing the set of FilterRules.
                var elemFilter = filter.GetElementFilter();
                // Check that the ElementFilter represents a conjunction of ElementFilters.
                // We will then check that each child ElementFilter contains just one FilterRule.
                var filterRules = FiltersUtil.GetConjunctionOfFilterRulesFromElementFilter(elemFilter);
                var numFilterRules = filterRules.Count;
                if (0 == numFilterRules)
                    return; // Error

                // Create filter rule data now 
                var ruleDataSet = new List<FilterRuleBuilder>();
                foreach (var filterRule in filterRules)
                {
                    var paramId = filterRule.GetRuleParameter();
                    var bip = (BuiltInParameter)paramId.Value;
                    var ruleData = FiltersUtil.CreateFilterRuleBuilder(bip, filterRule);
                    ruleDataSet.Add(ruleData);
                }

                // 
                // Create Filter data
                var filterData = new FilterData(m_doc, catIds, ruleDataSet);
                m_dictFilters.Add(filter.Name, filterData);
                //
                // also add to control 
                filtersListBox.Items.Add(filter.Name);
            }
        }

        /// <summary>
        ///     Check if some filter exists
        /// </summary>
        /// <returns>True if filter already exists, otherwise false.</returns>
        private bool HasFilterData()
        {
            return null != m_currentFilterData;
        }

        /// <summary>
        ///     This method will reset Category check list box with all applicable categories with document
        /// </summary>
        private void AddAppliableCategories()
        {
            categoryCheckedListBox.Items.Clear();
            var filterCatIds = ParameterFilterUtilities.GetAllFilterableCategories();
            foreach (var id in filterCatIds)
            {
                categoryCheckedListBox.Items.Add(EnumParseUtility<BuiltInCategory>.Parse((BuiltInCategory)id.Value));
            }
        }

        /// <summary>
        ///     Reset categories CheckListBox control
        /// </summary>
        /// <param name="reAddAll">Indicates if it's needed to reset control with all applicable categories.</param>
        private void ResetCategoriesControl(bool reAddAll)
        {
            m_catChangedEventSuppress = true; // suppress some events when checking categories during reset 
            ICollection<BuiltInCategory> filterCat = m_currentFilterData.FilterCategories;
            if (hideUnCheckCheckBox.Checked)
            {
                categoryCheckedListBox.Items.Clear();
                foreach (var cat in filterCat)
                {
                    var newCat = EnumParseUtility<BuiltInCategory>.Parse(cat);
                    categoryCheckedListBox.Items.Add(newCat, true);
                }
            }
            else
            {
                // Add all items firstly if needed, happen when user plan to hide none
                if (reAddAll)
                    AddAppliableCategories();
                //
                // Check those categories of current filter
                for (var ii = 0; ii < categoryCheckedListBox.Items.Count; ii++)
                {
                    // set all to unchecked firstly
                    categoryCheckedListBox.SetItemChecked(ii, false);
                    var catName = categoryCheckedListBox.Items[ii] as string;
                    var curCat = EnumParseUtility<BuiltInCategory>.Parse(catName);
                    if (filterCat.Contains(curCat))
                        categoryCheckedListBox.SetItemChecked(ii, true);
                }
            }

            m_catChangedEventSuppress = false;
        }

        /// <summary>
        ///     Reset controls when no any filter
        /// </summary>
        private void ResetControls_NoFilter()
        {
            // uncheck categories, disable/hide controls
            categoryCheckedListBox.Enabled = false;
            for (var ii = 0; ii < categoryCheckedListBox.Items.Count; ii++)
                categoryCheckedListBox.SetItemChecked(ii, false);
            checkAllButton.Enabled = false;
            checkNoneButton.Enabled = false;
            rulesListBox.Enabled = false;
            paramerComboBox.Enabled = false;
            criteriaComboBox.Enabled = false;
            ruleValueComboBox.Enabled = false;
            epsilonLabel.Visible = false;
            epsilonTextBox.Visible = false;
            newRuleButton.Enabled = false;
            deleRuleButton.Enabled = false;
            updateButton.Enabled = false;
        }

        /// <summary>
        ///     Update controls when has filter
        /// </summary>
        private void ResetControls_HasFilter()
        {
            categoryCheckedListBox.Enabled = true;
            checkAllButton.Enabled = true;
            checkNoneButton.Enabled = true;
            paramerComboBox.Enabled = true;
        }

        /// <summary>
        ///     Update control when no filter rule
        /// </summary>
        private void ResetControls_NoFilterRule()
        {
            criteriaComboBox.Enabled = false;
            ruleValueComboBox.Enabled = false;
            newRuleButton.Enabled = false;
            deleRuleButton.Enabled = false;
        }

        /// <summary>
        ///     Reset rules because categories were changed
        /// </summary>
        private void ResetRule_CategoriesChanged()
        {
            // Initialize all supported parameters for selected categories
            ICollection<BuiltInCategory> filterCat = m_currentFilterData.FilterCategories;
            ICollection<ElementId> filterCatIds = new List<ElementId>();
            foreach (var curCat in filterCat)
            {
                filterCatIds.Add(new ElementId(curCat));
            }

            var supportedParams =
                ParameterFilterUtilities.GetFilterableParametersInCommon(m_doc, filterCatIds);
            ResetParameterCombox(supportedParams);
            //
            // Reset filter rules controls and select 1st by default(if have)
            rulesListBox.Items.Clear();
            var ruleData = m_currentFilterData.RuleData;
            for (var ii = 1; ii <= ruleData.Count; ii++) rulesListBox.Items.Add(RuleNamePrefix + ii);
            if (rulesListBox.Items.Count > 0)
                rulesListBox.SetSelected(0, true);
            else
                ResetControls_NoFilterRule();
        }

        /// <summary>
        ///     Update controls' status and visibility according to storage type of current parameter
        /// </summary>
        /// <param name="paramType"></param>
        private void ResetControlByParamType(StorageType paramType)
        {
            switch (paramType)
            {
                case StorageType.String:
                    epsilonLabel.Visible = epsilonTextBox.Visible = false;
                    break;
                case StorageType.Double:
                    epsilonLabel.Visible = epsilonTextBox.Visible = true;
                    epsilonLabel.Enabled = epsilonTextBox.Enabled = true;
                    break;
                default:
                    epsilonLabel.Visible = epsilonTextBox.Visible = false;
                    break;
            }
        }

        /// <summary>
        ///     Check to see if rule for this parameter exists or not
        /// </summary>
        /// <param name="param">Parameter to be checked.</param>
        /// <returns>True if this parameter already has filter rule, otherwise false.</returns>
        private bool ParameterAlreadyExist(BuiltInParameter param)
        {
            if (m_currentFilterData == null || m_currentFilterData.RuleData.Count == 0)
                return false;
            foreach (var rule in m_currentFilterData.RuleData)
            {
                if (rule.Parameter == param)
                    return true;
            }

            return false;
        }

        /// <summary>
        ///     Get FilterRuleBuilder for current filter
        /// </summary>
        /// <returns></returns>
        private FilterRuleBuilder GetCurrentRuleData()
        {
            if (!HasFilterData()) return null;

            var ruleIndex = GetCurrentRuleDataIndex();
            //
            // Se current selected parameters
            return ruleIndex >= 0 ? m_currentFilterData.RuleData[ruleIndex] : null;
        }

        /// <summary>
        ///     Get index for selected filter rule
        /// </summary>
        /// <returns>Index of current rule.</returns>
        private int GetCurrentRuleDataIndex()
        {
            var ruleIndex = -1;
            for (var ii = 0; ii < rulesListBox.Items.Count; ii++)
                if (rulesListBox.GetSelected(ii))
                {
                    ruleIndex = ii;
                    break;
                }

            return ruleIndex;
        }

        /// <summary>
        ///     Reset paramerComboBox with new parameters
        /// </summary>
        /// <param name="paramSet"></param>
        private void ResetParameterCombox(ICollection<ElementId> paramSet)
        {
            paramerComboBox.Items.Clear();
            foreach (var paramId in paramSet)
            {
                paramerComboBox.Items.Add(EnumParseUtility<BuiltInParameter>.Parse((BuiltInParameter)paramId.Value));
            }

            //
            // always added one (none) 
            paramerComboBox.Items.Add(NoneParam);
            paramerComboBox.SelectedIndex = 0;
        }

        /// <summary>
        ///     Check if selected parameter already in use(has criteria)
        /// </summary>
        /// <param name="selParameter">Parameter to be checked.</param>
        /// <returns>True if this parameter already has criteria, otherwise false.</returns>
        private bool ParameterInUse(BuiltInParameter selParameter)
        {
            if (!HasFilterData() || m_currentFilterData.RuleData.Count == 0
                                 || rulesListBox.Items.Count == 0)
                return false;
            //
            // Get all existing rules and check if this parameter is in used
            var paramIndex = 0;
            var paramIsInUse = false;
            ICollection<FilterRuleBuilder> rules = m_currentFilterData.RuleData;
            foreach (var rule in rules)
            {
                if (rule.Parameter == selParameter)
                {
                    paramIsInUse = true;
                    break;
                }

                paramIndex++;
            }

            //
            // If parameter is in use, switch to this parameter and update criteria and rules
            if (paramIsInUse)
                rulesListBox.SetSelected(paramIndex, true);
            return paramIsInUse;
        }

        /// <summary>
        ///     Create new FilterRuleBuilder for current parameter
        /// </summary>
        /// <param name="curParam">Current selected parameter.</param>
        /// <returns>New FilterRuleBuilder for this parameter, null if parameter is not recognizable.</returns>
        private FilterRuleBuilder CreateNewFilterRule(BuiltInParameter curParam)
        {
            var paramType = m_doc.get_TypeOfStorage(curParam);
            var criteria = criteriaComboBox.SelectedItem as string;
            switch (paramType)
            {
                case StorageType.String:
                    return new FilterRuleBuilder(curParam, criteria, ruleValueComboBox.Text);
                case StorageType.Double:
                {
                    double ruleValue = 0, epsilon = 0;
                    if (!GetRuleValueDouble(false, ref ruleValue)) return null;
                    if (!GetRuleValueDouble(true, ref epsilon)) return null;
                    return new FilterRuleBuilder(curParam, criteria,
                        ruleValue, epsilon);
                }
                case StorageType.Integer:
                {
                    var ruleValue = 0;
                    if (!GetRuleValueInt(ref ruleValue)) return null;
                    return new FilterRuleBuilder(curParam, criteria, ruleValue);
                }
                case StorageType.ElementId:
                {
                    long ruleValue = 0;
                    if (!GetRuleValueLong(ref ruleValue)) return null;
                    return new FilterRuleBuilder(curParam, criteria, new ElementId(ruleValue));
                }
                default:
                    return null;
            }
        }

        /// <summary>
        ///     Get rule value of integer type
        /// </summary>
        /// <param name="ruleValue">Integer rule value.</param>
        /// <returns>True if control's text is valid int rule value.</returns>
        private bool GetRuleValueInt(ref int ruleValue)
        {
            try
            {
                ruleValue = int.Parse(ruleValueComboBox.Text);
            }
            catch (Exception)
            {
                MyMessageBox("Rule value is wrong, please input valid value.");
                ruleValueComboBox.Focus();
                return false;
            }

            return true;
        }

        /// <summary>
        ///     Get rule value of long type
        /// </summary>
        /// <param name="ruleValue">Integer rule value.</param>
        /// <returns>True if control's text is valid int rule value.</returns>
        private bool GetRuleValueLong(ref long ruleValue)
        {
            try
            {
                ruleValue = long.Parse(ruleValueComboBox.Text);
            }
            catch (Exception)
            {
                MyMessageBox("Rule value is wrong, please input valid value.");
                ruleValueComboBox.Focus();
                return false;
            }

            return true;
        }

        /// <summary>
        ///     Get rule value of double type from control
        /// </summary>
        /// <param name="isEpsilon">Indicate if method will get value from epsilon control.</param>
        /// <param name="ruleValue">Integer rule value.</param>
        /// <returns>True if control's text is valid int rule value.</returns>
        private bool GetRuleValueDouble(bool isEpsilon, ref double ruleValue)
        {
            try
            {
                if (isEpsilon)
                    ruleValue = double.Parse(epsilonTextBox.Text);
                else
                    ruleValue = double.Parse(ruleValueComboBox.Text);
            }
            catch (Exception)
            {
                if (isEpsilon)
                {
                    MyMessageBox("Epsilon value is invalid, please input valid value!");
                    epsilonTextBox.Focus();
                }
                else
                {
                    MyMessageBox("Rule value is invalid, please input valid value!");
                    ruleValueComboBox.Focus();
                }

                return false;
            }

            //
            // Check the value is valid
            if (double.IsInfinity(ruleValue) || double.IsNaN(ruleValue))
            {
                MyMessageBox("The input value is invalid float value!");
                if (isEpsilon) epsilonTextBox.Focus();
                else ruleValueComboBox.Focus();
                return false;
            }

            return true;
        }

        /// <summary>
        ///     Custom MessageBox for this sample, with special caption/button/icon.
        /// </summary>
        /// <param name="strMsg">Message to be displayed.</param>
        public static void MyMessageBox(string strMsg)
        {
            TaskDialog.Show("View Filters", strMsg, TaskDialogCommonButtons.Ok | TaskDialogCommonButtons.Cancel);
        }
    }
}

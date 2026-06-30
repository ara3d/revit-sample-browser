// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using System;
using System.Collections.Generic;
using System.Windows.Forms;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using ComboBox = System.Windows.Forms.ComboBox;
using Form = System.Windows.Forms.Form;

using Ara3D.RevitSampleBrowser.Common.Documents;
namespace Ara3D.RevitSampleBrowser.ElementFilterSample.CS
{
    public partial class ViewFiltersForm : Form
    {
        private const string NoneParam = "(none)";
        private const string RuleNamePrefix = "Filter Rule ";

        // Suppress categoryCheckedListBox_ItemCheck while SetItemChecked runs programmatically.
        private bool m_catChangedEventSuppress;

        private FilterData m_currentFilterData;
        private readonly Dictionary<string, FilterData> m_dictFilters = new Dictionary<string, FilterData>();
        private readonly Document m_doc;

        public ViewFiltersForm(ExternalCommandData commandData)
        {
            InitializeComponent();
            m_doc = commandData.Application.ActiveUIDocument.Document;
        }

        private void ViewFiltersForm_Load(object sender, EventArgs e)
        {
            SuspendLayout();
            InitializeFilterData();
            AddAppliableCategories();
            if (filtersListBox.Items.Count > 0)
                filtersListBox.SetSelected(0, true);
            else
                ResetControls_NoFilter();
            ResumeLayout();
        }

        private void filtersListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (filtersListBox.Items.Count == 0 || filtersListBox.SelectedItems.Count == 0)
                return;

            var filterName = filtersListBox.SelectedItems[0] as string;
            if (!m_dictFilters.TryGetValue(filterName, out m_currentFilterData))
                return;

            m_catChangedEventSuppress = true;
            ResetCategoriesControl(false);
            m_catChangedEventSuppress = false;
            ResetRule_CategoriesChanged();
        }

        private void newButton_Click(object sender, EventArgs e)
        {
            var inUseKeys = new List<string>();
            inUseKeys.AddRange(m_dictFilters.Keys);
            var newForm = new NewFilterForm(inUseKeys);
            var result = newForm.ShowDialog();
            if (result != DialogResult.OK)
                return;

            // In-memory only until OK commits to Revit.
            var newFilterName = newForm.FilterName;
            m_currentFilterData = new FilterData(m_doc, new List<BuiltInCategory>(), new List<FilterRuleBuilder>());
            m_dictFilters.Add(newFilterName, m_currentFilterData);
            filtersListBox.Items.Add(newFilterName);
            filtersListBox.SetSelected(filtersListBox.Items.Count - 1, true);
            ResetControls_HasFilter();
        }

        private void deleteButton_Click(object sender, EventArgs e)
        {
            if (filtersListBox.Items.Count == 0 || filtersListBox.SelectedItems.Count == 0)
                return;

            var curFilter = filtersListBox.Items[filtersListBox.SelectedIndex] as string;
            m_dictFilters.Remove(curFilter);
            filtersListBox.Items.Remove(curFilter);
            if (filtersListBox.Items.Count > 0)
                filtersListBox.SetSelected(0, true);
            else
                ResetControls_NoFilter();
        }

        private void categoryCheckedListBox_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            if (m_catChangedEventSuppress)
                return;

            // ItemCheck fires before the check state changes; include the pending item from e.
            var selCats = new List<BuiltInCategory>();
            var itemCount = categoryCheckedListBox.Items.Count;
            for (var ii = 0; ii < itemCount; ii++)
            {
                var addItemToChecked = false;
                if (null != e && e.Index == ii)
                {
                    addItemToChecked = e.NewValue == CheckState.Checked;
                    if (!addItemToChecked)
                        continue;
                }

                if (addItemToChecked || categoryCheckedListBox.GetItemChecked(ii))
                {
                    var curCat = categoryCheckedListBox.GetItemText(categoryCheckedListBox.Items[ii]);
                    var param = EnumParseUtility<BuiltInCategory>.Parse(curCat);
                    selCats.Add(param);
                }
            }

            var changed = m_currentFilterData.SetNewCategories(selCats);
            if (!changed)
                return;

            ResetRule_CategoriesChanged();
        }

        private void checkAllButton_Click(object sender, EventArgs e)
        {
            m_catChangedEventSuppress = true;
            for (var ii = 0; ii < categoryCheckedListBox.Items.Count; ii++)
                categoryCheckedListBox.SetItemChecked(ii, true);
            m_catChangedEventSuppress = false;
            categoryCheckedListBox_ItemCheck(null, null);
        }

        private void checkNoneButton_Click(object sender, EventArgs e)
        {
            m_catChangedEventSuppress = true;
            for (var ii = 0; ii < categoryCheckedListBox.Items.Count; ii++)
                categoryCheckedListBox.SetItemChecked(ii, false);
            m_catChangedEventSuppress = false;
            categoryCheckedListBox_ItemCheck(null, null);
        }

        private void hideUnCheckCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            if (!HasFilterData()) return;

            ResetCategoriesControl(true);
            categoryCheckedListBox_ItemCheck(null, null);
        }

        private void rulesListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!HasFilterData())
                return;

            var currentRule = GetCurrentRuleData();
            if (null == currentRule) return;
            var paramName = EnumParseUtility<BuiltInParameter>.Parse(currentRule.Parameter);
            paramerComboBox.SelectedItem = paramName;
        }

        private void paramerComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            var selectItem = paramerComboBox.SelectedItem.ToString();
            var isNone = selectItem == NoneParam;
            criteriaComboBox.Enabled = ruleValueComboBox.Enabled = newRuleButton.Enabled = !isNone;
            if (isNone)
            {
                ResetControlByParamType(StorageType.None);
                return;
            }

            var curParam = EnumParseUtility<BuiltInParameter>.Parse(selectItem);
            var paramType = m_doc.get_TypeOfStorage(curParam);
            ResetControlByParamType(paramType);
            ParameterInUse(curParam);

            var possibleCritias = RuleCriteraNames.Criterions(paramType);
            criteriaComboBox.Items.Clear();
            foreach (var criteria in possibleCritias)
            {
                criteriaComboBox.Items.Add(criteria);
            }

            var currentRule = GetCurrentRuleData();
            if (null != currentRule && currentRule.Parameter == curParam)
            {
                criteriaComboBox.SelectedItem = currentRule.RuleCriteria;
                ruleValueComboBox.Text = currentRule.RuleValue;
                epsilonTextBox.Text = $"{currentRule.Epsilon:N6}";
            }
            else
            {
                criteriaComboBox.SelectedIndex = 0;
                ruleValueComboBox.Text = string.Empty;
                epsilonTextBox.Text = "0.0";
            }
        }

        private void newRuleButton_Click(object sender, EventArgs e)
        {
            if (!HasFilterData() || paramerComboBox.SelectedText == NoneParam)
                return;

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

            m_currentFilterData.RuleData.Add(newRule);
            var ruleName = $"{RuleNamePrefix} {rulesListBox.Items.Count + 1}";
            rulesListBox.Items.Add(ruleName);
            rulesListBox.SelectedIndex = rulesListBox.Items.Count - 1;
            rulesListBox.Enabled = true;
            ViewFiltersForm_MouseMove(null, null);
        }

        private void deleRuleButton_Click(object sender, EventArgs e)
        {
            if (rulesListBox.SelectedItems.Count == 0)
            {
                MyMessageBox("Please select filter rule you want to delete.");
                return;
            }

            var ruleIndex = rulesListBox.SelectedIndex;
            m_currentFilterData.RuleData.RemoveAt(ruleIndex);
            rulesListBox.Items.RemoveAt(ruleIndex);
            if (rulesListBox.Items.Count > 0)
                rulesListBox.SetSelected(0, true);
        }

        // Edits the selected rule in memory; does not write to Revit until OK.
        private void updateButton_Click(object sender, EventArgs e)
        {
            var currentRule = GetCurrentRuleData();
            if (null == currentRule) return;
            var selParam = currentRule.Parameter;
            var newRule = CreateNewFilterRule(selParam);
            if (null == newRule) return;
            var oldRuleIndex = GetCurrentRuleDataIndex();
            if (oldRuleIndex > m_currentFilterData.RuleData.Count) return;
            m_currentFilterData.RuleData[oldRuleIndex] = newRule;
        }

        private void ViewFiltersForm_MouseMove(object sender, MouseEventArgs e)
        {
            // New when combo parameter differs from selected rule; Update/Delete when they match.
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
                newRuleButton.Enabled = !paramEquals;
                updateButton.Enabled = deleRuleButton.Enabled = paramEquals;
            }
        }

        private void okButton_Click(object sender, EventArgs e)
        {
            ICollection<string> updatedFilters = new List<string>();
            ICollection<ElementId> deleteElemIds = new List<ElementId>();
            var collector = new FilteredElementCollector(m_doc);
            ICollection<Element> oldFilters = collector.OfClass(typeof(ParameterFilterElement)).ToElements();

            var tran = new Transaction(m_doc, "Update View Filter");
            tran.Start();
            try
            {
                foreach (ParameterFilterElement filter in oldFilters)
                {
                    FilterData filterData;
                    var bExist = m_dictFilters.TryGetValue(filter.Name, out filterData);
                    if (!bExist)
                    {
                        deleteElemIds.Add(filter.Id);
                        continue;
                    }

                    ICollection<ElementId> newCatIds = filterData.GetCategoryIds();
                    if (!ListCompareUtility<ElementId>.Equals(filter.GetCategories(), newCatIds))
                        filter.SetCategories(newCatIds);

                    IList<FilterRule> newRules = new List<FilterRule>();
                    foreach (var ruleData in filterData.RuleData)
                    {
                        newRules.Add(ruleData.AsFilterRule());
                    }

                    var elemFilter = FilterBuilder.CreateElementFilterFromFilterRules(newRules);
                    filter.SetElementFilter(elemFilter);
                    updatedFilters.Add(filter.Name);
                }

                if (deleteElemIds.Count > 0)
                    m_doc.Delete(deleteElemIds);

                foreach (var myFilter in m_dictFilters)
                {
                    if (updatedFilters.Contains(myFilter.Key))
                        continue;

                    IList<FilterRule> rules = new List<FilterRule>();
                    foreach (var ruleData in myFilter.Value.RuleData)
                    {
                        rules.Add(ruleData.AsFilterRule());
                    }

                    var elemFilter = FilterBuilder.CreateElementFilterFromFilterRules(rules);

                    var categoryIdList = myFilter.Value.GetCategoryIds();
                    ISet<ElementId> categoryIdSet = new HashSet<ElementId>(categoryIdList);
                    if (!ParameterFilterElement.ElementFilterIsAcceptableForParameterFilterElement(
                            m_doc, categoryIdSet, elemFilter))
                        MyMessageBox("The combination of filter rules is not acceptable for a View Filter.");
                    else
                        ParameterFilterElement.Create(m_doc, myFilter.Key, categoryIdSet, elemFilter);
                }

                tran.Commit();
            }
            catch (Exception ex)
            {
                var failMsg = string.Format("Revit filters update failed and was aborted: " + ex);
                MyMessageBox(failMsg);
                tran.RollBack();
            }
        }

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

        private void InitializeFilterData()
        {
            var filters = FilterBuilder.GetViewFilters(m_doc);
            foreach (var filter in filters)
            {
                var catIds = filter.GetCategories();

                var elemFilter = filter.GetElementFilter();
                var filterRules = FilterBuilder.GetConjunctionOfFilterRulesFromElementFilter(elemFilter);
                var numFilterRules = filterRules.Count;
                if (0 == numFilterRules)
                    return;

                var ruleDataSet = new List<FilterRuleBuilder>();
                foreach (var filterRule in filterRules)
                {
                    var paramId = filterRule.GetRuleParameter();
                    var bip = (BuiltInParameter)paramId.Value;
                    var ruleData = FilterBuilder.CreateFilterRuleBuilder(bip, filterRule);
                    ruleDataSet.Add(ruleData);
                }

                var filterData = new FilterData(m_doc, catIds, ruleDataSet);
                m_dictFilters.Add(filter.Name, filterData);
                filtersListBox.Items.Add(filter.Name);
            }
        }

        private bool HasFilterData()
        {
            return null != m_currentFilterData;
        }

        private void AddAppliableCategories()
        {
            categoryCheckedListBox.Items.Clear();
            var filterCatIds = ParameterFilterUtilities.GetAllFilterableCategories();
            foreach (var id in filterCatIds)
            {
                categoryCheckedListBox.Items.Add(EnumParseUtility<BuiltInCategory>.Parse((BuiltInCategory)id.Value));
            }
        }

        private void ResetCategoriesControl(bool reAddAll)
        {
            m_catChangedEventSuppress = true;
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
                if (reAddAll)
                    AddAppliableCategories();

                for (var ii = 0; ii < categoryCheckedListBox.Items.Count; ii++)
                {
                    categoryCheckedListBox.SetItemChecked(ii, false);
                    var catName = categoryCheckedListBox.Items[ii] as string;
                    var curCat = EnumParseUtility<BuiltInCategory>.Parse(catName);
                    if (filterCat.Contains(curCat))
                        categoryCheckedListBox.SetItemChecked(ii, true);
                }
            }

            m_catChangedEventSuppress = false;
        }

        private void ResetControls_NoFilter()
        {
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

        private void ResetControls_HasFilter()
        {
            categoryCheckedListBox.Enabled = true;
            checkAllButton.Enabled = true;
            checkNoneButton.Enabled = true;
            paramerComboBox.Enabled = true;
        }

        private void ResetControls_NoFilterRule()
        {
            criteriaComboBox.Enabled = false;
            ruleValueComboBox.Enabled = false;
            newRuleButton.Enabled = false;
            deleRuleButton.Enabled = false;
        }

        private void ResetRule_CategoriesChanged()
        {
            ICollection<BuiltInCategory> filterCat = m_currentFilterData.FilterCategories;
            ICollection<ElementId> filterCatIds = new List<ElementId>();
            foreach (var curCat in filterCat)
            {
                filterCatIds.Add(new ElementId(curCat));
            }

            var supportedParams =
                ParameterFilterUtilities.GetFilterableParametersInCommon(m_doc, filterCatIds);
            ResetParameterCombox(supportedParams);

            rulesListBox.Items.Clear();
            var ruleData = m_currentFilterData.RuleData;
            for (var ii = 1; ii <= ruleData.Count; ii++) rulesListBox.Items.Add(RuleNamePrefix + ii);
            if (rulesListBox.Items.Count > 0)
                rulesListBox.SetSelected(0, true);
            else
                ResetControls_NoFilterRule();
        }

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

        private FilterRuleBuilder GetCurrentRuleData()
        {
            if (!HasFilterData()) return null;

            var ruleIndex = GetCurrentRuleDataIndex();
            return ruleIndex >= 0 ? m_currentFilterData.RuleData[ruleIndex] : null;
        }

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

        private void ResetParameterCombox(ICollection<ElementId> paramSet)
        {
            paramerComboBox.Items.Clear();
            foreach (var paramId in paramSet)
            {
                paramerComboBox.Items.Add(EnumParseUtility<BuiltInParameter>.Parse((BuiltInParameter)paramId.Value));
            }

            paramerComboBox.Items.Add(NoneParam);
            paramerComboBox.SelectedIndex = 0;
        }

        private bool ParameterInUse(BuiltInParameter selParameter)
        {
            if (!HasFilterData() || m_currentFilterData.RuleData.Count == 0
                                 || rulesListBox.Items.Count == 0)
                return false;

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

            if (paramIsInUse)
                rulesListBox.SetSelected(paramIndex, true);
            return paramIsInUse;
        }

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

            if (double.IsInfinity(ruleValue) || double.IsNaN(ruleValue))
            {
                MyMessageBox("The input value is invalid float value!");
                if (isEpsilon) epsilonTextBox.Focus();
                else ruleValueComboBox.Focus();
                return false;
            }

            return true;
        }

        public static void MyMessageBox(string strMsg)
        {
            TaskDialog.Show("View Filters", strMsg, TaskDialogCommonButtons.Ok | TaskDialogCommonButtons.Cancel);
        }
    }
}

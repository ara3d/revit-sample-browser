// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using System;
using System.Collections.Generic;
using Autodesk.Revit.DB;
using Form = System.Windows.Forms.Form;

namespace Ara3D.RevitSampleBrowser.ViewTemplateCreation.CS
{
    /// <summary>
    ///     A form for the sample, which allows user to create and configure a new view template.
    /// </summary>
    public partial class ViewTemplateCreationForm : Form
    {
        private const string IncludeString = "include";
        private const string ExcludeString = "exclude";
        private readonly Document m_activeDocument;

        private List<View> m_views;

        /// <summary>
        ///     Creates a form for the sample, which allows user to create and configure a new view template.
        /// </summary>
        public ViewTemplateCreationForm(Document activeDocument)
        {
            InitializeComponent();

            Text = Utils.SampleName;
            m_activeDocument = activeDocument;
            InitViewNameComboBox();
            InitPartVisibilityComboBox();
            InitDetailLevelValueComboBox();
        }

        /// <summary>
        ///     Populates the viewNameComboBox with the names of all views from the current document that are valid for view
        ///     template creation.
        /// </summary>
        private void InitViewNameComboBox()
        {
            var viewCollector = new FilteredElementCollector(m_activeDocument);
            viewCollector.OfCategory(BuiltInCategory.OST_Views);

            m_views = new List<View>();
            foreach (View curView in viewCollector)
                if (!curView.IsTemplate && curView.IsViewValidForTemplateCreation())
                {
                    m_views.Add(curView);
                    // add view type to prevent duplication of names with different view types 
                    var extendedViewName = $"{curView.ViewType}:{curView.Name}";
                    viewNameComboBox.Items.Add(extendedViewName);
                }
        }

        private void InitPartVisibilityComboBox()
        {
            partsVisibilityStateComboBox.Items.Add(IncludeString);
            partsVisibilityStateComboBox.Items.Add(ExcludeString);
        }

        private void InitDetailLevelValueComboBox()
        {
            detailLevelValueComboBox.Items.Add(ViewDetailLevel.Coarse.ToString());
            detailLevelValueComboBox.Items.Add(ViewDetailLevel.Medium.ToString());
            detailLevelValueComboBox.Items.Add(ViewDetailLevel.Fine.ToString());
        }

        private void CancelButton_Click(object sender, EventArgs e)
        {
            Close();
        }

        /// <summary>
        ///     Creates a new view template based on the selected view, applying all settings and assigning the view to the newly
        ///     created template.
        /// </summary>
        private void ApplyButton_Click(object sender, EventArgs e)
        {
            try
            {
                using (var transaction = new Transaction(m_activeDocument, "View Template Creation sample"))
                {
                    transaction.Start();

                    var selectedView = GetSelectedView();
                    var viewTemplate = selectedView.CreateViewTemplate();
                    Utils.ShowInformationMessageBox($"View template '{viewTemplate.Name}' has been created.");

                    SetPartsVisibilityIncludeState(viewTemplate);
                    SetDetailLevelValue(viewTemplate);
                    ChangeVgOverridesModelSettings(viewTemplate);

                    selectedView.ViewTemplateId = viewTemplate.Id;
                    Utils.ShowInformationMessageBox(
                        $"View template '{viewTemplate.Name}' has been assigned to '{selectedView.Name}' view.");

                    transaction.Commit();
                }
            }
            catch (Exception ex)
            {
                Utils.ShowWarningMessageBox(ex.ToString());
            }

            Close();
        }

        private View GetSelectedView()
        {
            var extendedViewName = viewNameComboBox.SelectedItem.ToString();
            // retrieve view name and type strings so we can find the right view by name and type
            var idxColon = extendedViewName.IndexOf(":");
            var viewName = extendedViewName.Substring(idxColon + 1);
            var viewType = extendedViewName.Substring(0, idxColon);

            return m_views.Find(v => v.Name == viewName && v.ViewType.ToString() == viewType);
        }

        private void SetPartsVisibilityIncludeState(View view)
        {
            if (!GetSelectedPartsVisibilityIncludeState())
            {
                var nonControlledParameterIds = view.GetNonControlledTemplateParameterIds();
                nonControlledParameterIds.Add(new ElementId(BuiltInParameter.VIEW_PARTS_VISIBILITY));
                view.SetNonControlledTemplateParameterIds(nonControlledParameterIds);
            }
        }

        private void SetDetailLevelValue(View view)
        {
            if (!view.HasDetailLevel())
            {
                Utils.ShowWarningMessageBox($"'{view.Name}' view does not have '{"Detail level"}' parameter.");
                return;
            }

            if (!view.CanModifyDetailLevel())
            {
                Utils.ShowWarningMessageBox($"'{"Detail level"}' can not be modified in view '{view.Name}'.");
                return;
            }

            view.DetailLevel = GetSelectedDetailLevelValue();
        }

        private bool GetSelectedPartsVisibilityIncludeState()
        {
            var partsVisibility = partsVisibilityStateComboBox.SelectedItem.ToString();
            return partsVisibility == IncludeString;
        }

        private ViewDetailLevel GetSelectedDetailLevelValue()
        {
            var newDetailLevelName = detailLevelValueComboBox.SelectedItem.ToString();
            return (ViewDetailLevel)Enum.Parse(typeof(ViewDetailLevel), newDetailLevelName);
        }

        private void ChangeVgOverridesModelSettings(View view)
        {
            if (!view.AreGraphicsOverridesAllowed())
            {
                Utils.ShowWarningMessageBox($"Graphic overrides are not alowed for the '{view.Name}' view");
                return;
            }

            var blackColor = new Color(0, 0, 0);
            var foregroundFillPattern =
                FillPatternElement.GetFillPatternElementByName(view.Document, FillPatternTarget.Drafting,
                    "<Solid fill>");

            SetCutPatternSettings(view, BuiltInCategory.OST_Columns, blackColor, foregroundFillPattern);
            SetCutPatternSettings(view, BuiltInCategory.OST_Doors, blackColor, foregroundFillPattern);
            SetCutPatternSettings(view, BuiltInCategory.OST_Walls, blackColor, foregroundFillPattern);
            SetCutPatternSettings(view, BuiltInCategory.OST_Windows, blackColor, foregroundFillPattern);
        }

        private void SetCutPatternSettings(View view, BuiltInCategory buildInCategory, Color backgroundColor,
            FillPatternElement foregroundFillPattern)
        {
            var categoryId = new ElementId(buildInCategory);

            var ogSettings = view.GetCategoryOverrides(categoryId);
            if (ogSettings == null || !ogSettings.IsValidObject)
            {
                Utils.ShowWarningMessageBox(
                    $"Graphic overrides category '{buildInCategory}' is not found or is not valid");
                return;
            }

            if (!view.IsCategoryOverridable(categoryId))
            {
                Utils.ShowWarningMessageBox(
                    $"Graphic overrides category '{buildInCategory}' is not overridable");
                return;
            }

            ogSettings.SetCutBackgroundPatternColor(backgroundColor);
            ogSettings.SetCutForegroundPatternId(foregroundFillPattern.Id);
            view.SetCategoryOverrides(categoryId, ogSettings);
        }
    }
}

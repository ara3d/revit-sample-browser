// Copyright 2023. See https://github.com/ara3d/revit-samples/LICENSE.txt
using System;
using Autodesk.Revit.DB;
using Form = System.Windows.Forms.Form;

namespace Revit.SDK.Samples.ColorFill.CS
{
    /// <summary>
    ///     This is a dialog should appear that contains the following:
    ///     A list view represents all room scheme names.
    /// </summary>
    public partial class ColorFillForm : Form
    {
        /// <summary>
        ///     constructor
        /// </summary>
        public ColorFillForm(ColorFillMgr colorFillMgr)
        {
            m_colorFillMgr = colorFillMgr;
            InitializeComponent();
            GetData();
        }

        private void GetData()
        {
            m_colorFillMgr.RetrieveData();
            BindData();
        }


        private void BindData()
        {
            tbSchemeName.Text = "";
            tbSchemeTitle.Text = "";

            lstSchemes.DataSource = m_colorFillMgr.RoomSchemes;
            lstSchemes.DisplayMember = "Name";
            lstSchemes.ValueMember = "Id";
            lstSchemes.Refresh();

            lstViews.DataSource = m_colorFillMgr.Views;
            lstViews.DisplayMember = "Name";
            lstViews.ValueMember = "Id";
            lstViews.Refresh();
        }

        private void btnDuplicate_Click(object sender, EventArgs e)
        {
            if (tbSchemeName.Text != string.Empty && tbSchemeTitle.Text != string.Empty)
                try
                {
                    var colorFillScheme = lstSchemes.SelectedItem as ColorFillScheme;
                    m_colorFillMgr.DuplicateScheme(colorFillScheme, tbSchemeName.Text, tbSchemeTitle.Text);
                    lbSchemeResults.Text = string.Format("New scheme {0} is created.", tbSchemeName.Text);
                    GetData();
                }
                catch (Exception ex)
                {
                    lbSchemeResults.Text = ex.Message;
                }
            else
                lbSchemeResults.Text = "Please set the name and title of the new color fill scheme.";

            lbSchemeResults.Refresh();
        }

        private void btnPlaceLegend_Click(object sender, EventArgs e)
        {
            if (lstSchemes.SelectedIndex != -1 && lstViews.SelectedIndex != -1)
                try
                {
                    var colorFillScheme = lstSchemes.SelectedItem as ColorFillScheme;
                    var view = lstViews.SelectedItem as View;
                    m_colorFillMgr.CreateAndPlaceLegend(colorFillScheme, view);
                    lbLegendResults.Text = string.Format("Color Fill legend is placed on view {0}.", view.Name);
                }
                catch (Exception ex)
                {
                    lbLegendResults.Text = ex.Message;
                }
            else
                lbLegendResults.Text = "Please select a scheme and a view to place the legend.";

            lbLegendResults.Refresh();
        }

        private void btnUpdateColor_Click(object sender, EventArgs e)
        {
            try
            {
                var colorFillScheme = lstSchemes.SelectedItem as ColorFillScheme;
                m_colorFillMgr.ModifyByValueScheme(colorFillScheme);

                lbSchemeResults.Text = string.Format("Entries for {0} have been updated.", colorFillScheme.Name);
            }
            catch (Exception ex)
            {
                lbSchemeResults.Text = ex.Message;
            }
        }
    }
}

#region Copyright
// (C) Copyright 2011-2014 by Autodesk, Inc. 
//
// Permission to use, copy, modify, and distribute this software
// in object code form for any purpose and without fee is hereby
// granted, provided that the above copyright notice appears in
// all copies and that both that copyright notice and the limited
// warranty and restricted rights notice below appear in all
// supporting documentation.
//
// AUTODESK PROVIDES THIS PROGRAM "AS IS" AND WITH ALL FAULTS. 
// AUTODESK SPECIFICALLY DISCLAIMS ANY IMPLIED WARRANTY OF
// MERCHANTABILITY OR FITNESS FOR A PARTICULAR USE.  AUTODESK,
// INC. DOES NOT WARRANT THAT THE OPERATION OF THE PROGRAM WILL
// BE UNINTERRUPTED OR ERROR FREE.
//
// Use, duplication, or disclosure by the U.S. Government is
// subject to restrictions set forth in FAR 52.227-19 (Commercial
// Computer Software - Restricted Rights) and DFAR 252.227-7013(c)
// (1)(ii)(Rights in Technical Data and Computer Software), as
// applicable.
#endregion // Copyright

#region Namespaces
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
#endregion // Namespaces

namespace Ara3D.RevitSampleBrowser.StringSearch.CS
{
  public partial class SearchForm : Form
  {
    enum RadioButtonChecked { Selection, View, Project };

    /// <summary>
    /// Remember the last size, location and search 
    /// options when closing the search form, so that 
    /// we can restore them when reopening it next 
    /// time.
    /// </summary>
    static SearchOptions _last_search_options = null;
    static bool _rb_element_type_checked = false;
    static RadioButtonChecked _rb_checked;
    static string _last_category;
    static Point _last_location;
    static Size _last_size;

    const string _all = "*";
    const string _bic_prefix = "OST_";
    const string _bic_invalid = "INVALID";
    const string _bip_invalid = "INVALID";
    string _log_path;
    bool _ok_clicked;

    static List<string> _previous_search_strings
      = new List<string>();

    static List<string> _previous_parameter_names
      = new List<string>();

    public SearchForm( string log_path )
    {
      _log_path = log_path;
      _ok_clicked = false;
      InitializeComponent();

      // Initial height is maximum:

      MaximumSize = new System.Drawing.Size( 800,
        Size.Height );

      // Mimimum height must not obscure the three combo boxes,
      // and minimum width must not obscure the OK button:

      int wmin = Size.Width - ( btnOk.Left - groupBox1.Left );

      int hmin = ( Size.Height - ClientSize.Height )
        + groupBox1.Top + 2 * btnOk.Height;

      MinimumSize = new System.Drawing.Size(
        wmin, hmin );

      PopulateBuiltInCategories();
      PopulateParameterNames();
      cmbSearchString.DataSource = _previous_search_strings;

      this.Load += new EventHandler(
        SearchForm_Load );

      this.FormClosing += new FormClosingEventHandler(
        SearchForm_FormClosing );

      this.SizeChanged += new EventHandler(
        SearchForm_SizeChanged );
    }

    void SearchForm_Load( 
      object sender, 
      EventArgs e )
    {
      if( null != _last_search_options )
      {
        chkMatchCase.Checked = _last_search_options.MatchCase;
        chkWholeWord.Checked = _last_search_options.WholeWord;
        chkRegex.Checked = _last_search_options.Regex;

        chkBuiltInParams.Checked
          = _last_search_options.BuiltInParams;

        chkStandardParams.Checked
          = _last_search_options.StandardParams;

        chkUserParams.Checked = _last_search_options.UserParams;
        rbElementType.Checked = _rb_element_type_checked;

        switch( _rb_checked )
        {
          case RadioButtonChecked.Selection:
            rbSelection.Checked = true;
            break;
          case RadioButtonChecked.View:
            rbView.Checked = true;
            break;
          case RadioButtonChecked.Project:
            rbProject.Checked = true;
            break;
        }
        Location = _last_location;
        Size = _last_size;

        cmbSearchString.Text = _last_search_options.SearchString;

        cmbParameter.Text
          = _last_search_options.ParameterName ?? _all;

        cmbCategory.Text = _last_category;
      }
    }

    void SearchForm_FormClosing(
      object sender,
      FormClosingEventArgs e )
    {
      if( _ok_clicked )
      {
        if( 0 == cmbSearchString.Text.Length )
        {
          e.Cancel = true;
          _ok_clicked = false;
          Command.InfoMsg(
            "Please specify a search string." );
        }

        else if( ElementType && ( CurrentSelection || CurrentView ) )
        {
          e.Cancel = true;
          _ok_clicked = false;
          Command.InfoMsg(
            "Element types can only be searched across the entire project, not when specifying the current selection or current view." );
        }

        _last_search_options = SearchOptions;
        _rb_element_type_checked = ElementType;

        _rb_checked = ( CurrentSelection )
          ? RadioButtonChecked.Selection
          : ( ( CurrentView )
            ? RadioButtonChecked.View
            : RadioButtonChecked.Project );

        _last_category = cmbCategory.Text;
        _last_location = Location;
        _last_size = Size;
      }
    }

    void SearchForm_SizeChanged( 
      object sender, 
      EventArgs e )
    {

#if HARD_WIRED_PIXEL_COUNT
    static int[] _minimum_height = new[] { 235, 348, 418, 485, 505 };
      int h = ClientRectangle.Height;
      groupBox1.Visible = _minimum_height[0] < h;
      groupBox2.Visible = _minimum_height[1] < h;
      groupBox4.Visible = _minimum_height[2] < h;
      groupBox3.Visible = _minimum_height[3] < h;
      label3.Visible = _minimum_height[4] < h;
#endif // HARD_WIRED_PIXEL_COUNT

      int h = btnOk.Top;
      groupBox1.Visible = groupBox1.Bottom < h;
      groupBox2.Visible = groupBox2.Bottom < h;
      groupBox5.Visible = groupBox5.Bottom < h;
      groupBox4.Visible = groupBox4.Bottom < h;
      groupBox3.Visible = groupBox3.Bottom < h;
      label3.Visible = label3.Bottom < h;
    }

    void PopulateBuiltInCategories()
    {
      Type t = typeof( Autodesk.Revit.DB.BuiltInCategory );

      string[] names = Enum.GetNames( t );

      List<string> bics = new List<string>( names.Length );

      foreach( string s in names )
      {
        Debug.Assert( s.Equals( _bic_invalid )
          || s.Substring( 0, 4 ).Equals( _bic_prefix ),
          "Expected all BuiltInCategory enum names to start with OST_" );

        bics.Add( s.Equals( _bic_invalid )
          ? _all
          : s.Substring( 4 ) );
      }

      bics.Sort();

      Debug.Assert( names.Length == bics.Count,
        "Expected all BuiltInCategory enum names to be added" );

      cmbCategory.DataSource = bics;
    }

    void PopulateBuiltInParameters()
    {
      lblParameter.Text = "Built-in parameter or * for all (slow):";

      Type t = typeof( Autodesk.Revit.DB.BuiltInParameter );

      string[] names = Enum.GetNames( t );

      List<string> bips = new List<string>( names.Length );

      foreach( string s in names )
      {
        bips.Add( _bip_invalid.Equals( s )
          ? _all
          : s );
      }

      bips.Sort();

      Debug.Assert( names.Length == bips.Count,
        "Expected all BuiltInCategory enum names to be added" );

      cmbParameter.DropDownStyle = ComboBoxStyle.DropDownList;
      cmbParameter.DataSource = bips;
    }

    void PopulateParameterNames()
    {
      lblParameter.Text = "Parameter name or * for all:";
      cmbParameter.DropDownStyle = ComboBoxStyle.DropDown;
      cmbParameter.DataSource = _previous_parameter_names;
      cmbParameter.Text = _all;
    }

    public string CategoryName
    {
      get
      {
        string s = cmbCategory.Text;
        return _all == s ? s : _bic_prefix + s;
      }
    }

    public string ParameterName
    {
      get
      {
        string s = cmbParameter.Text;
        return _all == s ? null : s;
      }
    }

    public bool AllCategories
    {
      get { return cmbCategory.Text.Equals( _all ); }
    }

    public SearchOptions SearchOptions
    {
      get
      {
        return new SearchOptions(
          cmbSearchString.Text,
          ParameterName,
          chkMatchCase.Checked,
          chkWholeWord.Checked,
          chkRegex.Checked,
          chkBuiltInParams.Checked,
          chkStandardParams.Checked,
          chkUserParams.Checked
        );
      }
    }

    public string SearchString
    {
      get { return cmbSearchString.Text; }
    }

    public bool CurrentSelection
    {
      get { return rbSelection.Checked; }
    }

    public bool CurrentView
    {
      get { return rbView.Checked; }
    }

    public bool ElementType
    {
      get { return rbElementType.Checked; }
    }

    public bool NonElementType
    {
      get { return !rbElementType.Checked; }
    }

#if NEED_INDIVIDUAL_FIELDS
    public bool MatchCase
    {
      get { return chkMatchCase.Checked; }
    }

    public bool WholeWord
    {
      get { return chkWholeWord.Checked; }
    }

    public bool Regex
    {
      get { return chkRegex.Checked; }
    }

    public bool BuiltInParams
    {
      get { return chkBuiltInParams.Checked; }
    }
#endif // NEED_INDIVIDUAL_FIELDS

    private void btnOk_Click(
      object sender,
      EventArgs e )
    {
      if( 0 < SearchString.Length
        && !_previous_search_strings.Contains( SearchString ) )
      {
        _previous_search_strings.Add( SearchString );
      }
      if( !chkBuiltInParams.Checked
        && 0 < cmbParameter.Text.Length
        && !cmbParameter.Text.Equals( _all )
        && !_previous_parameter_names.Contains( cmbParameter.Text ) )
      {
        _previous_parameter_names.Add( cmbParameter.Text );
      }
      _ok_clicked = true;
    }

    private void btnAbout_Click(
      object sender,
      EventArgs e )
    {
      AboutBox a = new AboutBox();
      DialogResult r = a.ShowDialog();
    }

    private void chkBuiltInParams_CheckedChanged(
      object sender,
      EventArgs e )
    {
      if( chkBuiltInParams.Checked )
      {
        PopulateBuiltInParameters();
      }
      else
      {
        PopulateParameterNames();
      }
    }

    private void rbElementType_CheckedChanged(
      object sender,
      EventArgs e )
    {
      if( rbElementType.Checked )
      {
        rbSelection.Checked = false;
        rbView.Checked = false;
        rbProject.Checked = true;

        rbSelection.Enabled = false;
        rbView.Enabled = false;
      }
      else
      {
        rbSelection.Enabled = true;
        rbView.Enabled = true;
      }
    }

    private void aboutToolStripMenuItem_Click(
      object sender,
      EventArgs e )
    {
      AboutBox a = new AboutBox();
      DialogResult r = a.ShowDialog();
    }

    private void helpToolStripMenuItem_Click(
      object sender,
      EventArgs e )
    {
      HelpDlg a = new HelpDlg();
      a.Show();
    }

    private void toolStripMenuItem1_Click(
      object sender,
      EventArgs e )
    {
      Command.InfoMsg(
        "The test suite is still under construction." );
    }

    private void displayLogFileToolStripMenuItem_Click(
      object sender,
      EventArgs e )
    {
      Process.Start( _log_path );
    }

    private void chkStandardParams_CheckedChanged(
      object sender,
      EventArgs e )
    {
      if( !chkStandardParams.Checked )
      {
        chkUserParams.Checked = true;
      }
    }

    private void chkUserParams_CheckedChanged(
      object sender,
      EventArgs e )
    {
      if( !chkUserParams.Checked )
      {
        chkStandardParams.Checked = true;
      }
    }

    private void chkRegex_CheckedChanged(
      object sender,
      EventArgs e )
    {
      if( chkRegex.Checked )
      {
        chkWholeWord.Checked = false;
        chkWholeWord.Enabled = false;
      }
      else
      {
        chkWholeWord.Enabled = true;
      }
    }
  }
}

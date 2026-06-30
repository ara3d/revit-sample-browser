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
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using BuildingCoder;
#endregion // Namespaces

namespace Ara3D.RevitSampleBrowser.StringSearch.CS
{
  /// <summary>
  /// Modeless form to dsiplay a list of all string search hits.
  /// User can double click on any row to zoom to and highlight
  /// that element.
  /// </summary>
  partial class SearchHitNavigator : Form
  {
    static SearchHitNavigator _singleton_instance = null;
    static Point? _last_location = null;
    static Size _last_size;

    public static bool IsShowing
    {
      get
      {
        return null != _singleton_instance;
      }
    }

    public static void Show(
      SortableBindingList<SearchHit> data,
      StringSearchHost.SetElementId set_id,
      JtWindowHandle h )
    {
      if( null == _singleton_instance )
      {
        _singleton_instance
          = new SearchHitNavigator(
            data, set_id );

        _singleton_instance.Load 
          += new EventHandler( OnLoad );

        _singleton_instance.FormClosing
          += new FormClosingEventHandler(
            OnFormClosing );

        _singleton_instance.FormClosing
          += new FormClosingEventHandler(
            OnFormClosing );

        _singleton_instance.Disposed 
          += new EventHandler( OnDisposed );

        _singleton_instance.Show( h );
      }
      else
      {
        _singleton_instance.dataGridView1.DataSource
          = data;
      }
    }

    public static void Shutdown()
    {
      if( null != _singleton_instance )
      {
        _singleton_instance.Close();
      }
    }

    static void OnLoad( 
      object sender, 
      EventArgs e )
    {
      if( null != _last_location )
      {
        _singleton_instance.Location 
          = ( Point ) _last_location;

        _singleton_instance.Size = _last_size;
      }
    }

    static void OnFormClosing( 
      object sender, 
      FormClosingEventArgs e )
    {
      _last_location = _singleton_instance.Location;
      _last_size = _singleton_instance.Size;
    }

    static void OnDisposed( 
      object sender, 
      EventArgs e )
    {
      _singleton_instance = null;
    }

    StringSearchHost.SetElementId _set_id;

    SearchHitNavigator(
      SortableBindingList<SearchHit> a,
      StringSearchHost.SetElementId set_id )
    {
      InitializeComponent();
      dataGridView1.DataSource = a;
      dataGridView1.CellDoubleClick 
        += new DataGridViewCellEventHandler( 
          dataGridView1_CellDoubleClick );
      _set_id = set_id;
    }

    void SetElementIdFromRow(
      int rowIndex,
      bool doubleClick )
    {
      // Do something on double click, 
      // except when on the header:

      if( rowIndex > -1 )
      {
        DataGridViewRow row
          = dataGridView1.Rows[rowIndex];

        int n = row.Cells.Count;

        DataGridViewCell cell = row.Cells[n - 1];

        long id = Convert.ToInt64(cell.Value);

        _set_id(id);

        Debug.Print(
          "{0} click on row {1} --> element id {2}",
          doubleClick ? "Double" : "Single",
          rowIndex, id );
      }
    }

    void dataGridView1_CellDoubleClick( 
      object sender, 
      DataGridViewCellEventArgs e )
    {
      SetElementIdFromRow( e.RowIndex, true );
    }
  }
}

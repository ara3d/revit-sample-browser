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
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;
using Autodesk.Revit.DB;
#endregion // Namespaces

namespace Ara3D.RevitSampleBrowser.StringSearch.CS
{
  /// <summary>
  /// Search for a string in the elements of a given 
  /// filtered element collector using a given set 
  /// of search options. The real string search uses
  /// the .NET Regex Matches method.
  /// </summary>
  class StringSearcher
  {
    class Counters
    {
      public Counters()
      {
        ElementsSearched = 0;
        ElementsHit = 0;
        ParametersSearched = 0;
        ParametersHit = 0;
        Hits = 0;
      }
      public int ElementsSearched { get; set; }
      public int ElementsHit { get; set; }
      public int ParametersSearched { get; set; }
      public int ParametersHit { get; set; }
      public int Hits { get; set; }
    }

    /// <summary>
    /// Return an English plural suffix 's' or
    /// nothing for the given number of items.
    /// </summary>
    public static string PluralSuffix( int n )
    {
      return 1 == n ? "" : "s";
    }

    FilteredElementCollector _collector;
    SearchOptions _searchOptions;
    Counters _counters;

    /// <summary>
    /// Retrieve a list of all *unique* 
    /// built-in parameter enum values.
    /// </summary>
    static int[] _bips
      = Enum.GetValues( typeof( BuiltInParameter ) )
        .Cast<int>().ToArray()
        .Distinct().ToArray();

    static string EscapeRegexChars( string s )
    {
      return s;
    }

    static string GetParameterString(
      Element e,
      Parameter p )
    {
      string s = null;

      switch( p.StorageType )
      {
        case StorageType.Integer:
          s = p.AsInteger().ToString();
          //Debug.Assert( s.Equals( p.AsValueString() ), 
          //  "expected integer representation to match value string" );
          break;
        case StorageType.Double:
          s = p.AsDouble().ToString();
          //Debug.Assert( s.Equals( p.AsValueString() ), 
          //  "expected double representation to match value string" );
          break;
        case StorageType.String:
          s = p.AsString();
          //Debug.Assert( s.Equals( p.AsValueString() ), 
          //  "expected string raw value to match value string" );
          break;
        case StorageType.ElementId:
          s = p.AsElementId().Value.ToString();
          //Debug.Assert( s.Equals( p.AsValueString() ), 
          //  "expected element id representation to match value string" );
          break;
      }

      //Debug.Assert( null != s,
      //  "expected as valid parameter value string" );

      return s;
    }

    /// <summary>
    /// Return number of occurrences of 
    /// search string in parameter value.
    /// </summary>
    bool SearchParameter(
      SortableBindingList<SearchHit> data,
      Element e,
      Parameter p,
      string bipName,
      Regex regex )
    {
      if( null == p )
      {
        return false;
      }

      if( StorageType.String != p.StorageType )
      {
        return false;
      }

      // Do not search non-visible user-defined 
      // parameters, e.g. data storage used by 
      // many applications prior to 2012:

      Definition def = p.Definition;

      ExternalDefinition extdef = p.Definition
        as ExternalDefinition;

      if( null != extdef && !extdef.Visible )
      {
        return false;
      }

      InternalDefinition intdef = def
        as InternalDefinition;

      if( null != intdef && !_searchOptions.StandardParams )
      {
        return false;
      }

      if( null != extdef && !_searchOptions.UserParams )
      {
        return false;
      }

      string s = GetParameterString( e, p );

      if( null == s || 0 == s.Length )
      {
        return false;
      }

      string bipName2 = ( null == intdef )
        ? null
        : intdef.BuiltInParameter.ToString();

      if( null == bipName )
      {
        bipName = bipName2;
      }
      else
      {
        Debug.Assert( !_searchOptions.BuiltInParams
          || bipName.Equals( bipName2 ),
          "expected equal built-in parameter name" );
      }

      bool foundOnParam = false;

      MatchCollection matches = regex.Matches( s );

      foreach( Match m in matches )
      {
        if( foundOnParam && 0 == m.Length )
        {
          // ".*" causes two matches, one at the beginning 
          // and one ot the end of the search string; the
          // second has a length of zero, so we skip it:

          continue;
        }

        ++_counters.Hits;
        foundOnParam = true;

        SearchHit hit = new SearchHit(
          e, bipName, def.Name, s, m.Index );

        data.Add( hit );
      }

      ++_counters.ParametersSearched;

      if( foundOnParam )
      {
        ++_counters.ParametersHit;
      }
      return foundOnParam;
    }

    public StringSearcher(
      FilteredElementCollector collector,
      SearchOptions searchOptions )
    {
      _collector = collector;
      _searchOptions = searchOptions;
      _counters = new Counters();
    }

    public SortableBindingList<SearchHit> Run(
      JtLogFile log,
      out string message )
    {
      message = string.Empty;

      SortableBindingList<SearchHit> data
        = new SortableBindingList<SearchHit>();

      Parameter p;

      // Do we search for some specific parameters?

      List<Definition> defs = null;

      string parameterName = _searchOptions.ParameterName;

      if( null != parameterName )
      {
        foreach( Element e in _collector )
        {
          if( _searchOptions.BuiltInParams )
          {
            BuiltInParameter bip = ( BuiltInParameter ) Enum.Parse(
              typeof( BuiltInParameter ), parameterName, true );

            p = e.get_Parameter( bip );

            if( null != p )
            {
              defs = new List<Definition>( 1 );
              defs.Add( p.Definition );
            }
          }
          else
          {
            //p = e.get_Parameter( parameterName );

            IList<Parameter> ps = e.GetParameters( 
              parameterName );

            if( 0 < ps.Count )
            {
              defs = new List<Definition>( ps.Count );

              foreach( Parameter q in ps )
              {
                defs.Add( q.Definition );
              }
            }
          }
          //if( null != p )
          //{
          //  def = p.Definition;
          //  break;
          //}
        }
        if( null == defs )
        {
          message = string.Format(
            "None of the selected elements have any parameter '{0}'",
            parameterName );

          return data;
        }
      }

      // Set up regular expression for search string

      string searchText = _searchOptions.SearchString;

      RegexOptions opt = RegexOptions.Compiled;

      if( !_searchOptions.Regex )
      {
        searchText = Regex.Escape( searchText );
      }
      if( !_searchOptions.MatchCase )
      {
        opt |= RegexOptions.IgnoreCase;
      }
      if( _searchOptions.WholeWord )
      {
        searchText = "^" + searchText + "$";
      }

      Regex regex = new Regex( searchText, opt );

      // Start searching

      foreach( Element e in _collector )
      {
        log.Log( "Document: " + e.Document.Title );
        break;
      }
      log.Log( "Search string: " + _searchOptions.SearchString );
      log.Log( "Parameter: " + _searchOptions.ParameterName );
      log.Log( "Match case: " + _searchOptions.MatchCase );
      log.Log( "Whole word: " + _searchOptions.WholeWord );
      log.Log( "Regular expression: " + _searchOptions.Regex );

      log.Log( "Built-in parameters: " 
        + _searchOptions.BuiltInParams );

      bool foundOnElement;

      foreach( Element e in _collector )
      {
        foundOnElement = false;

        if( null != defs )
        {
          foreach( Definition d in defs )
          {
            p = e.get_Parameter( d );
            foundOnElement |= SearchParameter(
              data, e, p, parameterName, regex );
          }
        }
        else if( _searchOptions.BuiltInParams )
        {
          foreach( BuiltInParameter a in _bips )
          {
            // Handle a potential exception for access
            // to certain parameters. This should not
            // be needed, because get_Parameter should 
            // simply return null if the parameter is 
            // not available or inaccessible. Currently 
            // only required due to SPR #211511 [exceptions 
            // accessing built-in parameters on views]:

            try
            {
              p = e.get_Parameter( a );

              foundOnElement |= SearchParameter(
                data, e, p, a.ToString(), regex );
            }
            catch( Exception ex )
            {
              SearchHit h = new SearchHit(
                e, a.ToString(), "", "", 0 );

              Debug.Print( string.Format(
                "{0} searching {1}",
                ex.Message, h.ToString() ) );
            }
          }
        }
        else
        {
          ParameterSet pset = e.Parameters;

          foreach( Parameter q in pset )
          {
            foundOnElement |= SearchParameter( 
              data, e, q, null, regex );
          }
        }
        ++_counters.ElementsSearched;

        if( foundOnElement )
        {
          ++_counters.ElementsHit;
        }
      }
      log.Log( string.Format(
        "{0} element{1} and {2} parameter{3} searched.",
        _counters.ElementsSearched,
        PluralSuffix( _counters.ElementsSearched ),
        _counters.ParametersSearched,
        PluralSuffix( _counters.ParametersSearched ) ) );

      log.Log( string.Format(
        "{0} total hit{1} found on {2} element{3} and {4} parameter{5}{6}",
        _counters.Hits,
        PluralSuffix( _counters.Hits ),
        _counters.ElementsHit,
        PluralSuffix( _counters.ElementsHit ),
        _counters.ParametersHit,
        PluralSuffix( _counters.ParametersHit ),
        0 == _counters.Hits ? "." : ":" ) );

      foreach( SearchHit h in data )
      {
        log.Log( h.ToString() );
      }

      return data;
    }
  }
}

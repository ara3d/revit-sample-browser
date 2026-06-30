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
using System.IO;
#endregion // Namespaces

namespace Ara3D.RevitSampleBrowser.StringSearch.CS
{
  /// <summary>
  /// Helper class to manage a log file.
  /// </summary>
  class JtLogFile : IDisposable
  {
    string _path;
    StreamWriter _sw;

    public JtLogFile( string basename )
    {
      _path = System.IO.Path.Combine(
        System.IO.Path.GetTempPath(),
        basename + ".log" );

      _sw = new StreamWriter( _path, true );

      _sw.WriteLine( "\r\n\r\n{0} Start string search\r\n",
        DateTime.Now.ToString( "u" ) );
    }

    public void Dispose()
    {
      _sw.WriteLine( "\r\n\r\n{0} Terminate string search\r\n",
        DateTime.Now.ToString( "u" ) );

      _sw.Close();
      _sw.Dispose();
    }

    /// <summary>
    /// Log a new entry to the file.
    /// </summary>
    public void Log( string s )
    {
      _sw.WriteLine( s );
      Debug.WriteLine( s );
    }

    public string Path
    {
      get
      {
        return _path;
      }
    }
  }
}

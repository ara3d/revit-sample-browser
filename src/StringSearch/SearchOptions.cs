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
using System.Text;
#endregion // Namespaces

namespace Ara3D.RevitSampleBrowser.StringSearch.CS
{
  public class SearchOptions
  {
    public SearchOptions(
      string searchString,
      string parameterName,
      bool matchCase,
      bool wholeWord,
      bool regex,
      //bool elementType,
      //bool nonElementType,
      bool builtInParams,
      //bool stringValued,
      //bool intValued,
      //bool realValued,
      //bool elementIdValued,
      //bool nonStringValued 
      bool standardParams,
      bool userParams
    )
    {
      SearchString = searchString;
      ParameterName = parameterName;
      MatchCase = matchCase;
      WholeWord = wholeWord;
      Regex = regex;
      //ElementType = elementType;
      //NonElementType = nonElementType;
      BuiltInParams = builtInParams;
      //StringValued = stringValued;
      //IntValued = intValued;
      //RealValued = realValued;
      //ElementIdValued = elementIdValued;
      //NonStringValued = nonStringValued;
      StandardParams = standardParams;
      UserParams = userParams;
    }

    public string SearchString { get; set; }
    public string ParameterName { get; set; }
    public bool MatchCase { get; set; }
    public bool WholeWord { get; set; }
    public bool Regex { get; set; }
    //public bool ElementType { get; set; }
    //public bool NonElementType { get; set; }
    public bool BuiltInParams { get; set; }
    //public bool StringValued { get; set; }
    //public bool IntValued { get; set; }
    //public bool RealValued { get; set; }
    //public bool ElementIdValued { get; set; }
    //public bool NonStringValued { get; set; }
    public bool StandardParams { get; set; }
    public bool UserParams { get; set; }
  }
}

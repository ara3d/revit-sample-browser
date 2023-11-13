﻿//
// (C) Copyright 2003-2019 by Autodesk, Inc.
//
// Permission to use, copy, modify, and distribute this software in
// object code form for any purpose and without fee is hereby granted,
// provided that the above copyright notice appears in all copies and
// that both that copyright notice and the limited warranty and
// restricted rights notice below appear in all supporting
// documentation.
//
// AUTODESK PROVIDES THIS PROGRAM "AS IS" AND WITH ALL FAULTS.
// AUTODESK SPECIFICALLY DISCLAIMS ANY IMPLIED WARRANTY OF
// MERCHANTABILITY OR FITNESS FOR A PARTICULAR USE. AUTODESK, INC.
// DOES NOT WARRANT THAT THE OPERATION OF THE PROGRAM WILL BE
// UNINTERRUPTED OR ERROR FREE.
//
// Use, duplication, or disclosure by the U.S. Government is subject to
// restrictions set forth in FAR 52.227-19 (Commercial Computer
// Software - Restricted Rights) and DFAR 252.227-7013(c)(1)(ii)
// (Rights in Technical Data and Computer Software), as applicable.
//
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Autodesk.Revit.UI;
using Autodesk.Revit.DB;

namespace Revit.SDK.Samples.PerformanceAdviserControl.CS
{
   /// <summary>
   /// A simple data class that holds Performance Adviser rule information for use with
   /// TestDisplayDialog
   /// </summary>
   public class RuleInfo
   {
      public RuleInfo(PerformanceAdviserRuleId id, bool isOurRule, string name, string description, bool isEnabled)
      {
         ID = id;
         IsOurRule = isOurRule;
         RuleName = name;
         RuleDescription = description;
         IsEnabled = isEnabled;
      }

      public readonly PerformanceAdviserRuleId ID;
      public readonly bool IsOurRule;
      public readonly string RuleName;
      public readonly string RuleDescription;
      public readonly bool IsEnabled;
   }
}

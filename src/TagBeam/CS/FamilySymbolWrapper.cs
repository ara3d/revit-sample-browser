//
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
using System.Text;
using Autodesk.Revit.DB;

namespace Revit.SDK.Samples.TagBeam.CS
{
    /// <summary>
    /// A wrapper of family symbol
    /// </summary>
    public class FamilySymbolWrapper
    {
        FamilySymbol m_familySymbol;

        /// <summary>
        /// Family symbol
        /// </summary>
        public FamilySymbol FamilySymbol
        {
            get { return m_familySymbol; }
        }

        /// <summary>
        /// Display name
        /// </summary>
        public string Name
        {
            get { return m_familySymbol.Family.Name + " : " + m_familySymbol.Name; }
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="tagSymbol"></param>
        public FamilySymbolWrapper(FamilySymbol familySymbol)
        {
            m_familySymbol = familySymbol;
        }
    };
}

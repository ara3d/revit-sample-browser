// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

//
// AUTODESK PROVIDES THIS PROGRAM 'AS IS' AND WITH ALL ITS FAULTS.
// AUTODESK SPECIFICALLY DISCLAIMS ANY IMPLIED WARRANTY OF
// MERCHANTABILITY OR FITNESS FOR A PARTICULAR USE. AUTODESK, INC.
// DOES NOT WARRANT THAT THE OPERATION OF THE PROGRAM WILL BE
// UNINTERRUPTED OR ERROR FREE.
//
// Use, duplication, or disclosure by the U.S. Government is subject to
// restrictions set forth in FAR 52.227-19 (Commercial Computer
// Software - Restricted Rights) and DFAR 252.227-7013(c)(1)(ii)
// (Rights in Technical Data and Computer Software), as applicable. 

using System.Collections;

namespace Ara3D.RevitSampleBrowser.CloudAPISample.CS.Coroutine
{
    public class Coroutine
    {
        private readonly IEnumerator m_innerEnumerator;

        public Coroutine(IEnumerator coroutine)
        {
            m_innerEnumerator = coroutine;
        }

        public bool IsFinished { get; set; }

        public Coroutine Previous { get; set; }

        public Coroutine Next { get; set; }

        public bool ExecuteOnStep()
        {
            return m_innerEnumerator.MoveNext();
        }
    }
}

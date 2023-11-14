// Copyright 2023. See https://github.com/ara3d/revit-samples/LICENSE.txt

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

namespace Revit.SDK.Samples.CloudAPISample.CS.Coroutine
{
    /// <summary>
    ///     Represent a coroutine instance.
    /// </summary>
    public class Coroutine
    {
        private readonly IEnumerator m_innerEnumerator;

        /// <summary>
        ///     Create a coroutine with a enumerator
        /// </summary>
        /// <param name="coroutine"></param>
        public Coroutine(IEnumerator coroutine)
        {
            m_innerEnumerator = coroutine;
        }

        /// <summary>
        ///     Indicates if this coroutine is finished.
        /// </summary>
        public bool IsFinished { get; set; }

        /// <summary>
        ///     The previous coroutine in this double linked list
        /// </summary>
        public Coroutine Previous { get; set; }

        /// <summary>
        ///     The next coroutine in this double linked list
        /// </summary>
        public Coroutine Next { get; set; }

        /// <summary>
        ///     Execute one step of the enumerator
        /// </summary>
        /// <returns></returns>
        public bool ExecuteOnStep()
        {
            return m_innerEnumerator.MoveNext();
        }
    }
}

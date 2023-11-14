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

using System;
using System.Collections;
using System.Windows.Threading;

namespace RevitMultiSample.CloudAPISample.CS.Coroutine
{
    /// <summary>
    ///     A simple coroutine scheduler.
    ///     This coroutine scheduler allows for control over the execution regime
    ///     of a set of coroutines.
    /// </summary>
    public class CoroutineScheduler
    {
        private static CoroutineScheduler _instance;

        private Coroutine m_coroutines;

        private DispatcherTimer m_dispatcherTimer;

        private CoroutineScheduler()
        {
        }

        private void Attach()
        {
            m_dispatcherTimer = new DispatcherTimer();
            m_dispatcherTimer.Tick += Update;
            m_dispatcherTimer.Interval = new TimeSpan(0, 0, 1);
            m_dispatcherTimer.Start();
        }

        private void Detach()
        {
            m_dispatcherTimer?.Stop();
            m_dispatcherTimer = null;
        }

        private void AddCoroutine(Coroutine coroutine)
        {
            if (m_coroutines != null)
            {
                coroutine.Next = m_coroutines;
                m_coroutines.Previous = coroutine;
            }

            m_coroutines = coroutine;
        }

        private void RemoveCoroutine(Coroutine coroutine)
        {
            if (m_coroutines == coroutine)
            {
                m_coroutines = m_coroutines.Next;
            }
            else
            {
                if (coroutine.Next != null)
                {
                    coroutine.Previous.Next = coroutine.Next;
                    coroutine.Next.Previous = coroutine.Previous;
                }
                else if (coroutine.Previous != null)
                {
                    coroutine.Previous.Next = null;
                }
            }

            coroutine.Previous = null;
            coroutine.Next = null;
        }

        private void Update(object sender, EventArgs eventArgs)
        {
            UpdateAllCoroutines();
        }

        private void UpdateAllCoroutines()
        {
            var iter = m_coroutines;

            while (iter != null)
            {
                if (!iter.ExecuteOnStep())
                {
                    iter.IsFinished = true;
                    RemoveCoroutine(iter);
                }

                iter = iter.Next;
            }
        }

        /// <summary>
        ///     Attach a scheduler to render loop.
        ///     Must be called before using coroutine.
        /// </summary>
        public static void Run()
        {
            if (_instance != null)
                return;
            _instance = new CoroutineScheduler();
            _instance.Attach();
        }

        /// <summary>
        ///     Stop a scheduler. All coroutines will be released.
        /// </summary>
        public static void Stop()
        {
            _instance?.Detach();
            _instance = null;
        }

        /// <summary>
        ///     Start a new coroutine with an enumerator
        /// </summary>
        /// <returns></returns>
        public static Coroutine StartCoroutine(IEnumerator enumerator)
        {
            if (enumerator == null || _instance == null) return null;

            var coroutine = new Coroutine(enumerator);
            _instance.AddCoroutine(coroutine);
            return coroutine;
        }
    }
}

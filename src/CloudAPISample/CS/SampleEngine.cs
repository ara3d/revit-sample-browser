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
using System.Collections.Generic;
using Autodesk.Revit.UI;
using RevitMultiSample.CloudAPISample.CS.Coroutine;
using RevitMultiSample.CloudAPISample.CS.View;

namespace RevitMultiSample.CloudAPISample.CS
{
    internal class SampleEngine : IDisposable
    {
        private readonly List<SampleContext> m_allContext = new List<SampleContext>();
        private readonly UIApplication m_application;
        private ViewSamplePortal m_viewPortal;

        public SampleEngine(UIApplication app)
        {
            m_application = app;
            m_viewPortal = new ViewSamplePortal(app);

            CoroutineScheduler.Run();
        }

        /// <inheritdoc />
        public void Dispose()
        {
            foreach (var context in m_allContext)
            {
                context.Terminate();
                context.Application = null;
            }

            m_allContext.Clear();
            m_viewPortal?.Close();
            m_viewPortal = null;

            CoroutineScheduler.Stop();
        }

        public void RegisterSample(string name, SampleContext sampleContext)
        {
            sampleContext.Application = m_application;
            m_allContext.Add(sampleContext);

            m_viewPortal.AddTab(name, sampleContext.View);
        }

        public void Run()
        {
            m_viewPortal.ShowDialog();
        }
    }
}

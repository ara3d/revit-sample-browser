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

using System;
using System.Linq;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace Ara3D.RevitSampleBrowser.Site.CS
{
    public class TopographyEditFailuresPreprocessor : IFailuresPreprocessor
    {
        public FailureProcessingResult PreprocessFailures(FailuresAccessor failuresAccessor)
        {
            return FailureProcessingResult.Continue;
        }
    }

    public class TopographyEditFailuresPreprocessorVerbose : IFailuresPreprocessor
    {
        // For debugging
        public FailureProcessingResult PreprocessFailures(FailuresAccessor failuresAccessor)
        {
            try
            {
                TaskDialog.Show("Preprocess failures", "Hello");
                var failureMessages = failuresAccessor.GetFailureMessages();
                var numberOfFailures = failureMessages.Count;
                TaskDialog.Show("Preprocess failures", "Found " + numberOfFailures + " failure messages.");
                if (numberOfFailures < 5)
                    foreach (var msgAccessor in failureMessages)
                        TaskDialog.Show("Failure!", msgAccessor.GetDescriptionText());
                else
                    TaskDialog.Show("Failure 1 of " + numberOfFailures, failureMessages.First().GetDescriptionText());
                TaskDialog.Show("Preprocess failures", "Goodbye");
                return FailureProcessingResult.Continue;
            }
            catch (Exception e)
            {
                TaskDialog.Show("Exception", e.ToString());
                return FailureProcessingResult.ProceedWithRollBack;
            }
        }
    }
}

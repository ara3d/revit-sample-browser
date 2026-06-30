// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using System;

namespace Ara3D.RevitSampleBrowser.CreateBeamSystem.CS
{
    public class ErrorMessageException : ApplicationException
    {
        public ErrorMessageException()
        {
        }

        public ErrorMessageException(string message)
            : base(message)
        {
        }
    }
}

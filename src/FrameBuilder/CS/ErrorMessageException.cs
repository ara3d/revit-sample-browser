// Copyright 2023. See https://github.com/ara3d/revit-samples/LICENSE.txt

using System;

namespace RevitMultiSample.FrameBuilder.CS
{
    /// <summary>
    ///     pass error message to UI or back to internal error messagebox by Execute method in IExternalCommand
    /// </summary>
    internal class ErrorMessageException : ApplicationException
    {
        /// <summary>
        ///     constructor entirely using baseclass'
        /// </summary>
        public ErrorMessageException()
        {
        }

        /// <summary>
        ///     constructor entirely using baseclass'
        /// </summary>
        /// <param name="message">error message</param>
        public ErrorMessageException(string message)
            : base(message)
        {
        }
    }
}

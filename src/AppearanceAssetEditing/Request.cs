// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using System.Threading;

namespace Ara3D.RevitSampleBrowser.AppearanceAssetEditing.CS
{
    /// <summary>
    ///     A list of requests the dialog has available
    /// </summary>
    public enum RequestId
    {
        /// <summary>
        ///     None
        /// </summary>
        None = 0,

        /// <summary>
        ///     "Select" request
        /// </summary>
        Select = 1,

        /// <summary>
        ///     "Lighter" request
        /// </summary>
        Lighter = 2,

        /// <summary>
        ///     "Darker" request
        /// </summary>
        Darker = 3
    }

    /// <summary>
    ///     A class around a variable holding the current request.
    /// </summary>
    /// <remarks>
    ///     Access to it is made thread-safe, even though we don't necessarily
    ///     need it if we always disable the dialog between individual requests.
    /// </remarks>
    public class Request
    {
        // Storing the value as a plain Int makes using the interlocking mechanism simpler
        private int m_request = (int)RequestId.None;

        /// <summary>
        ///     Take - The Idling handler calls this to obtain the latest request.
        /// </summary>
        /// <remarks>
        ///     This is not a getter! It takes the request and replaces it
        ///     with 'None' to indicate that the request has been "passed on".
        /// </remarks>
        public RequestId Take()
        {
            return (RequestId)Interlocked.Exchange(ref m_request, (int)RequestId.None);
        }

        /// <summary>
        ///     Make - The Dialog calls this when the user presses a command button there.
        /// </summary>
        /// <remarks>
        ///     It replaces any older request previously made.
        /// </remarks>
        public void Make(RequestId request)
        {
            Interlocked.Exchange(ref m_request, (int)request);
        }
    }
}

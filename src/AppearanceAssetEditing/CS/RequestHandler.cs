// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

namespace Ara3D.RevitSampleBrowser.AppearanceAssetEditing.CS
{
    /// <summary>
    ///     A class with methods to execute requests made by the dialog user.
    /// </summary>
    public static class RequestHandler
    {
        /// <summary>
        ///     The top function that distributes requests to individual methods.
        /// </summary>
        public static void Execute(Application app, RequestId request)
        {
            switch (request)
            {
                case RequestId.None:
                {
                    return; // no request at this time -> we can leave immediately
                }
                case RequestId.Select:
                {
                    app.GetPaintedMaterial();
                    break;
                }
                case RequestId.Lighter:
                {
                    app.ModifySelectedMaterial("Lighter", true);
                    break;
                }
                case RequestId.Darker:
                {
                    app.ModifySelectedMaterial("Darker", false);
                    break;
                }
            }
        }
    } // class
}

using System;
using Autodesk.Revit.UI;

namespace Ara3D.RevitSampleBrowser
{
    // https://stackoverflow.com/questions/31490990/starting-a-transaction-from-an-external-application-running-outside-of-api-conte
    // https://help.autodesk.com/cloudhelp/2018/ENU/Revit-API/Revit_API_Developers_Guide/Advanced_Topics/External_Events.html

    /// <summary>
    /// This class executes an action on the API's main thread using the Revit API external event mechanism.
    /// </summary>
    public class GenericExternalEventHandler : IExternalEventHandler, IDisposable
    {
        public Action Action { get; set; }
        public string Name { get; set; } = "External event";
        public ExternalEvent Event { get; }

        public GenericExternalEventHandler()
            => Event = ExternalEvent.Create(this);

        public void Execute(UIApplication app)
            => Action?.Invoke();

        public string GetName()
            => Name;

        public void Dispose()
            => Event.Dispose();

        public ExternalEventRequest Raise(Action action, string name)
        {
            Action = action;
            Name = name;
            return Event.Raise();
        }
    }
}
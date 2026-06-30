using Autodesk.Revit.UI;
using System;

namespace Ara3D.Bowerbird.RevitSamples
{
    /// <summary>
    /// Marshals work onto Revit's API thread without BowerbirdRevitApp.
    /// </summary>
    public static class RevitEventScheduler
    {
        private static readonly RevitEventHandler Handler = new();
        private static readonly ExternalEvent Event = ExternalEvent.Create(Handler);

        public static void Run(Action<UIApplication> action)
        {
            Handler.Action = action;
            Event.Raise();
        }

        private sealed class RevitEventHandler : IExternalEventHandler
        {
            public Action<UIApplication> Action { get; set; }

            public void Execute(UIApplication app)
            {
                Action?.Invoke(app);
            }

            public string GetName()
            {
                return "BB Revit Event Scheduler";
            }
        }
    }
}

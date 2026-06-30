using Autodesk.Revit.UI;
using System;
using System.Collections.Concurrent;

namespace Ara3D.Bowerbird.RevitSamples;

public static class RevitWorkQueue
{
    public static ConcurrentQueue<Action<UIApplication>> Queue = new();
    public static ExternalEvent WorkEvent;

    public static void Init()
    {
        if (WorkEvent != null)
            return;
        WorkEvent = RevitApiContext.CreateEvent(_executeQueuedWork, nameof(_executeQueuedWork));
    }

    private static void _executeQueuedWork(UIApplication app)
    {
        while (Queue.TryDequeue(out var action))
            action.Invoke(app);
    }

    public static void QueueWork(Action<UIApplication> action)
    {
        if (WorkEvent == null)
            throw new Exception($"{nameof(RevitWorkQueue)} has not been initialized");
        Queue.Enqueue(action);
        WorkEvent.Raise();
    }
}
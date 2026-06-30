// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using System;
using Autodesk.Revit.UI;
using Nice3point.Revit.Toolkit.External;
using ToolkitExternalEvent = Nice3point.Revit.Toolkit.External.ExternalEvent;

namespace Ara3D.RevitSampleBrowser.Common.Infrastructure
{
    public sealed class RevitActionEvent
    {
        private readonly ToolkitExternalEvent _externalEvent;
        private Action<UIApplication> _handler;

        public RevitActionEvent()
        {
            _externalEvent = new ToolkitExternalEvent(application => _handler?.Invoke(application));
        }

        public ExternalEventRequest Raise(Action action, string name = null)
        {
            _handler = _ => action();
            return _externalEvent.Raise();
        }

        public ExternalEventRequest Raise(Action<UIApplication> action, string name = null)
        {
            _handler = action;
            return _externalEvent.Raise();
        }
    }

    public sealed class RevitAsyncActionEvent
    {
        private readonly AsyncExternalEvent _externalEvent;
        private Action<UIApplication> _handler;

        public RevitAsyncActionEvent()
        {
            _externalEvent = new AsyncExternalEvent(application => _handler?.Invoke(application));
        }

        public System.Threading.Tasks.Task RaiseAsync(Action action)
        {
            _handler = _ => action();
            return _externalEvent.RaiseAsync();
        }

        public System.Threading.Tasks.Task RaiseAsync(Action<UIApplication> action)
        {
            _handler = action;
            return _externalEvent.RaiseAsync();
        }
    }
}

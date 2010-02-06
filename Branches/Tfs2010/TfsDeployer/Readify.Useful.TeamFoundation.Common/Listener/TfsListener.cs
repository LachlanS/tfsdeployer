using System;
using System.Collections.Generic;
using Microsoft.TeamFoundation.Framework.Client;
using Readify.Useful.TeamFoundation.Common.Notification;
using Microsoft.TeamFoundation.VersionControl.Common;

namespace Readify.Useful.TeamFoundation.Common.Listener
{
    public class TfsListener : ITfsListener
    {
        public event EventHandler<CheckinEventArgs> CheckinEventReceived;
        public event EventHandler<BuildCompletionEventArgs> BuildCompletionEventReceived;
        public event EventHandler<BuildStatusChangeEventArgs> BuildStatusChangeEventReceived;

        private readonly IList<ITfsEventListener> _eventListeners = new List<ITfsEventListener>();
        private readonly IEventService _eventService;

        public TfsListener(IEventService eventService)
        {
            _eventService = eventService;
        }

        public void Start()
        {
            RegisterEventListeners();

            foreach(var listener in _eventListeners)
            {
                listener.Start();
            }
            
        }

        private void RegisterEventListeners()
        {
            if (CheckinEventReceived != null)
            {
                var checkinListener = new TfsEventListener<CheckinEvent>(_eventService);
                TfsEventListener<CheckinEvent>.NotificationDelegate = 
                    (eventRaised, identity) => CheckinEventReceived(this, new CheckinEventArgs(eventRaised, identity));
                _eventListeners.Add(checkinListener);
            }

            if (BuildCompletionEventReceived != null)
            {
                var buildCompletionEvent = new TfsEventListener<BuildCompletionEvent>(_eventService);
                TfsEventListener<BuildCompletionEvent>.NotificationDelegate = 
                    (eventRaised, identity) => BuildCompletionEventReceived(this, new BuildCompletionEventArgs(eventRaised, identity));
                _eventListeners.Add(buildCompletionEvent);
            }

            if (BuildStatusChangeEventReceived != null)
            {
                var buildStatusChangeEvent = new TfsEventListener<BuildStatusChangeEvent>(_eventService);
                TfsEventListener<BuildStatusChangeEvent>.NotificationDelegate = 
                    (eventRaised, identity) => BuildStatusChangeEventReceived(this, new BuildStatusChangeEventArgs(eventRaised, identity));
                _eventListeners.Add(buildStatusChangeEvent);
            }
        }

        public void Stop()
        {
            foreach(var listener in _eventListeners)
            {
                listener.Stop();
            }
        }

    }
}


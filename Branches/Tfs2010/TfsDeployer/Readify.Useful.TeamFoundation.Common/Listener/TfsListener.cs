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
        private readonly Uri _baseAddress;

        public TfsListener(IEventService eventService, Uri baseAddress)
        {
            _eventService = eventService;
            _baseAddress = baseAddress;
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
                var checkinListener = new TfsEventListener<CheckinEvent>(_eventService, _baseAddress);
                TfsEventListener<CheckinEvent>.NotificationDelegate = 
                    (eventRaised, identity) => CheckinEventReceived(this, new CheckinEventArgs(eventRaised, identity));
                _eventListeners.Add(checkinListener);
            }

            if (BuildCompletionEventReceived != null)
            {
                var buildCompletionEvent = new TfsEventListener<BuildCompletionEvent>(_eventService, _baseAddress);
                TfsEventListener<BuildCompletionEvent>.NotificationDelegate = 
                    (eventRaised, identity) => BuildCompletionEventReceived(this, new BuildCompletionEventArgs(eventRaised, identity));
                _eventListeners.Add(buildCompletionEvent);
            }

            if (BuildStatusChangeEventReceived != null)
            {
                var buildStatusChangeEvent = new TfsEventListener<BuildStatusChangeEvent>(_eventService, _baseAddress);
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


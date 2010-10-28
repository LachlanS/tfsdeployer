using System;

namespace Readify.Useful.TeamFoundation.Common.Listener
{
    public interface ITfsListener
    {
        event EventHandler<CheckinEventArgs> CheckinEventReceived;
        event EventHandler<BuildCompletionEventArgs> BuildCompletionEventReceived;
        event EventHandler<BuildStatusChangeEventArgs> BuildStatusChangeEventReceived;

        void Start();
        void Stop();
    }
}

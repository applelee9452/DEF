using System;
using System.Collections.Generic;

namespace DEF
{
    public class Event
    {
        EventContext EventContext { get; set; }

        public void SetEventContext(EventContext event_context)
        {
            EventContext = event_context;
        }

        public void Broadcast()
        {
            string name = GetType().Name;
            EventContext._broadcastEvent(name, this);
        }
    }

    public abstract class EventListener
    {
        public EventContext EventContext { get; set; }
        List<string> ListBindEvent { get; set; } = new List<string>();

        public void _addBindEvent(string ev_name)
        {
            ListBindEvent.Add(ev_name);
        }

        public List<string> _getAllBindEvent()
        {
            return ListBindEvent;
        }

        public void _clearAllBindEvent()
        {
            ListBindEvent.Clear();
        }

        public abstract void HandleEvent(Event ev);
    }

    public class EventListenerDelegate : EventListener
    {
        public Delegate Action { get; private set; }

        private EventListenerDelegate(Delegate action)
        {
            Action = action;
        }

        public override void HandleEvent(Event ev)
        {
            Action?.DynamicInvoke(ev);
        }

        public void Delete()
        {
            Action = null;
        }

        public static implicit operator EventListenerDelegate(Delegate action)
        {
            return new EventListenerDelegate(action);
        }
    }
}
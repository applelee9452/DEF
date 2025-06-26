using System;
using System.Collections.Generic;
using System.Linq;

namespace DEF
{
    public class EventContext
    {
        Dictionary<string, HashSet<EventListener>> MapEvAndListener { get; set; } = new();
        Dictionary<string, Queue<Event>> MapEvPool { get; set; } = new();// 不同类型事件的内存池

        public void ListenEvent<T>(EventListener listener) where T : DEF.Event
        {
            string name = typeof(T).Name;
            MapEvAndListener.TryGetValue(name, out HashSet<EventListener> set_listener);

            if (set_listener == null)
            {
                set_listener = new HashSet<EventListener>();
                MapEvAndListener[name] = set_listener;
            }

            listener._addBindEvent(name);
            set_listener.Add(listener);
        }

        public void UnListenAllEvent(EventListener listener)
        {
            List<string> list_bindevent = listener._getAllBindEvent();
            foreach (var i in list_bindevent)
            {
                MapEvAndListener.TryGetValue(i, out HashSet<EventListener> set_listener);
                set_listener?.Remove(listener);
            }
        }

        public void ListenEvent<T>(Action<T> action) where T : DEF.Event
        {
            string name = typeof(T).Name;
            MapEvAndListener.TryGetValue(name, out HashSet<EventListener> set_listener);

            if (set_listener == null)
            {
                set_listener = new HashSet<EventListener>();
                MapEvAndListener[name] = set_listener;
            }
            var exsit = set_listener.FirstOrDefault((t) =>
            {
                if (t is EventListenerDelegate ed)
                {
                    return ed.Action == (Delegate)action;
                }
                return false;
            });
            if (exsit == null)
            {
                EventListenerDelegate listener = action;
                listener._addBindEvent(name);
                set_listener.Add(listener);
            }
        }

        public void UnListenEvent<T>(Action<T> action) where T : DEF.Event
        {
            string name = typeof(T).Name;

            MapEvAndListener.TryGetValue(name, out HashSet<EventListener> set_listener);
            if (set_listener == null)
            {
                return;
            }
            EventListenerDelegate to_remove = null;
            foreach (var v in set_listener)
            {
                if (v is EventListenerDelegate eld)
                {
                    if (eld.Action == (Delegate)action)
                    {
                        to_remove = eld;
                        eld.Delete();
                        break;
                    }
                }
            }
            if (to_remove != null)
            {
                set_listener?.Remove(to_remove);
            }
        }

        public void UnListenEvent(string type_name, Delegate action)
        {
            MapEvAndListener.TryGetValue(type_name, out HashSet<EventListener> set_listener);
            if (set_listener == null)
            {
                return;
            }
            EventListenerDelegate to_remove = null;
            foreach (var v in set_listener)
            {
                if (v is EventListenerDelegate eld)
                {
                    if (eld.Action == (Delegate)action)
                    {
                        to_remove = eld;
                        eld.Delete();
                        break;
                    }
                }
            }
            if (to_remove != null)
            {
                set_listener?.Remove(to_remove);
            }
        }

        public T GenEvent<T>() where T : DEF.Event, new()
        {
            string name = typeof(T).Name;
            MapEvPool.TryGetValue(name, out Queue<Event> que_ev);
            if (que_ev == null || que_ev.Count == 0)
            {
                T ev = new();
                ev.SetEventContext(this);
                return ev;
            }
            else
            {
                return (T)que_ev.Dequeue();
            }
        }

        public void _broadcastEvent(string ev_name, Event ev)
        {
            MapEvAndListener.TryGetValue(ev_name, out HashSet<EventListener> set_listener);
            if (set_listener != null)
            {
                List<EventListener> list_evlistener = new(set_listener);
                foreach (var i in list_evlistener)
                {
                    i.HandleEvent(ev);
                }
            }

            MapEvPool.TryGetValue(ev_name, out Queue<Event> que_ev);
            if (que_ev == null)
            {
                que_ev = new Queue<Event>();
                MapEvPool[ev_name] = que_ev;
            }

            que_ev.Enqueue(ev);
        }
    }
}
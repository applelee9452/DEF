using System.Collections.Generic;

namespace DEF
{
    public interface IEbAction2
    {
        string HandleAction(ref IEbEvent ev);
    }

    public class EbAction2 : IEbAction2
    {
        public delegate string DelegateSignature(ref IEbEvent ev);
        DelegateSignature Handler;

        public EbAction2(DelegateSignature handler)
        {
            Handler = handler;
        }

        public string HandleAction(ref IEbEvent ev)
        {
            return Handler(ref ev);
        }
    }

    public abstract class EbState2
    {
        protected Dictionary<string, IEbAction2> MapAction { get; set; } = new Dictionary<string, IEbAction2>();
        public string StateName { get; protected set; } = string.Empty;

        public virtual void Enter()
        {
        }

        public virtual void Exit()
        {
        }

        public string _onEvent(ref IEbEvent ev)
        {
            if (MapAction.ContainsKey(ev.name))
            {
                return MapAction[ev.name].HandleAction(ref ev);
            }
            else
            {
                return string.Empty;
            }
        }

        public void _defState(string state_name)
        {
            StateName = state_name;
        }

        public bool _isBindEvent(string event_name)
        {
            return MapAction.ContainsKey(event_name);
        }

        public void _bindAction(string event_name, IEbAction2 act)
        {
            MapAction[event_name] = act;
        }
    }
}
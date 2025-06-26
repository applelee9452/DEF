#if ENABLE_FSM

using System.Collections.Generic;
using System.Threading.Tasks;

namespace DEF
{
    public interface IEbAction
    {
        Task<string> HandleAction(IEbEvent ev);
    }

    public class EbAction : IEbAction
    {
        public delegate Task<string> DelegateSignature(IEbEvent ev);
        DelegateSignature Handler;

        public EbAction(DelegateSignature handler)
        {
            Handler = handler;
        }

        public Task<string> HandleAction(IEbEvent ev)
        {
            return Handler(ev);
        }
    }

    public abstract class EbState
    {
        protected Dictionary<string, IEbAction> MapAction { get; set; } = new Dictionary<string, IEbAction>();
        public string StateName { get; protected set; } = string.Empty;

        public virtual Task Enter()
        {
            return Task.CompletedTask;
        }

        public virtual Task Exit()
        {
            return Task.CompletedTask;
        }

        public virtual Task<string> Update(float tm)
        {
            return Task.FromResult(string.Empty);
        }

        public Task<string> _onEvent(IEbEvent ev)
        {
            if (MapAction.ContainsKey(ev.name))
            {
                return MapAction[ev.name].HandleAction(ev);
            }

            return Task.FromResult(string.Empty);
        }

        public void _defState(string state_name)
        {
            StateName = state_name;
        }

        public bool _isBindEvent(string event_name)
        {
            return MapAction.ContainsKey(event_name);
        }

        public void _bindAction(string event_name, IEbAction act)
        {
            MapAction[event_name] = act;
        }
    }
}

#endif
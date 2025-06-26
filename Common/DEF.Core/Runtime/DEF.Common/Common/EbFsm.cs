#if ENABLE_FSM

#if !DEF_CLIENT
using Microsoft.Extensions.Logging;
#endif
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DEF
{
    public class EbFsm
    {
        public EbState CurrentState { get; private set; }
        Dictionary<string, EbState> MapState { get; set; } = new Dictionary<string, EbState>();
        Queue<IEbEvent> QueEvent { get; set; } = new Queue<IEbEvent>();
#if !DEF_CLIENT
        ILogger Logger { get; set; }

        public EbFsm(ILogger logger)
        {
            Logger = logger;
        }
#endif

        public Task EnterInitState()
        {
            return CurrentState.Enter();
        }

        public Task ExitCurrentState()
        {
            return CurrentState.Exit();
        }

        public async Task Update(float tm)
        {
            if (CurrentState == null) return;

            string next_state_name = await CurrentState.Update(tm);
            if (string.IsNullOrEmpty(next_state_name)) return;

            await CurrentState.Exit();

            CurrentState = MapState[next_state_name];

            await CurrentState.Enter();
        }

        public EbFsm AddState(EbState state, bool is_init_state = false)
        {
            MapState[state.StateName] = state;

            if (is_init_state)
            {
                CurrentState = state;
            }

            return this;
        }

        public Task ProcessEvent(string ev_name)
        {
            IEbEvent ev = new EbEvent0(ev_name);
            QueEvent.Enqueue(ev);
            return _rattleOn();
        }

        public Task ProcessEvent<P1>(string ev_name, P1 p1)
        {
            IEbEvent ev = new EbEvent1<P1>(ev_name, p1);
            QueEvent.Enqueue(ev);
            return _rattleOn();
        }

        public Task ProcessEvent<P1, P2>(string ev_name, P1 p1, P2 p2)
        {
            IEbEvent ev = new EbEvent2<P1, P2>(ev_name, p1, p2);
            QueEvent.Enqueue(ev);
            return _rattleOn();
        }

        public Task ProcessEvent<P1, P2, P3>(string ev_name, P1 p1, P2 p2, P3 p3)
        {
            IEbEvent ev = new EbEvent3<P1, P2, P3>(ev_name, p1, p2, p3);
            QueEvent.Enqueue(ev);
            return _rattleOn();
        }

        public Task ProcessEvent<P1, P2, P3, P4>(string ev_name, P1 p1, P2 p2, P3 p3, P4 p4)
        {
            IEbEvent ev = new EbEvent4<P1, P2, P3, P4>(ev_name, p1, p2, p3, p4);
            QueEvent.Enqueue(ev);
            return _rattleOn();
        }

        public Task ProcessEvent<P1, P2, P3, P4, P5>(string ev_name, P1 p1, P2 p2, P3 p3, P4 p4, P5 p5)
        {
            IEbEvent ev = new EbEvent5<P1, P2, P3, P4, P5>(ev_name, p1, p2, p3, p4, p5);
            QueEvent.Enqueue(ev);
            return _rattleOn();
        }

        async Task _rattleOn()
        {
            while (QueEvent.Count > 0)
            {
                IEbEvent ev = QueEvent.Dequeue();

                EbState state_current = CurrentState;

                if (!state_current._isBindEvent(ev.name)) continue;

                string next_state_name = string.Empty;

                try
                {
                    next_state_name = await state_current._onEvent(ev);
                }
                catch (System.Exception ex)
                {
#if !DEF_CLIENT
                    Logger.LogCritical(ex, "EbFsm");
#endif
                }

                if (string.IsNullOrEmpty(next_state_name)) continue;

                MapState.TryGetValue(next_state_name, out var state_next);
                if (state_next != null && state_next != state_current)
                {
                    await state_current.Exit();

                    CurrentState = state_next;

                    await state_next.Enter();
                }
            }
        }
    }
}

#endif
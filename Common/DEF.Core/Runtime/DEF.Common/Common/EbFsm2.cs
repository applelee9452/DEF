using System.Collections.Generic;

namespace DEF
{
    public class EbFsm2
    {
        public EbState2 CurrentState { get; private set; }
        Dictionary<string, EbState2> MapState { get; set; } = new Dictionary<string, EbState2>();
        Queue<IEbEvent> QueEvent { get; set; } = new Queue<IEbEvent>();
        bool RattleOn { get; set; } = false;

        public EbFsm2()
        {
        }

        public void EnterInitState()
        {
            CurrentState.Enter();
        }

        public void ExitCurrentState()
        {
            CurrentState?.Exit();
        }

        public EbFsm2 AddState(EbState2 state, bool is_init_state = false)
        {
            MapState[state.StateName] = state;

            if (is_init_state)
            {
                CurrentState = state;
            }

            return this;
        }

        public void ProcessEvent(string ev_name)
        {
            IEbEvent ev = new EbEvent0(ev_name);
            QueEvent.Enqueue(ev);
            _rattleOn();
        }

        public void ProcessEvent<P1>(string ev_name, P1 p1)
        {
            IEbEvent ev = new EbEvent1<P1>(ev_name, p1);
            QueEvent.Enqueue(ev);
            _rattleOn();
        }

        public void ProcessEvent<P1, P2>(string ev_name, P1 p1, P2 p2)
        {
            IEbEvent ev = new EbEvent2<P1, P2>(ev_name, p1, p2);
            QueEvent.Enqueue(ev);
            _rattleOn();
        }

        public void ProcessEvent<P1, P2, P3>(string ev_name, P1 p1, P2 p2, P3 p3)
        {
            IEbEvent ev = new EbEvent3<P1, P2, P3>(ev_name, p1, p2, p3);
            QueEvent.Enqueue(ev);
            _rattleOn();
        }

        public void ProcessEvent<P1, P2, P3, P4>(string ev_name, P1 p1, P2 p2, P3 p3, P4 p4)
        {
            IEbEvent ev = new EbEvent4<P1, P2, P3, P4>(ev_name, p1, p2, p3, p4);
            QueEvent.Enqueue(ev);
            _rattleOn();
        }

        public void ProcessEvent<P1, P2, P3, P4, P5>(string ev_name, P1 p1, P2 p2, P3 p3, P4 p4, P5 p5)
        {
            IEbEvent ev = new EbEvent5<P1, P2, P3, P4, P5>(ev_name, p1, p2, p3, p4, p5);
            QueEvent.Enqueue(ev);
            _rattleOn();
        }

        void _rattleOn()
        {
            if (RattleOn) return;
            RattleOn = true;

            while (QueEvent.Count > 0)
            {
                IEbEvent ev = QueEvent.Dequeue();

                EbState2 state_current = CurrentState;

                if (!state_current._isBindEvent(ev.name)) continue;

                string next_state_name = state_current._onEvent(ref ev);

                if (string.IsNullOrEmpty(next_state_name)) continue;

                MapState.TryGetValue(next_state_name, out var state_next);
                if (state_next != null && state_next != state_current)
                {
                    state_current.Exit();

                    CurrentState = state_next;

                    state_next.Enter();
                }
            }

            RattleOn = false;
        }
    }
}
using System.Collections.Generic;

namespace DEF
{
    public abstract class SimpleState
    {
        public abstract string GetName();

        public abstract void Enter(string from_state_name);

        public abstract void Exit();

        public abstract string Update(float tm);

        public abstract string OnEvent(string ev_name, string ev_param);
    }

    public class SimpleFsm
    {
        Dictionary<string, SimpleState> MapState { get; set; } = new Dictionary<string, SimpleState>();
        SimpleState Current { get; set; }

        public void AddState(SimpleState state, bool is_default = false)
        {
            MapState[state.GetName()] = state;

            if (is_default)
            {
                Current = state;
            }
        }

        public string GetCurrentState()
        {
            return Current.GetName();
        }

        public void Enter()
        {
            Current.Enter(string.Empty);
        }

        public void Exit()
        {
            Current.Exit();
        }

        private void ChangeStateInterval(string from_state_name, string to_state_name)
        {
            Current.Exit();

            Current = MapState[to_state_name];

            Current.Enter(from_state_name);
        }

        public void Update(float tm)
        {
            string next_state_name = Current.Update(tm);
            if (string.IsNullOrEmpty(next_state_name)) return;

            ChangeStateInterval(Current.GetName(), next_state_name);
        }

        public void OnEvent(string ev_name, string ev_param)
        {
            string next_state_name = Current.OnEvent(ev_name, ev_param);
            if (string.IsNullOrEmpty(next_state_name)) return;

            ChangeStateInterval(Current.GetName(), next_state_name);
        }
    }
}
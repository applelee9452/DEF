#if DEF_CLIENT

using System.Collections.Generic;

//public class SimpleEv
//{
//}

//public delegate string DelegateSimpleStateAction(T ev) where T : SimpleEv;

public abstract class SimpleState2
{
    Dictionary<string, object> MapAction { get; set; }

    public abstract string GetName();

    public abstract void Enter();

    public abstract void Exit();

    public abstract string Update(float tm);

    public virtual string OnEvent(string ev_name, string ev_param)
    {
        return string.Empty;
    }

    public virtual string OnEvent(string ev_name, object ev_param)
    {
        return string.Empty;
    }

    public virtual string OnEvent(string ev_name, object ev_param1, object ev_param2)
    {
        return string.Empty;
    }

    //protected void BindEv<T>(DelegateSimpleStateAction<T> cb) where T : SimpleEv
    //{
    //}
}

public class SimpleFsm2
{
    Dictionary<string, SimpleState2> MapState { get; set; } = new();
    SimpleState2 Current { get; set; }

    public void AddState(SimpleState2 state, bool is_default = false)
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
        Current?.Enter();
    }

    public void Exit()
    {
        Current?.Exit();
    }

    public void Update(float tm)
    {
        if (Current == null) return;

        string next_state_name = Current.Update(tm);
        if (string.IsNullOrEmpty(next_state_name)) return;

        Current.Exit();

        Current = MapState[next_state_name];

        Current.Enter();
    }

    public void OnEvent(string ev_name, string ev_param)
    {
        if (Current == null) return;

        string next_state_name = Current.OnEvent(ev_name, ev_param);
        if (string.IsNullOrEmpty(next_state_name)) return;

        Current.Exit();

        Current = MapState[next_state_name];

        Current.Enter();
    }

    public void OnEvent(string ev_name, object ev_param)
    {
        if (Current == null) return;

        string next_state_name = Current.OnEvent(ev_name, ev_param);
        if (string.IsNullOrEmpty(next_state_name)) return;

        Current.Exit();

        Current = MapState[next_state_name];

        Current.Enter();
    }

    public void OnEvent(string ev_name, object ev_param1, object ev_param2)
    {
        if (Current == null) return;

        string next_state_name = Current.OnEvent(ev_name, ev_param1, ev_param2);
        if (string.IsNullOrEmpty(next_state_name)) return;

        Current.Exit();

        Current = MapState[next_state_name];

        Current.Enter();
    }
}

#endif